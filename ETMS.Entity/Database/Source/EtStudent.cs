using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace ETMS.Entity.Database.Source
{
    /// <summary>
    /// 学员
    /// </summary>
    [Table("EtStudent")]
    public class EtStudent : Entity<long>
    {
        /// <summary>
        /// 创建人
        /// </summary>
        public long CreateBy { get; set; }

        /// <summary>
        /// 创建时间  记录日期部分
        /// </summary>
        public DateTime Ot { get; set; }

        /// <summary>
        /// 卡号
        /// </summary>
        public string CardNo { get; set; }

        /// <summary>
        /// 姓名
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 姓名(首字母)
        /// </summary>
        public string NamePinyin { get; set; }

        /// <summary>
        /// 头像地址
        /// </summary>
        public string Avatar { get; set; }

        /// <summary>
        /// 人脸地址
        /// </summary>
        public string FaceKey { get; set; }

        /// <summary>
        /// 人脸图片 灰色
        /// </summary>
        public string FaceGreyKey { get; set; }

        /// <summary>
        /// 人脸使用最后一次时间
        /// </summary>
        public DateTime? FaceUseLastTime { get; set; }

        /// <summary>
        /// 年龄
        /// </summary>
        public int? Age { get; set; }

        /// <summary>
        /// 生日
        /// </summary>
        public DateTime? Birthday { get; set; }

        /// <summary>
        /// 生日月份
        /// </summary>
        public int? BirthdayMonth { get; set; }

        /// <summary>
        /// 生日天
        /// </summary>
        public int? BirthdayDay { get; set; }

        /// <summary>
        /// 性别  <see cref="ETMS.Entity.Enum.EmGender"/>
        /// </summary>
        public byte? Gender { get; set; }

        /// <summary>
        /// 手机号码描述
        /// </summary>
        public long PhoneRelationship { get; set; }

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

        /// <summary>
        /// 跟进状态  <see cref="ETMS.Entity.Enum.EmStudentTrackStatus"/>
        /// </summary>
        public byte TrackStatus { get; set; }

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
        public string Tags { get; set; }

        /// <summary>
        /// 积分
        /// </summary>
        public int Points { get; set; }

        /// <summary>
        /// 学管师
        /// </summary>
        public long? LearningManager { get; set; }

        /// <summary>
        /// 推荐人
        /// </summary>
        public long? RecommendStudentId { get; set; }

        /// <summary>
        /// 上一次跟进时间
        /// </summary>
        public DateTime? LastTrackTime { get; set; }

        /// <summary>
        /// 下一次跟进时间
        /// </summary>
        public DateTime? NextTrackTime { get; set; }

        /// <summary>
        /// 结课时间
        /// </summary>
        public DateTime? EndClassOt { get; set; }

        /// <summary>
        /// 学员类型   <see cref="ETMS.Entity.Enum.EmStudentType"/>
        /// </summary>
        public byte StudentType { get; set; }

        /// <summary> 
        /// 是否绑定微信  <see cref="ETMS.Entity.Enum.EmIsBindingWechat"/>
        /// </summary>
        public byte IsBindingWechat { get; set; }

        /// <summary>
        /// 是否排课 <see cref="ETMS.Entity.Enum.EmBool"/>
        /// </summary>
        public byte IsClassSchedule { get; set; }

        /// <summary>
        /// 是否加入班级  <see cref="ETMS.Entity.Enum.EmBool"/>
        /// </summary>
        public byte IsJoinClass { get; set; }

        /// <summary>
        /// 课程分析JOB最后执行时间
        /// </summary>
        public DateTime LastJobProcessTime { get; set; }

        /// <summary>
        /// 密码
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }
    }
}
