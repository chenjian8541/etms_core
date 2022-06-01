using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.EtmsManage.Dto.Agent.Output
{
    public class VersionGetOutput
    {
        /// <summary>
        /// 角色名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }

        public string DetailInfo { get; set; }

        public bool IsDataLimit { get; set; }

        public List<SysMenuViewOutput> Menus { get; set; }

        public bool IsLimitLoginPC { get; set; }

        public bool IsLimitLoginWxParent { get; set; }

        public bool IsLimitLoginWxTeacher { get; set; }

        public bool IsLimitLoginAppTeacher { get; set; }
    }
}
