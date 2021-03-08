using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;

namespace ETMS.Utility
{
    public class FileHelper
    {
        public const string ImportStudentTemplateFolderTag = "export_file";

        public const string ImgFolderTag = "img";

        public static CheckImportStudentTemplateFileResult CheckImportExcelTemplateFile(string serverPath, string fileFullName)
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

        public static Tuple<string, string> PreProcessFolder(string serverPath, string folderTag = ImgFolderTag)
        {
            var timeStr = DateTime.Now.ToString("yyyyMMdd");
            var strFolder = Path.Combine(serverPath, folderTag, timeStr);
            if (!Directory.Exists(strFolder))
            {
                Directory.CreateDirectory(strFolder);
            }
            return Tuple.Create(strFolder, $"{folderTag}/{timeStr}/");
        }

        public static string GetFilePath(string filePath)
        {
            return Path.Combine(AppContext.BaseDirectory, filePath);
        }

        public static List<string> GetDirectoryNewestFile(string folderPath, string searchPattern)
        {
            var allFiles = Directory.GetFiles(folderPath, searchPattern);
            if (allFiles.Length == 0)
            {
                return new List<string>();
            }
            var lastFile = allFiles.OrderByDescending(d => new FileInfo(d).LastWriteTime).ToList();
            var lastWriteTime = string.Empty;
            var myFile = new List<string>();
            for (var i = 0; i < lastFile.Count; i++)
            {
                var myFileInfoLastWriteTime = new FileInfo(lastFile[i]).LastWriteTime.EtmsToMinuteString();
                if (i == 0)
                {
                    lastWriteTime = myFileInfoLastWriteTime;
                    myFile.Add(lastFile[i]);
                }
                else if (myFileInfoLastWriteTime == lastWriteTime)
                {
                    myFile.Add(lastFile[i]);
                }
            }
            return myFile;
        }

        public static string CreateDirectory(string folderPath, string newDirectoryName)
        {
            var newPath = Path.Combine(folderPath, newDirectoryName);
            if (!System.IO.Directory.Exists(newPath))
            {
                System.IO.Directory.CreateDirectory(newPath);
            }
            return newPath;
        }

        public static void CompressZip(string folderPath, string zipFilePath)
        {
            ZipFile.CreateFromDirectory(folderPath, zipFilePath);
        }

        public static List<string> CompressZipFiles(string folderPath)
        {
            var zipFile = new List<string>();
            var allFiles = Directory.GetFiles(folderPath);
            if (allFiles.Length == 0)
            {
                return zipFile;
            }
            foreach (var file in allFiles)
            {
                var myFileFullName = Path.GetFileName(file);
                var myFileName = Path.GetFileNameWithoutExtension(file);
                var newFileFolderPath = FileHelper.CreateDirectory(folderPath, myFileName);
                File.Copy(file, Path.Combine(newFileFolderPath, myFileFullName), true);
                var myZipFileName = Path.Combine(folderPath, $"{myFileName}.zip");
                FileHelper.CompressZip(newFileFolderPath, myZipFileName);
                zipFile.Add(myZipFileName);
            }
            return zipFile;
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
