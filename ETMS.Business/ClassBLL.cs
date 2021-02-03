using ETMS.Entity.Common;
using ETMS.Entity.Database.Source;
using ETMS.Entity.Dto.Educational.Request;
using ETMS.Entity.Enum;
using ETMS.IBusiness;
using ETMS.IDataAccess;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using ETMS.Entity.Dto.Educational.Output;
using ETMS.Utility;
using ETMS.Entity.CacheBucket;
using ETMS.Entity.CacheBucket.RedisLock;
using ETMS.Entity.Config;
using ETMS.Entity.Temp;
using ETMS.IEventProvider;
using ETMS.Event.DataContract;
using ETMS.Business.Common;
using ETMS.Entity.Dto.Common.Request;
using ETMS.Entity.Dto.Student.Request;
using ETMS.Entity.Dto.Common.Output;

namespace ETMS.Business
{
    public class ClassBLL : IClassBLL
    {
        private readonly IClassDAL _classDAL;

        private readonly IUserOperationLogDAL _userOperationLogDAL;

        private readonly IClassCategoryDAL _classCategoryDAL;

        private readonly IUserDAL _userDAL;

        private readonly IClassRoomDAL _classRoomDAL;

        private readonly ICourseDAL _courseDAL;

        private readonly IStudentDAL _studentDAL;

        private readonly IDistributedLockDAL _distributedLockDAL;

        private readonly IHolidaySettingDAL _holidaySettingDAL;

        private readonly IClassTimesDAL _classTimesDAL;

        private readonly IEventPublisher _eventPublisher;

        private readonly IStudentCourseDAL _studentCourseDAL;

        private readonly IStudentCourseBLL _studentCourseBLL;

        public ClassBLL(IClassDAL classDAL, IUserOperationLogDAL userOperationLogDAL, IClassCategoryDAL classCategoryDAL,
           IUserDAL userDAL, IClassRoomDAL classRoomDAL, ICourseDAL courseDAL, IStudentDAL studentDAL, IDistributedLockDAL distributedLockDAL,
           IHolidaySettingDAL holidaySettingDAL, IClassTimesDAL classTimesDAL, IEventPublisher eventPublisher, IStudentCourseDAL studentCourseDAL,
           IStudentCourseBLL studentCourseBLL)
        {
            this._classDAL = classDAL;
            this._userOperationLogDAL = userOperationLogDAL;
            this._classCategoryDAL = classCategoryDAL;
            this._userDAL = userDAL;
            this._classRoomDAL = classRoomDAL;
            this._courseDAL = courseDAL;
            this._studentDAL = studentDAL;
            this._distributedLockDAL = distributedLockDAL;
            this._holidaySettingDAL = holidaySettingDAL;
            this._classTimesDAL = classTimesDAL;
            this._eventPublisher = eventPublisher;
            this._studentCourseDAL = studentCourseDAL;
            this._studentCourseBLL = studentCourseBLL;
        }

        public void InitTenantId(int tenantId)
        {
            this.InitDataAccess(tenantId, _classDAL, _userOperationLogDAL, _classCategoryDAL, _userDAL, _classRoomDAL, _courseDAL, _studentDAL,
                _holidaySettingDAL, _classTimesDAL, _studentCourseDAL);
            this._studentCourseBLL.InitTenantId(tenantId);
        }

        public string GetMuIds(IEnumerable<MultiSelectValueRequest> selectValues)
        {
            if (selectValues == null || !selectValues.Any())
            {
                return string.Empty;
            }
            return $",{string.Join(',', selectValues.Select(p => p.Value))},";
        }

        public async Task<ResponseBase> ClassAdd(ClassAddRequest request)
        {
            var etClass = new EtClass()
            {
                ClassCategoryId = request.ClassCategoryId,
                ClassRoomIds = EtmsHelper.GetMuIds(request.ClassRoomIds),
                CompleteStatus = EmClassCompleteStatus.UnComplete,
                CompleteTime = null,
                CourseList = GetMuIds(request.CourseIds),
                DefaultClassTimes = request.DefaultClassTimes,
                FinishClassTimes = 0,
                FinishCount = 0,
                IsDeleted = EmIsDeleted.Normal,
                IsLeaveCharge = request.IsLeaveCharge,
                IsNotComeCharge = request.IsNotComeCharge,
                LastJobProcessTime = DateTime.Now,
                LimitStudentNums = request.LimitStudentNums,
                Name = request.Name,
                Ot = DateTime.Now,
                PlanCount = 0,
                Remark = request.Remark,
                ScheduleStatus = EmClassScheduleStatus.Unscheduled,
                StudentNums = 0,
                TeacherNum = request.TeacherIds != null && request.TeacherIds.Any() ? request.TeacherIds.Count : 0,
                TenantId = request.LoginTenantId,
                UserId = request.LoginUserId,
                Type = EmClassType.OneToMany,
                Teachers = GetMuIds(request.TeacherIds),
                StudentIds = string.Empty,
                OrderId = null
            };
            await _classDAL.AddClass(etClass);
            await _userOperationLogDAL.AddUserLog(request, $"添加班级-{request.Name}", EmUserOperationType.ClassManage);
            return ResponseBase.Success();
        }

        public async Task<ResponseBase> ClassEdit(ClassEditRequest request)
        {
            var etClassBucket = await _classDAL.GetClassBucket(request.CId);
            if (etClassBucket == null)
            {
                return ResponseBase.CommonError("班级不存在");
            }
            var etClass = etClassBucket.EtClass;
            etClass.Name = request.Name;
            etClass.LimitStudentNums = request.LimitStudentNums;
            etClass.ClassCategoryId = request.ClassCategoryId;
            etClass.DefaultClassTimes = request.DefaultClassTimes;
            etClass.ClassRoomIds = EtmsHelper.GetMuIds(request.ClassRoomIds);
            etClass.Teachers = GetMuIds(request.TeacherIds);
            etClass.TeacherNum = request.TeacherIds != null && request.TeacherIds.Any() ? request.TeacherIds.Count : 0;
            etClass.Remark = request.Remark;
            etClass.IsLeaveCharge = request.IsLeaveCharge;
            etClass.IsNotComeCharge = request.IsNotComeCharge;
            await _classDAL.EditClass(etClass);
            _eventPublisher.Publish(new SyncClassInfoEvent(request.LoginTenantId, request.CId));
            await _userOperationLogDAL.AddUserLog(request, $"编辑班级-{request.Name}", EmUserOperationType.ClassManage);
            return ResponseBase.Success();
        }

