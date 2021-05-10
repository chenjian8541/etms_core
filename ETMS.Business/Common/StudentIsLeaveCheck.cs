using ETMS.Entity.Database.Source;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace ETMS.Business.Common
{
    public class StudentIsLeaveCheck
    {
        private List<EtStudentLeaveApplyLog> _studentLeaveApplyLogs;

        public StudentIsLeaveCheck(List<EtStudentLeaveApplyLog> studentLeaveApplyLogs)
        {
            this._studentLeaveApplyLogs = studentLeaveApplyLogs;
        }

        /// <summary>
        /// 判断学员是否请假
        /// </summary>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <param name="studentId"></param>
        /// <param name="classOt"></param>
        /// <returns></returns>
        public bool IsCheckStudentIsLeave(int startTime, int endTime, long studentId, DateTime classOt)
        {
            if (_studentLeaveApplyLogs != null && _studentLeaveApplyLogs.Count > 0)
            {
                var myLeaveApplyLog = _studentLeaveApplyLogs.FirstOrDefault(p => p.StudentId == studentId);
                if (myLeaveApplyLog != null)
                {
                    if (myLeaveApplyLog.StartDate < classOt && myLeaveApplyLog.EndDate > classOt)
                    {
                        return true;
                    }
                    var levelStartTime = myLeaveApplyLog.StartTime;
                    var levelEndTime = myLeaveApplyLog.EndTime;
                    if (myLeaveApplyLog.StartDate < classOt)
                    {
                        levelStartTime = 0;
                    }
                    if (myLeaveApplyLog.EndDate > classOt)
                    {
                        levelEndTime = 8888;
                    }
                    if (startTime > levelEndTime || endTime < levelStartTime)
                    {
                        LOG.Log.Info($"[IsCheckStudentIsLeave]判断是否为请假：startTime:{startTime},endTime:{endTime},levelStartTime:{levelStartTime},levelEndTime:{levelEndTime}", this.GetType());
                    }
                    else
                    {
                        return true;
                    }

                }
            }
            return false;
        }

        /// <summary>
        /// 判断学员是否请假
        /// </summary>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <param name="studentId"></param>
        /// <param name="classOt"></param>
        /// <returns></returns>
        public EtStudentLeaveApplyLog GeStudentLeaveLog(int startTime, int endTime, long studentId, DateTime classOt)
        {
            if (_studentLeaveApplyLogs != null && _studentLeaveApplyLogs.Count > 0)
            {
                var myLeaveApplyLogs = _studentLeaveApplyLogs.Where(p => p.StudentId == studentId);
                if (myLeaveApplyLogs.Any())
                {
                    foreach (var myLeaveApplyLog in myLeaveApplyLogs)
                    {
                        if (myLeaveApplyLog != null)
                        {
                            if (myLeaveApplyLog.StartDate < classOt && myLeaveApplyLog.EndDate > classOt)
                            {
                                return myLeaveApplyLog;
                            }
                            var levelStartTime = myLeaveApplyLog.StartTime;
                            var levelEndTime = myLeaveApplyLog.EndTime;
                            if (myLeaveApplyLog.StartDate < classOt)
                            {
                                levelStartTime = 0;
                            }
                            if (myLeaveApplyLog.EndDate > classOt)
                            {
                                levelEndTime = 8888;
                            }
                            if (startTime > levelEndTime || endTime < levelStartTime)
                            {
                                continue;
                            }
                            else
                            {
                                return myLeaveApplyLog;
                            }

                        }
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// 获取某时间段的请假学员列表
        /// </summary>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <param name="classOt"></param>
        /// <returns></returns>
        public List<EtStudentLeaveApplyLog> GetStudentLeaveList(int startTime, int endTime, DateTime classOt)
        {
            var newCheckTimeLeaveApplyLog = new List<EtStudentLeaveApplyLog>();
            if (_studentLeaveApplyLogs != null && _studentLeaveApplyLogs.Count > 0)
            {
                foreach (var myLeaveApplyLog in _studentLeaveApplyLogs)
                {
                    if (myLeaveApplyLog.StartDate < classOt && myLeaveApplyLog.EndDate > classOt)
                    {
                        newCheckTimeLeaveApplyLog.Add(myLeaveApplyLog);
                    }
                    var levelStartTime = myLeaveApplyLog.StartTime;
                    var levelEndTime = myLeaveApplyLog.EndTime;
                    if (myLeaveApplyLog.StartDate < classOt)
                    {
                        levelStartTime = 0;
                    }
                    if (myLeaveApplyLog.EndDate > classOt)
                    {
                        levelEndTime = 8888;
                    }
                    if (startTime > levelEndTime || endTime < levelStartTime)
                    {
                        //未请假
                    }
                    else
                    {
                        newCheckTimeLeaveApplyLog.Add(myLeaveApplyLog);
                    }
                }
            }
            return newCheckTimeLeaveApplyLog;
        }
    }
}
