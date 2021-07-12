using ETMS.Business.SendNotice;
using ETMS.Business.WxCore;
using ETMS.Entity.Database.Source;
using ETMS.Entity.Enum;
using ETMS.Entity.ExternalService.Dto.Request;
using ETMS.IBusiness.EventConsumer;
using ETMS.IBusiness.Wechart;
using ETMS.IDataAccess;
using ETMS.IDataAccess.EtmsManage;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq;

namespace ETMS.Business
{
    public abstract class SendStudentNoticeBase : SendNoticeBase
    {
        protected readonly IStudentWechatDAL _studentWechatDAL;

        public SendStudentNoticeBase(IStudentWechatDAL studentWechatDAL, IComponentAccessBLL componentAccessBLL, ISysTenantDAL sysTenantDAL, ITenantLibBLL tenantLibBLL)
            : base(componentAccessBLL, sysTenantDAL, tenantLibBLL)
        {
            this._studentWechatDAL = studentWechatDAL;
        }

        protected async Task<string> GetOpenId(bool isSendWeChat, string phone)
        {
            if (!isSendWeChat)
            {
                return string.Empty;
            }
            var wx = await _studentWechatDAL.GetStudentWechatByPhone(phone);
            return wx?.WechatOpenid;
        }

        /// <summary>
        /// <see cref="ETMS.Entity.Enum.EmNoticeConfigExType"/>
        /// </summary>
        public int ExType { get; set; }

        public string ConfigValue { get; set; }

        protected async Task InitNoticeConfig(int scenesType)
        {
            var log = await _tenantLibBLL.NoticeConfigGet(EmNoticeConfigType.NoticePeopleLimit, EmPeopleType.Student, scenesType);
            if (log != null)
            {
                this.ExType = log.ExType;
                this.ConfigValue = log.ConfigValue;
            }
        }

        protected bool CheckLimitNoticeClass(long classId)
        {
            if (string.IsNullOrEmpty(ConfigValue))
            {
                return false;
            }
            if (ExType != EmNoticeConfigExType.Class)
            {
                return false;
            }
            return ConfigValue.IndexOf($",{classId},") != -1;
        }

        protected async Task<bool> CheckLimitNoticeClassOfStudent(long studentId)
        {
            if (string.IsNullOrEmpty(ConfigValue))
            {
                return false;
            }
            if (ExType != EmNoticeConfigExType.Class)
            {
                return false;
            }
            var myClassList = await _tenantLibBLL.GetStudentInClass(studentId);
            if (myClassList != null && myClassList.Any())
            {
                foreach (var myClass in myClassList)
                {
                    if (ConfigValue.IndexOf($",{myClass.Id},") != -1)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        protected bool CheckLimitNoticeStudent(long studentId)
        {
            if (string.IsNullOrEmpty(ConfigValue))
            {
                return false;
            }
            if (ExType != EmNoticeConfigExType.People)
            {
                return false;
            }
            return ConfigValue.IndexOf($",{studentId},") != -1;
        }

        protected bool CheckLimitNoticeCourse(long courseId)
        {
            if (string.IsNullOrEmpty(ConfigValue))
            {
                return false;
            }
            if (ExType != EmNoticeConfigExType.Course)
            {
                return false;
            }
            return ConfigValue.IndexOf($",{courseId},") != -1;
        }
    }
}
