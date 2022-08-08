﻿using ETMS.Entity.Common;
using ETMS.Entity.Enum;
using ETMS.Utility;
using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.Dto.Educational.Request
{
    public class ClassTimesGetPagingRequest : RequestPagingBase, IDataLimit
    {
        public long? ClassId { get; set; }

        public long? StudentId { get; set; }

        public long? TeacherId { get; set; }

        public byte? ReservationType { get; set; }

        public byte? Status { get; set; }

        public byte? ClassType { get; set; }

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

        public long? ClassRoomId { get; set; }

        public long? CourseId { get; set; }

        public bool IsGetComplexInfo { get; set; } = false;

        public string GetDataLimitFilterWhere()
        {
            return $" AND Teachers LIKE '%,{LoginUserId},%'";
        }

        /// <summary>
        /// 获取SQL语句
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            var condition = new StringBuilder(DataFilterWhere);
            condition.Append($" AND DataType = {EmClassTimesDataType.Normal}");
            if (ClassId != null)
            {
                condition.Append($" AND ClassId = {ClassId.Value}");
            }
            if (StudentId != null)
            {
                condition.Append($" AND (StudentIdsTemp LIKE '%,{StudentId.Value},%' OR StudentIdsReservation LIKE '%,{StudentId.Value},%' OR StudentIdsClass LIKE '%,{StudentId.Value},%')");
            }
            if (TeacherId != null)
            {
                condition.Append($" AND Teachers LIKE '%,{TeacherId.Value},%'");
            }
            if (ClassRoomId != null)
            {
                condition.Append($" AND ClassRoomIds LIKE '%,{ClassRoomId.Value},%'");
            }
            if (StartOt != null)
            {
                condition.Append($" AND ClassOt >= '{StartOt.Value.EtmsToDateString()}'");
            }
            if (EndOt != null)
            {
                condition.Append($" AND ClassOt < '{EndOt.Value.EtmsToDateString()}'");
            }
            if (CourseId != null)
            {
                condition.Append($" AND CourseList LIKE '%,{CourseId.Value},%'");
            }
            if (ReservationType != null)
            {
                condition.Append($" AND ReservationType = {ReservationType.Value} ");
            }
            if (Status != null)
            {
                condition.Append($" AND [Status] = {Status.Value} ");
            }
            if (ClassType != null)
            {
                condition.Append($" AND [ClassType] = {ClassType.Value} ");
            }
            if (GetIsDataLimit(6))
            {
                condition.Append(GetDataLimitFilterWhere());
            }
            return condition.ToString();
        }

        /// <summary>
        /// 数据校验
        /// </summary>
        /// <returns></returns>
        public override string Validate()
        {
            if (StartOt != null && EndOt != null && StartOt > EndOt)
            {
                return "开始时间不能大于结束时间";
            }
            return base.Validate();
        }
    }
}
