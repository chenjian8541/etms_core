using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Entity.Dto.SysCom.Output
{
    public class SysBulletinGetOutput
    {
        public bool IsHaveData { get; set; }

        public SysBulletinGetInfo BulletinInfo { get; set; }
    }

    public class SysBulletinGetInfo
    {
        public string Title { get; set; }

        public string UrlLink { get; set; }

        public long Id { get; set; }
    }
}
