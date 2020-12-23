using ETMS.Entity.Database.Source;
using ETMS.Entity.Enum;
using ETMS.Event.DataContract;
using ETMS.IBusiness;
using ETMS.IDataAccess;
using ETMS.Utility;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Business
{
    public class TempDataGenerateBLL : ITempDataGenerateBLL
    {
        private readonly IClassTimesDAL _classTimesDAL;

        private readonly IClassDAL _classDAL;

        private readonly ITempStudentNeedCheckDAL _tempStudentNeedCheckDAL;

        public TempDataGenerateBLL(IClassTimesDAL classTimesDAL, IClassDAL classDAL, ITempStudentNeedCheckDAL tempStudentNeedCheckDAL)
        {
            this._classTimesDAL = classTimesDAL;
            this._classDAL = classDAL;
            this._tempStudentNeedCheckDAL = tempStudentNeedCheckDAL;
        }

        public void InitTenantId(int tenantId)
        {
            this.InitDataAccess(tenantId, _classTimesDAL, _classDAL, _tempStudentNeedCheckDAL);
        }

        public async Task TempStudentNeedCheckGenerateConsumerEvent(TempStudentNeedCheckGenerateEvent request)
        {
            var classTimes = await _classTimesDAL.GetClassTimes(request.ClassTimesId);
            if (classTimes == null)
            {
                LOG.Log.Fatal($"[定时生成待考勤学员信息]课次不存在:{request.TenantId}", this.GetType());
                return;
            }
            var myClassBucket = await _classDAL.GetClassBucket(classTimes.ClassId);
            if (myClassBucket == null || myClassBucket.EtClass == null)
            {
                LOG.Log.Fatal($"[定时生成待考勤学员信息]班级不存在:{request.TenantId}", this.GetType());
                return;
            }
            var classDesc = $"{EtmsHelper.GetTimeDesc(classTimes.StartTime)}~{EtmsHelper.GetTimeDesc(classTimes.EndTime)} {myClassBucket.EtClass.Name}";
            var tempStudentNeedCheckList = new List<EtTempStudentNeedCheck>();
            var tempTempStudentNeedCheckClassList = new List<EtTempStudentNeedCheckClass>();
            var myClassTimesStudent = await _classTimesDAL.GetClassTimesStudent(request.ClassTimesId);
            var classOt = request.ClassOt.Date;
            if (myClassTimesStudent != null && myClassTimesStudent.Count > 0)
            {
                foreach (var p in myClassTimesStudent)
                {
                    tempStudentNeedCheckList.Add(new EtTempStudentNeedCheck()
                    {
                        IsCheckIn = EmBool.False,
                        IsCheckOut = EmBool.False,
                        Ot = classOt,
                        IsDeleted = EmIsDeleted.Normal,
                        StartTime = classTimes.StartTime,
                        StudentId = p.StudentId,
                        TenantId = p.TenantId,
                        ClassDesc = classDesc
                    });
                    tempTempStudentNeedCheckClassList.Add(new EtTempStudentNeedCheckClass()
                    {
                        TenantId = p.TenantId,
                        StudentId = p.StudentId,
                        StartTime = classTimes.StartTime,
                        IsDeleted = EmIsDeleted.Normal,
                        ClassId = classTimes.ClassId,
                        ClassOt = classTimes.ClassOt,
                        Ot = classOt,
                        ClassTimesId = classTimes.Id,
                        CourseId = p.CourseId,
                        EndTime = classTimes.EndTime,
                        Week = classTimes.Week,
                        Status = EmTempStudentNeedCheckClassStatus.NotAttendClass
                    });
                }
            }
            if (myClassBucket.EtClassStudents != null && myClassBucket.EtClassStudents.Count > 0)
            {
                foreach (var p in myClassBucket.EtClassStudents)
                {
                    tempStudentNeedCheckList.Add(new EtTempStudentNeedCheck()
                    {
                        IsCheckIn = EmBool.False,
                        IsCheckOut = EmBool.False,
                        Ot = classOt,
                        IsDeleted = EmIsDeleted.Normal,
                        StartTime = classTimes.StartTime,
                        StudentId = p.StudentId,
                        TenantId = p.TenantId,
                        ClassDesc = classDesc
                    });
                    tempTempStudentNeedCheckClassList.Add(new EtTempStudentNeedCheckClass()
                    {
                        TenantId = p.TenantId,
                        StudentId = p.StudentId,
                        StartTime = classTimes.StartTime,
                        IsDeleted = EmIsDeleted.Normal,
                        ClassId = classTimes.ClassId,
                        ClassOt = classTimes.ClassOt,
                        Ot = classOt,
                        ClassTimesId = classTimes.Id,
                        CourseId = p.CourseId,
                        EndTime = classTimes.EndTime,
                        Week = classTimes.Week,
                        Status = EmTempStudentNeedCheckClassStatus.NotAttendClass
                    });
                }
            }
            if (tempStudentNeedCheckList.Count > 0)
            {
                await _tempStudentNeedCheckDAL.TempStudentNeedCheckAdd(tempStudentNeedCheckList);
            }
            if (tempTempStudentNeedCheckClassList.Count > 0)
            {
                await _tempStudentNeedCheckDAL.TempStudentNeedCheckClassAdd(tempTempStudentNeedCheckClassList);
            }
        }
    }
}
