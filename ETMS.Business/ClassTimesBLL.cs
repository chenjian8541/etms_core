using ETMS.Business.Common;
using ETMS.Entity.Common;
using ETMS.Entity.Dto.Educational.Output;
using ETMS.Entity.Dto.Educational.Request;
using ETMS.IBusiness;
using ETMS.IDataAccess;
using ETMS.Utility;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using ETMS.Entity.Enum;
using ETMS.Entity.Database.Source;

namespace ETMS.Business
{
    public class ClassTimesBLL : IClassTimesBLL
    {
        private readonly IClassDAL _classDAL;

        private readonly IClassRoomDAL _classRoomDAL;

        private readonly IStudentCourseDAL _studentCourseDAL;

        private readonly IClassTimesDAL _classTimesDAL;

        private readonly IUserDAL _userDAL;

        private readonly ICourseDAL _courseDAL;

        private readonly IStudentDAL _studentDAL;

        private readonly IUserOperationLogDAL _userOperationLogDAL;

        private readonly IStudentTrackLogDAL _studentTrackLogDAL;

        private readonly ITryCalssLogDAL _tryCalssLogDAL;

        public ClassTimesBLL(IClassDAL classDAL, IClassRoomDAL classRoomDAL, IStudentCourseDAL studentCourseDAL, IClassTimesDAL classTimesDAL,
            IUserDAL userDAL, ICourseDAL courseDAL, IStudentDAL studentDAL, IUserOperationLogDAL userOperationLogDAL, IStudentTrackLogDAL studentTrackLogDAL,
            ITryCalssLogDAL tryCalssLogDAL)
        {
            this._classDAL = classDAL;
            this._classRoomDAL = classRoomDAL;
            this._studentCourseDAL = studentCourseDAL;
            this._classTimesDAL = classTimesDAL;
            this._userDAL = userDAL;
            this._courseDAL = courseDAL;
            this._studentDAL = studentDAL;
            this._userOperationLogDAL = userOperationLogDAL;
            this._studentTrackLogDAL = studentTrackLogDAL;
            this._tryCalssLogDAL = tryCalssLogDAL;
        }

        public void InitTenantId(int tenantId)
        {
            this.InitDataAccess(tenantId, _classDAL, _classRoomDAL, _studentCourseDAL, _classTimesDAL, _userDAL, _courseDAL, _studentDAL,
                _userOperationLogDAL, this._studentTrackLogDAL, _tryCalssLogDAL);
        }

        public async Task<ResponseBase> ClassTimesGetView(ClassTimesGetViewRequest request)
        {
            var classTimes = await _classTimesDAL.GetClassTimes(request.CId);
            if (classTimes == null)
            {
                return ResponseBase.CommonError("课次不存在");
            }
            var etClass = await _classDAL.GetClassBucket(classTimes.ClassId);
            if (etClass == null || etClass.EtClass == null)
            {
                return ResponseBase.CommonError("课次所在班级不存在");
            }
            var classRoomIdsDesc = string.Empty;
            if (!string.IsNullOrEmpty(classTimes.ClassRoomIds))
            {
                var allClassRoom = await _classRoomDAL.GetAllClassRoom();
                classRoomIdsDesc = ComBusiness.GetDesc(allClassRoom, classTimes.ClassRoomIds);
            }
            var tempBoxCourse = new DataTempBox<EtCourse>();
            var tempBoxUser = new DataTempBox<EtUser>();
            return ResponseBase.Success(new ClassTimesGetViewOutput()
            {
                CId = classTimes.Id,
                ClassContent = classTimes.ClassContent,
                ClassId = classTimes.ClassId,
                ClassName = etClass.EtClass.Name,
                ClassOt = classTimes.ClassOt.EtmsToDateString(),
                ClassRoomIds = classTimes.ClassRoomIds,
                ClassRoomIdsDesc = classRoomIdsDesc,
                CourseList = classTimes.CourseList,
                CourseListDesc = await ComBusiness.GetCourseNames(tempBoxCourse, _courseDAL, classTimes.CourseList),
                EndTime = classTimes.EndTime,
                StartTime = classTimes.StartTime,
                Status = classTimes.Status,
                TimeDesc = $"{EtmsHelper.GetTimeDesc(classTimes.StartTime)}~{EtmsHelper.GetTimeDesc(classTimes.EndTime)}",
                Week = classTimes.Week,
                WeekDesc = $"周{EtmsHelper.GetWeekDesc(classTimes.Week)}",
                TeacherNum = classTimes.TeacherNum,
                Teachers = classTimes.Teachers,
                TeachersDesc = await ComBusiness.GetUserNames(tempBoxUser, _userDAL, classTimes.Teachers),
                DefaultClassTimes = etClass.EtClass.DefaultClassTimes
            });
        }

        public async Task<ResponseBase> ClassTimesClassStudentGet(ClassTimesClassStudentGetRequest request)
        {
            var etClass = await _classDAL.GetClassBucket(request.CId);
            if (etClass == null || etClass.EtClass == null)
            {
                return ResponseBase.CommonError("班级不存在");
            }
            var classStudent = etClass.EtClassStudents;
            var output = new List<ClassTimesStudentGetOutput>();
            if (classStudent != null && classStudent.Any())
            {
                foreach (var cMyStudent in classStudent)
                {
                    var classTimesStudent = await GetClassTimesStudent(cMyStudent.ClassId, cMyStudent.StudentId,
                        cMyStudent.CourseId, EmClassStudentType.ClassStudent, 0, 0, null, etClass.EtClass.DefaultClassTimes);
                    if (classTimesStudent != null)
                    {
                        output.Add(classTimesStudent);
                    }
                }
            }
            return ResponseBase.Success(output);
        }

        public async Task<ResponseBase> ClassTimesStudentGet(ClassTimesStudentGetRequest request)
        {
            var classTimes = await _classTimesDAL.GetClassTimes(request.CId);
            if (classTimes == null)
            {
                return ResponseBase.CommonError("课次不存在");
            }
            var etClass = await _classDAL.GetClassBucket(classTimes.ClassId);
            if (etClass == null || etClass.EtClass == null)
            {
                return ResponseBase.CommonError("课次所在班级不存在");
            }
            var classStudent = etClass.EtClassStudents;
            var tempStudent = await _classTimesDAL.GetClassTimesStudent(request.CId);
            var output = new List<ClassTimesStudentGetOutput>();
            if (classStudent != null && classStudent.Any())
            {
                foreach (var cMyStudent in classStudent)
                {
                    var classTimesStudent = await GetClassTimesStudent(cMyStudent.ClassId, cMyStudent.StudentId,
                        cMyStudent.CourseId, EmClassStudentType.ClassStudent, classTimes.Id, 0, null, etClass.EtClass.DefaultClassTimes);
                    if (classTimesStudent != null)
                    {
                        output.Add(classTimesStudent);
                    }
                }
            }
            if (tempStudent != null && tempStudent.Any())
            {
                foreach (var tMyStudent in tempStudent)
                {
                    var tempTimesStudent = await GetClassTimesStudent(tMyStudent.ClassId, tMyStudent.StudentId, tMyStudent.CourseId,
                        tMyStudent.StudentType, tMyStudent.ClassTimesId, tMyStudent.Id, tMyStudent.StudentTryCalssLogId, etClass.EtClass.DefaultClassTimes);
                    if (tempTimesStudent != null)
                    {
                        output.Add(tempTimesStudent);
                    }
                }
            }
            return ResponseBase.Success(output);
        }