        public async Task<ResponseBase> ClassGet(ClassGetRequest request)
        {
            var etClassBucket = await _classDAL.GetClassBucket(request.CId);
            if (etClassBucket == null)
            {
                return ResponseBase.CommonError("班级不存在");
            }
            var etClass = etClassBucket.EtClass;
            List<long> classRoomIds = null;
            if (!string.IsNullOrEmpty(etClass.ClassRoomIds))
            {
                classRoomIds = new List<long>();
                var tempIds = etClass.ClassRoomIds.Split(',');
                foreach (var strId in tempIds)
                {
                    if (string.IsNullOrEmpty(strId))
                    {
                        continue;
                    }
                    classRoomIds.Add(strId.ToLong());
                }
            }
            var tempBoxCourse = new DataTempBox<EtCourse>();
            var tempBoxUser = new DataTempBox<EtUser>();
            return ResponseBase.Success(new ClassGetOutput()
            {
                CId = etClass.Id,
                ClassCategoryId = etClass.ClassCategoryId,
                ClassRoomIds = classRoomIds,
                CourseList = etClass.CourseList,
                DefaultClassTimes = etClass.DefaultClassTimes,
                FinishClassTimes = etClass.FinishClassTimes,
                FinishCount = etClass.FinishCount,
                IsLeaveCharge = etClass.IsLeaveCharge,
                IsNotComeCharge = etClass.IsNotComeCharge,
                LimitStudentNums = etClass.LimitStudentNums,
                Name = etClass.Name,
                PlanCount = etClass.PlanCount,
                Remark = etClass.Remark,
                StudentNums = etClass.StudentNums,
                TeacherNum = etClass.TeacherNum,
                Teachers = etClass.Teachers,
                Type = etClass.Type,
                CourseIds = await ComBusiness.GetCourseMultiSelectValue(tempBoxCourse, _courseDAL, etClass.CourseList),
                TeacherIds = await ComBusiness.GetUserMultiSelectValue(tempBoxUser, _userDAL, etClass.Teachers)
            });
        }

        public async Task<ResponseBase> ClassViewGet(ClassViewGetRequest request)
        {
            var etClassBucket = await _classDAL.GetClassBucket(request.CId);
            if (etClassBucket == null)
            {
                return ResponseBase.CommonError("班级不存在");
            }
            var etClass = etClassBucket.EtClass;
            var student = await GetOneToOneStudent(etClassBucket);
            var classRooms = await _classRoomDAL.GetAllClassRoom();
            var tempBoxCourse = new DataTempBox<EtCourse>();
            var tempBoxUser = new DataTempBox<EtUser>();
            return ResponseBase.Success(new ClassViewOutput()
            {
                CId = etClass.Id,
                ClassCategoryDesc = await GetClassCategoryDesc(etClass.ClassCategoryId),
                IsLeaveCharge = etClass.IsLeaveCharge,
                FinishCount = etClass.FinishCount,
                FinishClassTimes = etClass.FinishClassTimes,
                DefaultClassTimes = etClass.DefaultClassTimes,
                IsNotComeCharge = etClass.IsNotComeCharge,
                LimitStudentNums = etClass.LimitStudentNums,
                Name = etClass.Name,
                PlanCount = etClass.PlanCount,
                CompleteStatus = etClass.CompleteStatus,
                CompleteStatusDesc = EmClassCompleteStatus.GetClassCompleteStatusDesc(etClass.CompleteStatus),
                CompleteTimeDesc = etClass.CompleteTime.EtmsToString(),
                Remark = etClass.Remark,
                TeacherNum = etClass.TeacherNum,
                Type = etClass.Type,
                StudentNums = etClass.StudentNums,
                ScheduleStatus = etClass.ScheduleStatus,
                ScheduleStatusDesc = EmClassScheduleStatus.GetClassScheduleStatusDesc(etClass.ScheduleStatus),
                ClassRoomDesc = ComBusiness.GetClassRoomDesc(classRooms, etClass.ClassRoomIds),
                TeachersDesc = await ComBusiness.GetUserNames(tempBoxUser, _userDAL, etClass.Teachers),
                CourseList = etClass.CourseList,
                CourseDesc = await ComBusiness.GetCourseNames(tempBoxCourse, _courseDAL, etClass.CourseList),
                OneToOneStudentName = student?.Name,
                OneToOneStudentPhone = student?.Phone,
                TypeDesc = EmClassType.GetClassTypeDesc(etClass.Type),
                LimitStudentNumsDesc = etClass.LimitStudentNums == null ? "未设置" : etClass.LimitStudentNums.Value.ToString()
            });
        }

        public async Task<ResponseBase> ClassBascGet(ClassBascGetRequest request)
        {
            var etClassBucket = await _classDAL.GetClassBucket(request.CId);
            if (etClassBucket == null)
            {
                return ResponseBase.CommonError("班级不存在");
            }
            var etClass = etClassBucket.EtClass;
            var output = new ClassBascGetOutput()
            {
                ClassCourses = new List<SelectItem>(),
                ClassRooms = new List<SelectItem>(),
                ClassTeachers = new List<SelectItem>(),
                ClassName = etClass.Name,
                DefaultClassTimes = etClass.DefaultClassTimes,
                Type = etClass.Type,
                TypeDesc = EmClassType.GetClassTypeDesc(etClass.Type),
                IsLeaveCharge = etClass.IsLeaveCharge,
                IsNotComeCharge = etClass.IsNotComeCharge
            };
            if (!string.IsNullOrEmpty(etClass.CourseList))
            {
                var arrCourse = etClass.CourseList.Split(',');
                foreach (var temp in arrCourse)
                {
                    if (string.IsNullOrEmpty(temp))
                    {
                        continue;
                    }
                    var myCourse = await _courseDAL.GetCourse(temp.ToLong());
                    if (myCourse != null && myCourse.Item1 != null)
                    {
                        output.ClassCourses.Add(new SelectItem()
                        {
                            Label = myCourse.Item1.Name,
                            Value = myCourse.Item1.Id
                        });
                    }
                }
            }

            if (!string.IsNullOrEmpty(etClass.Teachers))
            {
                var arrTeacher = etClass.Teachers.Split(',');
                foreach (var temp in arrTeacher)
                {
                    if (string.IsNullOrEmpty(temp))
                    {
                        continue;
                    }
                    var myTeacher = await _userDAL.GetUser(temp.ToLong());
                    if (myTeacher != null)
                    {
                        output.ClassTeachers.Add(new SelectItem()
                        {
                            Label = myTeacher.Name,
                            Value = myTeacher.Id
                        });
                    }
                }
            }

            var allClassRoom = await _classRoomDAL.GetAllClassRoom();
            if (!string.IsNullOrEmpty(etClass.ClassRoomIds))
            {
                var arrRooms = etClass.ClassRoomIds.Split(',');
                foreach (var temp in arrRooms)
                {
                    if (string.IsNullOrEmpty(temp))
                    {
                        continue;
                    }
                    var myRoom = allClassRoom.FirstOrDefault(p => p.Id == temp.ToLong());
                    if (myRoom != null)
                    {
                        output.ClassRooms.Add(new SelectItem()
                        {
                            Label = myRoom.Name,
                            Value = myRoom.Id
                        });
                    }
                }
            }
            return ResponseBase.Success(output);
        }

        private async Task<EtStudent> GetOneToOneStudent(ClassBucket classBucket)
        {
            if (classBucket.EtClass.Type != EmClassType.OneToOne)
            {
                return null;
            }
            if (classBucket.EtClassStudents != null && classBucket.EtClassStudents.Any())
            {
                var student = await _studentDAL.GetStudent(classBucket.EtClassStudents.First().StudentId);
                return student?.Student;
            }
            return null;
        }

        private async Task<string> GetClassCategoryDesc(long? classCategoryId)
        {
            if (classCategoryId == null)
            {
                return string.Empty;
            }
            var classCategory = await _classCategoryDAL.GetAllClassCategory();
            var myClassCategory = classCategory.FirstOrDefault(p => p.Id == classCategoryId.Value);
            return myClassCategory?.Name;
        }

