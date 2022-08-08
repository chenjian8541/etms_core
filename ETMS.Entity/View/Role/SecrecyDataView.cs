using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Entity.View.Role
{
    public class SecrecyDataView
    {
        public SecrecyDataView() { }

        public SecrecyDataView(bool isLimit)
        {
            this.StudentPhone = isLimit;
            this.ClassRecordDeSum = isLimit;
        }

        /// <summary>
        /// 学员手机号码
        /// </summary>
        public bool StudentPhone { get; set; }

        /// <summary>
        /// 课消金额
        /// </summary>
        public bool ClassRecordDeSum { get; set; }
    }
}