        private async Task<ClassTimesStudentGetOutput> GetClassTimesStudent(long classId, long studentId, long courseId, byte studentType,
            long classTimesId, long classTimesStudentId, long? studentTryCalssLogId, int defaultClassTimes)
        {
            var myStudent = await _studentDAL.GetStudent(studentId);
            if (myStudent == null)
            {
                return null;
            }
            var myCourse = await _courseDAL.GetCourse(courseId);
            if (myCourse == null || myCourse.Item1 == null)
            {
                return null;
            }
            var studentCourse = await _studentCourseDAL.GetStudentCourse(studentId, courseId);
            return new ClassTimesStudentGetOutput()
            {
                CourseId = courseId,
                ClassId = classId,
                CourseName = myCourse.Item1.Name,
                Gender = myStudent.Student.Gender,
                GenderDesc = EmGender.GetGenderDesc(myStudent.Student.Gender),
                StudentId = studentId,
                StudentName = myStudent.Student.Name,
                StudentPhone = myStudent.Student.Phone,
                CourseSurplusDesc = ComBusiness.GetStudentCourseDesc(studentCourse),
                StudentType = studentType,
                ClassTimesId = classTimesId,
                ClassTimesStudentId = classTimesStudentId,
                StudentTryCalssLogId = studentTryCalssLogId,
                StudentTypeDesc = EmClassStudentType.GetClassStudentTypeDesc(studentType),
                DefaultClassTimes = defaultClassTimes
            };
        }

        public async Task<ResponseBase> ClassTimesGetEditView(ClassTimesGetEditViewRequest request)
        {
            var classTimes = await _classTimesDAL.GetClassTimes(request.CId);
            if (classTimes == null)
            {
                return ResponseBase.CommonError("课次不存在");
            }
            var etClass = await _classDAL.GetClassBucket(classTimes.ClassId);
            if (etClass == null || etClass.EtClass == null)
            {
                return ResponseBase.CommonError("课次所在班级不存在");
            }
            var output = new ClassTimesGetEditViewOutput()
            {
                CId = classTimes.Id,
                ClassContent = classTimes.ClassContent,
                ClassId = classTimes.ClassId,
                ClassOt = classTimes.ClassOt.EtmsToDateString(),
                ClassRoomIds = EtmsHelper.AnalyzeMuIds(classTimes.ClassRoomIds),
                CourseIds = EtmsHelper.AnalyzeMuIds(classTimes.CourseList),
                StartTimeDesc = EtmsHelper.GetTimeDesc(classTimes.StartTime),
                EndTimeDesc = EtmsHelper.GetTimeDesc(classTimes.EndTime),
                TeacherIds = EtmsHelper.AnalyzeMuIds(classTimes.Teachers)
            };
            return ResponseBase.Success(output);
        }

        public async Task<ResponseBase> ClassTimesEdit(ClassTimesEditRequest request)
        {
            var classTimes = await _classTimesDAL.GetClassTimes(request.ClassTimesId);
            if (classTimes == null)
            {
                return ResponseBase.CommonError("课次不存在");
            }
            if (classTimes.Status == EmClassTimesStatus.BeRollcall)
            {
                return ResponseBase.CommonError("已点名无法修改");
            }
            var etClass = await _classDAL.GetClassBucket(classTimes.ClassId);
            if (etClass == null || etClass.EtClass == null)
            {
                return ResponseBase.CommonError("课次所在班级不存在");
            }
            if (await _classTimesDAL.ExistClassTimes(request.ClassOt, request.StartTime, request.EndTime, classTimes.ClassId, classTimes.Id))
            {
                return ResponseBase.CommonError("上课时间与其他课次有重叠，请重新设置");
            }
            classTimes.ClassOt = request.ClassOt;
            classTimes.Week = (byte)request.ClassOt.DayOfWeek;
            classTimes.StartTime = request.StartTime;
            classTimes.EndTime = request.EndTime;
            classTimes.ClassContent = request.ClassContent;
            classTimes.TeachersIsAlone = true;
            classTimes.Teachers = EtmsHelper.GetMuIds(request.TeacherIds);
            classTimes.TeacherNum = request.TeacherIds == null || !request.TeacherIds.Any() ? 0 : request.TeacherIds.Count;
            classTimes.CourseListIsAlone = true;
            classTimes.CourseList = EtmsHelper.GetMuIds(request.CourseIds);
            classTimes.ClassRoomIdsIsAlone = true;
            classTimes.ClassRoomIds = EtmsHelper.GetMuIds(request.ClassRoomIds);
            await _classTimesDAL.EditClassTimes(classTimes);
            await _classTimesDAL.UpdateClassTimesStudent(classTimes.Id, request.ClassOt);
            await _userOperationLogDAL.AddUserLog(request, $"班级:{etClass.EtClass.Name},编辑课次:{request.ClassOt.EtmsToDateString()}({EtmsHelper.GetTimeDesc(request.StartTime, request.EndTime)})", EmUserOperationType.ClassManage);
            return ResponseBase.Success();
        }

        public async Task<ResponseBase> ClassTimesDel(ClassTimesDelRequest request)
        {
            var classTimes = await _classTimesDAL.GetClassTimes(request.CId);
            if (classTimes == null)
            {
                return ResponseBase.CommonError("课次不存在");
            }
            if (classTimes.Status == EmClassTimesStatus.BeRollcall)
            {
                return ResponseBase.CommonError("已点名无法删除");
            }
            var etClass = await _classDAL.GetClassBucket(classTimes.ClassId);
            if (etClass == null || etClass.EtClass == null)
            {
                return ResponseBase.CommonError("课次所在班级不存在");
            }
            await _classTimesDAL.DelClassTimes(request.CId);
            await _userOperationLogDAL.AddUserLog(request, $"班级:{etClass.EtClass.Name},删除课次:{classTimes.ClassOt.EtmsToDateString()}({EtmsHelper.GetTimeDesc(classTimes.StartTime, classTimes.EndTime)})", EmUserOperationType.ClassManage);
            return ResponseBase.Success();
        }

