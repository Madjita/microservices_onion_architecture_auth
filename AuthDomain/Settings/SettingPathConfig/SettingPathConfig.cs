using System;
using System.IO;
using System.Reflection;
using AuthDAL.Settings;
using Logging;
using Newtonsoft.Json;

namespace AuthDomain.Settings
{
    public class SettingPathConfig : ISettingPathConfig
    {
        private string _path { get; set; } = Path.Join(AppDomain.CurrentDomain.BaseDirectory, "Settings/SettingsFiles/");
        private string _pathDefault { get; set; } = Path.Join(AppDomain.CurrentDomain.BaseDirectory, "Settings/SettingsFilesDefault/");
        public bool IsSettingPath => CheckSettingPath();
        public bool IsSettingDefaultPath => CheckSettingDefaultPath();

        public SettingPathConfig()
        {
            CheckSettingDefaultPath();
        }

        /// <summary>
        /// Метод CheckSettingPath() проверить существует ли папка с настройками
        /// Если папки нет, то создает папку.
        /// </summary>
        public  bool CheckSettingPath()
        {
            if (!Directory.Exists(_path))
            {
                Log.Error($"Не найден каталог с настройками \"{_path}\"");
                createSettingPath();
            }
            return true;
        }

        /// <summary>
        /// Метод CheckSettingDefaultPath() проверить существует ли папка с настройками по умолчанию
        /// Если папки нет, то создает папку и файл application.json.
        /// </summary>
        public bool CheckSettingDefaultPath()
        {
            if (!Directory.Exists(_pathDefault))
            {
                Log.Error($"Не найден каталог с настройками по умолчанию \"{_pathDefault}\"");

                createSettingDefaultPath();
                createSettingDefaultFiles();
            }
            return true;
        }

        /// <summary>
        /// Метод CheckSettingFile() проверяет на существование
        /// файлов настроек в папке настроек. Если файла нет, то копирует с папки настроек по умолчанию.
        /// </summary>
        public bool CheckSettingFile(string fileName)
        {
            if(IsSettingPath)
            {
                string pathItem = _path + fileName;
                if (!File.Exists(pathItem))
                {
                    try
                    {
                        File.Copy(_pathDefault + fileName, pathItem);
                    }
                    catch (IOException e)
                    {
                        Log.Error($"Не найден файл \"{fileName}\" в каталоге: \"{_pathDefault}\" : error = " + e.Message);
                        throw new IOException($"Не найден файл \"{fileName}\" в каталоге: \"{_pathDefault}\" : error = " + e.Message);
                    }
                }
                return true;
            }
            return false;
        }
        
        /// <summary>
        /// Метод CheckSettingFile() проверяет на существование
        /// файлов настроек в папке настроек. Если файла нет, то копирует с папки настроек по умолчанию.
        /// </summary>
        public bool CheckSettingFile(string fileName,bool flagPath)
        {
            if(IsSettingPath)
            {
                string addPath = flagPath ? _path : "";
                string pathItem = addPath + fileName;

                if(!File.Exists(pathItem))
                {
                    try
                    {
                        File.Copy(_pathDefault + fileName , pathItem);
                    }
                    catch(IOException e){
                        Log.Error($"Не найден файл \"{fileName}\" в каталоге: \"{_pathDefault}\" : error = "+e.Message);
                        throw new IOException($"Не найден файл \"{fileName}\" в каталоге: \"{_pathDefault}\" : error = "+e.Message);
                    }
                }

                return true;
            }

            return false;
        }

        /// <summary>
        /// Метод createSettingDefaultPath() создает
        /// папку по умолчанию для фалов настроек по умолчанию. Путь до папки берется из переменной _pathDefault
        /// </summary>
        private void createSettingDefaultPath()
        {
            Log.Information($"Создаем каталог с настройками \"{_pathDefault}\"");
            Directory.CreateDirectory(_pathDefault);
        }

        /// <summary>
        /// Метод createSettingPath() создает
        /// папку для фалов настроек. Путь до папки берется из переменной _path
        /// </summary>
        public void createSettingPath()
        {
            Log.Information($"Создаем каталог с настройками \"{_path}\"");
            Directory.CreateDirectory(_path);
        }

         /// <summary>
        /// Метод createSettingDefaultFiles() создает
        /// файлы по умолчанию в папке по умолчанию. Путь до папки берется из переменной _pathDefault
        /// </summary>
        private void createSettingDefaultFiles()
        {
            Log.Information($"Создаем файл \"appsettings.json\" в папку по умолчанию \"{_pathDefault}\"");
            AppSettings appSettings = new AppSettings();
            var json = JsonConvert.SerializeObject(appSettings, Formatting.Indented);
            File.WriteAllText(_pathDefault+"appsettings.json", json);
        }

        /// <summary>
        /// Метод GetSettingsPath() возвращает строковый
        /// путь до папки с настройками. "Settings/SettingsFiles/"
        /// </summary>
        /// <returns>
        /// Возвращает строковый путь до папки с настройками.
        /// </returns>
        public string GetSettingsPath()
        {
            return _path;
        }

        /// <summary>
        /// Метод GetSettingsDefaulPath() возвращает строковый
        /// путь до папки с настройками пл умолчанию. "Settings/SettingsDefaultFiles/"
        /// </summary>
        /// <returns>
        /// Возвращает строковый путь до папки с настройками.
        /// </returns>
        public string GetSettingsDefaulPath()
        {
            return _pathDefault;
        }

        public bool CheckSettingFile(string filepath, string fileName)
        {
            if(IsSettingPath)
            {
                string pathItem = filepath + fileName;
                if (!File.Exists(pathItem))
                {
                    try
                    {
                        File.Copy(_pathDefault + fileName, pathItem);
                        CheckSettingFile(fileName);
                    }
                    catch (IOException e)
                    {
                        Log.Error($"Не найден файл \"{fileName}\" в каталоге: \"{_pathDefault}\" : error = " + e.Message);
                        throw new IOException($"Не найден файл \"{fileName}\" в каталоге: \"{_pathDefault}\" : error = " + e.Message);
                    }
                }
                return true;
            }
            return false;
        }

        public bool CheckPath(string filepath)
        {
            if (!Directory.Exists(filepath))
            {
                Log.Error($"Не найден каталог \"{filepath}\"");

                Log.Information($"Создаем каталог с настройками \"{_pathDefault}\"");
                Directory.CreateDirectory(filepath);
            }
            
            return true;
        }
    }
}