using ETMS.Entity.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.Dto.Educational.Request
{
    public class ClassPlacementRequest : RequestBase
    {
        public long StudentId { get; set; }

        public long CourseId { get; set; }

        public List<ClassPlacementInfo> ClassPlacementInfos { get; set; }

        public override string Validate()
        {
            if (StudentId <= 0 || CourseId <= 0)
            {
                return "数据格式错误";
            }
            if (ClassPlacementInfos == null || ClassPlacementInfos.Count == 0)
            {
                return "选班调班信息不能为空";
            }
            return string.Empty;
        }
    }

    public class ClassPlacementInfo
    {
        public long ClassId { get; set; }

        /// <summary>
        /// <see cref="ClassPlacementInfoOpType"/>
        /// </summary>
        public byte OpType { get; set; }
    }

    public struct ClassPlacementInfoOpType
    {
        /// <summary>
        /// 新增
        /// </summary>
        public const byte Add = 0;

        /// <summary>
        /// 移除
        /// </summary>
        public const byte Remove = 1;
    }
}