        public async Task<ResponseBase> ClassTimesAddTempStudent(ClassTimesAddTempStudentRequest request)
        {
            var classTimes = await _classTimesDAL.GetClassTimes(request.CId);
            if (classTimes == null)
            {
                return ResponseBase.CommonError("课次不存在");
            }
            if (classTimes.Status == EmClassTimesStatus.BeRollcall)
            {
                return ResponseBase.CommonError("已点名无法添加学员");
            }
            var etClass = await _classDAL.GetClassBucket(classTimes.ClassId);
            if (etClass == null || etClass.EtClass == null)
            {
                return ResponseBase.CommonError("课次所在班级不存在");
            }
            var courseIds = classTimes.CourseList.Split(',');
            if (courseIds.FirstOrDefault(p => p == request.CourseId.ToString()) == null)
            {
                return ResponseBase.CommonError("请选择此课次关联的课程");
            }
            foreach (var student in request.StudentIds)
            {
                if (!string.IsNullOrEmpty(classTimes.StudentIdsClass) && classTimes.StudentIdsClass.Split(',').FirstOrDefault(p => p == student.Value.ToString()) != null)
                {
                    return ResponseBase.CommonError($"学员[{student.Label}]已存在");
                }
                if (!string.IsNullOrEmpty(classTimes.StudentIdsTemp) && classTimes.StudentIdsTemp.Split(',').FirstOrDefault(p => p == student.Value.ToString()) != null)
                {
                    return ResponseBase.CommonError($"学员[{student.Label}]已存在");
                }
            }
            var addClassTimesStudent = new List<EtClassTimesStudent>();
            var studentIds = new List<long>();
            var studentNames = new List<string>();
            foreach (var student in request.StudentIds)
            {
                studentIds.Add(student.Value);
                studentNames.Add(student.Label);
                addClassTimesStudent.Add(new EtClassTimesStudent()
                {
                    IsDeleted = EmIsDeleted.Normal,
                    ClassId = classTimes.ClassId,
                    ClassOt = classTimes.ClassOt,
                    ClassTimesId = classTimes.Id,
                    CourseId = request.CourseId,
                    Remark = string.Empty,
                    RuleId = classTimes.RuleId,
                    Status = classTimes.Status,
                    StudentId = student.Value,
                    StudentTryCalssLogId = null,
                    StudentType = EmClassStudentType.TempStudent,
                    TenantId = classTimes.TenantId
                });
            }
            _classTimesDAL.AddClassTimesStudent(addClassTimesStudent);

            var strStudent = string.Join(',', studentIds);
            if (string.IsNullOrEmpty(classTimes.StudentIdsTemp))
            {
                classTimes.StudentIdsTemp = $",{strStudent},";
            }
            else
            {
                classTimes.StudentIdsTemp = $"{classTimes.StudentIdsTemp}{strStudent},";
            }
            await _classTimesDAL.EditClassTimes(classTimes);
            var studenName = string.Join(',', studentNames);
            await _userOperationLogDAL.AddUserLog(request, $"班级:{etClass.EtClass.Name},课次:{classTimes.ClassOt.EtmsToDateString()}({EtmsHelper.GetTimeDesc(classTimes.StartTime, classTimes.EndTime)})添加临时学员:{studenName}", EmUserOperationType.ClassManage);
            return ResponseBase.Success();
        }

        public async Task<ResponseBase> ClassTimesAddTryStudent2(ClassTimesAddTryStudent2Request request)
        {
            var classTimes = await _classTimesDAL.GetClassTimes(request.CId);
            var courseIds = classTimes.CourseList.Split(',');
            long courseId = 0;
            foreach (var p in courseIds)
            {
                if (!string.IsNullOrEmpty(p))
                {
                    courseId = p.ToLong();
                    break;
                }
            }
            if (courseId == 0)
            {
                return ResponseBase.CommonError("请先设置课次的授课课程");
            }
            return await ClassTimesAddTryStudent(new ClassTimesAddTryStudentRequest()
            {
                CId = request.CId,
                CourseId = courseId,
                IpAddress = request.IpAddress,
                IsDataLimit = request.IsDataLimit,
                LoginTenantId = request.LoginTenantId,
                LoginUserId = request.LoginUserId,
                StudentId = request.StudentId
            });
        }

        public async Task<ResponseBase> ClassTimesAddTryStudent(ClassTimesAddTryStudentRequest request)
        {
            var classTimes = await _classTimesDAL.GetClassTimes(request.CId);
            if (classTimes == null)
            {
                return ResponseBase.CommonError("课次不存在");
            }
            if (classTimes.Status == EmClassTimesStatus.BeRollcall)
            {
                return ResponseBase.CommonError("已点名无法添加学员");
            }
            var etClass = await _classDAL.GetClassBucket(classTimes.ClassId);
            if (etClass == null || etClass.EtClass == null)
            {
                return ResponseBase.CommonError("课次所在班级不存在");
            }
            var courseIds = classTimes.CourseList.Split(',');
            if (courseIds.FirstOrDefault(p => p == request.CourseId.ToString()) == null)
            {
                return ResponseBase.CommonError("请选择此课次关联的课程");
            }
            if (!string.IsNullOrEmpty(classTimes.StudentIdsClass) && classTimes.StudentIdsClass.Split(',').FirstOrDefault(p => p == request.StudentId.ToString()) != null)
            {
                return ResponseBase.CommonError($"学员已在此课次中");
            }
            if (!string.IsNullOrEmpty(classTimes.StudentIdsTemp) && classTimes.StudentIdsTemp.Split(',').FirstOrDefault(p => p == request.StudentId.ToString()) != null)
            {
                return ResponseBase.CommonError($"学员已在此课次中");
            }

            var myCourse = await _courseDAL.GetCourse(request.CourseId);
            var trylogContent = $"[预约试听] 已预约试听课程:{myCourse.Item1.Name},所在班级:{etClass.EtClass.Name},上课时间:{classTimes.ClassOt.EtmsToDateString()} { EtmsHelper.GetTimeDesc(classTimes.StartTime) }~{ EtmsHelper.GetTimeDesc(classTimes.EndTime) } 周{ EtmsHelper.GetWeekDesc(classTimes.Week) }";
            var trackLog = new EtStudentTrackLog()
            {
                ContentType = EmStudentTrackContentType.ApplyTryClass,
                IsDeleted = EmIsDeleted.Normal,
                NextTrackTime = classTimes.ClassOt,
                TrackTime = DateTime.Now,
                RelatedInfo = null,
                StudentId = request.StudentId,
                TenantId = request.LoginTenantId,
                TrackUserId = request.LoginUserId,
                TrackContent = trylogContent
            };
            await _studentTrackLogDAL.AddStudentTrackLog(trackLog);
            var tryLogId = await _tryCalssLogDAL.AddTryCalssLog(new EtTryCalssLog()
            {
                ClassId = classTimes.ClassId,
                TryOt = classTimes.ClassOt,
                ClassTimesId = classTimes.Id,
                CourseId = request.CourseId,
                IsDeleted = EmIsDeleted.Normal,
                Status = EmTryCalssLogStatus.IsBooked,
                StudentId = request.StudentId,
                TenantId = request.LoginTenantId,
                UserId = request.LoginUserId
            });
            await _classTimesDAL.AddClassTimesStudent(new EtClassTimesStudent()
            {
                IsDeleted = EmIsDeleted.Normal,
                ClassId = classTimes.ClassId,
                ClassOt = classTimes.ClassOt,
                ClassTimesId = classTimes.Id,
                CourseId = request.CourseId,
                Remark = string.Empty,
                RuleId = classTimes.RuleId,
                Status = classTimes.Status,
                StudentId = request.StudentId,
                StudentTryCalssLogId = tryLogId,
                StudentType = EmClassStudentType.TryCalssStudent,
                TenantId = classTimes.TenantId,
            });
            if (string.IsNullOrEmpty(classTimes.StudentIdsTemp))
            {
                classTimes.StudentIdsTemp = $",{request.StudentId},";
            }
            else
            {
                classTimes.StudentIdsTemp = $"{classTimes.StudentIdsTemp}{request.StudentId},";
            }
            await _classTimesDAL.EditClassTimes(classTimes);
            await _userOperationLogDAL.AddUserLog(request, $"班级:{etClass.EtClass.Name},课次:{classTimes.ClassOt.EtmsToDateString()}({EtmsHelper.GetTimeDesc(classTimes.StartTime, classTimes.EndTime)})添加试听学员", EmUserOperationType.ClassManage);
            return ResponseBase.Success();
        }

