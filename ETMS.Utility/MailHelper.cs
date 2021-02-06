using System;
using System.Collections.Generic;
using System.Net.Mail;
using System.Text;

namespace ETMS.Utility
{
    public class MailHelper
    {
        /// <summary>
        /// 发送邮件
        /// </summary>
        /// <param name="mailConfig"></param>
        /// <param name="mailInfo"></param>
        /// <returns></returns>
        public static bool Send(MailSetting mailConfig, MailInfo mailInfo)
        {
            var message = new MailMessage();
            if (mailInfo.AttachmentAddress != null && mailInfo.AttachmentAddress.Count > 0)
            {
                foreach (var file in mailInfo.AttachmentAddress)
                {
                    message.Attachments.Add(new Attachment(file));
                }
            }
            foreach (var address in mailInfo.Recipients)
            {
                message.To.Add(new MailAddress(address));
            }
            message.From = new MailAddress(mailConfig.SenderAddress, mailConfig.SenderDisplayName);
            message.BodyEncoding = Encoding.GetEncoding(mailInfo.Encoding);
            message.Body = mailInfo.Body;
            message.SubjectEncoding = Encoding.GetEncoding(mailInfo.Encoding);
            message.Subject = mailInfo.Subject;
            message.IsBodyHtml = mailInfo.IsBodyHtml;
            var smtpclient = new SmtpClient(mailConfig.MailHost, mailConfig.MailPort);
            smtpclient.DeliveryMethod = SmtpDeliveryMethod.Network;
            smtpclient.EnableSsl = mailInfo.EnableSsl;
            smtpclient.UseDefaultCredentials = false;
            smtpclient.Credentials = new System.Net.NetworkCredential(mailConfig.SenderUserName, mailConfig.SenderPassword);
            smtpclient.Send(message);
            return true;
        }
    }

    public class MailSetting
    {
        public string SenderAddress { get; set; }

        public string SenderDisplayName { get; set; }

        public string SenderUserName { get; set; }

        public string SenderPassword { get; set; }

        public string MailHost { get; set; }

        public int MailPort { get; set; }
    }

    /// <summary>
    /// 邮件信息
    /// </summary>
    public class MailInfo
    {
        /// <summary>
        /// 收件人
        /// </summary>
        public string[] Recipients { get; set; }

        /// <summary>
        /// 主题
        /// </summary>
        public string Subject { get; set; }

        /// <summary>
        /// 内容
        /// </summary>
        public string Body { get; set; }

        /// <summary>
        /// 附件地址
        /// </summary>
        public List<string> AttachmentAddress { get; set; }

        /// <summary>
        /// 编码
        /// </summary>
        public string Encoding = "UTF-8";

        /// <summary>
        /// 邮件消息是html格式
        /// </summary>
        public bool IsBodyHtml = true;

        /// <summary>
        /// 启用安全套接字层(SSL)
        /// </summary>
        public bool EnableSsl = true;
    }
}