        public async Task<ResponseBase> ClassDel(ClassDelRequest request)
        {
            var etClassBucket = await _classDAL.GetClassBucket(request.CId);
            if (etClassBucket == null || etClassBucket.EtClass == null)
            {
                return ResponseBase.CommonError("班级不存在");
            }
            if (await _classDAL.IsClassCanNotBeDelete(request.CId))
            {
                return ResponseBase.CommonError("班级已存在点名记录，无法删除");
            }
            await _classDAL.DelClass(request.CId);
            await _userOperationLogDAL.AddUserLog(request, $"删除班级-{etClassBucket.EtClass.Name}", EmUserOperationType.ClassManage);
            return ResponseBase.Success();
        }

        public async Task<ResponseBase> ClassOverOneToMany(ClassOverRequest request)
        {
            var classIds = request.Items.Select(p => p.CId).ToList();
            await _classDAL.SetClassOverOneToMany(classIds, DateTime.Now);
            await _userOperationLogDAL.AddUserLog(request, $"一对多班级结业-{string.Join(",", request.Items.Select(p => p.ClassName))}", EmUserOperationType.ClassManage);
            return ResponseBase.Success();
        }

        public async Task<ResponseBase> ClassOverOneToOne(ClassOverOneToOneRequest request)
        {
            var etClassBucket = await _classDAL.GetClassBucket(request.CId);
            if (etClassBucket == null || etClassBucket.EtClass == null)
            {
                return ResponseBase.CommonError("班级不存在");
            }
            var etCLass = etClassBucket.EtClass;
            if (etCLass.Type != EmClassType.OneToOne)
            {
                return ResponseBase.CommonError("班级结业失败");
            }
            //var courseId = etCLass.CourseList.Trim(',').ToLong();
            //var studentCourseDetail = await _studentCourseDAL.GetEtStudentCourseDetail(etCLass.OrderId.Value, courseId);
            //if (studentCourseDetail != null && studentCourseDetail.Status != EmStudentCourseStatus.StopOfClass)
            //{
            //    await _studentCourseBLL.StudentCourseClassOver(new StudentCourseClassOverRequest()
            //    {
            //        CId = studentCourseDetail.Id,
            //        LoginTenantId = request.LoginTenantId,
            //        LoginUserId = request.LoginUserId,
            //        Remark = "一对一班级结业"
            //    });
            //}
            await _classDAL.SetClassOverOneToOne(request.CId, DateTime.Now);
            await _userOperationLogDAL.AddUserLog(request, $"一对一班级结业-{etCLass.Name}", EmUserOperationType.ClassManage);
            return ResponseBase.Success();
        }

        public async Task<ResponseBase> ClassSetTeachers(SetClassTeachersRequest request)
        {
            var teachers = EtmsHelper.GetMuIds(request.Teachers);
            var teacherNum = request.Teachers.Count;
            await _classDAL.UpdateClassTeachers(request.ClassIds, teachers, teacherNum);
            await _userOperationLogDAL.AddUserLog(request, $"批量分配老师", EmUserOperationType.ClassManage);
            return ResponseBase.Success();
        }

        public async Task<ResponseBase> ClassGetPaging(ClassGetPagingRequest request)
        {
            var pagingData = await _classDAL.GetPaging(request);
            var classViewList = new List<ClassViewOutput>();
            var classCategorys = await _classCategoryDAL.GetAllClassCategory();
            var classRooms = await _classRoomDAL.GetAllClassRoom();
            var tempBoxCourse = new DataTempBox<EtCourse>();
            var tempBoxUser = new DataTempBox<EtUser>();
            foreach (var p in pagingData.Item1)
            {
                EtStudent student = null;
                if (p.Type == EmClassType.OneToOne && request.IsGetOneToOneStudent)
                {
                    var classBucket = await _classDAL.GetClassBucket(p.Id);
                    if (classBucket.EtClassStudents != null && classBucket.EtClassStudents.Any())
                    {
                        var studentBucket = await _studentDAL.GetStudent(classBucket.EtClassStudents.First().StudentId);
                        student = studentBucket?.Student;
                    }
                }
                classViewList.Add(new ClassViewOutput()
                {
                    CId = p.Id,
                    Type = p.Type,
                    CourseList = p.CourseList,
                    CompleteStatus = p.CompleteStatus,
                    FinishCount = p.FinishCount,
                    FinishClassTimes = p.FinishClassTimes,
                    DefaultClassTimes = p.DefaultClassTimes,
                    IsLeaveCharge = p.IsLeaveCharge,
                    LimitStudentNums = p.LimitStudentNums,
                    LimitStudentNumsDesc = p.LimitStudentNums == null ? "未设置" : p.LimitStudentNums.Value.ToString(),
                    IsNotComeCharge = p.IsNotComeCharge,
                    Name = p.Name,
                    PlanCount = p.PlanCount,
                    Remark = p.Remark,
                    ScheduleStatus = p.ScheduleStatus,
                    TeacherNum = p.TeacherNum,
                    StudentNums = p.StudentNums,
                    ClassCategoryDesc = ComBusiness.GetClassCategoryDesc(classCategorys, p.ClassCategoryId),
                    ClassRoomDesc = ComBusiness.GetClassRoomDesc(classRooms, p.ClassRoomIds),
                    CompleteStatusDesc = EmClassCompleteStatus.GetClassCompleteStatusDesc(p.CompleteStatus),
                    CompleteTimeDesc = p.CompleteTime.EtmsToDateString(),
                    OneToOneStudentName = student?.Name,
                    OneToOneStudentPhone = student?.Phone,
                    ScheduleStatusDesc = EmClassScheduleStatus.GetClassScheduleStatusDesc(p.ScheduleStatus),
                    TeachersDesc = await ComBusiness.GetUserNames(tempBoxUser, _userDAL, p.Teachers),
                    CourseDesc = await ComBusiness.GetCourseNames(tempBoxCourse, _courseDAL, p.CourseList),
                    TypeDesc = EmClassType.GetClassTypeDesc(p.Type),
                    Label = p.Name,
                    Value = p.Id
                });
            }
            return ResponseBase.Success(new ResponsePagingDataBase<ClassViewOutput>(pagingData.Item2, classViewList));
        }

        public async Task<ResponseBase> ClassStudentAdd(ClassStudentAddRequest request)
        {
            var etClassBucket = await _classDAL.GetClassBucket(request.ClassId);
            if (etClassBucket == null || etClassBucket.EtClass == null)
            {
                return ResponseBase.CommonError("班级不存在");
            }
            if (etClassBucket.EtClass.CourseList.Split(',').FirstOrDefault(p => p == request.CourseId.ToString()) == null)
            {
                return ResponseBase.CommonError("请选择班级所关联的课程");
            }
            var classStudents = new List<EtClassStudent>();
            foreach (var studenInfo in request.StudentIds)
            {
                if (!await _classDAL.IsStudentBuyCourse(studenInfo.Value, request.CourseId))
                {
                    return ResponseBase.CommonError($"学员[{studenInfo.Label}]未购买此班级所关联的课程");
                }
                if (await _classDAL.IsStudentInClass(request.ClassId, studenInfo.Value))
                {
                    return ResponseBase.CommonError($"学员[{studenInfo.Label}]已经在此班级");
                }
                classStudents.Add(new EtClassStudent()
                {
                    ClassId = request.ClassId,
                    CourseId = request.CourseId,
                    IsDeleted = EmIsDeleted.Normal,
                    Remark = string.Empty,
                    StudentId = studenInfo.Value,
                    TenantId = request.LoginTenantId
                });
            }
            await _classDAL.AddClassStudent(classStudents);
            _eventPublisher.Publish(new SyncClassInfoEvent(request.LoginTenantId, request.ClassId));
            await _userOperationLogDAL.AddUserLog(request, $"班级添加学员-班级[{etClassBucket.EtClass.Name}]添加学员[{string.Join(',', request.StudentIds.Select(p => p.Label))}]", EmUserOperationType.ClassManage);
            return ResponseBase.Success();
        }

