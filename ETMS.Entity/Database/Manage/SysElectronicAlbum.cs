using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using ETMS.Entity.Enum.EtmsManage;

namespace ETMS.Entity.Database.Manage
{
    [Table("SysElectronicAlbum")]
    public class SysElectronicAlbum : EManageEntity<long>
    {
        /// <summary>
        /// <see cref="EmElectronicAlbumType"/>
        /// </summary>
        public int Type { get; set; }

        public string Name { get; set; }

        public string CoverKey { get; set; }

        public string RenderData { get; set; }

        public DateTime Ot { get; set; }

        /// <summary>
        /// <see cref="EmElectronicAlbumSysStatus"/>
        /// </summary>
        public byte Status { get; set; }
    }
}
