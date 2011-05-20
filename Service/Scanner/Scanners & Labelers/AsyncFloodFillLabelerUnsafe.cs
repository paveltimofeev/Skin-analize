using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;
using System.Collections;
using System.ComponentModel;
using System.Collections.Specialized;
using Service.Scanner.ScannersAndDataMaskBuilders;
using Service.Scanner.Filters.PreProcessFilters;
using Service.Scanner.Filters.RegionFilters;
using Service.Trainee;
using System.Diagnostics;
using System.Threading;
using Service.Scanner.Labelers.eventArgs;

namespace Service.Scanner.Labelers
{
    public class AsyncFloodFillLabelerUnsafe : ImageScanner, ILabeler
    {
        /// <summary>
        /// разреженность floodfill-сканирования (1 - сканировать всё, 2 - через строку, 3 - через две строки и т.д.)
        /// </summary>
        protected readonly int rarity = 1;    
        /// <summary>
        /// Binary image.
        /// </summary>
        /*protected*/ int[,] binaryMask;
        protected SRegion[] regions = null;
        public SRegion[] Regions { get { return regions; } }

        public AsyncFloodFillLabelerUnsafe(ITrainee learned, Bitmap image, int rarity, object userState) : base(learned, image) 
        { 
            this.rarity = rarity;

            AsyncOperation operation = AsyncOperationManager.CreateOperation(userState);
            userStates.Add(userState, operation);
        }

        public override void Execute()
        {
            this.binaryMask = new int[this.Image.Size.Width, this.Image.Size.Height];

            ///Quick binarization via unsafe pixel-by-pixel scan
            /// 0 is non-skin pixel
            ///-1 is skin pixel
            base.Image.UnsafeScan(new UnsafeScanHandler(MarkPoint), ImageLockMode.ReadOnly);

            ///Apply pre process filters (Runs before labelling, but ufter binarization)
            this.RunPreProcessFilters();

            ///Labelling via Flood-Fill algorithm and Create Region Collection
            this.FloodFillLabellingRerefied();

            ///Apply post process filters
            RunPostProcessFilters();
        }

        /// <summary>
        /// Labelling and Create Region Collection in one method
        /// </summary>
        protected virtual void FloodFillLabellingRerefied()
        {
            Dictionary<int, SRegion> regs = new Dictionary<int, SRegion>();
           
            int regionIndex = 1;
            for (int x = 0; x < this.Image.Size.Width - rarity; x += rarity)
            {
                for (int y = 0; y < this.Image.Size.Height - rarity; y += rarity)
                {
                    if (binaryMask[x, y] == -1)
                    {
                        ///Flood-Fill algorithm
                        Queue<Point> q = new Queue<Point>();

                        if (binaryMask[x, y] != -1)
                            continue;

                        regionIndex++;

                        q.Enqueue(new Point(x, y));

                        while (q.Count > 0)
                        {
                            Point p = q.Peek();
                            int x1 = p.X;
                            int y1 = p.Y;

                            if (IsValid(p))
                            {
                                binaryMask[x1, y1] = regionIndex;

                                if (regs.ContainsKey(regionIndex))
                                    regs[regionIndex].AddPoint(x1, y1);
                                else
                                    regs.Add(regionIndex, new SRegion(ref binaryMask, regionIndex, Image.Size));
                            }

                            if (IsValid(new Point(x1, y1 + 1)))
                            {
                                binaryMask[x1, y1 + 1] = regionIndex;
                                q.Enqueue(new Point(x1, y1 + 1));

                                if (regs.ContainsKey(regionIndex))
                                    regs[regionIndex].AddPoint(x1, y1 + 1);
                                else
                                    regs.Add(regionIndex, new SRegion(ref binaryMask, regionIndex, Image.Size));
                            }

                            if (IsValid(new Point(x1, y1 - 1)))
                            {
                                binaryMask[x1, y1 - 1] = regionIndex;
                                q.Enqueue(new Point(x1, y1 - 1));

                                if (regs.ContainsKey(regionIndex))
                                    regs[regionIndex].AddPoint(x1, y1 - 1);
                                else
                                    regs.Add(regionIndex, new SRegion(ref binaryMask, regionIndex, Image.Size));
                            }

                            if (IsValid(new Point(x1 + 1, y1)))
                            {
                                binaryMask[x1 + 1, y1] = regionIndex;
                                q.Enqueue(new Point(x1 + 1, y1));

                                if (regs.ContainsKey(regionIndex))
                                    regs[regionIndex].AddPoint(x1 + 1, y1);
                                else
                                    regs.Add(regionIndex, new SRegion(ref binaryMask, regionIndex, Image.Size));
                            }

                            if (IsValid(new Point(x1 - 1, y1)))
                            {
                                binaryMask[x1 - 1, y1] = regionIndex;
                                q.Enqueue(new Point(x1 - 1, y1));

                                if (regs.ContainsKey(regionIndex))
                                    regs[regionIndex].AddPoint(x1 - 1, y1);
                                else
                                    regs.Add(regionIndex, new SRegion(ref binaryMask, regionIndex, Image.Size));
                            }

                            q.Dequeue();
                        }
                        ///end of Flood-Fill algorithm
                    }
                }
            }

            regions = new SRegion[regs.Count];
            int index = 0;
            foreach (SRegion r in regs.Values)
            {
                regions[index] = r;
                index++;
            }
        }

