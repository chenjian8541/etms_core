using ETMS.Entity.Common;
using ETMS.Entity.Database.Source;
using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.View
{
    public class StudentAccountRechargeView : IHaveId<long>
    {
        public EtStudentAccountRecharge StudentAccountRecharge { get; set; }

        public List<StudentAccountRechargeBinderView> Binders { get; set; }

        public long Id
        {
            get
            {
                return StudentAccountRecharge.Id;
            }
            set { }
        }
    }

    public class StudentAccountRechargeBinderView
    {
        public long StudentAccountRechargeBinderId { get; set; }

        public long StudentId { get; set; }

        public string StudentName { get; set; }

        public string StudentPhone { get; set; }

        public string StudentAvatar { get; set; }

        public string StudentAvatarUrl { get; set; }

        public byte? Gender { get; set; }
    }
}
