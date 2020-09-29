using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace ETMS.Utility
{
    public class FileHelper
    {
        public static CheckImportStudentTemplateFileResult CheckImportStudentTemplateFile(string serverPath, string fileFullName)
        {
            var timeStr = DateTime.Now.ToString("yyyyMMdd");
            var imgTag = "export_file";
            var strFolder = Path.Combine(serverPath, imgTag, timeStr);
            if (!Directory.Exists(strFolder))
            {
                Directory.CreateDirectory(strFolder);
            }
            var strFileFullPath = Path.Combine(strFolder, fileFullName);
            var isExist = File.Exists(strFileFullPath);
            var urlKey = $"{imgTag}/{timeStr}/{fileFullName}";
            return new CheckImportStudentTemplateFileResult()
            {
                IsExist = isExist,
                StrFileFullPath = strFileFullPath,
                StrFolder = strFolder,
                UrlKey = urlKey
            };
        }

        public static Tuple<string, string> PreProcessImgFolder(string serverPath)
        {
            var timeStr = DateTime.Now.ToString("yyyyMMdd");
            var imgTag = "img";
            var strFolder = Path.Combine(serverPath, imgTag, timeStr);
            if (!Directory.Exists(strFolder))
            {
                Directory.CreateDirectory(strFolder);
            }
            return Tuple.Create(strFolder, $"{imgTag}/{timeStr}/");
        }

        public static string GetFilePath(string filePath)
        {
            return Path.Combine(AppContext.BaseDirectory, filePath);
        }
    }

    public class CheckImportStudentTemplateFileResult
    {
        public string StrFolder { get; set; }

        public string StrFileFullPath { get; set; }

        public bool IsExist { get; set; }

        public string UrlKey { get; set; }
    }
}
