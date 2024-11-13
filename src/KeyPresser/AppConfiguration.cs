namespace KeyPresser
{
    public class AppConfiguration
    {
        public AppDefaults AppDefaults { get; set; }
        public Intervals Intervals { get; set; }
        public List<AppCoordinate> Coordinates { get; set; }
        public List<AppColor> Colors { get; set; }
    }

    public class AppDefaults
    {
        public int WaitMinMs { get; set; }
        public int WaitMaxMs { get; set; }
        public int HoldMinMs { get; set; }
        public int HoldMaxMs { get; set; }
        public int ActionDelayS { get; set; }
        public string WindowName { get; set; }
        
    }
    public class Intervals
    {
        public int PickColourTimerMs { get; set; }
        public int FloatDelayTimerMs { get; set; }
        public int FishOnDisplayMonitorMs { get; set; }
        public int MonitorColourTimerMs { get; set; }
        public int ReadyToCastTimerMs { get; set; }
        public int DiggingWaitMinMs { get; set; }
        public int DiggingWaitMaxMs { get; set; }
        public int SnackingWaitMinMs { get; set; }
        public int SnackingWaitMaxMs { get; set; }
        public int ErrorMonitorTimerMs { get; set; }
        
    }

    public class AppCoordinate
    {
        public string Name { get; set; }
        public Coordinate CoordinateX { get; set; }
        public Coordinate CoordinateY { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
    }

    public class Coordinate
    {
        public string RelatedTo { get; set; }
        public int Offset { get; set; }

    }
    public class AppColor
    {
        public string Name { get; set; }
        public List<RGBColor> Colors { get; set; }
    }
    public class RGBColor
    {
        public string Name { get; set; }
        public string GreaterLess { get; set; }
        public byte Value { get; set; }
        public bool Disabled { get; set; }
    }
}
