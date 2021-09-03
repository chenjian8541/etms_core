using ETMS.Entity.Common;
using ETMS.Utility;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using ETMS.Entity.Enum;

namespace ETMS.Entity.Dto.Student.Request
{
    public class StudentGetPagingRequest : RequestPagingBase, IDataLimit
    {
        /// <summary>
        /// 场景
        /// 0：要限制数据  1：不限制
        /// </summary>
        public int SceneType { get; set; }

        public string StudentKey { get; set; }

        public byte? TrackStatus { get; set; }

        public long? TrackUser { get; set; }

        public long? LearningManager { get; set; }

        public byte? StudentType { get; set; }

        public string CardNo { get; set; }

        /// <summary>
        /// 查询时间
        /// </summary>
        public List<string> Ot { get; set; }

        private DateTime? _startOt;

        /// <summary>
        /// 开始时间
        /// </summary>
        public DateTime? StartOt
        {
            get
            {
                if (_startOt != null)
                {
                    return _startOt;
                }
                if (Ot == null || Ot.Count == 0)
                {
                    return null;
                }
                _startOt = Convert.ToDateTime(Ot[0]);
                return _startOt;
            }
        }

        private DateTime? _endOt;

        /// <summary>
        /// 结束时间
        /// </summary>
        public DateTime? EndOt
        {
            get
            {
                if (_endOt != null)
                {
                    return _endOt;
                }
                if (Ot == null || Ot.Count < 2)
                {
                    return null;
                }
                _endOt = Convert.ToDateTime(Ot[1]).AddDays(1); ;
                return _endOt;
            }
        }

        public long? SourceId { get; set; }

        public byte? IntentionLevel { get; set; }

        public bool? SerchTodayMustTrack { get; set; }

        public bool? IsBindingWechat { get; set; }

        public int? Age { get; set; }

        public string SchoolName { get; set; }

        public long? GradeId { get; set; }

        public string[] Tags { get; set; }

        public string HomeAddress { get; set; }

        public List<string> EndClassOt { get; set; }

        private DateTime? _endClassOtStart;

        public DateTime? EndClassOtStart
        {
            get
            {
                if (_endClassOtStart != null)
                {
                    return _endClassOtStart;
                }
                if (EndClassOt == null || EndClassOt.Count == 0)
                {
                    return null;
                }
                _endClassOtStart = Convert.ToDateTime(EndClassOt[0]);
                return _endClassOtStart;
            }
        }

        private DateTime? _endClassOtEnd;

        public DateTime? EndClassOtEnd
        {
            get
            {
                if (_endClassOtEnd != null)
                {
                    return _endClassOtEnd;
                }
                if (EndClassOt == null || EndClassOt.Count < 2)
                {
                    return null;
                }
                _endClassOtEnd = Convert.ToDateTime(EndClassOt[1]).AddDays(1); ;
                return _endClassOtEnd;
            }
        }

        public string Remark { get; set; }

        public int? BirthdayMonth { get; set; }

        /// <summary>
        /// 是否排课 <see cref="ETMS.Entity.Enum.EmBool"/>
        /// </summary>
        public byte? IsClassSchedule { get; set; }

        /// <summary>
        /// 是否加入班级  <see cref="ETMS.Entity.Enum.EmBool"/>
        /// </summary>
        public byte? IsJoinClass { get; set; }

        /// <summary>
        /// 是否采集人脸
        /// </summary>
        public byte? IsHasFaceKey { get; set; }

        public string GetDataLimitFilterWhere()
        {
            return $" AND (CreateBy = {LoginUserId} OR TrackUser = {LoginUserId} OR LearningManager = {LoginUserId})";
        }