        public async Task<ResponseBase> ClassTimesAddTryStudentOneToOne(ClassTimesAddTryStudentOneToOneRequest request)
        {
            var student = await _studentDAL.GetStudent(request.StudentId);
            if (student == null || student.Student == null)
            {
                return ResponseBase.Success("不存在此学生");
            }
            var log = await _classTimesDAL.GetClassTimesTryStudent(request.StudentId, request.CourseId, request.ClassOt.Value);
            if (log != null)
            {
                return ResponseBase.Success("每天每个课程只能安排一节试听课");
            }
            var rooms = EtmsHelper.GetMuIds(request.ClassRoomIds);
            var courseIds = $",{request.CourseId},";
            var students = $",{request.StudentId},";
            var teachers = $",{request.TeacherId},";
            var etClass = new EtClass()
            {
                ClassCategoryId = null,
                ClassRoomIds = rooms,
                CompleteStatus = EmClassCompleteStatus.UnComplete,
                CompleteTime = null,
                CourseList = courseIds,
                DataType = EmClassDataType.Temp,
                DefaultClassTimes = 1,
                FinishClassTimes = 0,
                FinishCount = 0,
                IsDeleted = EmIsDeleted.Normal,
                IsLeaveCharge = false,
                IsNotComeCharge = false,
                LastJobProcessTime = DateTime.Now,
                LimitStudentNums = null,
                Name = $"一对一试听_{student.Student.Name}",
                OrderId = null,
                Ot = DateTime.Now,
                PlanCount = 1,
                Remark = string.Empty,
                ScheduleStatus = EmClassScheduleStatus.Scheduled,
                StudentIds = students,
                StudentNums = 1,
                TeacherNum = 1,
                Teachers = teachers,
                TenantId = request.LoginTenantId,
                Type = EmClassType.OneToOne,
                UserId = request.LoginUserId
            };
            var classId = await _classDAL.AddClass(etClass);
            var classTimes = new EtClassTimes()
            {
                ClassContent = request.ClassContent,
                ClassId = classId,
                ClassOt = request.ClassOt.Value,
                ClassRecordId = null,
                ClassRoomIds = rooms,
                ClassRoomIdsIsAlone = true,
                CourseList = courseIds,
                CourseListIsAlone = true,
                EndTime = request.EndTime,
                IsDeleted = EmIsDeleted.Normal,
                RuleId = 0,
                StartTime = request.StartTime,
                Status = EmClassTimesStatus.UnRollcall,
                StudentIdsClass = string.Empty,
                StudentIdsTemp = students,
                TeacherNum = 1,
                Teachers = teachers,
                TeachersIsAlone = true,
                TenantId = request.LoginTenantId,
                Week = (byte)request.ClassOt.Value.DayOfWeek
            };
            var classTimesId = await this._classTimesDAL.AddClassTimes(classTimes);
            classTimes.Id = classTimesId;
            var myCourse = await _courseDAL.GetCourse(request.CourseId);
            var trylogContent = $"[预约试听] 已预约一对一试听课程:{myCourse.Item1.Name},上课时间:{classTimes.ClassOt.EtmsToDateString()} { EtmsHelper.GetTimeDesc(classTimes.StartTime) }~{ EtmsHelper.GetTimeDesc(classTimes.EndTime) } 周{ EtmsHelper.GetWeekDesc(classTimes.Week) }";
            var trackLog = new EtStudentTrackLog()
            {
                ContentType = EmStudentTrackContentType.ApplyTryClass,
                IsDeleted = EmIsDeleted.Normal,
                NextTrackTime = classTimes.ClassOt,
                TrackTime = DateTime.Now,
                RelatedInfo = null,
                StudentId = request.StudentId,
                TenantId = request.LoginTenantId,
                TrackUserId = request.LoginUserId,
                TrackContent = trylogContent
            };
            await _studentTrackLogDAL.AddStudentTrackLog(trackLog);
            var tryLogId = await _tryCalssLogDAL.AddTryCalssLog(new EtTryCalssLog()
            {
                ClassId = classTimes.ClassId,
                TryOt = classTimes.ClassOt,
                ClassTimesId = classTimes.Id,
                CourseId = request.CourseId,
                IsDeleted = EmIsDeleted.Normal,
                Status = EmTryCalssLogStatus.IsBooked,
                StudentId = request.StudentId,
                TenantId = request.LoginTenantId,
                UserId = request.LoginUserId
            });
            await _classTimesDAL.AddClassTimesStudent(new EtClassTimesStudent()
            {
                IsDeleted = EmIsDeleted.Normal,
                ClassId = classTimes.ClassId,
                ClassOt = classTimes.ClassOt,
                ClassTimesId = classTimes.Id,
                CourseId = request.CourseId,
                Remark = string.Empty,
                RuleId = classTimes.RuleId,
                Status = classTimes.Status,
                StudentId = request.StudentId,
                StudentTryCalssLogId = tryLogId,
                StudentType = EmClassStudentType.TryCalssStudent,
                TenantId = classTimes.TenantId,
            });
            await _userOperationLogDAL.AddUserLog(request, $"一对一试听,课程:{myCourse.Item1.Name} 课次:{classTimes.ClassOt.EtmsToDateString()}({EtmsHelper.GetTimeDesc(classTimes.StartTime, classTimes.EndTime)})添加试听学员", EmUserOperationType.ClassManage);
            return ResponseBase.Success();
        }

        public async Task<ResponseBase> ClassTimesDelTempOrTryStudent(ClassTimesDelTempOrTryStudentRequest request)
        {
            var classTimes = await _classTimesDAL.GetClassTimes(request.ClassTimesId);
            if (classTimes == null)
            {
                return ResponseBase.CommonError("课次不存在");
            }
            if (classTimes.Status == EmClassTimesStatus.BeRollcall)
            {
                return ResponseBase.CommonError("已点名无法移除学员");
            }
            var etClass = await _classDAL.GetClassBucket(classTimes.ClassId);
            if (etClass == null || etClass.EtClass == null)
            {
                return ResponseBase.CommonError("课次所在班级不存在");
            }
            var classTimesStudent = await _classTimesDAL.GetClassTimesStudentById(request.ClassTimesStudentId);
            if (classTimesStudent.StudentType == EmClassStudentType.TempStudent)
            {
                return await ClassTimesDelTempStudent(request, etClass.EtClass, classTimes);
            }
            if (classTimesStudent.StudentType == EmClassStudentType.TryCalssStudent)
            {
                return await ClassTimesDelTryStudent(request, etClass.EtClass, classTimes, classTimesStudent);
            }
            return ResponseBase.CommonError("无法移除此学员");
        }
        private async Task<ResponseBase> ClassTimesDelTempStudent(ClassTimesDelTempOrTryStudentRequest request, EtClass etClass, EtClassTimes etClassTimes)
        {
            await _classTimesDAL.DelClassTimesStudent(request.ClassTimesStudentId);
            var classTimesStudents = await _classTimesDAL.GetClassTimesStudent(request.ClassTimesId);
            var studentIdsTemps = string.Empty;
            if (classTimesStudents != null && classTimesStudents.Any())
            {
                studentIdsTemps = EtmsHelper.GetMuIds(classTimesStudents.Select(p => p.StudentId));
            }
            etClassTimes.StudentIdsTemp = studentIdsTemps;
            await _classTimesDAL.EditClassTimes(etClassTimes);
            await _userOperationLogDAL.AddUserLog(request, $"班级:{etClass.Name},课次:{etClassTimes.ClassOt.EtmsToDateString()}({EtmsHelper.GetTimeDesc(etClassTimes.StartTime, etClassTimes.EndTime)})移除临时学员:{request.StudentName}", EmUserOperationType.ClassManage);
            return ResponseBase.Success();
        }

