using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Entity.Dto.Activity.Output
{
    public class ActivitySystemGetPagingOutput
    {
        public long CId { get; set; }

        /// <summary>
        /// <see cref="EmActivityType"/>
        /// </summary>
        public int ActivityType { get; set; }

        public string ActivityTypeDesc { get; set; }

        /// <summary>
        /// <see cref="EmActivityScenetype"/>
        /// </summary>
        public int Scenetype { get; set; }

        public string ScenetypeDesc { get; set; }

        public string Name { get; set; }

        public string CoverImage { get; set; }

        public string Title { get; set; }

        public string CourseName { get; set; }

        /// <summary>
        /// <see cref="EmActivityStyleType"/>
        /// </summary>
        public int StyleType { get; set; }

        public string StyleColumnColor { get; set; }

        public bool IsAllowPay { get; set; }
    }
}
