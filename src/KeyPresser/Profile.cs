namespace KeyPresser
{
    public class Profile
    {
        public int Id { get; set; }
        public int PauseMin { get; set; }
        public int PauseMax { get; set; }
        public int ActionMin { get; set; }
        public int ActionMax { get; set; }
        public int ActionDelay { get; set; }
        public int ColourPickerX { get; set; }
        public int ColourPickerY { get; set; }
        public ActionType ActionType { get; set; }
        public bool MontiorColour { get; set; } 
        public bool SeaFishing { get;set; }
        public bool IsFloat { get; set; }
        public bool RiseRod { get; set; }
    }
}