        public async Task<ResponseBase> ClassTimesDelTryStudent(ClassTimesDelTempOrTryStudentRequest request,
            EtClass etClass, EtClassTimes etClassTimes, EtClassTimesStudent etClassTimesStudent)
        {
            var myCourse = await _courseDAL.GetCourse(etClassTimesStudent.CourseId);
            await _classTimesDAL.DelClassTimesStudent(request.ClassTimesStudentId);
            var classTimesStudents = await _classTimesDAL.GetClassTimesStudent(request.ClassTimesId);
            var studentIdsTemps = string.Empty;
            if (classTimesStudents != null && classTimesStudents.Any())
            {
                studentIdsTemps = EtmsHelper.GetMuIds(classTimesStudents.Select(p => p.StudentId));
            }
            if (etClass.DataType == EmClassDataType.Temp && string.IsNullOrEmpty(studentIdsTemps))
            {
                //试听课程，如果没有学员了，则删除数据
                await _classDAL.DelClass(etClassTimes.ClassId);
                await _classTimesDAL.DelClassTimes(etClassTimes.Id);
            }
            else
            {
                etClassTimes.StudentIdsTemp = studentIdsTemps;
                await _classTimesDAL.EditClassTimes(etClassTimes);
            }
            var trackLog = new EtStudentTrackLog()
            {
                ContentType = EmStudentTrackContentType.CancelApplyTryClass,
                IsDeleted = EmIsDeleted.Normal,
                NextTrackTime = null,
                TrackTime = DateTime.Now,
                RelatedInfo = null,
                StudentId = etClassTimesStudent.StudentId,
                TenantId = request.LoginTenantId,
                TrackUserId = request.LoginUserId,
                TrackContent = $"[取消试听] 课程:{myCourse.Item1.Name},所在班级:{etClass.Name}"
            };
            await _studentTrackLogDAL.AddStudentTrackLog(trackLog);
            if (etClassTimesStudent.StudentTryCalssLogId != null)
            {
                await _tryCalssLogDAL.UpdateStatus(etClassTimesStudent.StudentTryCalssLogId.Value, EmTryCalssLogStatus.IsCancel);
            }
            await _userOperationLogDAL.AddUserLog(request, $"班级:{etClass.Name},课次:{etClassTimes.ClassOt.EtmsToDateString()}({EtmsHelper.GetTimeDesc(etClassTimes.StartTime, etClassTimes.EndTime)})移除试听学员:{request.StudentName}", EmUserOperationType.ClassManage);
            return ResponseBase.Success();
        }

        public async Task<ResponseBase> ClassTimesGetPaging(ClassTimesGetPagingRequest request)
        {
            var pagingData = await _classTimesDAL.GetPaging(request);
            var output = new List<ClassTimesGetPagingOutput>();
            List<EtClassRoom> allClassRoom = null;
            if (request.IsGetComplexInfo)
            {
                allClassRoom = await _classRoomDAL.GetAllClassRoom();
            }

            var tempBoxCourse = new DataTempBox<EtCourse>();
            var tempBoxUser = new DataTempBox<EtUser>();
            foreach (var classTimes in pagingData.Item1)
            {
                var classRoomIdsDesc = string.Empty;
                var courseListDesc = string.Empty;
                var courseStyleColor = string.Empty;
                var className = string.Empty;
                var teachersDesc = string.Empty;
                if (request.IsGetComplexInfo)
                {
                    var etClass = await _classDAL.GetClassBucket(classTimes.ClassId);
                    var courseInfo = await ComBusiness.GetCourseNameAndColor(tempBoxCourse, _courseDAL, classTimes.CourseList);
                    classRoomIdsDesc = ComBusiness.GetDesc(allClassRoom, classTimes.ClassRoomIds);
                    className = etClass.EtClass.Name;
                    courseListDesc = courseInfo.Item1;
                    courseStyleColor = courseInfo.Item2;
                    teachersDesc = await ComBusiness.GetUserNames(tempBoxUser, _userDAL, classTimes.Teachers);
                }
                output.Add(new ClassTimesGetPagingOutput()
                {
                    CId = classTimes.Id,
                    ClassContent = classTimes.ClassContent,
                    ClassId = classTimes.ClassId,
                    ClassName = className,
                    ClassOt = classTimes.ClassOt.EtmsToDateString(),
                    ClassRoomIds = classTimes.ClassRoomIds,
                    ClassRoomIdsDesc = classRoomIdsDesc,
                    CourseList = classTimes.CourseList,
                    CourseListDesc = courseListDesc,
                    CourseStyleColor = courseStyleColor,
                    EndTime = classTimes.EndTime,
                    StartTime = classTimes.StartTime,
                    Status = classTimes.Status,
                    TimeDesc = $"{EtmsHelper.GetTimeDesc(classTimes.StartTime)}~{EtmsHelper.GetTimeDesc(classTimes.EndTime)}",
                    Week = classTimes.Week,
                    WeekDesc = $"周{EtmsHelper.GetWeekDesc(classTimes.Week)}",
                    TeacherNum = classTimes.TeacherNum,
                    Teachers = classTimes.Teachers,
                    TeachersDesc = teachersDesc
                });
            }
            return ResponseBase.Success(new ResponsePagingDataBase<ClassTimesGetPagingOutput>(pagingData.Item2, output));
        }

        public async Task<ResponseBase> ClassTimesGetOfWeekTime(ClassTimesGetOfWeekTimeRequest request)
        {
            if (request.StartOt == null || request.StartOt.Value.DayOfWeek != DayOfWeek.Monday)
            {
                return ResponseBase.CommonError("请求时间格式错误");
            }
            return await ClassTimestableMgrGet(request);
        }

        public async Task<ResponseBase> ClassTimesCancelTryClassStudent(ClassTimesCancelTryClassStudentRequest request)
        {
            var classTimesStudent = await _classTimesDAL.GetClassTimesStudent(request.ClassTimesId, request.StudentTryCalssLogId);
            if (classTimesStudent == null)
            {
                return ResponseBase.CommonError("试听记录不存在");
            }
            var responseResult = await ClassTimesDelTempOrTryStudent(new ClassTimesDelTempOrTryStudentRequest()
            {
                ClassTimesId = request.ClassTimesId,
                ClassTimesStudentId = classTimesStudent.Id,
                IpAddress = request.IpAddress,
                LoginTenantId = request.LoginTenantId,
                LoginUserId = request.LoginUserId,
                StudentName = request.StudentName
            });
            if (!responseResult.IsResponseSuccess())
            {
                return responseResult;
            }
            await _tryCalssLogDAL.UpdateStatus(request.StudentTryCalssLogId, EmTryCalssLogStatus.IsCancel);
            await _userOperationLogDAL.AddUserLog(request, $"取消试听,学员:{request.StudentName}", EmUserOperationType.CancelTryClassStudent);
            return ResponseBase.Success();
        }

