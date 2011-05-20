using System;
using System.Drawing;
namespace Service.Trainee
{
    public interface ITrainee
    {
        bool Contains(byte r, byte g, byte b);
        bool Contains(Color c);
        int Execute();
    }
}
