using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.Dto.Student.Output
{
    public class StudentGetPagingOutput
    {
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
        public string AvatarUrl { get; set; }

        /// <summary>
        /// 头像地址
        /// </summary>
        public string AvatarKey { get; set; }

        /// <summary>
        /// 年龄
        /// </summary>
        public int? Age { get; set; }

        /// <summary>
        /// 生日
        /// </summary>
        public string BirthdayDesc { get; set; }

        /// <summary>
        /// 性别  <see cref="ETMS.Entity.Enum.EmGender"/>
        /// </summary>
        public byte? Gender { get; set; }

        public string GenderDesc { get; set; }

        /// <summary>
        /// 手机号码描述
        /// </summary>
        public long PhoneRelationship { get; set; }

        public string PhoneRelationshipDesc { get; set; }

        /// <summary>
        /// 手机号码
        /// </summary>
        public string Phone { get; set; }

        /// <summary>
        /// 备用号码描述
        /// </summary>
        public long? PhoneBakRelationship { get; set; }

        public string PhoneBakRelationshipDesc { get; set; }

        /// <summary>
        /// 备用号码
        /// </summary>
        public string PhoneBak { get; set; }

        /// <summary>
        /// 年级
        /// </summary>
        public long? GradeId { get; set; }

        public string GradeIdDesc { get; set; }

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

        public string TrackUserDesc { get; set; }

        /// <summary>
        /// 跟进状态  <see cref="ETMS.Entity.Enum.EmStudentTrackStatus"/>
        /// </summary>
        public byte TrackStatus { get; set; }

        public string TrackStatusDesc { get; set; }

        /// <summary> 
        /// 意向级别   <see cref="ETMS.Entity.Enum.EmStudentIntentionLevel"/>
        /// </summary>
        public byte IntentionLevel { get; set; }

        public string IntentionLevelDesc { get; set; }

        /// <summary>
        /// 来源
        /// </summary>
        public long? SourceId { get; set; }

        public string SourceIdDesc { get; set; }

        /// <summary>
        /// 标签
        /// </summary>
        public string Tags { get; set; }

        public string TagsDesc { get; set; }

        /// <summary>
        /// 积分
        /// </summary>
        public int Points { get; set; }

        /// <summary>
        /// 学管师
        /// </summary>
        public long? LearningManager { get; set; }

        public string LearningManagerDesc { get; set; }

        public string LastTrackTimeDesc { get; set; }

        public string NextTrackTimeDesc { get; set; }

        /// <summary>
        /// 结课时间
        /// </summary>
        public string EndClassOtDesc { get; set; }

        /// <summary>
        /// 学员类型   <see cref="ETMS.Entity.Enum.EmStudentType"/>
        /// </summary>
        public byte StudentType { get; set; }

        public string StudentTypeDesc { get; set; }

        /// <summary> 
        /// 是否绑定微信  <see cref="ETMS.Entity.Enum.EmIsBindingWechat"/>
        /// </summary>
        public byte IsBindingWechat { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }

        public string OtDesc { get; set; }

        public long Value { get; set; }

        public string Label { get; set; }
    }
}