        public async Task<ResponseBase> ClassStudentRemove(ClassStudentRemoveRequest request)
        {
            var etClassBucket = await _classDAL.GetClassBucket(request.ClassId);
            if (etClassBucket == null || etClassBucket.EtClass == null)
            {
                return ResponseBase.CommonError("班级不存在");
            }
            await _classDAL.DelClassStudent(request.ClassId, request.CId);
            _eventPublisher.Publish(new SyncClassInfoEvent(request.LoginTenantId, request.ClassId));
            await _userOperationLogDAL.AddUserLog(request, $"班级移出学员-班级[{etClassBucket.EtClass.Name}]移出班级学员", EmUserOperationType.ClassManage);
            return ResponseBase.Success();
        }

        public async Task<ResponseBase> ClassStudentGet(ClassStudentGetRequest request)
        {
            var etClassBucket = await _classDAL.GetClassBucket(request.ClassId);
            if (etClassBucket == null || etClassBucket.EtClass == null)
            {
                return ResponseBase.CommonError("班级不存在");
            }
            var output = new List<ClassStudentGetOutput>();
            if (etClassBucket.EtClassStudents != null && etClassBucket.EtClassStudents.Any())
            {
                foreach (var students in etClassBucket.EtClassStudents)
                {
                    var myStudent = await _studentDAL.GetStudent(students.StudentId);
                    if (myStudent == null)
                    {
                        continue;
                    }
                    var myCourse = await _courseDAL.GetCourse(students.CourseId);
                    if (myCourse == null || myCourse.Item1 == null)
                    {
                        continue;
                    }
                    var studentCourse = await _studentCourseDAL.GetStudentCourse(students.StudentId, students.CourseId);
                    output.Add(new ClassStudentGetOutput()
                    {
                        CourseId = students.CourseId,
                        ClassId = students.ClassId,
                        CourseName = myCourse.Item1.Name,
                        Gender = myStudent.Student.Gender,
                        GenderDesc = EmGender.GetGenderDesc(myStudent.Student.Gender),
                        StudentId = students.StudentId,
                        StudentName = myStudent.Student.Name,
                        StudentPhone = myStudent.Student.Phone,
                        CourseSurplusDesc = ComBusiness.GetStudentCourseDesc(studentCourse),
                        CId = students.Id
                    });
                }
            }
            return ResponseBase.Success(output);
        }

        public async Task<ResponseBase> ClassTimesRuleAdd1(ClassTimesRuleAdd1Request request)
        {
            var lockObj = new ClassTimesRuleAddToken(request.LoginTenantId, request.ClassId);
            if (_distributedLockDAL.LockTake(lockObj))
            {
                try
                {
                    return await ProcessClassTimesRuleAdd1(request);
                }
                finally
                {
                    _distributedLockDAL.LockRelease(lockObj);
                }
            }
            else
            {
                return ResponseBase.BusyError();
            }
        }

        public async Task<ResponseBase> ClassTimesRuleAdd2(ClassTimesRuleAdd2Request request)
        {
            var lockObj = new ClassTimesRuleAddToken(request.LoginTenantId, request.ClassId);
            if (_distributedLockDAL.LockTake(lockObj))
            {
                try
                {
                    return await ProcessClassTimesRuleAdd2(request);
                }
                finally
                {
                    _distributedLockDAL.LockRelease(lockObj);
                }
            }
            else
            {
                return ResponseBase.BusyError();
            }
        }

        private async Task<ResponseBase> ProcessClassTimesRuleAdd1(ClassTimesRuleAdd1Request request)
        {
            if (request.EndType != ClassTimesRuleEndType.Count) //如果不是按次数，则统一用时间的方式处理
            {
                DateTime? endDate = null;
                if (request.EndType == ClassTimesRuleEndType.DateTime)
                {
                    endDate = Convert.ToDateTime(request.EndValue);
                }
                return await ProcessClassTimesRuleAddOfTime(new ProcessClassTimesRuleAddOfTimeRequest()
                {
                    ClassContent = request.ClassContent,
                    ClassId = request.ClassId,
                    ClassRoomIds = request.ClassRoomIds,
                    EndDate = endDate,
                    EndTime = request.EndTime,
                    IpAddress = request.IpAddress,
                    IsJumpHoliday = request.IsJumpHoliday,
                    LoginTenantId = request.LoginTenantId,
                    LoginUserId = request.LoginUserId,
                    StartDate = request.StartDate,
                    StartTime = request.StartTime,
                    TeacherIds = request.TeacherIds,
                    Weeks = request.Weeks,
                    CourseIds = request.CourseIds
                });
            }
            return await ProcessClassTimesRuleAddOfLimitCount(request);
        }

        /// <summary>
        /// 按日期排课
        /// 如果日期大于一个月则先生成30课次的排课信息，否则直接生成完排课信息
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        private async Task<ResponseBase> ProcessClassTimesRuleAdd2(ClassTimesRuleAdd2Request request)
        {
            return await ProcessClassTimesRuleAddOfDixedDate(request);
        }

        private ClassTimesPreInfo GetClassTimesPreInfo(GetClassTimesPreInfoRequest request)
        {
            var preInfo = new ClassTimesPreInfo();
            if (request.EtClassStudents.Any())
            {
                preInfo.StudentIdsClass = EtmsHelper.GetMuIds(request.EtClassStudents.Select(p => p.StudentId));
            }

            //课程
            if (request.CourseIds != null && request.CourseIds.Any())
            {
                preInfo.CourseListIsAlone = true;
                preInfo.CourseList = EtmsHelper.GetMuIds(request.CourseIds);
            }
            else
            {
                preInfo.CourseListIsAlone = false;
                preInfo.CourseList = request.EtClass.CourseList;
            }

            //教室
            if (request.ClassRoomIds != null && request.ClassRoomIds.Any())
            {
                preInfo.ClassRoomIdsIsAlone = true;
                preInfo.ClassRoomIds = EtmsHelper.GetMuIds(request.ClassRoomIds);
            }
            else
            {
                preInfo.ClassRoomIdsIsAlone = false;
                preInfo.ClassRoomIds = request.EtClass.ClassRoomIds;
            }

            //老师
            if (request.TeacherIds != null && request.TeacherIds.Any())
            {
                preInfo.TeacherCount = request.TeacherIds.Count;
                preInfo.TeachersIsAlone = true;
                preInfo.TeacherIds = EtmsHelper.GetMuIds(request.TeacherIds);
            }
            else
            {
                preInfo.TeachersIsAlone = false;
                preInfo.TeacherCount = request.EtClass.TeacherNum;
                preInfo.TeacherIds = request.EtClass.Teachers;
            }

            return preInfo;
        }

