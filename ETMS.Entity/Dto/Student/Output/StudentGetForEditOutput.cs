using ETMS.Entity.Dto.Student.Request;
using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.Dto.Student.Output
{
    public class StudentGetForEditOutput
    {
        /// <summary>
        /// ID
        /// </summary>
        public long CId { get; set; }

        /// <summary>
        /// 卡号
        /// </summary>
        public string CardNo { get; set; }

        /// <summary>
        /// 姓名
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 头像地址
        /// </summary>
        public string AvatarKey { get; set; }

        public string AvatarUrl { get; set; }

        /// <summary>
        /// 生日
        /// </summary>
        public string Birthday { get; set; }

        /// <summary>
        /// 性别  <see cref="ETMS.Entity.Enum.EmGender"/>
        /// </summary>
        public byte? Gender { get; set; }

        /// <summary>
        /// 手机号码描述
        /// </summary>
        public long? PhoneRelationship { get; set; }

        /// <summary>
        /// 手机号码
        /// </summary>
        public string Phone { get; set; }

        /// <summary>
        /// 备用号码描述
        /// </summary>
        public long? PhoneBakRelationship { get; set; }

        /// <summary>
        /// 备用号码
        /// </summary>
        public string PhoneBak { get; set; }

        /// <summary>
        /// 年级
        /// </summary>
        public long? GradeId { get; set; }

        /// <summary>
        /// 学校名称
        /// </summary>
        public string SchoolName { get; set; }

        /// <summary>
        /// 家庭住址
        /// </summary>
        public string HomeAddress { get; set; }

        /// <summary>
        /// 跟进人
        /// </summary>
        public long? TrackUser { get; set; }

        public string TrackUserName { get; set; }

        public long? RecommendStudentId { get; set; }

        public string RecommendStudentName { get; set; }

        /// <summary> 
        /// 意向级别   <see cref="ETMS.Entity.Enum.EmStudentIntentionLevel"/>
        /// </summary>
        public byte IntentionLevel { get; set; }

        /// <summary>
        /// 来源
        /// </summary>
        public long? SourceId { get; set; }

        /// <summary>
        /// 标签
        /// </summary>
        public string[] Tags { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }

        /// <summary>
        /// 学员类型   <see cref="ETMS.Entity.Enum.EmStudentType"/>
        /// </summary>
        public byte StudentType { get; set; }

        /// <summary>
        /// 跟进状态  <see cref="ETMS.Entity.Enum.EmStudentTrackStatus"/>
        /// </summary>
        public byte TrackStatus { get; set; }

        public List<StudentExtendItem> StudentExtendItems { get; set; }
    }
}
