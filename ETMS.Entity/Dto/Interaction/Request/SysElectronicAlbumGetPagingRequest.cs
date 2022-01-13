using ETMS.Entity.Common;
using ETMS.Entity.Enum;
using ETMS.Entity.Enum.EtmsManage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Entity.Dto.Interaction.Request
{
    public class SysElectronicAlbumGetPagingRequest : RequestPagingBase
    {
        public string Name { get; set; }

        public int? Type { get; set; }

        /// <summary>
        /// 获取SQL语句
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            var condition = new StringBuilder($" IsDeleted = {EmIsDeleted.Normal}");
            condition.Append($" AND [Status] = {EmElectronicAlbumSysStatus.Online}");
            if (!string.IsNullOrEmpty(Name))
            {
                condition.Append($" AND Name LIKE '%{Name}%'");
            }
            if (Type != null)
            {
                condition.Append($" AND [Type] = {Type.Value}");
            }
            return condition.ToString();
        }
    }
}
