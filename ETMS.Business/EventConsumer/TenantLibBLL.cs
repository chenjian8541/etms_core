using ETMS.Entity.Database.Source;
using ETMS.IBusiness.EventConsumer;
using ETMS.IDataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Business.EventConsumer
{
    public class TenantLibBLL : ITenantLibBLL
    {
        private readonly IStudentDAL _studentDAL;

        private readonly ICourseDAL _courseDAL;

        private readonly IClassDAL _classDAL;

        private readonly INoticeConfigDAL _noticeConfigDAL;

        public TenantLibBLL(IStudentDAL studentDAL, ICourseDAL courseDAL, IClassDAL classDAL, INoticeConfigDAL noticeConfigDAL)
        {
            this._studentDAL = studentDAL;
            this._courseDAL = courseDAL;
            this._classDAL = classDAL;
            this._noticeConfigDAL = noticeConfigDAL;
        }

        public void InitTenantId(int tenantId)
        {
            this.InitDataAccess(tenantId, _studentDAL, _courseDAL, _classDAL, _noticeConfigDAL);
        }

        public async Task<EtNoticeConfig> NoticeConfigGet(int type, byte peopleType, int scenesType)
        {
            return await _noticeConfigDAL.GetNoticeConfig(type, peopleType, scenesType);
        }

        public async Task<IEnumerable<EtClass>> GetStudentInClass(long studentId)
        {
            return await _classDAL.GetStudentClass(studentId);
        }
    }
}