        /// <summary>
        /// 获取SQL语句
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            var condition = new StringBuilder(DataFilterWhere);
            if (StudentType != null)
            {
                condition.Append($" AND StudentType = {StudentType.Value}");
            }
            if (StartOt != null)
            {
                condition.Append($" AND Ot >= '{StartOt.Value.EtmsToString()}'");
            }
            if (EndOt != null)
            {
                condition.Append($" AND Ot < '{EndOt.Value.EtmsToString()}'");
            }
            if (!string.IsNullOrEmpty(StudentKey))
            {
                condition.Append($" AND (Name LIKE '%{StudentKey}%' OR Phone LIKE '{StudentKey}%' OR PhoneBak LIKE '{StudentKey}%' OR CardNo = '{StudentKey}' OR NamePinyin = '{StudentKey.ToLower()}')");
            }
            if (TrackStatus != null)
            {
                condition.Append($" AND TrackStatus = {TrackStatus.Value}");
            }
            if (TrackUser != null && TrackUser > 0)
            {
                condition.Append($" AND TrackUser = {TrackUser.Value}");
            }
            if (LearningManager != null && LearningManager > 0)
            {
                condition.Append($" AND LearningManager = {LearningManager.Value}");
            }
            if (!string.IsNullOrEmpty(CardNo))
            {
                condition.Append($" AND CardNo = '{CardNo}'");
            }
            if (SourceId != null && SourceId > 0)
            {
                condition.Append($" AND SourceId = {SourceId.Value}");
            }
            if (IntentionLevel != null)
            {
                condition.Append($" AND IntentionLevel = {IntentionLevel.Value}");
            }
            if (SerchTodayMustTrack != null && SerchTodayMustTrack.Value)
            {
                condition.Append($" AND NextTrackTime = '{DateTime.Now.EtmsToDateString()}'");
            }
            if (IsBindingWechat != null)
            {
                condition.Append($" AND IsBindingWechat = '{(IsBindingWechat.Value ? 1 : 0)}'");
            }
            if (Age != null)
            {
                condition.Append($" AND Age = '{Age}'");
            }
            if (!string.IsNullOrEmpty(SchoolName))
            {
                condition.Append($" AND SchoolName LIKE '{SchoolName}%'");
            }
            if (GradeId != null)
            {
                condition.Append($" AND GradeId = {GradeId.Value}");
            }
            if (!string.IsNullOrEmpty(HomeAddress))
            {
                condition.Append($" AND  HomeAddress LIKE '{HomeAddress}%'");
            }
            if (Tags != null && Tags.Any())
            {
                if (Tags.Length == 1)
                {
                    condition.Append($" AND Tags LIKE '%,{Tags[0]},%'");
                }
                else
                {
                    var tagSql = new StringBuilder();
                    tagSql.Append($" Tags LIKE '%,{Tags[0]},%'");
                    for (var i = 1; i < Tags.Length; i++)
                    {
                        tagSql.Append($" OR Tags LIKE '%,{Tags[i]},%'");
                    }
                    condition.Append($" AND ({tagSql})");
                }
            }
            if (EndClassOtStart != null)
            {
                condition.Append($" AND EndClassOt >= '{EndClassOtStart.Value.EtmsToString()}'");
            }
            if (EndClassOtEnd != null)
            {
                condition.Append($" AND EndClassOt < '{EndClassOtEnd.Value.EtmsToString()}'");
            }
            if (!string.IsNullOrEmpty(Remark))
            {
                condition.Append($" AND Remark LIKE '%{Remark}%'");
            }
            if (IsDataLimit && SceneType == 0)
            {
                condition.Append(GetDataLimitFilterWhere());
            }
            if (BirthdayMonth != null)
            {
                condition.Append($" AND BirthdayMonth = {BirthdayMonth}");
            }
            if (IsClassSchedule != null)
            {
                condition.Append($" AND IsClassSchedule = {IsClassSchedule.Value}");
            }
            if (IsJoinClass != null)
            {
                condition.Append($" AND IsJoinClass = {IsJoinClass.Value}");
            }
            if (IsHasFaceKey != null)
            {
                if (IsHasFaceKey.Value == EmBool.True)
                {
                    condition.Append(" AND FaceKey <> ''");
                }
                else
                {
                    condition.Append(" AND (FaceKey = '' OR FaceKey IS NULL)");
                }
            }
            return condition.ToString();
        }

        /// <summary>
        /// 数据校验
        /// </summary>
        /// <returns></returns>
        public override string Validate()
        {
            if (StartOt != null && EndOt != null && StartOt >= EndOt)
            {
                return "开始时间不能大于结束时间";
            }
            return base.Validate();
        }
    }
}
