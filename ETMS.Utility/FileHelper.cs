using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace ETMS.Utility
{
    public class FileHelper
    {
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
}