        private async Task<ResponseBase> ProcessClassTimesRuleAddOfTime(ProcessClassTimesRuleAddOfTimeRequest request)
        {
            var etClassBucket = await _classDAL.GetClassBucket(request.ClassId);
            if (etClassBucket == null || etClassBucket.EtClass == null)
            {
                return ResponseBase.CommonError("班级不存在");
            }
            var ruleCount = await _classDAL.GetClassTimesRuleCount(request.ClassId);
            if (ruleCount >= SystemConfig.ClssTimesConfig.ClassTimesRuleMaxCount)
            {
                return ResponseBase.CommonError("排课规则已超过限制条数");
            }
            var etClass = etClassBucket.EtClass;

            //初始信息
            var preInfo = GetClassTimesPreInfo(new GetClassTimesPreInfoRequest()
            {
                ClassRoomIds = request.ClassRoomIds,
                CourseIds = request.CourseIds,
                TeacherIds = request.TeacherIds,
                EtClass = etClassBucket.EtClass,
                EtClassStudents = etClassBucket.EtClassStudents
            });
            var classTimes = new List<EtClassTimes>();
            var weekDays = new List<byte>();
            var currentDate = request.StartDate.Date;
            DateTime? endDate = null;
            if (request.EndDate != null)
            {
                endDate = request.EndDate.Value.Date;
            }
            var limitWeeks = request.Weeks;
            var indexCount = 1;
            var isOver = false;
            var holidaySettings = new List<EtHolidaySetting>();
            if (request.IsJumpHoliday)
            {
                holidaySettings = await _holidaySettingDAL.GetAllHolidaySetting();
            }
            while (true)
            {
                if (indexCount > SystemConfig.ClssTimesConfig.PreGenerateClassTimesCount)
                {
                    break;
                }
                if (endDate != null && currentDate > endDate) //判断是否结束
                {
                    isOver = true;
                    break;
                }
                if (request.IsJumpHoliday && holidaySettings != null && holidaySettings.Any())  //节假日 限制
                {
                    var isHday = holidaySettings.Where(p => currentDate >= p.StartTime && currentDate <= p.EndTime);
                    if (isHday.Any())
                    {
                        currentDate = currentDate.AddDays(1);
                        continue;
                    }
                }
                var week = (byte)currentDate.DayOfWeek;
                if (limitWeeks != null && limitWeeks.Any() && !limitWeeks.Exists(p => p == week)) //周 限制
                {
                    currentDate = currentDate.AddDays(1);
                    continue;
                }
                if (!weekDays.Exists(p => p == week))
                {
                    weekDays.Add(week);
                }
                classTimes.Add(new EtClassTimes()
                {
                    ClassContent = request.ClassContent,
                    ClassId = request.ClassId,
                    ClassOt = currentDate,
                    StartTime = request.StartTime,
                    EndTime = request.EndTime,
                    IsDeleted = EmIsDeleted.Normal,
                    Status = EmClassTimesStatus.UnRollcall,
                    TenantId = request.LoginTenantId,
                    Week = week,
                    ClassRecordId = null,
                    TeacherNum = preInfo.TeacherCount,
                    Teachers = preInfo.TeacherIds,
                    TeachersIsAlone = preInfo.TeachersIsAlone,
                    ClassRoomIds = preInfo.ClassRoomIds,
                    ClassRoomIdsIsAlone = preInfo.ClassRoomIdsIsAlone,
                    CourseList = preInfo.CourseList,
                    CourseListIsAlone = preInfo.CourseListIsAlone,
                    StudentIdsTemp = string.Empty,
                    StudentIdsClass = preInfo.StudentIdsClass
                });
                indexCount++;
                currentDate = currentDate.AddDays(1);
            };
            if (weekDays.Count == 0)
            {
                return ResponseBase.CommonError("按此规则未生成任何课次，请重新设置");
            }
            //验证是否有时间重叠
            var sameTimeRule = await _classDAL.GetClassTimesRule(request.ClassId, request.StartTime, request.EndTime, weekDays);
            if (sameTimeRule.Any())
            {
                var startDate = request.StartDate.Date;
                var errMsg = "存在有重叠的排课时间，请重新设置";
                foreach (var ruleLog in sameTimeRule)
                {
                    if (endDate != null) //判断是否有结束日期
                    {
                        if (ruleLog.EndDate == null)
                        {
                            if (endDate >= ruleLog.StartDate)
                            {
                                return ResponseBase.CommonError(errMsg);
                            }
                        }
                        else
                        {
                            if (startDate <= ruleLog.StartDate && endDate >= ruleLog.StartDate)
                            {
                                return ResponseBase.CommonError(errMsg);
                            }
                            if (startDate <= ruleLog.EndDate && endDate >= ruleLog.EndDate)
                            {
                                return ResponseBase.CommonError(errMsg);
                            }
                            if (startDate >= ruleLog.StartDate && endDate <= ruleLog.EndDate)
                            {
                                return ResponseBase.CommonError(errMsg);
                            }
                        }
                    }
                    else
                    {
                        if (ruleLog.EndDate == null)
                        {
                            return ResponseBase.CommonError(errMsg);
                        }
                        else
                        {
                            if (startDate <= ruleLog.EndDate)
                            {
                                return ResponseBase.CommonError(errMsg);
                            }
                        }
                    }
                }
            }

            //插入排课规则
            foreach (var week in weekDays)
            {
                var thisWeekTimes = classTimes.Where(p => p.Week == week);
                var dateDesc = string.Empty;
                byte type;
                if (isOver && thisWeekTimes.Count() == 1)
                {
                    type = EmClassTimesRuleType.UnLoop;
                    dateDesc = $"{thisWeekTimes.First().ClassOt.EtmsToDateString()}(周{EtmsHelper.GetWeekDesc(week)})";
                }
                else
                {
                    type = EmClassTimesRuleType.Loop;
                    if (endDate == null)
                    {
                        dateDesc = $"{request.StartDate.EtmsToDateString()}开始(每周{EtmsHelper.GetWeekDesc(week)})";
                    }
                    else
                    {
                        dateDesc = $"{request.StartDate.EtmsToDateString()}~{endDate.EtmsToDateString()}(每周{EtmsHelper.GetWeekDesc(week)})";
                    }
                }
                var classTimesRule = new EtClassTimesRule()
                {
                    ClassContent = request.ClassContent,
                    Remark = string.Empty,
                    IsNeedLoop = !isOver,
                    ClassId = request.ClassId,
                    ClassRoomIds = EtmsHelper.GetMuIds(request.ClassRoomIds),
                    StartDate = request.StartDate,
                    EndDate = endDate,
                    Week = week,
                    StartTime = request.StartTime,
                    EndTime = request.EndTime,
                    IsDeleted = EmIsDeleted.Normal,
                    IsJumpHoliday = request.IsJumpHoliday,
                    Teachers = EtmsHelper.GetMuIds(request.TeacherIds),
                    CourseList = EtmsHelper.GetMuIds(request.CourseIds),
                    LastJobProcessTime = currentDate.AddDays(-1),
                    TenantId = request.LoginTenantId,
                    DateDesc = dateDesc,
                    TimeDesc = $"{EtmsHelper.GetTimeDesc(request.StartTime)}~{EtmsHelper.GetTimeDesc(request.EndTime)}",
                    Type = type,
                    RuleDesc = type == EmClassTimesRuleType.Loop ? "每周循环" : "单次"
                };
                var ruleId = await _classDAL.AddClassTimesRule(request.ClassId, classTimesRule);
                foreach (var times in thisWeekTimes)
                {
                    times.RuleId = ruleId;
                }
                _classTimesDAL.AddClassTimes(thisWeekTimes);
            }

            //更新班级&日志
            await _classDAL.UpdateClassPlanTimes(request.ClassId, EmClassScheduleStatus.Scheduled);
            _eventPublisher.Publish(new ResetTenantToDoThingEvent(request.LoginTenantId));

            await _userOperationLogDAL.AddUserLog(request, $"快速排课-班级[{etClassBucket.EtClass.Name}]排课", EmUserOperationType.ClassManage);
            return ResponseBase.Success();
        }

