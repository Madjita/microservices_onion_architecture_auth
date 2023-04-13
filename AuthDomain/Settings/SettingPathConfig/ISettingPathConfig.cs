namespace AuthDomain.Settings
{
   public interface ISettingPathConfig : 
        ISettingDefaultPathConfig, 
        ISettingPathInWorkConfig
    {
        public abstract string GetSettingsPath();
    }

    public interface ISettingDefaultPathConfig
    {
        public abstract bool CheckPath(string filepath);
        public abstract bool CheckSettingPath();
        public abstract bool CheckSettingFile(string fileName);
        public abstract bool CheckSettingFile(string filepath,string fileName);
        public abstract bool CheckSettingFile(string fileName,bool flagPath);
        public abstract void createSettingPath();
    }

    public interface ISettingPathInWorkConfig
    {
        public abstract bool CheckSettingDefaultPath();
    }
}