        ///Quick binarization via unsafe image scan
        /// 0 is non-skin pixel
        ///-1 is skin pixel
        protected virtual void MarkPoint(int x, int y, byte r, byte g, byte b)
        {
            if (learned.Contains(r, g, b))
                binaryMask[x, y] = -1; //-1 is skin pixel, 0 is not skin
        }

        /// <summary>
        /// Check validity of point location at the image.
        /// </summary>
        /// <param name="p">Point</param>
        protected virtual bool IsValid(Point p)
        {
            if (
                (
                p.X > 1 &
                p.Y > 1 &
                p.X < Image.Size.Width - 1 &
                p.Y < Image.Size.Height - 1
                )
                &&
                binaryMask[p.X, p.Y] == -1
                )
                return true;
            else
                return false;
        }

        #region EventBasedAsynchronousPattern
        private Object syncObj = new Object();
        protected Hashtable userStates = new Hashtable();
        public bool IsBusy { get; private set; }
        public bool Canceled { get; private set; }
        public delegate void AsynchronousOperationHandler(object userState);
        public void StartAsync(object userState)
        {
            Reset(userState);
            this.IsBusy = true;

            AsynchronousOperationHandler handler = new AsynchronousOperationHandler(this.AsynchronousOperation);
            handler.BeginInvoke(userState, new AsyncCallback(Finish), null); //?
            //handler.Invoke(userState);
            //AsynchronousOperation(userState);

            AsyncOperation operation = (AsyncOperation)userStates[userState];
            operation.Post(new SendOrPostCallback(PostStarted), new AsynchronousActionsEventArgs());
        }
        public void CancelAsync(object userState)
        {
            Canceled = true;
            AsyncOperation operation = (AsyncOperation)userStates[userState];

            operation.PostOperationCompleted(
                new SendOrPostCallback(PostCanceled),
                new AsynchronousActionsComletedEventArgs(null, Canceled, userState, regions));
        }
        private void Reset(object userState)
        {
            AsyncOperation operation = (AsyncOperation)userStates[userState];
            operation = AsyncOperationManager.CreateOperation(userState);
            this.IsBusy = false;
            this.Canceled = false;
        }
        private void AsynchronousOperation(object userState)
        {
            Thread.CurrentThread.IsBackground = true;
            Thread.CurrentThread.Name = string.Format("BackgroundScanAndLabelingThread{0}", userState);

            AsyncOperation operation = (AsyncOperation)userStates[userState];
            Exception exception = null;

            try
            {
                Operation(userState);
            }
            catch (Exception ex)
            {
                exception = ex;
            }
            finally
            {
                operation.PostOperationCompleted(new SendOrPostCallback(PostCompleted), new AsynchronousActionsComletedEventArgs(exception, Canceled, userState, regions));
                Reset(userState);
            }
        }
        protected virtual void Operation(object userState)
        {
            //AsyncOperation operation = (AsyncOperation)userStates[userState];
            //operation.Post(new SendOrPostCallback(PostProgress), new AsynchronousActionsEventArgs());

            Execute();
        }
        private void Finish(IAsyncResult ar)
        {
            ar.AsyncWaitHandle.Close();
        }
        #endregion

