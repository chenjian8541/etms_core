using ETMS.Entity.Common;
using System;
using System.Collections.Generic;
using System.Text;
using ETMS.Entity.Enum;

namespace ETMS.Entity.Dto.Interaction.Request
{
    public class ActiveWxMessageAddRequest : RequestBase
    {
        /// <summary>
        /// 接受者类型  <see cref="EmActiveWxMessageType"/>
        /// </summary>
        public byte Type { get; set; }

        /// <summary>
        /// 关联ID集合
        /// </summary>
        public List<long> RelatedIds { get; set; }

        /// <summary>
        /// 标题
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// 内容
        /// </summary>
        public string MessageContent { get; set; }

        /// <summary>
        /// 是否需要家长确认  <see cref="EmBool"/>
        /// </summary>
        public byte IsNeedConfirm { get; set; }

        /// <summary>
        /// 学员类型   <see cref="ETMS.Entity.Enum.EmStudentType"/>
        /// </summary>
        public byte? StudentType { get; set; }

        public override string Validate()
        {
            if (Type != EmActiveWxMessageType.AllStudent && (RelatedIds == null || RelatedIds.Count == 0))
            {
                return "请选择接收人";
            }
            if (RelatedIds != null)
            {
                if (Type == EmActiveWxMessageType.Class && RelatedIds.Count > 5)
                {
                    return "一次性最多选择5个班级";
                }
                if (Type == EmActiveWxMessageType.Student && RelatedIds.Count > 100)
                {
                    return "一次性最多选择100名学员";
                }
            }
            if (string.IsNullOrEmpty(Title))
            {
                return "请输入标题";
            }
            if (string.IsNullOrEmpty(MessageContent))
            {
                return "请输入内容";
            }
            return base.Validate();
        }
    }
}
