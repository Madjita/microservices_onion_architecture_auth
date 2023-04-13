using System.Globalization;
using System;
using System.Linq;
using System.Runtime.CompilerServices;

using Serilog;
using System.Reflection;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;

namespace Logging
{
    public static class Log
    {
        public static RIT_Serilog _settings = new RIT_Serilog();
        private static string _projectSignature = "MD";
        private static string _fileNameDefalut => "main";

        private static ILogger EnrichWithCallerInformation(string tag, string fileName, string filepath, string memberName, int lineNumber)
        {
            return Serilog.Log
                .ForContext("FileName",fileName)
                .ForContext("Tag",tag)
                .ForContext("ProjectSignature", _projectSignature)
                .ForContext("FilePath", filepath.Split('/').Last())
                .ForContext("MemberName", memberName)
                .ForContext("LineNumber", lineNumber);
        }

        private static ILogger EnrichWithCallerInformation(string tag, string filepath, string memberName, int lineNumber)
        {
            return Serilog.Log
                .ForContext("Tag",tag)
                .ForContext("ProjectSignature", _projectSignature)
                .ForContext("FilePath", filepath.Split('/').Last())
                .ForContext("MemberName", memberName)
                .ForContext("LineNumber", lineNumber);
        }

        public static void Verbose(string message, 
            string tag = "",
            [CallerFilePath] string filepath = "",
            [CallerMemberName] string memberName = "",
            [CallerLineNumber] int lineNumber = 0)
        {
            var check = CheckTag(tag);
            if(check.Item2)
            {
                EnrichWithCallerInformation(tag, check.Item1, filepath, memberName, lineNumber).Verbose(message);

                if(_settings.MainAll && check.Item1 != _fileNameDefalut)
                    EnrichWithCallerInformation(tag, check.Item1, memberName, lineNumber).Verbose(message);
            }
            
        }

        public static void Information(string message, 
            string tag = "",
            [CallerFilePath] string filepath = "",
            [CallerMemberName] string memberName = "",
            [CallerLineNumber] int lineNumber = 0)
        {
            var check = CheckTag(tag);
            if(check.Item2)
            {
                EnrichWithCallerInformation(tag, check.Item1, filepath, memberName, lineNumber).Information(message);

                if(_settings.MainAll && check.Item1 != _fileNameDefalut)
                    EnrichWithCallerInformation(tag, check.Item1, memberName, lineNumber).Information(message);
            }
           
        }

        public static async Task InformationAsync(string message, 
            string tag = "",
            [CallerFilePath] string filepath = "",
            [CallerMemberName] string memberName = "",
            [CallerLineNumber] int lineNumber = 0)
        {
            await Task.Run(() => Information(message, tag, filepath, memberName, lineNumber));
        }


        public static void Debug(string message, 
            string tag = "",
            [CallerFilePath] string filepath = "",
            [CallerMemberName] string memberName = "",
            [CallerLineNumber] int lineNumber = 0)
        {
            var check = CheckTag(tag);
            if(check.Item2)
            {
                EnrichWithCallerInformation(tag, check.Item1, filepath, memberName, lineNumber).Debug(message);

                if(_settings.MainAll && check.Item1 != _fileNameDefalut)
                    EnrichWithCallerInformation(tag, check.Item1, memberName, lineNumber).Debug(message);
            }
           
        }

        public static void Warning(string message, 
            string tag = "",
            [CallerFilePath] string filepath = "",
            [CallerMemberName] string memberName = "",
            [CallerLineNumber] int lineNumber = 0)
        {
            var check = CheckTag(tag);
            if(check.Item2)
            {
                EnrichWithCallerInformation(tag, check.Item1, filepath, memberName, lineNumber).Warning(message);

                if(_settings.MainAll && check.Item1 != _fileNameDefalut)
                    EnrichWithCallerInformation(tag, check.Item1, memberName, lineNumber).Warning(message);
            }
            
        }

        public static async Task WarningAsync(string message, 
            string tag = "",
            [CallerFilePath] string filepath = "",
            [CallerMemberName] string memberName = "",
            [CallerLineNumber] int lineNumber = 0)
        {
            await Task.Run(() => Warning(message, tag, filepath, memberName, lineNumber));
        }

        public static void Error(string message, 
            string tag = "",
            [CallerFilePath] string filepath = "",
            [CallerMemberName] string memberName = "",
            [CallerLineNumber] int lineNumber = 0)
        {
            var check = CheckTag(tag);
            if(check.Item2)
            {
                EnrichWithCallerInformation(tag, check.Item1, filepath, memberName, lineNumber).Error(message);

                if(_settings.MainAll && check.Item1 != _fileNameDefalut)
                    EnrichWithCallerInformation(tag, check.Item1, memberName, lineNumber).Error(message);
            }
        }

        public static async Task ErrorAsync(string message, 
            string tag = "",
            [CallerFilePath] string filepath = "",
            [CallerMemberName] string memberName = "",
            [CallerLineNumber] int lineNumber = 0)
        {
            await Task.Run(() => Error(message, tag, filepath, memberName, lineNumber));
        }

        private static (string,bool) CheckTag(string tag)
        {
            // try
            // {
            //     if(_settings == null || tag =="") //Отлавливание ошибок, когда "Settings" является null, тогда записывать логи в файл  стандартный файл.. 
            //         return (_fileNameDefalut, true);

            //     Tag obj = _settings.Tags[tag];
            //     return (obj.FileName, obj.Write);
            // }
            // catch
            // {
                if(_settings.TagUndefinedToFile)
                {
                    //Записываем  в отдельный файл по тэгу те тэги которых нет в списке отслеживаемых тэгов, если данная настройка включенна
                    return (tag, true);
                }
                return (_fileNameDefalut, _settings.TagUndefined);
            // }
        }


    }
}
