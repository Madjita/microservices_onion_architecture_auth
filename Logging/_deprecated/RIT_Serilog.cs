namespace Logging
{
    public class RIT_Serilog
    {
        public  virtual bool MainAll { get; set; }
        public  virtual bool TagUndefined { get; set; }
        public  virtual bool TagUndefinedToFile { get; set; }
        public  virtual Dictionary<string,Tag> Tags { get; set; }

        public virtual bool CheckTag(string tag)
        {
            return Tags.Any(x => x.Key == tag);
        }
    }
    public class Tag
    {
        public string FileName { get; set; } = "";
        public bool Write { get; set; }
    }
}