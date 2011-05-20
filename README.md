## Simple example

    	public partial class Frame : Form
    	{
         ITrainee tra;
         ILabeler lab;

         private void ScanImage()
         {
            tra = new RgbHCrCbTrainee();

            lab = new FloodFillLabelerUnsafe(tra, source, 10);
            lab.AddFilter(new RemoveDustPreProcessFilter());
            lab.AddFilter(new RemoveNoiseRegionFilter());
            lab.Execute();
         }
	}