        #region EventBasedAsynchronousPattern Posts

        private void PostStarted(object obj)
        {
            AsynchronousActionsEventArgs e = obj as AsynchronousActionsEventArgs;
            if (e != null)
                onStarted(e);
        }

        protected void PostProgress(object obj)
        {
            AsynchronousActionsEventArgs e = obj as AsynchronousActionsEventArgs;
            if (e != null)
                onProgress(e);
        }

        private void PostCanceled(object obj)
        {
            IsBusy = false;

            AsynchronousActionsComletedEventArgs e = obj as AsynchronousActionsComletedEventArgs;
            if (e != null)
                onCancel(e);
        }

        private void PostCompleted(object obj)
        {
            IsBusy = false;

            AsynchronousActionsComletedEventArgs e = obj as AsynchronousActionsComletedEventArgs;
            if (e != null)
                onComplete(e);
        }

        #endregion

        #region EventBasedAsynchronousPattern Events

        public event AsynchronousActionsEventHandler ActionStarted;
        public event AsynchronousActionsEventHandler ActionProgress;
        public event AsynchronousActionsComletedEventHandler ActionCanceled;
        public event AsynchronousActionsComletedEventHandler ActionCompleted;

        private void onStarted(AsynchronousActionsEventArgs e)
        {
            AsynchronousActionsEventHandler handler = ActionStarted;
            if (handler != null)
                handler(this, e);
        }

        private void onProgress(AsynchronousActionsEventArgs e)
        {
            AsynchronousActionsEventHandler handler = ActionProgress;
            if (handler != null)
                handler(this, e);
        }

        private void onCancel(AsynchronousActionsComletedEventArgs e)
        {
            AsynchronousActionsComletedEventHandler handler = ActionCanceled;
            if (handler != null)
                handler(this, e);
        }

        private void onComplete(AsynchronousActionsComletedEventArgs e)
        {
            AsynchronousActionsComletedEventHandler handler = ActionCompleted;
            if (handler != null)
                handler(this, e);
        }

        #endregion

        #region Filters

        protected List<IPostProcessFilter> regionFilters = new List<IPostProcessFilter>();
        public void AddFilter(IPostProcessFilter filter)
        {
            regionFilters.Add(filter);
        }
        public void RemoveFilter(IPostProcessFilter filter)
        {
            regionFilters.Remove(filter);
        }
        public bool ContainsFilter(IPostProcessFilter filter)
        {
            return regionFilters.Contains(filter);
        }

        protected List<IPreProcessFilter> preProcessFilters = new List<IPreProcessFilter>();
        public void AddFilter(IPreProcessFilter filter)
        {
            preProcessFilters.Add(filter);
        }
        public void RemoveFilter(IPreProcessFilter filter)
        {
            preProcessFilters.Remove(filter);
        }
        public bool ContainsFilter(IPreProcessFilter filter)
        {
            return preProcessFilters.Contains(filter);
        }
        
        protected void RunPreProcessFilters()
        {
            foreach (IPreProcessFilter filter in preProcessFilters)
            {
                filter.Apply(this.Image.Size, this.binaryMask);
            }
        }

        protected void RunPostProcessFilters()
        {
            foreach (IPostProcessFilter filter in regionFilters)
            {
                filter.Apply(ref regions);
            }

            foreach (IPostProcessFilter filter in regionFilters)
            {
                foreach (SRegion region in regions)
                {
                    filter.Apply(region);
                }
            }
        }

        #endregion

        #region IDisposable Members

        public void Dispose()
        {
            this.Image.Dispose();
        }

        #endregion
    }
}