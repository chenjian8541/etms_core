using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace ETMS.Utility
{
    public class FileHelper
    {
        public const string ImportStudentTemplateFolderTag = "export_file";

        public const string ImgFolderTag = "img";

        public static CheckImportStudentTemplateFileResult CheckImportStudentTemplateFile(string serverPath, string fileFullName)
        {
            var timeStr = DateTime.Now.ToString("yyyyMMdd");
            var strFolder = Path.Combine(serverPath, ImportStudentTemplateFolderTag, timeStr);
            if (!Directory.Exists(strFolder))
            {
                Directory.CreateDirectory(strFolder);
            }
            var strFileFullPath = Path.Combine(strFolder, fileFullName);
            var isExist = File.Exists(strFileFullPath);
            var urlKey = $"{ImportStudentTemplateFolderTag}/{timeStr}/{fileFullName}";
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
            var strFolder = Path.Combine(serverPath, ImgFolderTag, timeStr);
            if (!Directory.Exists(strFolder))
            {
                Directory.CreateDirectory(strFolder);
            }
            return Tuple.Create(strFolder, $"{ImgFolderTag}/{timeStr}/");
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
