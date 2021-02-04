using ETMS.Event.DataContract;
using ETMS.IBusiness.EventConsumer;
using ETMS.IDataAccess;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using ETMS.Entity.Enum;
using ETMS.IEventProvider;

namespace ETMS.Business.EventConsumer
{
    public class EvClassBLL : IEvClassBLL
    {
        private readonly IClassDAL _classDAL;

        private readonly IEventPublisher _eventPublisher;

        public EvClassBLL(IClassDAL classDAL, IEventPublisher eventPublisher)
        {
            this._classDAL = classDAL;
            this._eventPublisher = eventPublisher;
        }

        public void InitTenantId(int tenantId)
        {
            this.InitDataAccess(tenantId, _classDAL);
        }

        public async Task ClassOfOneAutoOverConsumerEvent(ClassOfOneAutoOverEvent request)
        {
            var myOneToOneClass = await _classDAL.GetStudentOneToOneClassNormal(request.StudentId, request.CourseId);
            if (myOneToOneClass.Any())
            {
                var now = DateTime.Now;
                foreach (var p in myOneToOneClass)
                {
                    var thisClassBucket = await _classDAL.GetClassBucket(p.Id);
                    var thisClass = thisClassBucket.EtClass;
                    var thisClassStudent = thisClassBucket.EtClassStudents;
                    if (thisClass.Type == EmClassType.OneToOne)
                    {
                        if (thisClassStudent == null || thisClassStudent.Count == 0 ||
                            thisClassStudent.FirstOrDefault(j => j.StudentId == request.StudentId) != null)
                        {
                            await _classDAL.SetClassOverOneToOne(p.Id, now);
                            LOG.Log.Info($"[ClassOfOneAutoOverConsumerEvent]TenantId:{p.TenantId},ClassId:{p.Id}自动结课", this.GetType());
                        }
                    }
                }
            }
        }

        public async Task ClassRemoveStudentConsumerEvent(ClassRemoveStudentEvent request)
        {
            var myInClassList = await _classDAL.GetStudentCourseInClass(request.StudentId, request.CourseId);
            foreach (var myClass in myInClassList)
            {
                await _classDAL.DelClassStudentByStudentId(myClass.ClassId, request.StudentId);
                _eventPublisher.Publish(new SyncClassInfoEvent(request.TenantId, myClass.ClassId));
            }
        }
    }
}
