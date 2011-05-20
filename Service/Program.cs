using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using System.IO;
using System.Diagnostics;
using System.Collections;
using System.Drawing;
using Service.Scanner.Labelers;
using Service.Scanner.Filters.RegionFilters;
using Service.Trainee;
using Service.Scanner.ScannersAndDataMaskBuilders;
using System.Data.SqlClient;
using Service.Scanner.Analysers;
using Service.Scanner.Drawers;
using System.Xml;
using ANN.Lib;

namespace Service
{
    class Program
    {
        static void Main(string[] args)
        {
            NeuralNetworkTest();
            NeuralNetworkTest();
            NeuralNetworkTest();
            //Test();
        }

        private static void NeuralNetworkTest()
        {
            Dictionary<string, double[]> trainingSet = new Dictionary<string, double[]>();

            trainingSet.Add("1", ImageProcessing.ToMatrix((Bitmap)Bitmap.FromFile(@"D:\My Documents\My Pictures\NAL\trainset\1.JPG"), 10, 10));
            trainingSet.Add("2", ImageProcessing.ToMatrix((Bitmap)Bitmap.FromFile(@"D:\My Documents\My Pictures\NAL\trainset\2.JPG"), 10, 10));
            trainingSet.Add("3", ImageProcessing.ToMatrix((Bitmap)Bitmap.FromFile(@"D:\My Documents\My Pictures\NAL\trainset\3.JPG"), 10, 10));
            trainingSet.Add("4", ImageProcessing.ToMatrix((Bitmap)Bitmap.FromFile(@"D:\My Documents\My Pictures\NAL\trainset\4.JPG"), 10, 10));
            trainingSet.Add("5", ImageProcessing.ToMatrix((Bitmap)Bitmap.FromFile(@"D:\My Documents\My Pictures\NAL\trainset\5.JPG"), 10, 10));
            trainingSet.Add("6", ImageProcessing.ToMatrix((Bitmap)Bitmap.FromFile(@"D:\My Documents\My Pictures\NAL\trainset\6.JPG"), 10, 10));
            trainingSet.Add("7", ImageProcessing.ToMatrix((Bitmap)Bitmap.FromFile(@"D:\My Documents\My Pictures\NAL\trainset\7.JPG"), 10, 10));
            trainingSet.Add("8", ImageProcessing.ToMatrix((Bitmap)Bitmap.FromFile(@"D:\My Documents\My Pictures\NAL\trainset\8.JPG"), 10, 10));
            trainingSet.Add("9", ImageProcessing.ToMatrix((Bitmap)Bitmap.FromFile(@"D:\My Documents\My Pictures\NAL\trainset\9.JPG"), 10, 10));
            trainingSet.Add("10", ImageProcessing.ToMatrix((Bitmap)Bitmap.FromFile(@"D:\My Documents\My Pictures\NAL\trainset\10.JPG"), 10, 10));
            trainingSet.Add("11", ImageProcessing.ToMatrix((Bitmap)Bitmap.FromFile(@"D:\My Documents\My Pictures\NAL\trainset\11.JPG"), 10, 10));

            NeuralNetwork<string> network = new NeuralNetwork<string>(new BP3Layer<string>((10 * 10), 100, 100, 11), trainingSet);

            network.MaximumError = 0.1D;
            network.MaximumIteration = 1000;

            if(network.Train())
                Console.WriteLine("trained");
            else
                Console.WriteLine("non-trained");

            network.SaveNetwork("temp.net");

            //double[] imgV = ImageProcessing.ToMatrix((Bitmap)Bitmap.FromFile(@"D:\My Documents\My Pictures\NAL\trainset\ok.JPG"), 10, 10);
            double[] imgV = ImageProcessing.ToMatrix((Bitmap)Bitmap.FromFile(@"d:\Documents and Settings\Timofp01\Desktop\Other Programm Files\BPSimplified_demo\PATTERNS\x.bmp"), 10, 10);
            
            string max    = "-";
            string min    = "-";
            double maxVal = 0;
            double minVal = 0;

            network.Recognize(imgV, ref max, ref maxVal, ref min, ref minVal);

            Console.WriteLine("max    {0}", max);
            Console.WriteLine("min    {0}", min);
            Console.WriteLine("maxVal {0}", maxVal);
            Console.WriteLine("minVal {0}", minVal);

            Console.ReadLine();
        }

        private static void PointersTest()
        {
            DateTime d1 = DateTime.Now;
            unsafe
            {
                int* min = (int*)0;
                int* max = (int*)100;
                int* x =   (int*)0;
                int* y =   (int*)100;

                for (x = min; x < max; x++)
                {
                    for (y = min; y < max; y++)
                    {
                        System.Threading.Thread.Sleep(1);
                    }
                }
            }

            Console.WriteLine(((TimeSpan)(DateTime.Now - d1)).TotalMilliseconds.ToString());
            Console.ReadLine();
        }

        private static void UnsafeTest()
        {
            DateTime d1 = DateTime.Now;

            ILabeler lab = new FloodFillLabelerUnsafe(new RgbHCrCbTrainee(), new Bitmap(@"D:\My Documents\My Pictures\NAL\Group portret\Pack\UnsafeTest\test17.jpg"), 2);
            lab.AddFilter(new RemoveNoiseRegionFilter(0.01F, false));
            lab.AddFilter(new ClosingRegionFilter(MorfologicalRegionFilterBase.StructuralElement.Diamond5));
            lab.Execute();

            Console.WriteLine(((TimeSpan)(DateTime.Now - d1)).TotalMilliseconds.ToString());
            DateTime d2 = DateTime.Now;

            IClassifier analyse = new FaceClassifier(lab);

            IDrawer dis = new RegionDrawer(lab.Image, analyse.GetRegions());
            dis.SkinLayer = SkinLayerMode.SKIN;
            dis.DrawRegions().Save(@"D:\My Documents\My Pictures\NAL\Group portret\Pack\UnsafeTest\test17d.jpg");

            Console.WriteLine(((TimeSpan)(DateTime.Now - d2)).TotalMilliseconds.ToString());
            Console.WriteLine(((TimeSpan)(DateTime.Now - d1)).TotalMilliseconds.ToString());
            Console.Read();
        }