        public async Task<ResponseBase> ClassTimesGetMyWeek(ClassTimesGetMyRequest request)
        {
            request.TeacherId = request.LoginUserId;
            return await ClassTimestableMgrGet(request);
        }

        public async Task<ResponseBase> ClassTimesGetMyOt(ClassTimesGetMyOtRequest request)
        {
            request.TeacherId = request.LoginUserId;
            var classTimesData = (await _classTimesDAL.GetList(request)).OrderByDescending(p => p.ClassOt).ThenBy(p => p.StartTime);
            var output = new List<ClassTimesGetOfWeekTimeItem>();
            if (classTimesData != null && classTimesData.Any())
            {
                var allClassRoom = await _classRoomDAL.GetAllClassRoom();
                var tempBoxCourse = new DataTempBox<EtCourse>();
                var tempBoxUser = new DataTempBox<EtUser>();
                var tempClass = new DataTempBox<EtClass>();
                foreach (var classTimes in classTimesData)
                {
                    var classRoomIdsDesc = string.Empty;
                    var courseListDesc = string.Empty;
                    var courseStyleColor = string.Empty;
                    var className = string.Empty;
                    var teachersDesc = string.Empty;
                    var etClass = await ComBusiness.GetClass(tempClass, _classDAL, classTimes.ClassId);
                    var courseInfo = await ComBusiness.GetCourseNameAndColor(tempBoxCourse, _courseDAL, classTimes.CourseList);
                    classRoomIdsDesc = ComBusiness.GetDesc(allClassRoom, classTimes.ClassRoomIds);
                    className = etClass.Name;
                    courseListDesc = courseInfo.Item1;
                    courseStyleColor = courseInfo.Item2;
                    teachersDesc = await ComBusiness.GetUserNames(tempBoxUser, _userDAL, classTimes.Teachers);
                    if (string.IsNullOrEmpty(courseStyleColor))
                    {
                        courseStyleColor = "#1E90FF";
                    }
                    if (classTimes.Status == EmClassTimesStatus.BeRollcall)
                    {
                        courseStyleColor = "#C0C4CC";
                    }
                    var classTimesDesc = string.Empty;
                    if (string.IsNullOrEmpty(teachersDesc))
                    {
                        classTimesDesc = className;
                    }
                    else
                    {
                        classTimesDesc = $"{className}({teachersDesc})";
                    }
                    var temp = new ClassTimesGetOfWeekTimeItem()
                    {
                        CId = classTimes.Id,
                        ClassContent = classTimes.ClassContent,
                        ClassId = classTimes.ClassId,
                        ClassName = className,
                        ClassOt = classTimes.ClassOt.EtmsToDateString(),
                        ClassRoomIds = classTimes.ClassRoomIds,
                        ClassRoomIdsDesc = classRoomIdsDesc,
                        CourseList = classTimes.CourseList,
                        CourseListDesc = courseListDesc,
                        ClassTimesColor = courseStyleColor,
                        EndTime = EtmsHelper.GetTimeDesc(classTimes.EndTime),
                        StartTime = EtmsHelper.GetTimeDesc(classTimes.StartTime),
                        Startop = EtmsHelper.GetTimeDesc(classTimes.StartTime),
                        Status = classTimes.Status,
                        Week = classTimes.Week,
                        WeekDesc = $"周{EtmsHelper.GetWeekDesc(classTimes.Week)}",
                        TimeDesc = $"{EtmsHelper.GetTimeDesc(classTimes.StartTime)}~{EtmsHelper.GetTimeDesc(classTimes.EndTime)}",
                        TeacherNum = classTimes.TeacherNum,
                        Teachers = classTimes.Teachers,
                        TeachersDesc = teachersDesc,
                        Duration = EtmsHelper.GetTimeDuration(classTimes.StartTime, classTimes.EndTime),
                        ClassTimesDesc = classTimesDesc,
                        DefaultClassTimes = etClass.DefaultClassTimes
                    };
                    output.Add(temp);
                }
            }
            return ResponseBase.Success(output);
        }

        private async Task<ResponseBase> ClassTimestableMgrGet(RequestBase request)
        {
            var classTimesData = (await _classTimesDAL.GetList(request)).OrderBy(p => p.StartTime);
            var output = new ClassTimesGetOfWeekTimeOutput()
            {
                FridayList = new List<ClassTimesGetOfWeekTimeItem>(),
                MondyList = new List<ClassTimesGetOfWeekTimeItem>(),
                SaturdayList = new List<ClassTimesGetOfWeekTimeItem>(),
                SundayList = new List<ClassTimesGetOfWeekTimeItem>(),
                ThursdayList = new List<ClassTimesGetOfWeekTimeItem>(),
                TuesdayList = new List<ClassTimesGetOfWeekTimeItem>(),
                WednesdayList = new List<ClassTimesGetOfWeekTimeItem>()
            };
            if (classTimesData != null && classTimesData.Any())
            {
                var allClassRoom = await _classRoomDAL.GetAllClassRoom();
                var tempBoxCourse = new DataTempBox<EtCourse>();
                var tempBoxUser = new DataTempBox<EtUser>();
                var tempClass = new DataTempBox<EtClass>();
                foreach (var classTimes in classTimesData)
                {
                    var classRoomIdsDesc = string.Empty;
                    var courseListDesc = string.Empty;
                    var courseStyleColor = string.Empty;
                    var className = string.Empty;
                    var teachersDesc = string.Empty;
                    var etClass = await ComBusiness.GetClass(tempClass, _classDAL, classTimes.ClassId);
                    var courseInfo = await ComBusiness.GetCourseNameAndColor(tempBoxCourse, _courseDAL, classTimes.CourseList);
                    classRoomIdsDesc = ComBusiness.GetDesc(allClassRoom, classTimes.ClassRoomIds);
                    className = etClass.Name;
                    courseListDesc = courseInfo.Item1;
                    courseStyleColor = courseInfo.Item2;
                    teachersDesc = await ComBusiness.GetUserNames(tempBoxUser, _userDAL, classTimes.Teachers);
                    if (string.IsNullOrEmpty(courseStyleColor))
                    {
                        courseStyleColor = "#1E90FF";
                    }
                    if (classTimes.Status == EmClassTimesStatus.BeRollcall)
                    {
                        courseStyleColor = "#C0C4CC";
                    }
                    var classTimesDesc = string.Empty;
                    if (string.IsNullOrEmpty(teachersDesc))
                    {
                        classTimesDesc = className;
                    }
                    else
                    {
                        classTimesDesc = $"{className}({teachersDesc})";
                    }
                    var temp = new ClassTimesGetOfWeekTimeItem()
                    {
                        CId = classTimes.Id,
                        ClassContent = classTimes.ClassContent,
                        ClassId = classTimes.ClassId,
                        ClassName = className,
                        ClassOt = classTimes.ClassOt.EtmsToDateString(),
                        ClassRoomIds = classTimes.ClassRoomIds,
                        ClassRoomIdsDesc = classRoomIdsDesc,
                        CourseList = classTimes.CourseList,
                        CourseListDesc = courseListDesc,
                        ClassTimesColor = courseStyleColor,
                        EndTime = EtmsHelper.GetTimeDesc(classTimes.EndTime),
                        StartTime = EtmsHelper.GetTimeDesc(classTimes.StartTime),
                        Startop = EtmsHelper.GetTimeDesc(classTimes.StartTime),
                        Status = classTimes.Status,
                        Week = classTimes.Week,
                        WeekDesc = $"周{EtmsHelper.GetWeekDesc(classTimes.Week)}",
                        TimeDesc = $"{EtmsHelper.GetTimeDesc(classTimes.StartTime)}~{EtmsHelper.GetTimeDesc(classTimes.EndTime)}",
                        TeacherNum = classTimes.TeacherNum,
                        Teachers = classTimes.Teachers,
                        TeachersDesc = teachersDesc,
                        Duration = EtmsHelper.GetTimeDuration(classTimes.StartTime, classTimes.EndTime),
                        ClassTimesDesc = classTimesDesc,
                        DefaultClassTimes = etClass.DefaultClassTimes
                    };
                    switch (classTimes.ClassOt.DayOfWeek)
                    {
                        case DayOfWeek.Monday:
                            output.MondyList.Add(temp);
                            break;
                        case DayOfWeek.Tuesday:
                            output.TuesdayList.Add(temp);
                            break;
                        case DayOfWeek.Wednesday:
                            output.WednesdayList.Add(temp);
                            break;
                        case DayOfWeek.Thursday:
                            output.ThursdayList.Add(temp);
                            break;
                        case DayOfWeek.Friday:
                            output.FridayList.Add(temp);
                            break;
                        case DayOfWeek.Saturday:
                            output.SaturdayList.Add(temp);
                            break;
                        case DayOfWeek.Sunday:
                            output.SundayList.Add(temp);
                            break;
                    }
                }
            }
            return ResponseBase.Success(output);
        }