        private async Task<ResponseBase> ProcessClassTimesRuleAddOfLimitCount(ClassTimesRuleAdd1Request request)
        {
            var etClassBucket = await _classDAL.GetClassBucket(request.ClassId);
            if (etClassBucket == null || etClassBucket.EtClass == null)
            {
                return ResponseBase.CommonError("班级不存在");
            }
            var ruleCount = await _classDAL.GetClassTimesRuleCount(request.ClassId);
            if (ruleCount >= SystemConfig.ClssTimesConfig.ClassTimesRuleMaxCount)
            {
                return ResponseBase.CommonError("排课规则已超过限制条数");
            }

            var preInfo = GetClassTimesPreInfo(new GetClassTimesPreInfoRequest()
            {
                ClassRoomIds = request.ClassRoomIds,
                CourseIds = request.CourseIds,
                TeacherIds = request.TeacherIds,
                EtClass = etClassBucket.EtClass,
                EtClassStudents = etClassBucket.EtClassStudents
            });
            var classTimes = new List<EtClassTimes>();
            var weekDays = new List<byte>();
            var currentDate = request.StartDate.Date;
            var limitCount = request.EndValue.ToInt();
            var limitWeeks = request.Weeks;
            var indexCount = 1;
            var holidaySettings = new List<EtHolidaySetting>();
            if (request.IsJumpHoliday)
            {
                holidaySettings = await _holidaySettingDAL.GetAllHolidaySetting();
            }
            while (true)
            {
                if (indexCount > limitCount)
                {
                    break;
                }
                if (request.IsJumpHoliday && holidaySettings != null && holidaySettings.Any())  //节假日 限制
                {
                    var isHday = holidaySettings.Where(p => currentDate >= p.StartTime && currentDate <= p.EndTime);
                    if (isHday.Any())
                    {
                        currentDate = currentDate.AddDays(1);
                        continue;
                    }
                }
                var week = (byte)currentDate.DayOfWeek;
                if (limitWeeks != null && limitWeeks.Any() && !limitWeeks.Exists(p => p == week)) //周 限制
                {
                    currentDate = currentDate.AddDays(1);
                    continue;
                }
                if (!weekDays.Exists(p => p == week))
                {
                    weekDays.Add(week);
                }
                classTimes.Add(new EtClassTimes()
                {
                    ClassContent = request.ClassContent,
                    ClassId = request.ClassId,
                    ClassOt = currentDate,
                    StartTime = request.StartTime,
                    EndTime = request.EndTime,
                    IsDeleted = EmIsDeleted.Normal,
                    Status = EmClassTimesStatus.UnRollcall,
                    TenantId = request.LoginTenantId,
                    Week = week,
                    ClassRecordId = null,
                    ClassRoomIds = preInfo.ClassRoomIds,
                    ClassRoomIdsIsAlone = preInfo.ClassRoomIdsIsAlone,
                    CourseList = preInfo.CourseList,
                    CourseListIsAlone = preInfo.CourseListIsAlone,
                    StudentIdsClass = preInfo.StudentIdsClass,
                    StudentIdsTemp = string.Empty,
                    TeacherNum = preInfo.TeacherCount,
                    Teachers = preInfo.TeacherIds,
                    TeachersIsAlone = preInfo.TeachersIsAlone
                });
                indexCount++;
                currentDate = currentDate.AddDays(1);
            };
            if (weekDays.Count == 0)
            {
                return ResponseBase.CommonError("按此规则未生成任何课次，请重新设置");
            }
            var endDate = currentDate.AddDays(-1);
            //验证是否有时间重叠
            var sameTimeRule = await _classDAL.GetClassTimesRule(request.ClassId, request.StartTime, request.EndTime, weekDays);
            if (sameTimeRule.Any())
            {
                var startDate = request.StartDate.Date;
                var errMsg = "存在有重叠的排课时间，请重新设置";
                foreach (var ruleLog in sameTimeRule)
                {
                    if (endDate != null) //判断是否有结束日期
                    {
                        if (ruleLog.EndDate == null)
                        {
                            if (endDate >= ruleLog.StartDate)
                            {
                                return ResponseBase.CommonError(errMsg);
                            }
                        }
                        else
                        {
                            if (startDate <= ruleLog.StartDate && endDate >= ruleLog.StartDate)
                            {
                                return ResponseBase.CommonError(errMsg);
                            }
                            if (startDate <= ruleLog.EndDate && endDate >= ruleLog.EndDate)
                            {
                                return ResponseBase.CommonError(errMsg);
                            }
                            if (startDate >= ruleLog.StartDate && endDate <= ruleLog.EndDate)
                            {
                                return ResponseBase.CommonError(errMsg);
                            }
                        }
                    }
                    else
                    {
                        if (ruleLog.EndDate == null)
                        {
                            return ResponseBase.CommonError(errMsg);
                        }
                        else
                        {
                            if (startDate <= ruleLog.EndDate)
                            {
                                return ResponseBase.CommonError(errMsg);
                            }
                        }
                    }
                }
            }

            //插入排课规则
            foreach (var week in weekDays)
            {
                var thisWeekTimes = classTimes.Where(p => p.Week == week);
                var dateDesc = string.Empty;
                byte type;
                if (thisWeekTimes.Count() == 1)
                {
                    type = EmClassTimesRuleType.UnLoop;
                    dateDesc = $"{thisWeekTimes.First().ClassOt.EtmsToDateString()}(周{EtmsHelper.GetWeekDesc(week)})";
                }
                else
                {
                    type = EmClassTimesRuleType.Loop;
                    dateDesc = $"{request.StartDate.EtmsToDateString()}~{endDate.EtmsToDateString()}(每周{EtmsHelper.GetWeekDesc(week)})";
                }
                var classTimesRule = new EtClassTimesRule()
                {
                    ClassContent = request.ClassContent,
                    Remark = string.Empty,
                    IsNeedLoop = false,
                    ClassId = request.ClassId,
                    ClassRoomIds = EtmsHelper.GetMuIds(request.ClassRoomIds),
                    StartDate = request.StartDate,
                    EndDate = endDate,
                    Week = week,
                    StartTime = request.StartTime,
                    EndTime = request.EndTime,
                    IsDeleted = EmIsDeleted.Normal,
                    IsJumpHoliday = request.IsJumpHoliday,
                    Teachers = EtmsHelper.GetMuIds(request.TeacherIds),
                    CourseList = EtmsHelper.GetMuIds(request.CourseIds),
                    LastJobProcessTime = currentDate.AddDays(-1),
                    TenantId = request.LoginTenantId,
                    DateDesc = dateDesc,
                    TimeDesc = $"{EtmsHelper.GetTimeDesc(request.StartTime)}~{EtmsHelper.GetTimeDesc(request.EndTime)}",
                    Type = type,
                    RuleDesc = type == EmClassTimesRuleType.Loop ? "每周循环" : "单次"
                };
                var ruleId = await _classDAL.AddClassTimesRule(request.ClassId, classTimesRule);
                foreach (var times in thisWeekTimes)
                {
                    times.RuleId = ruleId;
                }
                _classTimesDAL.AddClassTimes(thisWeekTimes);
            }

            //更新班级&日志
            await _classDAL.UpdateClassPlanTimes(request.ClassId, EmClassScheduleStatus.Scheduled);
            _eventPublisher.Publish(new ResetTenantToDoThingEvent(request.LoginTenantId));

            await _userOperationLogDAL.AddUserLog(request, $"快速排课-班级[{etClassBucket.EtClass.Name}]排课", EmUserOperationType.ClassManage);
            return ResponseBase.Success();
        }

