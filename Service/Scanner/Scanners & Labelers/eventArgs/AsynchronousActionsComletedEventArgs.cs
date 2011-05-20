using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using Service.Scanner.ScannersAndDataMaskBuilders;

namespace Service.Scanner.Labelers.eventArgs
{
    public delegate void AsynchronousActionsComletedEventHandler(object sender, AsynchronousActionsComletedEventArgs e);

    public class AsynchronousActionsComletedEventArgs : AsyncCompletedEventArgs
    {
        public AsynchronousActionsComletedEventArgs(Exception error, bool cancelled, object userState, SRegion[] regions)
            : base(error, cancelled, userState)
        {
            this.Regions = regions;
        }

        public SRegion[] Regions { get; private set; }
    }
}
