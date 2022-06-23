﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using ETMS.Entity.Enum.EtmsManage;
using ETMS.Entity.Enum;

namespace ETMS.Entity.Database.Source
{
    [Table("EtActivityRouteItem")]
    public class EtActivityRouteItem: Entity<long>
    {
        public long ActivityId { get; set; }

        public long ActivityRouteId { get; set; }

        public long MiniPgmUserId { get; set; }

        public long? StudentId { get; set; }

        public string OpenId { get; set; }

        public string Unionid { get; set; }

        public string AvatarUrl { get; set; }

        public string NickName { get; set; }

        public string StudentName { get; set; }

        public string StudentPhone { get; set; }

        public string StudentFieldValue1 { get; set; }

        public string StudentFieldValue2 { get; set; }

        /// <summary>
        /// <see cref="EmActivityType"/>
        /// </summary>
        public int ActivityType { get; set; }

        public string ActivityTypeStyleClass { get; set; }

        public string ScenetypeStyleClass { get; set; }

        /// <summary>
        ///  <see cref="EmActivityScenetype"/>
        /// </summary>
        public int ActivityScenetype { get; set; }

        public string ActivityName { get; set; }

        public string ActivityCoverImage { get; set; }

        public string ActivityTitle { get; set; }

        public DateTime ActivityStartTime { get; set; }

        public DateTime ActivityEndTime { get; set; }

        public decimal ActivityOriginalPrice { get; set; }

        public string ActivityRuleItemContent { get; set; }

        public string ActivityRuleEx1 { get; set; }

        public string ActivityRuleEx2 { get; set; }

        public decimal PaySum { get; set; }

        public DateTime? PayFinishTime { get; set; }

        /// <summary>
        /// <see cref="EmActivityRouteStatus"/>
        /// </summary>
        public int RouteStatus { get; set; }

        public bool IsTeamLeader { get; set; }

        public DateTime CreateTime { get; set; }
    }
}
