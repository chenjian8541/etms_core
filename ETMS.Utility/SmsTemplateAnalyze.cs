using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Utility
{
    public class SmsTemplateAnalyze
    {
        public static List<SmsTemplateWildcard> SmsTemplate { get; set; }

        public static string GetSmsContent(string smsTemplate, List<SmsReplaceWord> smsReplaces)
        {
            var s = new StringBuilder(smsTemplate);
            foreach (var myReplace in smsReplaces)
            {
                s.Replace(myReplace.Wildcard, myReplace.Value);
            }
            return s.ToString().TrimEnd('，').TrimEnd(',');
        }

        static SmsTemplateAnalyze()
        {
            SmsTemplate = new List<SmsTemplateWildcard>();
            SmsTemplate.Add(new SmsTemplateWildcard()
            {
                Type = 0,
                Words = new List<string>() { "学员姓名", "上课时间", "课程名称", "上课教室" },
                Wildcards = new List<string>() { "{学员姓名}", "{上课时间}", "{课程名称}", "{上课教室}" },
            });
            SmsTemplate.Add(new SmsTemplateWildcard()
            {
                Type = 1,
                Words = new List<string>() { "学员姓名", "上课时间", "课程名称", "上课教室", },
                Wildcards = new List<string>() { "{学员姓名}", "{上课时间}", "{课程名称}", "{上课教室}", },
            });
            SmsTemplate.Add(new SmsTemplateWildcard()
            {
                Type = 2,
                Words = new List<string>() { "学员名称", "上课时间", "课程名称", "到课状态", "消耗课时", "剩余课时", "课程到期描述" },
                Wildcards = new List<string>() { "{学员名称}", "{上课时间}", "{课程名称}", "{到课状态}", "{消耗课时}", "{剩余课时}", "{课程到期描述}" },
            });
            SmsTemplate.Add(new SmsTemplateWildcard()
            {
                Type = 3,
                Words = new List<string>() { "学员姓名", "请假时间", "审核状态" },
                Wildcards = new List<string>() { "{学员姓名}", "{请假时间}", "{审核状态}" }
            });
            SmsTemplate.Add(new SmsTemplateWildcard()
            {
                Type = 4,
                Words = new List<string>() { "学员姓名", "订单时间", "订单描述", "消费金额", "支付金额" },
                Wildcards = new List<string>() { "{学员姓名}", "{订单时间}", "{订单描述}", "{消费金额}", "{支付金额}" },
            });
            SmsTemplate.Add(new SmsTemplateWildcard()
            {
                Type = 5,
                Words = new List<string>() { "学员姓名", "课程名称", "课时不足描述" },
                Wildcards = new List<string>() { "{学员姓名}", "{课程名称}", "{课时不足描述}" }
            });
            SmsTemplate.Add(new SmsTemplateWildcard()
            {
                Type = 6,
                Words = new List<string>() { "学员姓名", "考勤时间", "记上课描述" },
                Wildcards = new List<string>() { "{学员姓名}", "{考勤时间}", "{记上课描述}" }
            });
            SmsTemplate.Add(new SmsTemplateWildcard()
            {
                Type = 7,
                Words = new List<string>() { "学员姓名", "考勤时间" },
                Wildcards = new List<string>() { "{学员姓名}", "{考勤时间}" }
            });
            SmsTemplate.Add(new SmsTemplateWildcard()
            {
                Type = 8,
                Words = new List<string>() { "学员姓名", "剩余金额" },
                Wildcards = new List<string>() { "{学员姓名}", "{剩余金额}" }
            });
            SmsTemplate.Add(new SmsTemplateWildcard()
            {
                Type = 9,
                Words = new List<string>() { "学员姓名", "课程名称", "剩余课时", "课程到期描述" },
                Wildcards = new List<string>() { "{学员姓名}", "{课程名称}", "{剩余课时}", "{课程到期描述}" },
            });
            SmsTemplate.Add(new SmsTemplateWildcard()
            {
                Type = 31,
                Words = new List<string>() { "老师姓名", "课程名称", "上课时间", "上课教室" },
                Wildcards = new List<string>() { "{老师姓名}", "{课程名称}", "{上课时间}", "{上课教室}" }
            });
        }
    }

    public class SmsTemplateWildcard
    {
        public int Type { get; set; }

        public List<string> Words { get; set; }

        public List<string> Wildcards { get; set; }
    }

    public class SmsReplaceWord
    {
        public string Wildcard { get; set; }

        public string Value { get; set; }
    }
}
