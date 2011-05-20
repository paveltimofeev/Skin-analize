using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;

namespace Service.Scanner.Labelers.eventArgs
{
    public delegate void AsynchronousActionsEventHandler(object sender, AsynchronousActionsEventArgs e);

    public class AsynchronousActionsEventArgs : EventArgs
    {

    }
}