        public async Task<ResponseBase> ClassTimesGetOfWeekTimeTeacher(ClassTimesGetOfWeekTimeTeacherRequest request)
        {
            if (request.StartOt == null || request.StartOt.Value.DayOfWeek != DayOfWeek.Monday)
            {
                return ResponseBase.CommonError("请求时间格式错误");
            }
            var classTimesData = await _classTimesDAL.GetList(request);
            var outPut = new List<ClassTimesGetOfWeekTime2Output>();
            if (classTimesData.Any())
            {
                var strTeacherAllIds = string.Join(null, classTimesData.Select(p => p.Teachers));
                var arrTeacherAllIds = strTeacherAllIds.Split(',');
                var preTeacherIds = new List<long>();
                foreach (var strId in arrTeacherAllIds)
                {
                    if (string.IsNullOrEmpty(strId))
                    {
                        continue;
                    }
                    preTeacherIds.Add(strId.ToLong());
                }
                var teacherIds = preTeacherIds.Distinct();
                var tempBoxCourse = new DataTempBox<EtCourse>();
                var tempBoxClass = new DataTempBox<EtClass>();
                var allClassRoom = await _classRoomDAL.GetAllClassRoom();
                var tempBoxUser = new DataTempBox<EtUser>();
                foreach (var teacherId in teacherIds)
                {
                    var item = new ClassTimesGetOfWeekTime2Output();
                    var teacher = await ComBusiness.GetUser(tempBoxUser, _userDAL, teacherId);
                    if (teacher == null)
                    {
                        continue;
                    }
                    item.Name = teacher.Name;
                    var myTeacherClassTimes = classTimesData.Where(p => !string.IsNullOrEmpty(p.Teachers) && p.Teachers.IndexOf($",{teacherId},") != -1).OrderBy(p => p.StartTime);
                    item.TotalCount = myTeacherClassTimes.Count();

                    var myProcessedClassTimes = new List<ClassTimesGetOfWeekTimeTeacherItems>();
                    foreach (var classTimes in myTeacherClassTimes)
                    {
                        var classRoomIdsDesc = string.Empty;
                        var courseListDesc = string.Empty;
                        var courseStyleColor = string.Empty;
                        var className = string.Empty;
                        var teachersDesc = string.Empty;
                        var etClass = await ComBusiness.GetClass(tempBoxClass, _classDAL, classTimes.ClassId);
                        var courseInfo = await ComBusiness.GetCourseNameAndColor(tempBoxCourse, _courseDAL, classTimes.CourseList);
                        classRoomIdsDesc = ComBusiness.GetDesc(allClassRoom, classTimes.ClassRoomIds);
                        className = etClass.Name;
                        courseListDesc = courseInfo.Item1;
                        courseStyleColor = courseInfo.Item2;
                        teachersDesc = await ComBusiness.GetUserNames(tempBoxUser, _userDAL, classTimes.Teachers);
                        if (string.IsNullOrEmpty(courseStyleColor))
                        {
                            courseStyleColor = "#1E90FF";
                        }
                        if (classTimes.Status == EmClassTimesStatus.BeRollcall)
                        {
                            courseStyleColor = "#C0C4CC";
                        }
                        var classTimesDesc = string.Empty;
                        if (string.IsNullOrEmpty(teachersDesc))
                        {
                            classTimesDesc = className;
                        }
                        else
                        {
                            classTimesDesc = $"{className}({teachersDesc})";
                        }
                        var temp = new ClassTimesGetOfWeekTimeTeacherItems()
                        {
                            CId = classTimes.Id,
                            ClassContent = classTimes.ClassContent,
                            ClassId = classTimes.ClassId,
                            ClassName = className,
                            ClassOt = classTimes.ClassOt.EtmsToDateString(),
                            ClassRoomIds = classTimes.ClassRoomIds,
                            ClassRoomIdsDesc = classRoomIdsDesc,
                            CourseList = classTimes.CourseList,
                            CourseListDesc = courseListDesc,
                            ClassTimesColor = courseStyleColor,
                            EndTime = EtmsHelper.GetTimeDesc(classTimes.EndTime),
                            StartTime = EtmsHelper.GetTimeDesc(classTimes.StartTime),
                            Startop = EtmsHelper.GetTimeDesc(classTimes.StartTime),
                            Status = classTimes.Status,
                            Week = classTimes.Week,
                            WeekDesc = $"周{EtmsHelper.GetWeekDesc(classTimes.Week)}",
                            TimeDesc = $"{EtmsHelper.GetTimeDesc(classTimes.StartTime)}~{EtmsHelper.GetTimeDesc(classTimes.EndTime)}",
                            TeacherNum = classTimes.TeacherNum,
                            Teachers = classTimes.Teachers,
                            TeachersDesc = teachersDesc,
                            Duration = EtmsHelper.GetTimeDuration(classTimes.StartTime, classTimes.EndTime),
                            ClassTimesDesc = classTimesDesc,
                            DefaultClassTimes = etClass.DefaultClassTimes,
                            Color = courseStyleColor
                        };
                        myProcessedClassTimes.Add(temp);
                    }

                    //周一
                    item.MondyList = myProcessedClassTimes.Where(p => p.Week == (int)DayOfWeek.Monday);

                    //周二
                    item.TuesdayList = myProcessedClassTimes.Where(p => p.Week == (int)DayOfWeek.Tuesday);

                    //周三
                    item.WednesdayList = myProcessedClassTimes.Where(p => p.Week == (int)DayOfWeek.Wednesday);

                    //周四
                    item.ThursdayList = myProcessedClassTimes.Where(p => p.Week == (int)DayOfWeek.Thursday);

                    //周五
                    item.FridayList = myProcessedClassTimes.Where(p => p.Week == (int)DayOfWeek.Friday);

                    //周六
                    item.SaturdayList = myProcessedClassTimes.Where(p => p.Week == (int)DayOfWeek.Saturday);

                    //周天
                    item.SundayList = myProcessedClassTimes.Where(p => p.Week == (int)DayOfWeek.Sunday);

                    outPut.Add(item);
                }
                return ResponseBase.Success(outPut.OrderByDescending(p => p.TotalCount));
            }
            return ResponseBase.Success(outPut);
        }

