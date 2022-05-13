using ETMS.Business.Common;
using ETMS.Entity.Common;
using ETMS.Entity.Database.Source;
using ETMS.Entity.Dto.Educational2.Output;
using ETMS.Entity.Dto.Educational2.Request;
using ETMS.Entity.Enum;
using ETMS.Entity.View;
using ETMS.Entity.View.Rq;
using ETMS.IBusiness;
using ETMS.IDataAccess;
using ETMS.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Business
{
    public class TeacherSchooltimeConfigBLL : ITeacherSchooltimeConfigBLL
    {
        private readonly IUserDAL _userDAL;

        private readonly ITeacherSchooltimeConfigDAL _teacherSchooltimeConfigDAL;

        private readonly IUserOperationLogDAL _userOperationLogDAL;

        private readonly ICourseDAL _courseDAL;
        public TeacherSchooltimeConfigBLL(IUserDAL userDAL, ITeacherSchooltimeConfigDAL teacherSchooltimeConfigDAL,
            IUserOperationLogDAL userOperationLogDAL, ICourseDAL courseDAL)
        {
            this._userDAL = userDAL;
            this._teacherSchooltimeConfigDAL = teacherSchooltimeConfigDAL;
            this._userOperationLogDAL = userOperationLogDAL;
            this._courseDAL = courseDAL;
        }

        public void InitTenantId(int tenantId)
        {
            this.InitDataAccess(tenantId, _userDAL, _teacherSchooltimeConfigDAL, _userOperationLogDAL,
                _courseDAL);
        }

        public async Task<ResponseBase> TeacherSchooltimeConfigGetPaging(TeacherSchooltimeConfigGetPagingRequest request)
        {
            var output = new List<TeacherSchooltimeConfigGetPagingOutput>();
            var userView = await _userDAL.GetUserPaging(request);
            if (userView.Item1.Any())
            {
                foreach (var p in userView.Item1)
                {
                    var item = new TeacherSchooltimeConfigGetPagingOutput()
                    {
                        CId = p.Id,
                        Name = p.Name
                    };
                    var teacherSchooltimeConfigBucket = await _teacherSchooltimeConfigDAL.TeacherSchooltimeConfigGet(p.Id);
                    if (teacherSchooltimeConfigBucket != null)
                    {
                        var log = teacherSchooltimeConfigBucket.TeacherSchooltimeConfigs;
                        if (log != null && log.Any())
                        {
                            item.RuleDescs = log.Select(j => j.RuleDesc);
                        }
                        var exclude = teacherSchooltimeConfigBucket.TeacherSchooltimeConfigExclude;
                        if (exclude != null && exclude.AllExcludeDate != null && exclude.AllExcludeDate.Any())
                        {
                            item.ExcludeDate = exclude.AllExcludeDate.Select(j => j.EtmsToDateString());
                        }
                    }
                    output.Add(item);
                }
            }
            return ResponseBase.Success(new ResponsePagingDataBase<TeacherSchooltimeConfigGetPagingOutput>(userView.Item2, output));
        }

        public async Task<ResponseBase> TeacherSchooltimeConfigGet(TeacherSchooltimeConfigGetRequest request)
        {
            var user = await _userDAL.GetUser(request.TeacherId);
            if (user == null)
            {
                return ResponseBase.CommonError("老师不存在");
            }
            var item = new TeacherSchooltimeConfigGetOutput()
            {
                CId = user.Id,
                Name = user.Name,
            };
            var teacherSchooltimeConfigBucket = await _teacherSchooltimeConfigDAL.TeacherSchooltimeConfigGet(request.TeacherId);
            if (teacherSchooltimeConfigBucket != null)
            {
                var log = teacherSchooltimeConfigBucket.TeacherSchooltimeConfigs;
                if (log != null && log.Any())
                {
                    item.RuleDescs = log.Select(j => new TeacherSchooltimeConfigRuleOutput()
                    {
                        CId = j.Id,
                        RuleDesc = j.RuleDesc
                    });
                }
                var exclude = teacherSchooltimeConfigBucket.TeacherSchooltimeConfigExclude;
                if (exclude != null && exclude.AllExcludeDate != null && exclude.AllExcludeDate.Any())
                {
                    item.ExcludeDate = exclude.AllExcludeDate.Select(j => j.EtmsToDateString());
                }
            }
            else
            {
                item.RuleDescs = new List<TeacherSchooltimeConfigRuleOutput>();
            }
            return ResponseBase.Success(item);
        }

        public async Task<ResponseBase> TeacherSchooltimeConfigAdd(TeacherSchooltimeConfigAddRequest request)
        {
            var user = await _userDAL.GetUser(request.TeacherId);
            if (user == null)
            {
                return ResponseBase.CommonError("老师不存在");
            }
            var myCourseName = string.Empty;
            if (request.CourseId != null)
            {
                var setCourse = await _courseDAL.GetCourse(request.CourseId.Value);
                if (setCourse == null || setCourse.Item1 == null)
                {
                    return ResponseBase.CommonError("课程不存在");
                }
                myCourseName = setCourse.Item1.Name;
            }
            var teacherSchooltimeConfigBucket = await _teacherSchooltimeConfigDAL.TeacherSchooltimeConfigGet(request.TeacherId);
            if (teacherSchooltimeConfigBucket != null)
            {
                if (teacherSchooltimeConfigBucket.TeacherSchooltimeConfigs.Count >= 10)
                {
                    return ResponseBase.CommonError("最多设置10条规则");
                }
                var details = teacherSchooltimeConfigBucket.EtTeacherSchooltimeConfigDetails;
                if (details != null && details.Any())
                {
                    foreach (var myWeek in request.Weeks)
                    {
                        var overlappingTimeLog = details.Where(j => j.Week == myWeek && !(request.StartTime >= j.EndTime || request.EndTime <= j.StartTime)).FirstOrDefault();
                        if (overlappingTimeLog != null)
                        {
                            return ResponseBase.CommonError("存在重叠的时间段，起重新设置");
                        }
                    }
                }
            }

            var timeDesc = EtmsHelper.GetTimeDesc(request.StartTime, request.EndTime);
            var strRuleDesc = new StringBuilder();
            if (request.CourseId != null)
            {
                strRuleDesc.Append($"{myCourseName}：");
            }
            strRuleDesc.Append($"{EtmsHelper3.GetWeeksDesc(request.Weeks)}{timeDesc}");
            if (request.IsJumpHoliday)
            {
                strRuleDesc.Append("(跳过节假日)");
            }
            var config = new EtTeacherSchooltimeConfig()
            {
                CourseId = request.CourseId,
                EndTime = request.EndTime,
                StartTime = request.StartTime,
                TeacherId = request.TeacherId,
                TenantId = request.LoginTenantId,
                TimeDesc = timeDesc,
                IsDeleted = EmIsDeleted.Normal,
                IsJumpHoliday = request.IsJumpHoliday,
                Weeks = EtmsHelper.GetMuIds(request.Weeks),
                RuleDesc = strRuleDesc.ToString()
            };
            var configDetail = new List<EtTeacherSchooltimeConfigDetail>();
            foreach (var myWeek in request.Weeks)
            {
                configDetail.Add(new EtTeacherSchooltimeConfigDetail()
                {
                    CourseId = request.CourseId,
                    EndTime = request.EndTime,
                    StartTime = request.StartTime,
                    IsDeleted = EmIsDeleted.Normal,
                    IsJumpHoliday = request.IsJumpHoliday,
                    TeacherId = request.TeacherId,
                    TenantId = request.LoginTenantId,
                    Week = myWeek
                });
            }
            await _teacherSchooltimeConfigDAL.AddTeacherSchooltimeConfig(config, configDetail);

            await _userOperationLogDAL.AddUserLog(request, $"添加1v1老师可约课时间—{user.Name}",
                EmUserOperationType.ClassTimesReservationSetting);

            return ResponseBase.Success();
        }

        public async Task<ResponseBase> TeacherSchooltimeConfigDel(TeacherSchooltimeConfigDelRequest request)
        {
            var user = await _userDAL.GetUser(request.TeacherId);
            if (user == null)
            {
                return ResponseBase.CommonError("老师不存在");
            }
            await _teacherSchooltimeConfigDAL.DelTeacherSchooltimeConfig(request.SchooltimeConfigId, request.TeacherId);

            await _userOperationLogDAL.AddUserLog(request, $"删除1v1老师可约课时间—{user.Name}",
                EmUserOperationType.ClassTimesReservationSetting);

            return ResponseBase.Success();
        }

        public async Task<ResponseBase> TeacherSchooltimeConfigExcludeSave(TeacherSchooltimeConfigExcludeSaveRequest request)
        {
            var user = await _userDAL.GetUser(request.TeacherId);
            if (user == null)
            {
                return ResponseBase.CommonError("老师不存在");
            }
            IEnumerable<DateTime> effectiveExcludeDate = null;
            if (request.ExcludeDate != null && request.ExcludeDate.Any())
            {
                effectiveExcludeDate = request.ExcludeDate.Where(p => p >= DateTime.Now.Date).OrderBy(p => p).Distinct();
            }
            else
            {
                effectiveExcludeDate = new List<DateTime>();
            }
            var data = new TeacherSchooltimeConfigExcludeView()
            {
                AllExcludeDate = effectiveExcludeDate
            };
            var configExclude = new EtTeacherSchooltimeConfigExclude()
            {
                TeacherId = request.TeacherId,
                TenantId = request.LoginTenantId,
                IsDeleted = EmIsDeleted.Normal,
                ExcludeDateContent = Newtonsoft.Json.JsonConvert.SerializeObject(data)
            };
            await _teacherSchooltimeConfigDAL.SaveTeacherSchooltimeConfigExclude(configExclude);

            await _userOperationLogDAL.AddUserLog(request, $"设置1v1老师可约课时间的特定日期—{user.Name}",
                EmUserOperationType.ClassTimesReservationSetting);

            return ResponseBase.Success();
        }

        public async Task<ResponseBase> TeacherSchooltimeSetBatch(TeacherSchooltimeSetBatchRequest request)
        {
            IEnumerable<DateTime> effectiveExcludeDate = null;
            if (request.ExcludeDate != null && request.ExcludeDate.Any())
            {
                effectiveExcludeDate = request.ExcludeDate.Where(p => p >= DateTime.Now.Date).OrderBy(p => p).Distinct();
            }
            else
            {
                effectiveExcludeDate = new List<DateTime>();
            }
            var data = new TeacherSchooltimeConfigExcludeView()
            {
                AllExcludeDate = effectiveExcludeDate
            };
            var excludeDateContent = Newtonsoft.Json.JsonConvert.SerializeObject(data);

            var tempBoxCourse = new DataTempBox<EtCourse>();
            foreach (var item in request.Items)
            {
                item.TimeDesc = EtmsHelper.GetTimeDesc(item.NewIntStartTime, item.NewIntEndTime);
                item.StrWeeks = EtmsHelper.GetMuIds(item.Weeks);
                var strRuleDesc = new StringBuilder();
                if (item.CourseId != null)
                {
                    var myCourse = await ComBusiness.GetCourse(tempBoxCourse, _courseDAL, item.CourseId.Value);
                    if (myCourse == null)
                    {
                        item.CourseId = null;
                    }
                    else
                    {
                        strRuleDesc.Append($"{myCourse.Name}：");
                    }
                }
                strRuleDesc.Append($"{EtmsHelper3.GetWeeksDesc(item.Weeks)}{ item.TimeDesc}");
                if (item.IsJumpHoliday)
                {
                    strRuleDesc.Append("(跳过节假日)");
                }
                item.RuleDesc = strRuleDesc.ToString();
            }

            foreach (var teacherId in request.TeacherIds)
            {
                var resetInput = new ResetTeacherSchooltimeConfigInput()
                {
                    TeacherId = teacherId,
                    Items = new List<ResetTeacherSchooltimeConfigInputItem>(),
                    ExcludeConfig = new EtTeacherSchooltimeConfigExclude()
                    {
                        TeacherId = teacherId,
                        TenantId = request.LoginTenantId,
                        IsDeleted = EmIsDeleted.Normal,
                        ExcludeDateContent = excludeDateContent
                    }
                };
                foreach (var p in request.Items)
                {
                    var itemRe = new ResetTeacherSchooltimeConfigInputItem()
                    {
                        TeacherSchooltimeConfig = new EtTeacherSchooltimeConfig()
                        {
                            CourseId = p.CourseId,
                            EndTime = p.NewIntEndTime,
                            IsDeleted = EmIsDeleted.Normal,
                            IsJumpHoliday = p.IsJumpHoliday,
                            RuleDesc = p.RuleDesc,
                            StartTime = p.NewIntStartTime,
                            TeacherId = teacherId,
                            TenantId = request.LoginTenantId,
                            TimeDesc = p.TimeDesc,
                            Weeks = p.StrWeeks
                        },
                        TeacherSchooltimeConfigDetails = new List<EtTeacherSchooltimeConfigDetail>()
                    };
                    foreach (var itemWeek in p.Weeks)
                    {
                        itemRe.TeacherSchooltimeConfigDetails.Add(new EtTeacherSchooltimeConfigDetail()
                        {
                            CourseId = p.CourseId,
                            EndTime = p.NewIntEndTime,
                            IsDeleted = EmIsDeleted.Normal,
                            IsJumpHoliday = p.IsJumpHoliday,
                            StartTime = p.NewIntStartTime,
                            TeacherId = teacherId,
                            TenantId = request.LoginTenantId,
                            Week = itemWeek
                        });
                    }
                    resetInput.Items.Add(itemRe);
                }
                await _teacherSchooltimeConfigDAL.ResetTeacherSchooltimeConfig(resetInput);
            }

            await _userOperationLogDAL.AddUserLog(request, "批量设置1v1老师可约课时间",
                EmUserOperationType.ClassTimesReservationSetting);

            return ResponseBase.Success();
        }
    }
}