        private async Task<ResponseBase> ProcessClassTimesRuleAddOfDixedDate(ClassTimesRuleAdd2Request request)
        {
            var etClassBucket = await _classDAL.GetClassBucket(request.ClassId);
            if (etClassBucket == null || etClassBucket.EtClass == null)
            {
                return ResponseBase.CommonError("班级不存在");
            }
            var ruleCount = await _classDAL.GetClassTimesRuleCount(request.ClassId);
            if (ruleCount >= SystemConfig.ClssTimesConfig.ClassTimesRuleMaxCount)
            {
                return ResponseBase.CommonError("排课规则已超过限制条数");
            }

            var preInfo = GetClassTimesPreInfo(new GetClassTimesPreInfoRequest()
            {
                ClassRoomIds = request.ClassRoomIds,
                CourseIds = request.CourseIds,
                TeacherIds = request.TeacherIds,
                EtClass = etClassBucket.EtClass,
                EtClassStudents = etClassBucket.EtClassStudents
            });
            var classTimes = new List<EtClassTimes>();
            var weekDays = new List<byte>();
            var holidaySettings = new List<EtHolidaySetting>();
            if (request.IsJumpHoliday)
            {
                holidaySettings = await _holidaySettingDAL.GetAllHolidaySetting();
            }
            foreach (var strDate in request.ClassDate)
            {
                var currentDate = Convert.ToDateTime(strDate).Date;
                if (request.IsJumpHoliday && holidaySettings != null && holidaySettings.Any())  //节假日 限制
                {
                    var isHday = holidaySettings.Where(p => currentDate >= p.StartTime && currentDate <= p.EndTime);
                    if (isHday.Any())
                    {
                        continue;
                    }
                }
                var week = (byte)currentDate.DayOfWeek;
                if (!weekDays.Exists(p => p == week))
                {
                    weekDays.Add(week);
                }
                classTimes.Add(new EtClassTimes()
                {
                    ClassContent = request.ClassContent,
                    ClassId = request.ClassId,
                    ClassOt = currentDate,
                    StartTime = request.StartTime,
                    EndTime = request.EndTime,
                    IsDeleted = EmIsDeleted.Normal,
                    Status = EmClassTimesStatus.UnRollcall,
                    TenantId = request.LoginTenantId,
                    Week = week,
                    ClassRecordId = null,
                    ClassRoomIds = preInfo.ClassRoomIds,
                    ClassRoomIdsIsAlone = preInfo.ClassRoomIdsIsAlone,
                    CourseList = preInfo.CourseList,
                    CourseListIsAlone = preInfo.CourseListIsAlone,
                    StudentIdsClass = preInfo.StudentIdsClass,
                    StudentIdsTemp = string.Empty,
                    TeacherNum = preInfo.TeacherCount,
                    Teachers = preInfo.TeacherIds,
                    TeachersIsAlone = preInfo.TeachersIsAlone
                });
            };
            if (weekDays.Count == 0)
            {
                return ResponseBase.CommonError("按此规则未生成任何课次，请重新设置");
            }
            //验证是否有时间重叠
            var sameTimeRule = await _classDAL.GetClassTimesRule(request.ClassId, request.StartTime, request.EndTime, weekDays);
            if (sameTimeRule.Any())
            {
                var errMsg = "存在有重叠的排课时间，请重新设置";
                foreach (var ruleLog in sameTimeRule)
                {
                    foreach (var myTime in classTimes)
                    {
                        if (myTime.Week == ruleLog.Week)
                        {
                            if (ruleLog.StartDate == myTime.ClassOt)
                            {
                                return ResponseBase.CommonError(errMsg);
                            }
                            if (ruleLog.StartDate < myTime.ClassOt && (ruleLog.EndDate == null || ruleLog.EndDate >= myTime.ClassOt))
                            {
                                return ResponseBase.CommonError(errMsg);
                            }
                        }
                    }
                }
            }

            //插入排课规则
            foreach (var myTime in classTimes)
            {
                var dateDesc = $"{myTime.ClassOt.EtmsToDateString()}(周{EtmsHelper.GetWeekDesc(myTime.Week)})";
                var classTimesRule = new EtClassTimesRule()
                {
                    ClassContent = request.ClassContent,
                    Remark = string.Empty,
                    IsNeedLoop = false,
                    ClassId = request.ClassId,
                    ClassRoomIds = EtmsHelper.GetMuIds(request.ClassRoomIds),
                    StartDate = myTime.ClassOt,
                    EndDate = myTime.ClassOt,
                    Week = myTime.Week,
                    StartTime = request.StartTime,
                    EndTime = request.EndTime,
                    IsDeleted = EmIsDeleted.Normal,
                    IsJumpHoliday = request.IsJumpHoliday,
                    Teachers = EtmsHelper.GetMuIds(request.TeacherIds),
                    CourseList = EtmsHelper.GetMuIds(request.CourseIds),
                    LastJobProcessTime = myTime.ClassOt,
                    TenantId = request.LoginTenantId,
                    DateDesc = dateDesc,
                    TimeDesc = $"{EtmsHelper.GetTimeDesc(request.StartTime)}~{EtmsHelper.GetTimeDesc(request.EndTime)}",
                    Type = EmClassTimesRuleType.UnLoop,
                    RuleDesc = "单次"
                };
                var ruleId = await _classDAL.AddClassTimesRule(request.ClassId, classTimesRule);
                myTime.RuleId = ruleId;
            }
            _classTimesDAL.AddClassTimes(classTimes);

            //更新班级&日志
            await _classDAL.UpdateClassPlanTimes(request.ClassId, EmClassScheduleStatus.Scheduled);
            _eventPublisher.Publish(new ResetTenantToDoThingEvent(request.LoginTenantId));

            await _userOperationLogDAL.AddUserLog(request, $"快速排课-班级[{etClassBucket.EtClass.Name}]排课", EmUserOperationType.ClassManage);
            return ResponseBase.Success();
        }

        public async Task<ResponseBase> ClassTimesRuleDel(ClassTimesRuleDelRequest request)
        {
            var etClassBucket = await _classDAL.GetClassBucket(request.ClassId);
            if (etClassBucket == null || etClassBucket.EtClass == null)
            {
                return ResponseBase.CommonError("班级不存在");
            }
            await _classDAL.DelClassTimesRule(request.ClassId, request.RuleId);
            await _userOperationLogDAL.AddUserLog(request, $"删除排课-删除班级[{etClassBucket.EtClass.Name}]的排课", EmUserOperationType.ClassManage);
            return ResponseBase.Success();
        }

