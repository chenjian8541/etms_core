using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Entity.View.Role
{
    public class AuthorityValueDataDetailView
    {
        public AuthorityValueDataDetailView() { }

        public AuthorityValueDataDetailView(bool isLimit) {
            this.Student = isLimit;
            this.StudentTrackLog = isLimit;
            this.ClassRecord = isLimit;
            this.Order = isLimit;
            this.Class = isLimit;
            this.ClassTimes = isLimit;
            this.Finance = isLimit;
            this.BascData = isLimit;
            this.Interaction = isLimit;
        }

        /// <summary>
        /// 1
        /// </summary>
        public bool Student { get; set; }

        /// <summary>
        /// 2
        /// </summary>
        public bool StudentTrackLog { get; set; }

        /// <summary>
        /// 3
        /// </summary>
        public bool ClassRecord { get; set; }

        /// <summary>
        /// 4
        /// </summary>
        public bool Order { get; set; }

        /// <summary>
        /// 5
        /// </summary>
        public bool Class { get; set; }

        /// <summary>
        /// 6
        /// </summary>
        public bool ClassTimes { get; set; }
        
        /// <summary>
        /// 7
        /// </summary>
        public bool Finance { get; set; }

        /// <summary>
        /// 8
        /// </summary>
        public bool BascData { get; set; }

        /// <summary>
        /// 9
        /// </summary>
        public bool Interaction { get; set; }
    }
}
