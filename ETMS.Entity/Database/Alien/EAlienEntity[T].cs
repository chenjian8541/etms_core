using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.Database.Alien
{
    public abstract class EAlienEntity<TPrimaryKey> : EAlienEntityBase<TPrimaryKey>
    {
        /// <summary>
        /// 校区Id
        /// </summary>
        public int HeadId { get; set; }
    }
}
