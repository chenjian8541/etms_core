using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.Dto.Student.Output
{
    public class StudentGetOutput
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
        /// 年龄
        /// </summary>
        public int? AgeMonth { get; set; }

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

        public long? RecommendStudentId { get; set; }

        public string RecommendStudentDesc { get; set; }

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

        /// <summary>
        /// 是否绑定磁卡
        /// </summary>
        public bool IsBindingCard { get; set; }

        /// <summary>
        /// 是否采集了人脸
        /// </summary>
        public bool IsBindingFaceKey { get; set; }

        /// <summary>
        /// 人脸图片
        /// </summary>
        public string FaceKeyUrl { get; set; }

        /// <summary>
        /// 是否排课 <see cref="ETMS.Entity.Enum.EmBool"/>
        /// </summary>
        public byte IsClassSchedule { get; set; }

        /// <summary>
        /// 是否加入班级  <see cref="ETMS.Entity.Enum.EmBool"/>
        /// </summary>
        public byte IsJoinClass { get; set; }

        public List<StudentExtendItemOutput> StudentExtendItems { get; set; }

        /// <summary>
        /// 生日月份
        /// </summary>
        public int? BirthdayMonth { get; set; }

        /// <summary>
        /// 生日天
        /// </summary>
        public int? BirthdayDay { get; set; }

        public byte HkFaceStatus { get; set; }

        public byte HkCardStatus { get; set; }

        public string StuNo { get; set; }
    }
}