        public async Task SyncClassInfoProcessEvent(SyncClassInfoEvent request)
        {
            //班级学员
            var etClassBucket = await _classDAL.GetClassBucket(request.ClassId);
            var etClass = etClassBucket.EtClass;
            var etClassStudents = etClassBucket.EtClassStudents;
            var studentIds = string.Empty;
            var studentCount = etClassStudents.Count;
            if (etClassStudents.Any())
            {
                studentIds = EtmsHelper.GetMuIds(etClassStudents.Select(p => p.StudentId));
            }
            await _classDAL.ClassEditStudentInfo(request.ClassId, studentIds, studentCount);

            //同步课次信息（授课课程、教室、老师、班级学员） 
            await _classDAL.SyncClassInfo(request.ClassId, studentIds, etClass.CourseList, etClass.ClassRoomIds, etClass.Teachers, etClass.TeacherNum);
        }

        public async Task<ResponseBase> ClassTimesRuleGet(ClassTimesRuleGetRequest request)
        {
            var etClassBucket = await _classDAL.GetClassBucket(request.CId);
            if (etClassBucket == null || etClassBucket.EtClass == null)
            {
                return ResponseBase.CommonError("班级不存在");
            }
            var classTimesRules = await _classDAL.GetClassTimesRule(request.CId);
            var output = new List<ClassTimesRuleGetOutput>();
            if (classTimesRules.Any())
            {
                var classRooms = await _classRoomDAL.GetAllClassRoom();
                var tempBoxCourse = new DataTempBox<EtCourse>();
                var tempBoxUser = new DataTempBox<EtUser>();
                var classRoomDesc = ComBusiness.GetClassRoomDesc(classRooms, etClassBucket.EtClass.ClassRoomIds);
                var classTeacherDesc = await ComBusiness.GetUserNames(tempBoxUser, _userDAL, etClassBucket.EtClass.Teachers);
                var classCourseDesc = await ComBusiness.GetCourseNames(tempBoxCourse, _courseDAL, etClassBucket.EtClass.CourseList);
                foreach (var rule in classTimesRules)
                {
                    var myRoomDesc = string.Empty;
                    if (!string.IsNullOrEmpty(rule.ClassRoomIds))
                    {
                        myRoomDesc = ComBusiness.GetClassRoomDesc(classRooms, rule.ClassRoomIds);
                    }
                    else
                    {
                        myRoomDesc = classRoomDesc;
                    }

                    var myTeacherDesc = string.Empty;
                    if (!string.IsNullOrEmpty(rule.Teachers))
                    {
                        myTeacherDesc = await ComBusiness.GetUserNames(tempBoxUser, _userDAL, rule.Teachers);
                    }
                    else
                    {
                        myTeacherDesc = classTeacherDesc;
                    }

                    var myCourseDesc = string.Empty;
                    if (!string.IsNullOrEmpty(rule.CourseList))
                    {
                        myCourseDesc = await ComBusiness.GetCourseNames(tempBoxCourse, _courseDAL, rule.CourseList);
                    }
                    else
                    {
                        myCourseDesc = classCourseDesc;
                    }

                    output.Add(new ClassTimesRuleGetOutput()
                    {
                        ClassRoomDesc = myRoomDesc,
                        CourseDesc = myCourseDesc,
                        TeachersDesc = myTeacherDesc,
                        ClassContent = rule.ClassContent,
                        DateDesc = rule.DateDesc,
                        RuleDesc = rule.RuleDesc,
                        TimeDesc = rule.TimeDesc,
                        ClassId = rule.ClassId,
                        RuleId = rule.Id,
                        IsJumpHoliday = rule.IsJumpHoliday
                    });
                }
            }
            return ResponseBase.Success(output);
        }

        public async Task<ResponseBase> ClassMyGet(ClassMyGetRequest request)
        {
            var myClass = await _classDAL.GetClassOfTeacher(request.LoginUserId);
            var classViewList = new List<ClassMyGetOutput>();
            var classRooms = await _classRoomDAL.GetAllClassRoom();
            var tempBoxCourse = new DataTempBox<EtCourse>();
            var tempBoxUser = new DataTempBox<EtUser>();
            foreach (var p in myClass)
            {
                classViewList.Add(new ClassMyGetOutput()
                {
                    CId = p.Id,
                    Type = p.Type,
                    CourseList = p.CourseList,
                    LimitStudentNums = p.LimitStudentNums,
                    LimitStudentNumsDesc = p.LimitStudentNums == null ? "未设置" : p.LimitStudentNums.Value.ToString(),
                    Name = p.Name,
                    StudentNums = p.StudentNums,
                    ClassRoomDesc = ComBusiness.GetClassRoomDesc(classRooms, p.ClassRoomIds),
                    TeachersDesc = await ComBusiness.GetUserNames(tempBoxUser, _userDAL, p.Teachers),
                    CourseDesc = await ComBusiness.GetCourseNames(tempBoxCourse, _courseDAL, p.CourseList),
                    TypeDesc = EmClassType.GetClassTypeDesc(p.Type)
                });
            }
            return ResponseBase.Success(classViewList);
        }

        public async Task<ResponseBase> ClassStudentTransfer(ClassStudentTransferRequest request)
        {
            var oldClass = await _classDAL.GetClassBucket(request.ClassId);
            if (oldClass == null || oldClass.EtClass == null)
            {
                return ResponseBase.CommonError("班级不存在");
            }
            var studentBucket = await _studentDAL.GetStudent(request.StudentId);
            if (studentBucket == null || studentBucket.Student == null)
            {
                return ResponseBase.CommonError("学员不存在");
            }

            var newClass = await _classDAL.GetClassBucket(request.NewClassId);
            if (newClass == null || newClass.EtClass == null)
            {
                return ResponseBase.CommonError("调至的班级不存在");
            }
            if (newClass.EtClassStudents != null && newClass.EtClassStudents.Count > 0)
            {
                var existStudent = newClass.EtClassStudents.FirstOrDefault(p => p.StudentId == request.StudentId);
                if (existStudent != null)
                {
                    return ResponseBase.CommonError("该学员已在调至的班级");
                }
            }
            if (newClass.EtClass.CourseList.IndexOf(request.CourseId.ToString()) == -1)
            {
                return ResponseBase.CommonError("调至的班级未教授此课程");
            }

            //处理原班级
            await _classDAL.DelClassStudent(request.ClassId, request.CId);
            _eventPublisher.Publish(new SyncClassInfoEvent(request.LoginTenantId, request.ClassId));

            //新班级
            await _classDAL.AddClassStudent(new EtClassStudent()
            {
                ClassId = request.NewClassId,
                CourseId = request.CourseId,
                IsDeleted = EmIsDeleted.Normal,
                Remark = string.Empty,
                StudentId = request.StudentId,
                TenantId = request.LoginTenantId
            });
            _eventPublisher.Publish(new SyncClassInfoEvent(request.LoginTenantId, request.NewClassId));

            await _userOperationLogDAL.AddUserLog(request, $"调至其他班-将学员[{studentBucket.Student.Name}]从班级[{oldClass.EtClass.Name}]调至[{newClass.EtClass.Name}]", EmUserOperationType.ClassManage);
            return ResponseBase.Success();
        }
    }
}