        private static void Test()
        {
            string folder = @"D:\My Documents\My Pictures\NAL\Group portret\Pack\";
            DirectoryInfo di = new DirectoryInfo(folder);
            if (di.Exists)
            {
                string resultPath = Path.Combine(di.FullName, "Detected faces");
                string guid = Guid.NewGuid().ToString().Substring(0, 4);

                XmlDocument html = new XmlDocument();
                XmlNode table = html.CreateNode(XmlNodeType.Element, "table", "");
              
                using (StreamWriter log = new StreamWriter(Path.Combine(resultPath, "log.txt"), true))
                {
                    log.AutoFlush = true;
                    int i = 0;
                    int faceCount = 0;

                    Console.WriteLine("Name;Number;   \tA;\tS");

                    foreach (FileInfo f in di.GetFiles("*.jpg", SearchOption.TopDirectoryOnly))
                    {
                        try
                        {
                            DateTime d1 = DateTime.Now;

                            string filePath = f.FullName;

                            ITrainee train = new RgbHCrCbTrainee();

                            ILabeler lab = new FloodFillLabelerUnsafe(train, new Bitmap(filePath), 2);
                            lab.AddFilter(new RemoveNoiseRegionFilter(0.01F));
                            lab.AddFilter(new ClosingRegionFilter(MorfologicalRegionFilterBase.StructuralElement.Diamond5));
                            lab.Execute();

                            DateTime d2 = DateTime.Now;

                            IClassifier analyse = new FaceClassifier(lab);
                            SRegion[] faces = analyse.GetRegions();
                            //SRegion[] faces = lab.Regions;

                            if (faces != null)
                                faceCount += faces.Length;

                            IDrawer display = new RegionDrawer(lab.Image, faces);
                            //IDrawer display = new InterestPointDrawer(lab.Image, faces);
                            display.BackLayer = BackLayerMode.BACKGROUNDIMAGE;
                            display.SkinLayer = SkinLayerMode.SKIN;
                            display.SkinLayerTransparent = 125;
                            display.InfoLayers = InfoLayerModes.CENTER | InfoLayerModes.FRAME;

                            string filename = string.Format("{0}_{1}.jpg", guid, f.Name);
                            display.DrawRegions().Save(Path.Combine(resultPath, filename));

                            string name = string.Format("{0};{1}", f.Name, i);
                            string msg = string.Format("{2};   \t{0};\t{1}", (d2 - d1).TotalSeconds, (DateTime.Now - d2).TotalSeconds, name);
                            Console.WriteLine(msg);
                            log.WriteLine(msg);


                            XmlNode tr = html.CreateNode(XmlNodeType.Element, "tr", "");
                            XmlNode td1 = html.CreateNode(XmlNodeType.Element, "td", "");
                            XmlNode td2 = html.CreateNode(XmlNodeType.Element, "td", "");
                            XmlNode img = html.CreateNode(XmlNodeType.Element, "img", "");
                            XmlAttribute src = html.CreateAttribute("src");

                            src.Value = filename;
                            td2.InnerText = msg;
                            
                            img.Attributes.Append(src);
                            td1.AppendChild(img);
                            tr.AppendChild(td1);
                            tr.AppendChild(td2);
                            table.AppendChild(tr);

                        }
                        catch (StackOverflowException)
                        {
                            log.WriteLine("StackOverflowException");
                        }
                        catch (OutOfMemoryException)
                        {
                            log.WriteLine("OutOfMemoryException");
                        }

                        i++;
                    }

                    log.WriteLine("{0} faces found", faceCount);
                }

                html.AppendChild(table);
                html.Save(Path.Combine(resultPath, "result.html"));
            }
            Console.WriteLine("Ready");
            //Console.Read();
        }

        private static void Test(string folder)
        {
            
            RgbHCrCbTrainee train = new RgbHCrCbTrainee();

            DirectoryInfo di = new DirectoryInfo(folder);
            if (di.Exists)
            {
                foreach (FileInfo f in di.GetFiles("*.jpg"))
                {

                    DateTime d1 = DateTime.Now;

                    FloodFillLabeler lab = new FloodFillLabeler(train, new Bitmap(f.FullName));
                    lab.AddFilter(new RemoveNoiseRegionFilter(0.01F));
                    lab.Execute();

                    Console.WriteLine("Analized: {0} sec.", (DateTime.Now - d1).TotalSeconds);

                    FaceClassifier analyse = new FaceClassifier(lab);
                    IDrawer display = new RegionDrawer(lab.Image, analyse.GetRegions());
                    display.SkinLayer = SkinLayerMode.SKIN;
                    display.InfoLayers = InfoLayerModes.INFORMATION | InfoLayerModes.CENTER;
                    display.DrawRegions().Save(f.FullName + "result.jpg");

                    Console.WriteLine("Saved: {0} sec.", (DateTime.Now - d1).TotalSeconds);
                    Console.WriteLine("");
                }
            }

            Console.Read();
        }
    }
}