        public async Task<ResponseBase> ClassTimesGetOfWeekTimeRoom(ClassTimesGetOfWeekTimeRoomRequest request)
        {
            if (request.StartOt == null || request.StartOt.Value.DayOfWeek != DayOfWeek.Monday)
            {
                return ResponseBase.CommonError("请求时间格式错误");
            }
            var classTimesData = await _classTimesDAL.GetList(request);
            var outPut = new List<ClassTimesGetOfWeekTime2Output>();
            if (classTimesData.Any())
            {
                var strRoomAllIds = string.Join(null, classTimesData.Select(p => p.ClassRoomIds));
                var arrRoomAllIds = strRoomAllIds.Split(',');
                var preRoomIds = new List<long>();
                foreach (var strId in arrRoomAllIds)
                {
                    if (string.IsNullOrEmpty(strId))
                    {
                        continue;
                    }
                    preRoomIds.Add(strId.ToLong());
                }
                var roomIds = preRoomIds.Distinct();
                var tempBoxCourse = new DataTempBox<EtCourse>();
                var tempBoxClass = new DataTempBox<EtClass>();
                var allClassRoom = await _classRoomDAL.GetAllClassRoom();
                var tempBoxUser = new DataTempBox<EtUser>();
                foreach (var roomId in roomIds)
                {
                    var item = new ClassTimesGetOfWeekTime2Output();
                    var room = allClassRoom.FirstOrDefault(p => p.Id == roomId);
                    if (room == null)
                    {
                        continue;
                    }
                    item.Name = room.Name;
                    var myRoomClassTimes = classTimesData.Where(p => !string.IsNullOrEmpty(p.ClassRoomIds) && p.ClassRoomIds.IndexOf($",{roomId},") != -1).OrderBy(p => p.StartTime);
                    item.TotalCount = myRoomClassTimes.Count();

                    var myProcessedClassTimes = new List<ClassTimesGetOfWeekTimeTeacherItems>();
                    foreach (var classTimes in myRoomClassTimes)
                    {
                        var classRoomIdsDesc = string.Empty;
                        var courseListDesc = string.Empty;
                        var courseStyleColor = string.Empty;
                        var className = string.Empty;
                        var teachersDesc = string.Empty;
                        var etClass = await ComBusiness.GetClass(tempBoxClass, _classDAL, classTimes.ClassId);
                        var courseInfo = await ComBusiness.GetCourseNameAndColor(tempBoxCourse, _courseDAL, classTimes.CourseList);
                        classRoomIdsDesc = ComBusiness.GetDesc(allClassRoom, classTimes.ClassRoomIds);
                        className = etClass.Name;
                        courseListDesc = courseInfo.Item1;
                        courseStyleColor = courseInfo.Item2;
                        teachersDesc = await ComBusiness.GetUserNames(tempBoxUser, _userDAL, classTimes.Teachers);
                        if (string.IsNullOrEmpty(courseStyleColor))
                        {
                            courseStyleColor = "#1E90FF";
                        }
                        if (classTimes.Status == EmClassTimesStatus.BeRollcall)
                        {
                            courseStyleColor = "#C0C4CC";
                        }
                        var classTimesDesc = string.Empty;
                        if (string.IsNullOrEmpty(teachersDesc))
                        {
                            classTimesDesc = className;
                        }
                        else
                        {
                            classTimesDesc = $"{className}({teachersDesc})";
                        }
                        var temp = new ClassTimesGetOfWeekTimeTeacherItems()
                        {
                            CId = classTimes.Id,
                            ClassContent = classTimes.ClassContent,
                            ClassId = classTimes.ClassId,
                            ClassName = className,
                            ClassOt = classTimes.ClassOt.EtmsToDateString(),
                            ClassRoomIds = classTimes.ClassRoomIds,
                            ClassRoomIdsDesc = classRoomIdsDesc,
                            CourseList = classTimes.CourseList,
                            CourseListDesc = courseListDesc,
                            ClassTimesColor = courseStyleColor,
                            EndTime = EtmsHelper.GetTimeDesc(classTimes.EndTime),
                            StartTime = EtmsHelper.GetTimeDesc(classTimes.StartTime),
                            Startop = EtmsHelper.GetTimeDesc(classTimes.StartTime),
                            Status = classTimes.Status,
                            Week = classTimes.Week,
                            WeekDesc = $"周{EtmsHelper.GetWeekDesc(classTimes.Week)}",
                            TimeDesc = $"{EtmsHelper.GetTimeDesc(classTimes.StartTime)}~{EtmsHelper.GetTimeDesc(classTimes.EndTime)}",
                            TeacherNum = classTimes.TeacherNum,
                            Teachers = classTimes.Teachers,
                            TeachersDesc = teachersDesc,
                            Duration = EtmsHelper.GetTimeDuration(classTimes.StartTime, classTimes.EndTime),
                            ClassTimesDesc = classTimesDesc,
                            DefaultClassTimes = etClass.DefaultClassTimes,
                            Color = courseStyleColor
                        };
                        myProcessedClassTimes.Add(temp);
                    }

                    //周一
                    item.MondyList = myProcessedClassTimes.Where(p => p.Week == (int)DayOfWeek.Monday);

                    //周二
                    item.TuesdayList = myProcessedClassTimes.Where(p => p.Week == (int)DayOfWeek.Tuesday);

                    //周三
                    item.WednesdayList = myProcessedClassTimes.Where(p => p.Week == (int)DayOfWeek.Wednesday);

                    //周四
                    item.ThursdayList = myProcessedClassTimes.Where(p => p.Week == (int)DayOfWeek.Thursday);

                    //周五
                    item.FridayList = myProcessedClassTimes.Where(p => p.Week == (int)DayOfWeek.Friday);

                    //周六
                    item.SaturdayList = myProcessedClassTimes.Where(p => p.Week == (int)DayOfWeek.Saturday);

                    //周天
                    item.SundayList = myProcessedClassTimes.Where(p => p.Week == (int)DayOfWeek.Sunday);

                    outPut.Add(item);
                }
                return ResponseBase.Success(outPut.OrderByDescending(p => p.TotalCount));
            }
            return ResponseBase.Success(outPut);
        }

    }
}
