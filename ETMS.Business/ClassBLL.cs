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
using ETMS.Entity.View;
using ETMS.Event.DataContract.Statistics;

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

        private readonly IClassTimesRuleStudentDAL _classTimesRuleStudentDAL;

        private readonly ITenantConfigDAL _tenantConfigDAL;

        public ClassBLL(IClassDAL classDAL, IUserOperationLogDAL userOperationLogDAL, IClassCategoryDAL classCategoryDAL,
           IUserDAL userDAL, IClassRoomDAL classRoomDAL, ICourseDAL courseDAL, IStudentDAL studentDAL, IDistributedLockDAL distributedLockDAL,
           IHolidaySettingDAL holidaySettingDAL, IClassTimesDAL classTimesDAL, IEventPublisher eventPublisher, IStudentCourseDAL studentCourseDAL,
           IStudentCourseBLL studentCourseBLL, IClassTimesRuleStudentDAL classTimesRuleStudentDAL, ITenantConfigDAL tenantConfigDAL)
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
            this._classTimesRuleStudentDAL = classTimesRuleStudentDAL;
            this._tenantConfigDAL = tenantConfigDAL;
        }

        public void InitTenantId(int tenantId)
        {
            this.InitDataAccess(tenantId, _classDAL, _userOperationLogDAL, _classCategoryDAL, _userDAL, _classRoomDAL, _courseDAL, _studentDAL,
                _holidaySettingDAL, _classTimesDAL, _studentCourseDAL, _classTimesRuleStudentDAL, _tenantConfigDAL);
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
                OrderId = null,
                LimitStudentNumsType = request.LimitStudentNumsType,
                IsCanOnlineSelClass = request.IsCanOnlineSelClass
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
            if (etClass.Type == EmClassType.OneToOne && request.ReservationType == EmBool.True)
            {
                if (request.DurationHour == 0 && request.DurationMinute == 0)
                {
                    return ResponseBase.CommonError("请设置老师上课时长");
                }
            }

            var oldClassCategoryId = etClass.ClassCategoryId;

            etClass.Name = request.Name;
            etClass.LimitStudentNums = request.LimitStudentNums;
            etClass.LimitStudentNumsType = request.LimitStudentNumsType;
            etClass.ClassCategoryId = request.ClassCategoryId;
            etClass.DefaultClassTimes = request.DefaultClassTimes;
            etClass.ClassRoomIds = EtmsHelper.GetMuIds(request.ClassRoomIds);
            etClass.Teachers = GetMuIds(request.TeacherIds);
            etClass.TeacherNum = request.TeacherIds != null && request.TeacherIds.Any() ? request.TeacherIds.Count : 0;
            etClass.Remark = request.Remark;
            etClass.IsLeaveCharge = request.IsLeaveCharge;
            etClass.IsNotComeCharge = request.IsNotComeCharge;
            etClass.IsCanOnlineSelClass = request.IsCanOnlineSelClass;
            if (etClass.Type == EmClassType.OneToOne)
            {
                etClass.ReservationType = request.ReservationType;
                etClass.DurationHour = request.DurationHour;
                etClass.DurationMinute = request.DurationMinute;
                etClass.DunIntervalMinute = request.DunIntervalMinute;
            }
            if (etClass.Type == EmClassType.OneToMany)
            {
                etClass.CourseList = GetMuIds(request.CourseIds);
            }
            await _classDAL.EditClass(etClass);
            _eventPublisher.Publish(new SyncClassInfoEvent(request.LoginTenantId, request.CId));
            if (etClass.ClassCategoryId != oldClassCategoryId)
            {
                _eventPublisher.Publish(new SyncClassCategoryIdEvent(request.LoginTenantId)
                {
                    ClassId = etClass.Id,
                    NewClassCategoryId = etClass.ClassCategoryId
                });
            }

            await _userOperationLogDAL.AddUserLog(request, $"编辑班级-{request.Name}", EmUserOperationType.ClassManage);
            return ResponseBase.Success();
        }

        public async Task<ResponseBase> ClassOneToOneSetReservationBatch(ClassOneToOneSetReservationBatchRequest request)
        {
            await _classDAL.UpdateReservationInfo(request.ClassIds, request.ReservationType, request.DurationHour, request.DurationMinute,
                request.DunIntervalMinute);

            await _userOperationLogDAL.AddUserLog(request, "批量设置在线约课", EmUserOperationType.ClassManage);
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
                DefaultClassTimes = etClass.DefaultClassTimes.EtmsToString(),
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
                LimitStudentNumsType = etClass.LimitStudentNumsType,
                IsCanOnlineSelClass = etClass.IsCanOnlineSelClass,
                Type = etClass.Type,
                DurationHour = etClass.DurationHour,
                DurationMinute = etClass.DurationMinute,
                ReservationType = etClass.ReservationType,
                DunIntervalMinute = etClass.DunIntervalMinute,
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
                DefaultClassTimes = etClass.DefaultClassTimes.EtmsToString(),
                IsNotComeCharge = etClass.IsNotComeCharge,
                LimitStudentNums = etClass.LimitStudentNums,
                LimitStudentNumsType = etClass.LimitStudentNumsType,
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
                OneToOneStudentPhone = ComBusiness3.PhoneSecrecy(student?.Phone, request.SecrecyType, request.SecrecyDataBag),
                TypeDesc = EmClassType.GetClassTypeDesc(etClass.Type),
                IsCanOnlineSelClass = etClass.IsCanOnlineSelClass,
                ReservationType = etClass.ReservationType,
                DurationHour = etClass.DurationHour,
                DurationMinute = etClass.DurationMinute,
                DunIntervalMinute = etClass.DunIntervalMinute,
                Label = etClass.Name,
                Value = etClass.Id,
                LimitStudentNumsDesc = EmLimitStudentNumsType.GetLimitStudentNumsDesc(etClass.StudentNums, etClass.LimitStudentNums, etClass.LimitStudentNumsType)
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
                DefaultClassTimes = etClass.DefaultClassTimes.EtmsToString(),
                Type = etClass.Type,
                TypeDesc = EmClassType.GetClassTypeDesc(etClass.Type),
                IsLeaveCharge = etClass.IsLeaveCharge,
                IsNotComeCharge = etClass.IsNotComeCharge,
                IsCanOnlineSelClass = etClass.IsCanOnlineSelClass
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
            if (!request.IsIgnoreCheck)
            {
                if (await _classDAL.IsClassCanNotBeDelete(request.CId))
                {
                    return ResponseBase.Success(new DelOutput(false, true));
                }
            }
            if (request.IsIgnoreCheck)
            {
                var classRecordAllDate = await _classDAL.GetClassRecordAllDate(request.CId);
                var classRecordALLTeachers = await _classDAL.GetClassRecordTeacherInfoView(request.CId);
                await _classDAL.DelClassDepth(request.CId);

                var myChangeDate = new List<YearAndMonth>();
                //处理班级教务数据
                if (classRecordAllDate.Any())
                {
                    foreach (var myClassRecordDate in classRecordAllDate)
                    {
                        _eventPublisher.Publish(new StatisticsEducationEvent(request.LoginTenantId)
                        {
                            Time = myClassRecordDate.Ot
                        });
                        _eventPublisher.Publish(new StatisticsClassEvent(request.LoginTenantId)
                        {
                            ClassOt = myClassRecordDate.Ot
                        });
                        _eventPublisher.Publish(new StatisticsTeacherSalaryClassDayEvent(request.LoginTenantId)
                        {
                            Time = myClassRecordDate.Ot
                        });
                        var log = myChangeDate.FirstOrDefault(p => p.Year == myClassRecordDate.Ot.Year && p.Month == myClassRecordDate.Ot.Month);
                        if (log == null)
                        {
                            myChangeDate.Add(new YearAndMonth()
                            {
                                Year = myClassRecordDate.Ot.Year,
                                Month = myClassRecordDate.Ot.Month
                            });
                        }
                    }
                }

                //处理班级点名记录 所影响的老师课时
                if (classRecordALLTeachers.Any())
                {
                    var classDeepDelAllTeacherId = new List<long>();
                    foreach (var p in classRecordALLTeachers)
                    {
                        var allTeacherId = EtmsHelper.AnalyzeMuIds(p.Teachers);
                        foreach (var teacherId in allTeacherId)
                        {
                            var log = classDeepDelAllTeacherId.Exists(j => j == teacherId);
                            if (!log)
                            {
                                classDeepDelAllTeacherId.Add(teacherId);
                            }
                        }
                    }
                    if (classDeepDelAllTeacherId.Any())
                    {
                        _eventPublisher.Publish(new SyncTeacherMonthClassTimesEvent(request.LoginTenantId)
                        {
                            YearAndMonthList = myChangeDate,
                            TeacherIds = classDeepDelAllTeacherId
                        });
                    }
                }
            }
            else
            {
                await _classDAL.DelClass(request.CId);
            }
            this.AnalyzeClassStudent(etClassBucket);
            await _userOperationLogDAL.AddUserLog(request, $"删除班级-{etClassBucket.EtClass.Name}", EmUserOperationType.ClassManage);
            return ResponseBase.Success(new DelOutput(true));
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
                    DefaultClassTimes = p.DefaultClassTimes.EtmsToString(),
                    IsLeaveCharge = p.IsLeaveCharge,
                    LimitStudentNums = p.LimitStudentNums,
                    LimitStudentNumsDesc = EmLimitStudentNumsType.GetLimitStudentNumsDesc(p.StudentNums, p.LimitStudentNums, p.LimitStudentNumsType),
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
                    OneToOneStudentPhone = ComBusiness3.PhoneSecrecy(student?.Phone, request.SecrecyType, request.SecrecyDataBag),
                    ScheduleStatusDesc = EmClassScheduleStatus.GetClassScheduleStatusDesc(p.ScheduleStatus),
                    TeachersDesc = await ComBusiness.GetUserNames(tempBoxUser, _userDAL, p.Teachers),
                    CourseDesc = await ComBusiness.GetCourseNames(tempBoxCourse, _courseDAL, p.CourseList),
                    TypeDesc = EmClassType.GetClassTypeDesc(p.Type),
                    Label = p.Name,
                    Value = p.Id,
                    LimitStudentNumsType = p.LimitStudentNumsType,
                    IsCanOnlineSelClass = p.IsCanOnlineSelClass,
                    ReservationType = p.ReservationType,
                    DurationHour = p.DurationHour,
                    DurationMinute = p.DurationMinute,
                    DunIntervalMinute = p.DunIntervalMinute
                });
            }
            return ResponseBase.Success(new ResponsePagingDataBase<ClassViewOutput>(pagingData.Item2, classViewList));
        }

        public async Task<ResponseBase> ClassGetPagingSimple(ClassGetPagingRequest request)
        {
            var pagingData = await _classDAL.GetPaging(request);
            var classViewList = new List<SimpleDataView>();
            foreach (var p in pagingData.Item1)
            {
                classViewList.Add(new SimpleDataView()
                {
                    Key = p.Id,
                    Label = p.Name
                });
            }
            return ResponseBase.Success(new ResponsePagingDataBase<SimpleDataView>(pagingData.Item2, classViewList));
        }

        public async Task<ResponseBase> ClassStudentAdd(ClassStudentAddRequest request)
        {
            var etClassBucket = await _classDAL.GetClassBucket(request.ClassId);
            if (etClassBucket == null || etClassBucket.EtClass == null)
            {
                return ResponseBase.CommonError("班级不存在");
            }
            var myClass = etClassBucket.EtClass;
            var myClassStudent = etClassBucket.EtClassStudents;
            if (etClassBucket.EtClass.CourseList.Split(',').FirstOrDefault(p => p == request.CourseId.ToString()) == null)
            {
                return ResponseBase.CommonError("请选择班级所关联的课程");
            }
            var newStudentIds = new List<long>();
            foreach (var studenInfo in request.StudentIds)
            {
                if (!await _classDAL.IsStudentBuyCourse(studenInfo.Value, request.CourseId))
                {
                    return ResponseBase.CommonError($"学员[{studenInfo.Label}]未购买此班级所关联的课程");
                }
                var hisStu = myClassStudent.FirstOrDefault(p => p.StudentId == studenInfo.Value);
                if (hisStu != null)
                {
                    continue;
                }
                newStudentIds.Add(studenInfo.Value);
            }
            newStudentIds = newStudentIds.Distinct().ToList();

            var totalCount = etClassBucket.EtClassStudents.Count + newStudentIds.Count;
            if (totalCount > 200)
            {
                return ResponseBase.CommonError("一个班级学员的数量不能超过200人");
            }
            if (myClass.LimitStudentNums != null && myClass.LimitStudentNumsType == EmLimitStudentNumsType.NotOverflow)
            {
                if (totalCount > myClass.LimitStudentNums.Value)
                {
                    return ResponseBase.CommonError($"班级容量限制{myClass.LimitStudentNums.Value}人，无法添加学员");
                }
            }
            if (newStudentIds.Count == 0)
            {
                return ResponseBase.CommonError("学员已在此班级中");
            }

            var classStudents = new List<EtClassStudent>();
            foreach (var myStudentId in newStudentIds)
            {
                classStudents.Add(new EtClassStudent()
                {
                    ClassId = request.ClassId,
                    CourseId = request.CourseId,
                    IsDeleted = EmIsDeleted.Normal,
                    Remark = string.Empty,
                    StudentId = myStudentId,
                    TenantId = request.LoginTenantId,
                    Type = myClass.Type
                });
                _eventPublisher.Publish(new SyncStudentClassInfoEvent(request.LoginTenantId)
                {
                    StudentId = myStudentId
                });
            }

            await _classDAL.AddClassStudent(classStudents);
            foreach (var p in classStudents)
            {
                _eventPublisher.Publish(new SyncStudentStudentClassIdsEvent(request.LoginTenantId, p.StudentId));
            }
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

            if (etClassBucket.EtClassStudents != null && etClassBucket.EtClassStudents.Any())
            {
                var myStudentLog = etClassBucket.EtClassStudents.FirstOrDefault(p => p.Id == request.CId);
                if (myStudentLog != null)
                {
                    await _classTimesRuleStudentDAL.DelClassTimesRuleStudentByStudentId(myStudentLog.StudentId, request.ClassId);
                    _eventPublisher.Publish(new SyncStudentClassInfoEvent(request.LoginTenantId)
                    {
                        StudentId = myStudentLog.StudentId
                    });
                    _eventPublisher.Publish(new SyncStudentStudentClassIdsEvent(request.LoginTenantId, myStudentLog.StudentId));
                }
            }

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
                var tempBoxCourse = new DataTempBox<EtCourse>();
                foreach (var students in etClassBucket.EtClassStudents)
                {
                    var myStudent = await _studentDAL.GetStudent(students.StudentId);
                    if (myStudent == null)
                    {
                        continue;
                    }
                    var myCourse = await ComBusiness.GetCourse(tempBoxCourse, _courseDAL, students.CourseId);
                    if (myCourse == null)
                    {
                        continue;
                    }
                    var studentCourse = await _studentCourseDAL.GetStudentCourse(students.StudentId, students.CourseId);
                    output.Add(new ClassStudentGetOutput()
                    {
                        CourseId = students.CourseId,
                        ClassId = students.ClassId,
                        CourseName = myCourse.Name,
                        Gender = myStudent.Student.Gender,
                        GenderDesc = EmGender.GetGenderDesc(myStudent.Student.Gender),
                        StudentId = students.StudentId,
                        StudentName = myStudent.Student.Name,
                        StudentPhone = ComBusiness3.PhoneSecrecy(myStudent.Student.Phone, request.SecrecyType, request.SecrecyDataBag),
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

        private void AnalyzeClassStudent(ClassBucket classBucket)
        {
            if (classBucket == null || classBucket.EtClassStudents == null || classBucket.EtClassStudents.Count == 0)
            {
                return;
            }
            foreach (var myStudent in classBucket.EtClassStudents)
            {
                _eventPublisher.Publish(new SyncStudentClassInfoEvent(myStudent.TenantId)
                {
                    StudentId = myStudent.StudentId
                });
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
                    StartDate = request.StartDate.Value,
                    StartTime = request.StartTime,
                    TeacherIds = request.TeacherIds,
                    Weeks = request.Weeks,
                    CourseIds = request.CourseIds,
                    ReservationType = request.ReservationType,
                    IsJumpTeacherLimit = request.IsJumpTeacherLimit,
                    IsJumpStudentLimit = request.IsJumpStudentLimit,
                    IsJumpClassRoomLimit = request.IsJumpClassRoomLimit,
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

        private async Task<string> CheckClassTimesStudentTimeConflict(List<EtClassStudent> classStudents,
            List<DateTime> myDateList, List<byte> weekDays, int startTime, int endTime)
        {
            if (classStudents == null || classStudents.Count == 0)
            {
                return null;
            }
            var endDate = myDateList.Max();
            var startDate = myDateList.Min();
            var limitStudent = new List<string>();
            var tempStudents = classStudents.Take(2);
            foreach (var itemStudentInfo in tempStudents)
            {
                var studentLimits = await _classDAL.GetClassTimesRuleStudent(itemStudentInfo.StudentId, startTime,
                    endTime, weekDays, startDate, endDate, 100);
                if (studentLimits != null && studentLimits.Any())
                {
                    foreach (var p in studentLimits)
                    {
                        if (myDateList.Exists(j => j == p.ClassOt))
                        {
                            var studentBucket = await _studentDAL.GetStudent(itemStudentInfo.StudentId);
                            if (studentBucket != null && studentBucket.Student != null)
                            {
                                limitStudent.Add(studentBucket.Student.Name);
                            }
                        }
                    }
                }
            }
            if (limitStudent.Count > 0)
            {
                return string.Join(',', limitStudent);
            }
            return null;
        }

        private async Task<string> CheckClassTimesStudentTimeConflict2(List<EtClassStudent> classStudents,
            List<byte> weekDays, int startTime, int endTime, DateTime startDate, DateTime? endDate)
        {
            if (classStudents == null || classStudents.Count == 0)
            {
                return null;
            }
            var tempStudents = classStudents.Take(2);
            var limitStudents = new List<string>();
            foreach (var itemStudentInfo in tempStudents)
            {
                var studentLimit = await _classDAL.GetClassTimesRuleStudent(itemStudentInfo.StudentId, startTime,
                    endTime, weekDays, startDate, endDate, 1);
                if (studentLimit != null && studentLimit.Any())
                {
                    var studentBucket = await _studentDAL.GetStudent(itemStudentInfo.StudentId);
                    if (studentBucket != null && studentBucket.Student != null)
                    {
                        limitStudents.Add(studentBucket.Student.Name);
                    }
                }
            }
            if (limitStudents.Count > 0)
            {
                return string.Join(',', limitStudents);
            }
            return null;
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

            var reqTeacherIds = EtmsHelper.GetMuIds(request.TeacherIds);
            var reqClassRoomIds = EtmsHelper.GetMuIds(request.ClassRoomIds);
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
                    ClassType = etClass.Type,
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
                    StudentIdsClass = preInfo.StudentIdsClass,
                    LimitStudentNums = etClass.LimitStudentNums,
                    LimitStudentNumsIsAlone = false,
                    LimitStudentNumsType = etClass.LimitStudentNumsType,
                    ReservationType = request.ReservationType,
                    StudentIdsReservation = string.Empty,
                    StudentCount = etClass.StudentNums
                });
                indexCount++;
                currentDate = currentDate.AddDays(1);
            };
            if (weekDays.Count == 0)
            {
                return ResponseBase.CommonError("按此规则未生成任何课次，请重新设置");
            }
            //验证是否有时间重叠
            var startDate = request.StartDate.Date;
            var sameTimeRule = await _classDAL.GetClassTimesRule(request.ClassId, request.StartTime, request.EndTime, weekDays);
            if (sameTimeRule.Any())
            {
                var errMsg = "存在有重叠的排课时间，请重新设置";
                var isExist = false;
                foreach (var ruleLog in sameTimeRule)
                {
                    //判断是否是不同的老师和不同的教室
                    if (!string.IsNullOrEmpty(reqTeacherIds) && !string.IsNullOrEmpty(ruleLog.Teachers)) //同时设置了老师
                    {
                        var oneArrayTeachers = EtmsHelper.AnalyzeMuIds(reqTeacherIds);
                        var twoArrayTeachers = EtmsHelper.AnalyzeMuIds(ruleLog.Teachers);
                        isExist = false;
                        foreach (var a1 in oneArrayTeachers)
                        {
                            foreach (var a2 in twoArrayTeachers)
                            {
                                if (a1 == a2)
                                {
                                    isExist = true;
                                    break;
                                }
                            }
                            if (isExist)
                            {
                                break;
                            }
                        }
                        if (!isExist) //老师不重叠，则判断是否在不同的班级
                        {
                            if (string.IsNullOrEmpty(etClass.ClassRoomIds))
                            {
                                continue;
                            }
                            if (!string.IsNullOrEmpty(etClass.ClassRoomIds) && !string.IsNullOrEmpty(reqClassRoomIds) && !string.IsNullOrEmpty(ruleLog.ClassRoomIds))
                            {
                                var oneArrayClassRoom = EtmsHelper.AnalyzeMuIds(reqClassRoomIds);
                                var twoArrayClassRoom = EtmsHelper.AnalyzeMuIds(ruleLog.ClassRoomIds);
                                isExist = false;
                                foreach (var b1 in oneArrayClassRoom)
                                {
                                    foreach (var b2 in twoArrayClassRoom)
                                    {
                                        if (b1 == b2)
                                        {
                                            isExist = true;
                                            break;
                                        }
                                    }
                                    if (isExist)
                                    {
                                        break;
                                    }
                                }
                                if (!isExist)
                                {
                                    continue;  //老师和教室都不同，允许排课
                                }
                            }
                        }
                    }

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

            //判断老师上课时间是否有重叠
            if (!request.IsJumpTeacherLimit)
            {
                List<long> teacherIds = null;
                if (request.TeacherIds != null && request.TeacherIds.Count > 0)
                {
                    teacherIds = request.TeacherIds;
                }
                else
                {
                    teacherIds = EtmsHelper.AnalyzeMuIds(etClass.Teachers);
                }
                if (teacherIds.Count > 0)
                {
                    var limitTeachers = new List<string>();
                    foreach (var myTeacherId in teacherIds)
                    {
                        var teacherLimit = await _classDAL.GetClassTimesRuleTeacher(myTeacherId, request.StartTime,
                            request.EndTime, weekDays, startDate, endDate, 1);
                        if (teacherLimit != null && teacherLimit.Any())
                        {
                            var teacher = await _userDAL.GetUser(myTeacherId);
                            if (teacher != null)
                            {
                                limitTeachers.Add(teacher.Name);
                            }
                        }
                    }
                    if (limitTeachers.Count > 0)
                    {
                        return ResponseBase.Success(new ClassTimesRuleAddOutput()
                        {
                            IsLimit = true,
                            LimitTeacherName = string.Join(',', limitTeachers)
                        });
                    }
                }
            }

            if (!request.IsJumpStudentLimit)
            {
                var strStudentConflict = await CheckClassTimesStudentTimeConflict2(etClassBucket.EtClassStudents, weekDays, request.StartTime,
                    request.EndTime, startDate, endDate);
                if (!string.IsNullOrEmpty(strStudentConflict))
                {
                    return ResponseBase.Success(new ClassTimesRuleAddOutput()
                    {
                        IsLimitStudent = true,
                        LimitStudentName = strStudentConflict
                    });
                }
            }

            //判断教室是否重叠
            if (!request.IsJumpClassRoomLimit)
            {
                List<long> myClassRoomIds = null;
                if (request.ClassRoomIds != null && request.ClassRoomIds.Count > 0)
                {
                    myClassRoomIds = request.ClassRoomIds;
                }
                else
                {
                    myClassRoomIds = EtmsHelper.AnalyzeMuIds(etClass.ClassRoomIds);
                }
                if (myClassRoomIds.Count > 0)
                {
                    var allClassRoom = await _classRoomDAL.GetAllClassRoom();
                    var limitClassRooms = new List<string>();
                    foreach (var myClassRoomId in myClassRoomIds)
                    {
                        var classRoomLimit = await _classDAL.GetClassTimesRuleClassRoom(myClassRoomId, request.StartTime,
                            request.EndTime, weekDays, startDate, endDate, 1);
                        if (classRoomLimit != null && classRoomLimit.Any())
                        {
                            var classRoom = allClassRoom.FirstOrDefault(j => j.Id == myClassRoomId);
                            if (classRoom != null)
                            {
                                limitClassRooms.Add(classRoom.Name);
                            }
                        }
                    }
                    if (limitClassRooms.Count > 0)
                    {
                        return ResponseBase.Success(new ClassTimesRuleAddOutput()
                        {
                            IsLimitClassRoom = true,
                            LimitClassRoomName = string.Join(',', limitClassRooms)
                        });
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
                    ClassRoomIds = reqClassRoomIds,
                    StartDate = request.StartDate,
                    EndDate = endDate,
                    Week = week,
                    StartTime = request.StartTime,
                    EndTime = request.EndTime,
                    IsDeleted = EmIsDeleted.Normal,
                    IsJumpHoliday = request.IsJumpHoliday,
                    Teachers = reqTeacherIds,
                    CourseList = EtmsHelper.GetMuIds(request.CourseIds),
                    LastJobProcessTime = currentDate.AddDays(-1),
                    TenantId = request.LoginTenantId,
                    DateDesc = dateDesc,
                    TimeDesc = $"{EtmsHelper.GetTimeDesc(request.StartTime)}~{EtmsHelper.GetTimeDesc(request.EndTime)}",
                    Type = type,
                    RuleDesc = type == EmClassTimesRuleType.Loop ? "每周循环" : "单次",
                    ReservationType = request.ReservationType
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
            this.AnalyzeClassStudent(etClassBucket);

            await _userOperationLogDAL.AddUserLog(request, $"快速排课-班级[{etClassBucket.EtClass.Name}]排课", EmUserOperationType.ClassManage);
            return ResponseBase.Success(new ClassTimesRuleAddOutput() { IsLimit = false });
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
            var etClass = etClassBucket.EtClass;

            var reqTeacherIds = EtmsHelper.GetMuIds(request.TeacherIds);
            var reqClassRoomIds = EtmsHelper.GetMuIds(request.ClassRoomIds);
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
            var currentDate = request.StartDate.Value.Date;
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
                    ClassType = etClass.Type,
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
                    TeachersIsAlone = preInfo.TeachersIsAlone,
                    LimitStudentNums = etClass.LimitStudentNums,
                    StudentIdsReservation = string.Empty,
                    LimitStudentNumsIsAlone = false,
                    LimitStudentNumsType = etClass.LimitStudentNumsType,
                    ReservationType = request.ReservationType,
                    StudentCount = etClass.StudentNums
                });
                indexCount++;
                currentDate = currentDate.AddDays(1);
            };
            if (weekDays.Count == 0)
            {
                return ResponseBase.CommonError("按此规则未生成任何课次，请重新设置");
            }
            var endDate = currentDate.AddDays(-1);
            var startDate = request.StartDate.Value.Date;
            //验证是否有时间重叠
            var sameTimeRule = await _classDAL.GetClassTimesRule(request.ClassId, request.StartTime, request.EndTime, weekDays);
            if (sameTimeRule.Any())
            {
                var errMsg = "存在有重叠的排课时间，请重新设置";
                var isExist = false;
                foreach (var ruleLog in sameTimeRule)
                {
                    //判断是否是不同的老师和不同的教室
                    if (!string.IsNullOrEmpty(reqTeacherIds) && !string.IsNullOrEmpty(ruleLog.Teachers)) //同时设置了老师
                    {
                        var oneArrayTeachers = EtmsHelper.AnalyzeMuIds(reqTeacherIds);
                        var twoArrayTeachers = EtmsHelper.AnalyzeMuIds(ruleLog.Teachers);
                        isExist = false;
                        foreach (var a1 in oneArrayTeachers)
                        {
                            foreach (var a2 in twoArrayTeachers)
                            {
                                if (a1 == a2)
                                {
                                    isExist = true;
                                    break;
                                }
                            }
                            if (isExist)
                            {
                                break;
                            }
                        }
                        if (!isExist) //老师不重叠，则判断是否在不同的班级
                        {
                            if (string.IsNullOrEmpty(etClass.ClassRoomIds))
                            {
                                continue;
                            }
                            if (!string.IsNullOrEmpty(etClass.ClassRoomIds) && !string.IsNullOrEmpty(reqClassRoomIds) && !string.IsNullOrEmpty(ruleLog.ClassRoomIds))
                            {
                                var oneArrayClassRoom = EtmsHelper.AnalyzeMuIds(reqClassRoomIds);
                                var twoArrayClassRoom = EtmsHelper.AnalyzeMuIds(ruleLog.ClassRoomIds);
                                isExist = false;
                                foreach (var b1 in oneArrayClassRoom)
                                {
                                    foreach (var b2 in twoArrayClassRoom)
                                    {
                                        if (b1 == b2)
                                        {
                                            isExist = true;
                                            break;
                                        }
                                    }
                                    if (isExist)
                                    {
                                        break;
                                    }
                                }
                                if (!isExist)
                                {
                                    continue;  //老师和教室都不同，允许排课
                                }
                            }
                        }
                    }

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

            //判断老师上课时间是否有重叠
            if (!request.IsJumpTeacherLimit)
            {
                var teacherIds = new List<long>();
                if (request.TeacherIds != null && request.TeacherIds.Count > 0)
                {
                    teacherIds = request.TeacherIds;
                }
                else
                {
                    teacherIds = EtmsHelper.AnalyzeMuIds(etClass.Teachers);
                }
                if (teacherIds.Count > 0)
                {
                    var limitTeachers = new List<string>();
                    foreach (var myTeacherId in teacherIds)
                    {
                        var teacherLimit = await _classDAL.GetClassTimesRuleTeacher(myTeacherId, request.StartTime,
                            request.EndTime, weekDays, startDate, endDate, 1);
                        if (teacherLimit != null && teacherLimit.Any())
                        {
                            var teacher = await _userDAL.GetUser(myTeacherId);
                            if (teacher != null)
                            {
                                limitTeachers.Add(teacher.Name);
                            }
                        }
                    }
                    if (limitTeachers.Count > 0)
                    {
                        return ResponseBase.Success(new ClassTimesRuleAddOutput()
                        {
                            IsLimit = true,
                            LimitTeacherName = string.Join(',', limitTeachers)
                        });
                    }
                }
            }

            if (!request.IsJumpStudentLimit)
            {
                var strStudentConflict = await CheckClassTimesStudentTimeConflict2(etClassBucket.EtClassStudents, weekDays, request.StartTime,
                    request.EndTime, startDate, endDate);
                if (!string.IsNullOrEmpty(strStudentConflict))
                {
                    return ResponseBase.Success(new ClassTimesRuleAddOutput()
                    {
                        IsLimitStudent = true,
                        LimitStudentName = strStudentConflict
                    });
                }
            }

            //判断教室是否重叠
            if (!request.IsJumpClassRoomLimit)
            {
                List<long> myClassRoomIds = null;
                if (request.ClassRoomIds != null && request.ClassRoomIds.Count > 0)
                {
                    myClassRoomIds = request.ClassRoomIds;
                }
                else
                {
                    myClassRoomIds = EtmsHelper.AnalyzeMuIds(etClass.ClassRoomIds);
                }
                if (myClassRoomIds.Count > 0)
                {
                    var allClassRoom = await _classRoomDAL.GetAllClassRoom();
                    var limitClassRooms = new List<string>();
                    foreach (var myClassRoomId in myClassRoomIds)
                    {
                        var classRoomLimit = await _classDAL.GetClassTimesRuleClassRoom(myClassRoomId, request.StartTime,
                            request.EndTime, weekDays, startDate, endDate, 1);
                        if (classRoomLimit != null && classRoomLimit.Any())
                        {
                            var classRoom = allClassRoom.FirstOrDefault(j => j.Id == myClassRoomId);
                            if (classRoom != null)
                            {
                                limitClassRooms.Add(classRoom.Name);
                            }
                        }
                    }
                    if (limitClassRooms.Count > 0)
                    {
                        return ResponseBase.Success(new ClassTimesRuleAddOutput()
                        {
                            IsLimitClassRoom = true,
                            LimitClassRoomName = string.Join(',', limitClassRooms)
                        });
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
                    ClassRoomIds = reqClassRoomIds,
                    StartDate = request.StartDate.Value,
                    EndDate = endDate,
                    Week = week,
                    StartTime = request.StartTime,
                    EndTime = request.EndTime,
                    IsDeleted = EmIsDeleted.Normal,
                    IsJumpHoliday = request.IsJumpHoliday,
                    Teachers = reqTeacherIds,
                    CourseList = EtmsHelper.GetMuIds(request.CourseIds),
                    LastJobProcessTime = currentDate.AddDays(-1),
                    TenantId = request.LoginTenantId,
                    DateDesc = dateDesc,
                    TimeDesc = $"{EtmsHelper.GetTimeDesc(request.StartTime)}~{EtmsHelper.GetTimeDesc(request.EndTime)}",
                    Type = type,
                    RuleDesc = type == EmClassTimesRuleType.Loop ? "每周循环" : "单次",
                    ReservationType = request.ReservationType
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
            this.AnalyzeClassStudent(etClassBucket);

            await _userOperationLogDAL.AddUserLog(request, $"快速排课-班级[{etClassBucket.EtClass.Name}]排课", EmUserOperationType.ClassManage);
            return ResponseBase.Success(new ClassTimesRuleAddOutput() { IsLimit = false });
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
            var etClass = etClassBucket.EtClass;
            var reqTeacherIds = EtmsHelper.GetMuIds(request.TeacherIds);
            var reqClassRoomIds = EtmsHelper.GetMuIds(request.ClassRoomIds);

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
            var myDateList = new List<DateTime>();
            foreach (var strDate in request.ClassDate)
            {
                var currentDate = Convert.ToDateTime(strDate).Date;
                myDateList.Add(currentDate);
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
                    ClassType = etClass.Type,
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
                    TeachersIsAlone = preInfo.TeachersIsAlone,
                    LimitStudentNums = etClass.LimitStudentNums,
                    LimitStudentNumsIsAlone = false,
                    LimitStudentNumsType = etClass.LimitStudentNumsType,
                    StudentCount = etClass.StudentNums,
                    ReservationType = request.ReservationType,
                    StudentIdsReservation = string.Empty
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
                var isExist = false;
                foreach (var ruleLog in sameTimeRule)
                { //判断是否是不同的老师和不同的教室
                    if (!string.IsNullOrEmpty(reqTeacherIds) && !string.IsNullOrEmpty(ruleLog.Teachers)) //同时设置了老师
                    {
                        var oneArrayTeachers = EtmsHelper.AnalyzeMuIds(reqTeacherIds);
                        var twoArrayTeachers = EtmsHelper.AnalyzeMuIds(ruleLog.Teachers);
                        isExist = false;
                        foreach (var a1 in oneArrayTeachers)
                        {
                            foreach (var a2 in twoArrayTeachers)
                            {
                                if (a1 == a2)
                                {
                                    isExist = true;
                                    break;
                                }
                            }
                            if (isExist)
                            {
                                break;
                            }
                        }
                        if (!isExist) //老师不重叠，则判断是否在不同的班级
                        {
                            if (string.IsNullOrEmpty(etClass.ClassRoomIds))
                            {
                                continue;
                            }
                            if (!string.IsNullOrEmpty(etClass.ClassRoomIds) && !string.IsNullOrEmpty(reqClassRoomIds) && !string.IsNullOrEmpty(ruleLog.ClassRoomIds))
                            {
                                var oneArrayClassRoom = EtmsHelper.AnalyzeMuIds(reqClassRoomIds);
                                var twoArrayClassRoom = EtmsHelper.AnalyzeMuIds(ruleLog.ClassRoomIds);
                                isExist = false;
                                foreach (var b1 in oneArrayClassRoom)
                                {
                                    foreach (var b2 in twoArrayClassRoom)
                                    {
                                        if (b1 == b2)
                                        {
                                            isExist = true;
                                            break;
                                        }
                                    }
                                    if (isExist)
                                    {
                                        break;
                                    }
                                }
                                if (!isExist)
                                {
                                    continue;  //老师和教室都不同，允许排课
                                }
                            }
                        }
                    }
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

            //判断老师上课时间是否有重叠
            if (!request.IsJumpTeacherLimit)
            {
                List<long> teacherIds = null;
                if (request.TeacherIds != null && request.TeacherIds.Count > 0)
                {
                    teacherIds = request.TeacherIds;
                }
                else
                {
                    teacherIds = EtmsHelper.AnalyzeMuIds(etClass.Teachers);
                }
                if (teacherIds.Count > 0)
                {
                    var endDate = myDateList.Max();
                    var startDate = myDateList.Min();
                    var limitTeachers = new List<string>();
                    foreach (var myTeacherId in teacherIds)
                    {
                        var teacherLimit = await _classDAL.GetClassTimesRuleTeacher(myTeacherId, request.StartTime,
                            request.EndTime, weekDays, startDate, endDate, 100);
                        if (teacherLimit != null && teacherLimit.Any())
                        {
                            foreach (var p in teacherLimit)
                            {
                                if (myDateList.Exists(j => j == p.ClassOt))
                                {
                                    var teacher = await _userDAL.GetUser(myTeacherId);
                                    if (teacher != null)
                                    {
                                        limitTeachers.Add(teacher.Name);
                                    }
                                }
                            }
                        }
                    }
                    if (limitTeachers.Count > 0)
                    {
                        return ResponseBase.Success(new ClassTimesRuleAddOutput()
                        {
                            IsLimit = true,
                            LimitTeacherName = string.Join(',', limitTeachers)
                        });
                    }
                }
            }

            if (!request.IsJumpStudentLimit)
            {
                var strStudentConflict = await CheckClassTimesStudentTimeConflict(etClassBucket.EtClassStudents, myDateList, weekDays, request.StartTime,
                    request.EndTime);
                if (!string.IsNullOrEmpty(strStudentConflict))
                {
                    return ResponseBase.Success(new ClassTimesRuleAddOutput()
                    {
                        IsLimitStudent = true,
                        LimitStudentName = strStudentConflict
                    });
                }
            }

            //判断教室是否重叠
            if (!request.IsJumpClassRoomLimit)
            {
                List<long> myClassRoomIds = null;
                if (request.ClassRoomIds != null && request.ClassRoomIds.Count > 0)
                {
                    myClassRoomIds = request.ClassRoomIds;
                }
                else
                {
                    myClassRoomIds = EtmsHelper.AnalyzeMuIds(etClass.ClassRoomIds);
                }
                if (myClassRoomIds.Count > 0)
                {
                    var endDate = myDateList.Max();
                    var startDate = myDateList.Min();
                    var allClassRoom = await _classRoomDAL.GetAllClassRoom();
                    var limitClassRooms = new List<string>();
                    foreach (var myClassRoomId in myClassRoomIds)
                    {
                        var classRoomLimit = await _classDAL.GetClassTimesRuleClassRoom(myClassRoomId, request.StartTime,
                            request.EndTime, weekDays, startDate, endDate, 100);
                        if (classRoomLimit != null && classRoomLimit.Any())
                        {
                            foreach (var p in classRoomLimit)
                            {
                                if (myDateList.Exists(j => j == p.ClassOt))
                                {
                                    var classRoom = allClassRoom.FirstOrDefault(j => j.Id == myClassRoomId);
                                    if (classRoom != null)
                                    {
                                        limitClassRooms.Add(classRoom.Name);
                                    }
                                }
                            }
                        }
                    }
                    if (limitClassRooms.Count > 0)
                    {
                        return ResponseBase.Success(new ClassTimesRuleAddOutput()
                        {
                            IsLimitClassRoom = true,
                            LimitClassRoomName = string.Join(',', limitClassRooms)
                        });
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
                    ClassRoomIds = reqClassRoomIds,
                    StartDate = myTime.ClassOt,
                    EndDate = myTime.ClassOt,
                    Week = myTime.Week,
                    StartTime = request.StartTime,
                    EndTime = request.EndTime,
                    IsDeleted = EmIsDeleted.Normal,
                    IsJumpHoliday = request.IsJumpHoliday,
                    Teachers = reqTeacherIds,
                    CourseList = EtmsHelper.GetMuIds(request.CourseIds),
                    LastJobProcessTime = myTime.ClassOt,
                    TenantId = request.LoginTenantId,
                    DateDesc = dateDesc,
                    TimeDesc = $"{EtmsHelper.GetTimeDesc(request.StartTime)}~{EtmsHelper.GetTimeDesc(request.EndTime)}",
                    Type = EmClassTimesRuleType.UnLoop,
                    RuleDesc = "单次",
                    ReservationType = request.ReservationType
                };
                var ruleId = await _classDAL.AddClassTimesRule(request.ClassId, classTimesRule);
                myTime.RuleId = ruleId;
            }
            _classTimesDAL.AddClassTimes(classTimes);

            //更新班级&日志
            await _classDAL.UpdateClassPlanTimes(request.ClassId, EmClassScheduleStatus.Scheduled);
            _eventPublisher.Publish(new ResetTenantToDoThingEvent(request.LoginTenantId));
            this.AnalyzeClassStudent(etClassBucket);

            await _userOperationLogDAL.AddUserLog(request, $"快速排课-班级[{etClassBucket.EtClass.Name}]排课", EmUserOperationType.ClassManage);
            return ResponseBase.Success(new ClassTimesRuleAddOutput() { IsLimit = false });
        }

        public async Task<ResponseBase> ClassTimesRuleDel(ClassTimesRuleDelRequest request)
        {
            var etClassBucket = await _classDAL.GetClassBucket(request.ClassId);
            if (etClassBucket == null || etClassBucket.EtClass == null)
            {
                return ResponseBase.CommonError("班级不存在");
            }
            await _classDAL.DelClassTimesRule(request.ClassId, request.RuleId);
            this.AnalyzeClassStudent(etClassBucket);

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
            if (request.DelStudentId > 0)
            {
                etClassStudents = etClassStudents.Where(p => p.StudentId != request.DelStudentId).ToList();
            }
            var studentCount = etClassStudents.Count;
            if (etClassStudents.Any())
            {
                //去除重复项
                var myStudentIds = new List<long>();
                foreach (var item in etClassStudents)
                {
                    if (myStudentIds.Exists(j => j == item.StudentId))
                    {
                        //删除重复项
                        await _classDAL.DelClassStudent(request.ClassId, item.Id);
                        continue;
                    }
                    myStudentIds.Add(item.StudentId);
                }

                studentIds = EtmsHelper.GetMuIds(myStudentIds);
                studentCount = myStudentIds.Count();
            }
            await _classDAL.ClassEditStudentInfo(request.ClassId, studentIds, studentCount);

            //同步课次信息（授课课程、教室、老师、班级学员） 
            await _classDAL.SyncClassStudentInfo(request.ClassId, studentIds, etClass.CourseList,
                etClass.ClassRoomIds, etClass.Teachers, etClass.TeacherNum, etClass.LimitStudentNums, etClass.LimitStudentNumsType,
                studentCount);

            var sqlList = _classDAL.GetSyncClassInfoSql(request.ClassId, studentIds, etClass.CourseList,
                etClass.ClassRoomIds, etClass.Teachers, etClass.TeacherNum, etClass.LimitStudentNums, etClass.LimitStudentNumsType,
                studentCount);
            foreach (var s in sqlList)
            {
                _eventPublisher.Publish(new ComSqlHandleEvent(request.TenantId)
                {
                    Sql = s
                });
            }

            var isSetRuleIds = await _classTimesRuleStudentDAL.GetIsSetRuleStudent(request.ClassId);
            if (isSetRuleIds.Any())
            {
                foreach (var p in isSetRuleIds)
                {
                    _eventPublisher.Publish(new SyncClassTimesRuleStudentInfoEvent(request.TenantId)
                    {
                        ClassId = request.ClassId,
                        RuleId = p.RuleId
                    });
                }
            }
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
                        IsJumpHoliday = rule.IsJumpHoliday,
                        ReservationType = rule.ReservationType,
                        DataType = rule.DataType,
                        DataTypeDesc = EmClassTimesDataType.GetClassTimesDataTypeDesc(rule.DataType)
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
            await _classTimesRuleStudentDAL.DelClassTimesRuleStudentByStudentId(request.StudentId, request.ClassId);
            _eventPublisher.Publish(new SyncClassInfoEvent(request.LoginTenantId, request.ClassId));

            //新班级
            await _classDAL.AddClassStudent(new EtClassStudent()
            {
                ClassId = request.NewClassId,
                CourseId = request.CourseId,
                IsDeleted = EmIsDeleted.Normal,
                Remark = string.Empty,
                StudentId = request.StudentId,
                TenantId = request.LoginTenantId,
                Type = newClass.EtClass.Type
            });
            _eventPublisher.Publish(new SyncStudentStudentClassIdsEvent(request.LoginTenantId, request.StudentId));
            _eventPublisher.Publish(new SyncClassInfoEvent(request.LoginTenantId, request.NewClassId));
            _eventPublisher.Publish(new SyncStudentClassInfoEvent(request.LoginTenantId)
            {
                StudentId = request.StudentId
            });

            await _userOperationLogDAL.AddUserLog(request, $"调至其他班-将学员[{studentBucket.Student.Name}]从班级[{oldClass.EtClass.Name}]调至[{newClass.EtClass.Name}]", EmUserOperationType.ClassManage);
            return ResponseBase.Success();
        }

        public async Task<ResponseBase> ClassPlacement(ClassPlacementRequest request)
        {
            var studentBucket = await _studentDAL.GetStudent(request.StudentId);
            if (studentBucket == null || studentBucket.Student == null)
            {
                return ResponseBase.CommonError("学员不存在");
            }
            var courseBucket = await _courseDAL.GetCourse(request.CourseId);
            if (courseBucket == null || courseBucket.Item1 == null)
            {
                return ResponseBase.CommonError("课程不存在");
            }

            var errMsg = string.Empty;
            var isChanged = false;
            foreach (var placementInfo in request.ClassPlacementInfos)
            {
                var myClassBucket = await _classDAL.GetClassBucket(placementInfo.ClassId);
                if (myClassBucket == null || myClassBucket.EtClass == null)
                {
                    LOG.Log.Error("[ClassPlacement]班级不存在", request, this.GetType());
                    continue;
                }
                var myClass = myClassBucket.EtClass;
                var existStudent = myClassBucket.EtClassStudents.FirstOrDefault(p => p.StudentId == request.StudentId);
                if (placementInfo.OpType == ClassPlacementInfoOpType.Add)
                {
                    if (existStudent != null)
                    {
                        continue;
                    }
                    if (myClassBucket.EtClass.CourseList.IndexOf(request.CourseId.ToString()) == -1)
                    {
                        LOG.Log.Error("[ClassPlacement]班级未关联课程", request, this.GetType());
                        continue;
                    }
                    if (myClass.LimitStudentNums != null && myClass.LimitStudentNumsType == EmLimitStudentNumsType.NotOverflow)
                    {
                        if (myClassBucket.EtClassStudents.Count + 1 > myClass.LimitStudentNums.Value)
                        {
                            errMsg = "班级人数已超额";
                            continue;
                        }
                    }
                    isChanged = true;
                    await _classDAL.AddClassStudent(new EtClassStudent()
                    {
                        ClassId = placementInfo.ClassId,
                        CourseId = request.CourseId,
                        IsDeleted = EmIsDeleted.Normal,
                        Remark = string.Empty,
                        StudentId = request.StudentId,
                        TenantId = request.LoginTenantId,
                        Type = myClassBucket.EtClass.Type
                    });
                    _eventPublisher.Publish(new SyncStudentStudentClassIdsEvent(request.LoginTenantId, request.StudentId));
                }
                else
                {
                    if (existStudent == null)
                    {
                        continue;
                    }
                    isChanged = true;
                    await _classDAL.DelClassStudentByStudentId(placementInfo.ClassId, request.StudentId);
                    _eventPublisher.Publish(new SyncStudentStudentClassIdsEvent(request.LoginTenantId, request.StudentId));
                }
                _eventPublisher.Publish(new SyncClassInfoEvent(request.LoginTenantId, placementInfo.ClassId));
            }
            _eventPublisher.Publish(new SyncStudentClassInfoEvent(request.LoginTenantId)
            {
                StudentId = request.StudentId
            });

            if (!isChanged && !string.IsNullOrEmpty(errMsg))
            {
                return ResponseBase.CommonError(errMsg);
            }

            await _userOperationLogDAL.AddUserLog(request, $"选班调班-学员名称[{studentBucket.Student.Name}],手机号码[{studentBucket.Student.Phone}]", EmUserOperationType.ClassManage);
            return ResponseBase.Success();
        }

        public async Task<ResponseBase> ClassTimesRuleDefiniteGet(ClassTimesRuleDefiniteGetRequest request)
        {
            var classRule = await _classDAL.GetClassTimesRuleBuyId(request.ClassRuleId);
            if (classRule == null)
            {
                return ResponseBase.CommonError("排课信息未找到");
            }
            var output = new ClassTimesRuleDefiniteGetOutput()
            {
                ClassContent = classRule.ClassContent,
                ClassId = classRule.ClassId,
                ClassRoomIds = EtmsHelper.AnalyzeMuIds(classRule.ClassRoomIds),
                CourseIds = EtmsHelper.AnalyzeMuIds(classRule.CourseList),
                StartTimeDesc = EtmsHelper.GetTimeDesc(classRule.StartTime),
                EndTimeDesc = EtmsHelper.GetTimeDesc(classRule.EndTime),
                IsJumpHoliday = classRule.IsJumpHoliday,
                Remark = classRule.Remark,
                ReservationType = classRule.ReservationType,
                TeacherIds = EtmsHelper.AnalyzeMuIds(classRule.Teachers),
                Id = classRule.Id
            };
            return ResponseBase.Success(output);
        }

        public async Task<ResponseBase> ClassTimesRuleEdit(ClassTimesRuleEditRequest request)
        {
            var classRule = await _classDAL.GetClassTimesRuleBuyId(request.ClassRuleId);
            if (classRule == null)
            {
                return ResponseBase.CommonError("排课信息未找到");
            }
            var etClassBucket = await _classDAL.GetClassBucket(classRule.ClassId);
            if (etClassBucket == null || etClassBucket.EtClass == null)
            {
                return ResponseBase.CommonError("班级不存在");
            }

            //判断老师上课时间是否有重叠
            var weekDays = new List<byte>() { classRule.Week };
            if (!request.IsJumpTeacherLimit)
            {
                if (request.TeacherIds != null && request.TeacherIds.Count > 0)
                {
                    var limitTeachers = new List<string>();
                    foreach (var myTeacherId in request.TeacherIds)
                    {
                        var teacherLimit = await _classDAL.GetClassTimesRuleTeacher(myTeacherId, request.StartTime,
                            request.EndTime, weekDays, classRule.StartDate, classRule.EndDate, 1, request.ClassRuleId);
                        if (teacherLimit != null && teacherLimit.Any())
                        {
                            var teacher = await _userDAL.GetUser(myTeacherId);
                            if (teacher != null)
                            {
                                limitTeachers.Add(teacher.Name);
                            }
                        }
                    }
                    if (limitTeachers.Count > 0)
                    {
                        return ResponseBase.Success(new ClassTimesRuleEditOutput()
                        {
                            IsLimit = true,
                            LimitTeacherName = string.Join(',', limitTeachers)
                        });
                    }
                }
            }

            if (!request.IsJumpStudentLimit)
            {
                var strStudentConflict = await CheckClassTimesStudentTimeConflict2(etClassBucket.EtClassStudents, weekDays, request.StartTime,
                    request.EndTime, classRule.StartDate, classRule.EndDate);
                if (!string.IsNullOrEmpty(strStudentConflict))
                {
                    return ResponseBase.Success(new ClassTimesRuleAddOutput()
                    {
                        IsLimitStudent = true,
                        LimitStudentName = strStudentConflict
                    });
                }
            }

            if (!request.IsJumpClassRoomLimit)
            {
                if (request.ClassRoomIds != null && request.ClassRoomIds.Count > 0)
                {
                    var allClassRooms = await _classRoomDAL.GetAllClassRoom();
                    var limitClassRooms = new List<string>();
                    foreach (var myClassRoomId in request.ClassRoomIds)
                    {
                        var classRoomLimit = await _classDAL.GetClassTimesRuleClassRoom(myClassRoomId, request.StartTime,
                            request.EndTime, weekDays, classRule.StartDate, classRule.EndDate, 1, request.ClassRuleId);
                        if (classRoomLimit != null && classRoomLimit.Any())
                        {
                            var myClassRoom = allClassRooms.FirstOrDefault(j => j.Id == myClassRoomId);
                            if (myClassRoom != null)
                            {
                                limitClassRooms.Add(myClassRoom.Name);
                            }
                        }
                    }
                    if (limitClassRooms.Count > 0)
                    {
                        return ResponseBase.Success(new ClassTimesRuleEditOutput()
                        {
                            IsLimitClassRoom = true,
                            LimitClassRoomName = string.Join(',', limitClassRooms)
                        });
                    }
                }
            }

            classRule.IsJumpHoliday = request.IsJumpHoliday;
            classRule.StartTime = request.StartTime;
            classRule.EndTime = request.EndTime;
            classRule.TimeDesc = $"{EtmsHelper.GetTimeDesc(request.StartTime)}~{EtmsHelper.GetTimeDesc(request.EndTime)}";
            classRule.ClassRoomIds = EtmsHelper.GetMuIds(request.ClassRoomIds);
            classRule.Teachers = EtmsHelper.GetMuIds(request.TeacherIds);
            classRule.CourseList = EtmsHelper.GetMuIds(request.CourseIds);
            classRule.ClassContent = request.ClassContent;
            classRule.ReservationType = request.ReservationType;
            await _classDAL.EditClassTimesRule(classRule);
            await _classTimesDAL.SyncClassTimesOfClassTimesRule(classRule);

            await _userOperationLogDAL.AddUserLog(request, $"编辑排课-{etClassBucket.EtClass.Name}", EmUserOperationType.ClassManage);
            return ResponseBase.Success(new ClassTimesRuleEditOutput() { IsLimit = false });
        }

        public async Task<ResponseBase> ClassChangeOnlineSelStatus(ClassChangeOnlineSelStatusRequest request)
        {
            await _classDAL.ChangeClassOnlineSelClassStatus(request.Ids, request.NewOnlineSelStatus);
            await _userOperationLogDAL.AddUserLog(request, "批量编辑在线选班", EmUserOperationType.ClassManage);
            return ResponseBase.Success();
        }

        public async Task<ResponseBase> ClassTimesRuleChangeDataType(ClassTimesRuleChangeDataTypeRequest request)
        {
            var classRule = await _classDAL.GetClassTimesRuleBuyId(request.ClassRuleId);
            if (classRule == null)
            {
                return ResponseBase.CommonError("排课信息未找到");
            }
            byte newDataType = classRule.DataType == EmClassTimesDataType.Normal ? EmClassTimesDataType.Stop : EmClassTimesDataType.Normal;
            await _classDAL.UpdateClassTimesRuleDataType(classRule.Id, newDataType);
            await _classTimesDAL.UpdateClassTimesDataType(classRule.Id, newDataType);

            var desc = newDataType == EmClassTimesDataType.Stop ? "班级排课-暂停课表" : "班级排课-恢复课表";
            if (!string.IsNullOrEmpty(request.Remark))
            {
                desc = $"{desc}：{request.Remark}";
            }
            await _userOperationLogDAL.AddUserLog(request, desc, EmUserOperationType.ClassManage);
            return ResponseBase.Success(new ClassTimesRuleEditOutput() { IsLimit = false });
        }

        public async Task<ResponseBase> ClassTimesRuleChangeDataTypeBatch(ClassTimesRuleChangeDataTypeBatchRequest request)
        {
            await _classDAL.UpdateClassTimesRuleDataType(request.ClassRuleIds, request.NewDataType);
            await _classTimesDAL.UpdateClassTimesDataType(request.ClassRuleIds, request.NewDataType);

            var desc = request.NewDataType == EmClassTimesDataType.Stop ? "班级排课-批量暂停课表" : "班级排课-批量恢复课表";
            if (!string.IsNullOrEmpty(request.Remark))
            {
                desc = $"{desc}：{request.Remark}";
            }
            await _userOperationLogDAL.AddUserLog(request, desc, EmUserOperationType.ClassManage);
            return ResponseBase.Success(new ClassTimesRuleEditOutput() { IsLimit = false });
        }

        public async Task<ResponseBase> ClassTimesRuleStudentGet(ClassTimesRuleStudentGetRequest request)
        {
            var output = new List<ClassTimesRuleStudentGetOutput>();
            var myStudents = await _classTimesRuleStudentDAL.GetClassTimesRuleStudent(request.ClassId, request.RuleId);
            if (myStudents != null && myStudents.Any())
            {
                var tempBoxCourse = new DataTempBox<EtCourse>();
                foreach (var item in myStudents)
                {
                    var myStudent = await _studentDAL.GetStudent(item.StudentId);
                    if (myStudent == null)
                    {
                        continue;
                    }
                    var myCourse = await ComBusiness.GetCourse(tempBoxCourse, _courseDAL, item.CourseId);
                    if (myCourse == null)
                    {
                        continue;
                    }
                    var studentCourse = await _studentCourseDAL.GetStudentCourse(item.StudentId, item.CourseId);
                    output.Add(new ClassTimesRuleStudentGetOutput()
                    {
                        CourseId = item.CourseId,
                        ClassId = item.ClassId,
                        CourseName = myCourse.Name,
                        Gender = myStudent.Student.Gender,
                        GenderDesc = EmGender.GetGenderDesc(myStudent.Student.Gender),
                        StudentId = item.StudentId,
                        StudentName = myStudent.Student.Name,
                        StudentPhone = ComBusiness3.PhoneSecrecy(myStudent.Student.Phone, request.SecrecyType, request.SecrecyDataBag),
                        CourseSurplusDesc = ComBusiness.GetStudentCourseDesc(studentCourse),
                        Id = item.Id,
                        RuleId = item.RuleId
                    });
                }
            }
            return ResponseBase.Success(output);
        }

        public async Task<ResponseBase> ClassTimesRuleStudentAdd(ClassTimesRuleStudentAddRequest request)
        {
            var classBucket = await _classDAL.GetClassBucket(request.ClassId);
            if (classBucket == null || classBucket.EtClass == null)
            {
                return ResponseBase.CommonError("班级不存在");
            }

            var myClass = classBucket.EtClass;
            var addEntitys = new List<EtClassTimesRuleStudent>();
            foreach (var p in request.StudentItems)
            {
                var isExist = await _classTimesRuleStudentDAL.ExistStudent(p.StudentId, request.ClassId, request.RuleId);
                if (isExist)
                {
                    continue;
                }
                var newExist = addEntitys.Exists(a => a.StudentId == p.StudentId);
                if (newExist)
                {
                    continue;
                }

                addEntitys.Add(new EtClassTimesRuleStudent()
                {
                    CourseId = p.CourseId,
                    ClassId = request.ClassId,
                    IsDeleted = myClass.IsDeleted,
                    Remark = string.Empty,
                    RuleId = request.RuleId,
                    StudentId = p.StudentId,
                    TenantId = myClass.TenantId,
                    Type = myClass.Type
                });
            }
            if (addEntitys.Count == 0)
            {
                return ResponseBase.CommonError("学员已存在");
            }

            await _classTimesRuleStudentDAL.AddClassTimesRuleStudent(addEntitys);

            _eventPublisher.Publish(new SyncClassTimesRuleStudentInfoEvent(request.LoginTenantId)
            {
                ClassId = request.ClassId,
                RuleId = request.RuleId
            });

            var studetnNames = string.Join(',', request.StudentItems.Select(p => p.StudentName));
            await _userOperationLogDAL.AddUserLog(request, $"班级排课添加上课学员-班级:{classBucket.EtClass.Name},学员:{studetnNames}", EmUserOperationType.ClassManage);
            return ResponseBase.Success(new ClassTimesRuleEditOutput() { IsLimit = false });
        }

        public async Task<ResponseBase> ClassTimesRuleStudentRemove(ClassTimesRuleStudentRemoveRequest request)
        {
            var classBucket = await _classDAL.GetClassBucket(request.ClassId);
            if (classBucket == null || classBucket.EtClass == null)
            {
                return ResponseBase.CommonError("班级不存在");
            }
            await _classTimesRuleStudentDAL.DelClassTimesRuleStudent(request.Id, request.ClassId, request.RuleId);

            _eventPublisher.Publish(new SyncClassTimesRuleStudentInfoEvent(request.LoginTenantId)
            {
                ClassId = request.ClassId,
                RuleId = request.RuleId
            });
            await _userOperationLogDAL.AddUserLog(request, $"班级排课移除上课学员-班级:{classBucket.EtClass.Name},学员:{request.StudentName}", EmUserOperationType.ClassManage);
            return ResponseBase.Success(new ClassTimesRuleEditOutput() { IsLimit = false });
        }

        public async Task<ResponseBase> ClassTimesRuleStudentBatchSet(ClassTimesRuleStudentBatchSetRequest request)
        {
            var classBucket = await _classDAL.GetClassBucket(request.ClassId);
            if (classBucket == null || classBucket.EtClass == null)
            {
                return ResponseBase.CommonError("班级不存在");
            }
            var myClass = classBucket.EtClass;

            //清除旧数据
            await _classTimesRuleStudentDAL.ClearClassTimesRuleStudent(request.ClassId, request.RuleIds);

            if (request.StudentItems != null && request.StudentItems.Any())
            {
                var addEntitys = new List<EtClassTimesRuleStudent>();
                foreach (var myRuleId in request.RuleIds)
                {
                    foreach (var p in request.StudentItems)
                    {
                        addEntitys.Add(new EtClassTimesRuleStudent()
                        {
                            CourseId = p.CourseId,
                            ClassId = request.ClassId,
                            IsDeleted = myClass.IsDeleted,
                            Remark = string.Empty,
                            RuleId = myRuleId,
                            StudentId = p.StudentId,
                            TenantId = myClass.TenantId,
                            Type = myClass.Type
                        });
                    }
                }
                await _classTimesRuleStudentDAL.AddClassTimesRuleStudent(addEntitys);
            }

            foreach (var ruleId in request.RuleIds)
            {
                _eventPublisher.Publish(new SyncClassTimesRuleStudentInfoEvent(request.LoginTenantId)
                {
                    ClassId = request.ClassId,
                    RuleId = ruleId
                });
            }

            var config = await _tenantConfigDAL.GetTenantConfig();
            if (config.TenantOtherConfig.IsClassTimeRuleSetStudentAutoSyncClass)
            {
                if (request.StudentItems != null && request.StudentItems.Any())
                {
                    _eventPublisher.Publish(new SyncClassAddBatchStudentEvent(request.LoginTenantId)
                    {
                        ClassId = request.ClassId,
                        Students = request.StudentItems.Select(j => new ClassAddBatchStudent()
                        {
                            CourseId = j.CourseId,
                            StudentId = j.StudentId,
                        }).ToList(),
                    });
                }
            }

            var studetnNames = string.Join(',', request.StudentItems.Select(p => p.StudentName));
            await _userOperationLogDAL.AddUserLog(request, $"班级排课批量设置上课学员-班级:{classBucket.EtClass.Name},学员:{studetnNames}", EmUserOperationType.ClassManage);
            return ResponseBase.Success(new ClassTimesRuleEditOutput() { IsLimit = false });
        }

        public async Task<ResponseBase> ClassTimesRuleStudentAdd2(ClassTimesRuleStudentAdd2Request request)
        {
            var classBucket = await _classDAL.GetClassBucket(request.ClassId);
            if (classBucket == null || classBucket.EtClass == null)
            {
                return ResponseBase.CommonError("班级不存在");
            }

            var myClass = classBucket.EtClass;
            var addEntitys = new List<EtClassTimesRuleStudent>();
            foreach (var p in request.StudentIds)
            {
                var isExist = await _classTimesRuleStudentDAL.ExistStudent(p.Value, request.ClassId, request.RuleId);
                if (isExist)
                {
                    continue;
                }
                var newExist = addEntitys.Exists(a => a.StudentId == p.Value);
                if (newExist)
                {
                    continue;
                }

                addEntitys.Add(new EtClassTimesRuleStudent()
                {
                    CourseId = request.CourseId,
                    ClassId = request.ClassId,
                    IsDeleted = myClass.IsDeleted,
                    Remark = string.Empty,
                    RuleId = request.RuleId,
                    StudentId = p.Value,
                    TenantId = myClass.TenantId,
                    Type = myClass.Type
                });
            }
            if (addEntitys.Count == 0)
            {
                return ResponseBase.CommonError("学员已存在");
            }

            await _classTimesRuleStudentDAL.AddClassTimesRuleStudent(addEntitys);

            _eventPublisher.Publish(new SyncClassTimesRuleStudentInfoEvent(request.LoginTenantId)
            {
                ClassId = request.ClassId,
                RuleId = request.RuleId
            });

            var config = await _tenantConfigDAL.GetTenantConfig();
            if (config.TenantOtherConfig.IsClassTimeRuleSetStudentAutoSyncClass)
            {
                _eventPublisher.Publish(new SyncClassAddBatchStudentEvent(request.LoginTenantId)
                {
                    ClassId = request.ClassId,
                    Students = request.StudentIds.Select(p => new ClassAddBatchStudent()
                    {
                        CourseId = request.CourseId,
                        StudentId = p.Value
                    }).ToList(),
                });
            }

            var studetnNames = string.Join(',', request.StudentIds.Select(p => p.Label));
            await _userOperationLogDAL.AddUserLog(request, $"班级排课添加上课学员-班级:{classBucket.EtClass.Name},学员:{studetnNames}", EmUserOperationType.ClassManage);
            return ResponseBase.Success(new ClassTimesRuleEditOutput() { IsLimit = false });
        }

    }
}
