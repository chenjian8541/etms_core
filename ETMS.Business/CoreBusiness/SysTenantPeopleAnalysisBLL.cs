using ETMS.Entity.Database.Source;
using ETMS.Entity.Temp.Request;
using ETMS.Event.DataContract;
using ETMS.IBusiness;
using ETMS.IDataAccess;
using ETMS.IDataAccess.EtmsManage;
using ETMS.IEventProvider;
using ETMS.Utility;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using ETMS.Entity.Enum;
using ETMS.Entity.Temp.View;

namespace ETMS.Business
{
    public class SysTenantPeopleAnalysisBLL : ISysTenantPeopleAnalysisBLL
    {
        private readonly IJobAnalyze2DAL _jobAnalyze2DAL;

        private readonly ISysTenantUserDAL _sysTenantUserDAL;

        private readonly ISysTenantStudentDAL _sysTenantStudentDAL;

        private readonly IEventPublisher _eventPublisher;

        private readonly IUserDAL _userDAL;

        private readonly IStudentDAL _studentDAL;

        private const int _pageSize = 100;

        public SysTenantPeopleAnalysisBLL(IJobAnalyze2DAL jobAnalyze2DAL, ISysTenantUserDAL sysTenantUserDAL,
            ISysTenantStudentDAL sysTenantStudentDAL, IEventPublisher eventPublisher, IUserDAL userDAL, IStudentDAL studentDAL)
        {
            this._jobAnalyze2DAL = jobAnalyze2DAL;
            this._sysTenantUserDAL = sysTenantUserDAL;
            this._sysTenantStudentDAL = sysTenantStudentDAL;
            this._eventPublisher = eventPublisher;
            this._userDAL = userDAL;
            this._studentDAL = studentDAL;
        }

        public void InitTenantId(int tenantId)
        {
            this.InitDataAccess(tenantId, _jobAnalyze2DAL, _userDAL, _studentDAL);
        }

        public async Task SysTenantUserAnalysisConsumerEvent(SysTenantUserAnalysisEvent request)
        {
            await _sysTenantUserDAL.ResetTenantAllUser(request.TenantId);
            var userRequest = new TenantAnalysisUserGetPagingRequest()
            {
                IpAddress = string.Empty,
                IsDataLimit = false,
                LoginClientType = EmUserOperationLogClientType.PC,
                LoginTenantId = request.TenantId,
                LoginUserId = request.UserId,
                PageCurrent = 1,
                PageSize = _pageSize
            };
            var userPagingData = await _jobAnalyze2DAL.GetUserPaging(userRequest);
            if (userPagingData.Item2 == 0)
            {
                LOG.Log.Warn($"[SysTenantUserAnalysisConsumerEvent]未找到机构任何用户,TenantId:{request.TenantId}", this.GetType());
                return;
            }
            HandleTenantUserAnalysis(userPagingData.Item1, request.TenantId);
            var totalPage = EtmsHelper.GetTotalPage(userPagingData.Item2, _pageSize);
            userRequest.PageCurrent++;
            while (userRequest.PageCurrent <= totalPage)
            {
                var userResult = await _jobAnalyze2DAL.GetUserPaging(userRequest);
                HandleTenantUserAnalysis(userResult.Item1, request.TenantId);
                userRequest.PageCurrent++;
            }
        }

        private void HandleTenantUserAnalysis(IEnumerable<UserPagingView> users, int tenantId)
        {
            if (users != null && users.Any())
            {
                foreach (var p in users)
                {
                    _eventPublisher.Publish(new SysTenantUserAnalysisAddEvent(tenantId)
                    {
                        AddUserId = p.Id,
                        AddPhone = p.Phone,
                        IsRefreshCache = false
                    });
                }
            }
            else
            {
                LOG.Log.Error("[HandleTenantUserAnalysis]分页数据计算错误", this.GetType());
            }
        }

        public async Task SysTenantUserAnalysisAddConsumerEvent(SysTenantUserAnalysisAddEvent request)
        {
            await _sysTenantUserDAL.AddTenantUser(request.TenantId, request.AddUserId, request.AddPhone, request.IsRefreshCache);
        }

        /// <summary>
        /// 移除用户 先判断下手机号码是否存在
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task SysTenantUserAnalysisRemoveConsumerEvent(SysTenantUserAnalysisRemoveEvent request)
        {
            var phoneUser = await _userDAL.ExistUserPhone(request.RemovePhone);
            if (!phoneUser)
            {
                await _sysTenantUserDAL.RemoveTenantUser(request.TenantId, request.RemovePhone);
            }
        }

        public async Task SysTenantStudentAnalysisConsumerEvent(SysTenantStudentAnalysisEvent request)
        {
            await _sysTenantStudentDAL.ResetTenantAllStudent(request.TenantId);
            var studentRequest = new TenantStudentAnalysisStudentGetPagingRequest()
            {
                IpAddress = string.Empty,
                IsDataLimit = false,
                LoginClientType = EmUserOperationLogClientType.PC,
                LoginTenantId = request.TenantId,
                LoginUserId = request.UserId,
                PageCurrent = 1,
                PageSize = _pageSize
            };
            var studentPagingData = await _jobAnalyze2DAL.GetStudentPaging(studentRequest);
            if (studentPagingData.Item2 == 0)
            {
                LOG.Log.Warn($"[SysTenantStudentAnalysisConsumerEvent]未找到机构任何学员,TenantId:{request.TenantId}", this.GetType());
                return;
            }
            HandleTenantStudentAnalysis(studentPagingData.Item1, request.TenantId);
            var totalPage = EtmsHelper.GetTotalPage(studentPagingData.Item2, _pageSize);
            studentRequest.PageCurrent++;
            while (studentRequest.PageCurrent <= totalPage)
            {
                var studentResult = await _jobAnalyze2DAL.GetStudentPaging(studentRequest);
                HandleTenantStudentAnalysis(studentResult.Item1, request.TenantId);
                studentRequest.PageCurrent++;
            }
        }

        private void HandleTenantStudentAnalysis(IEnumerable<StudentPagingView> students, int tenantId)
        {
            if (students != null && students.Any())
            {
                foreach (var p in students)
                {
                    _eventPublisher.Publish(new SysTenantStudentAnalysisAddEvent(tenantId)
                    {
                        AddPhone = p.Phone,
                        IsRefreshCache = false,
                        AddStudentId = p.Id
                    });
                    if (!string.IsNullOrEmpty(p.PhoneBak))
                    {
                        _eventPublisher.Publish(new SysTenantStudentAnalysisAddEvent(tenantId)
                        {
                            AddPhone = p.PhoneBak,
                            IsRefreshCache = false,
                            AddStudentId = p.Id
                        });
                    }
                }
            }
            else
            {
                LOG.Log.Error("[HandleTenantUserAnalysis]分页数据计算错误", this.GetType());
            }
        }

        public async Task SysTenantStudentAnalysisAddConsumerEvent(SysTenantStudentAnalysisAddEvent request)
        {
            await _sysTenantStudentDAL.AddTenantStudent(request.TenantId, request.AddStudentId, request.AddPhone, request.IsRefreshCache);
        }

        /// <summary>
        /// 移除学员，需要注意 手机号码是否绑定了其它学员
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task SysTenantStudentAnalysisRemoveConsumerEvent(SysTenantStudentAnalysisRemoveEvent request)
        {
            var phoneStudent = await _studentDAL.ExistStudentPhone(request.RemovePhone);
            if (!phoneStudent)
            {
                await _sysTenantStudentDAL.RemoveTenantStudent(request.TenantId, request.RemovePhone);
            }
        }
    }
}
