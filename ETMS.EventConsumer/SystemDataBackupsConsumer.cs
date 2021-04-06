using ETMS.Entity.Config;
using ETMS.Event.DataContract;
using ETMS.IOC;
using ETMS.Utility;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.EventConsumer
{
    [QueueConsumerAttribution("SystemDataBackupsQueue")]
    public class SystemDataBackupsConsumer : ConsumerBase<SystemDataBackupsEvent>
    {
        protected override async Task Receive(SystemDataBackupsEvent giftExchange)
        {
            var mailConfig = CustomServiceLocator.GetInstance<IAppConfigurtaionServices>().AppSettings.MailConfig;
            var newBakFiles = FileHelper.GetDirectoryNewestFile(mailConfig.SystemDataBackupsServerPath, mailConfig.SystemDataBackupsSearchPattern);
            if (newBakFiles.Count == 0)
            {
                LOG.Log.Error("[SystemDataBackupsConsumer]未找到数据库备份文件", this.GetType());
                return;
            }
            LOG.Log.Info($"[SystemDataBackupsConsumer]需要备份的数据库文件:{Newtonsoft.Json.JsonConvert.SerializeObject(newBakFiles)}", this.GetType());
            var desc = $"etms_bak_{DateTime.Now.ToString("yyyMMddHHmm")}_{newBakFiles.Count}";
            var newDirectory = FileHelper.CreateDirectory(mailConfig.SystemDataBackupsServerPath, desc);
            foreach (var file in newBakFiles)
            {
                File.Copy(file, Path.Combine(newDirectory, Path.GetFileName(file)), true);
            }
            var myZipFile = FileHelper.CompressZipFiles(newDirectory);
            if (myZipFile.Count == 0)
            {
                LOG.Log.Error("[SystemDataBackupsConsumer]创建zip文件失败", this.GetType());
                return;
            }
            ////所有文件 放入一个zip压缩
            //var newZipFileName = Path.Combine(mailConfig.SystemDataBackupsServerPath, $"{desc}.zip");
            //if (!File.Exists(newZipFileName))
            //{
            //    FileHelper.CompressZip(newDirectory, newZipFileName);
            //}
            foreach (var zipFile in myZipFile)
            {
                var fileName = Path.GetFileNameWithoutExtension(zipFile);
                var attachment = new List<string>();
                attachment.Add(zipFile);
                try
                {
                    MailHelper.Send(new MailSetting()
                    {
                        MailHost = mailConfig.MailHost,
                        MailPort = mailConfig.MailPort,
                        SenderAddress = mailConfig.SenderAddress,
                        SenderDisplayName = mailConfig.SenderDisplayName,
                        SenderPassword = mailConfig.SenderPassword,
                        SenderUserName = mailConfig.SenderUserName
                    }, new MailInfo()
                    {
                        AttachmentAddress = attachment,
                        Subject = desc,
                        Body = fileName,
                        Recipients = mailConfig.SystemDataBackupsGetUser
                    });
                }
                catch (Exception ex)
                {
                    LOG.Log.Error("[SystemDataBackupsConsumer]备份文件出错", ex, this.GetType());
                }
            }
            Directory.Delete(newDirectory, true);
        }
    }
}

