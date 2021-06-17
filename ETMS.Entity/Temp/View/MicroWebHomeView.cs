using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.Temp.View
{
    public class MicroWebHomeView
    {
        public List<MicroWebHomeBanner> Banners { get; set; }

        public List<MicroWebHomeMenus> Menus { get; set; }

        public MicroWebHomeAddress Address { get; set; }

        public List<MicroWebHomeColumn> Columns { get; set; }
    }

    public class MicroWebHomeBanner
    {
        public string ImgUrl { get; set; }
    }

    public class MicroWebHomeMenus
    {
        public long Id { get; set; }

        public string ShowInMenuIconUrl { get; set; }

        /// <summary>
        ///  <see cref="ETMS.Entity.Enum.EmMicroWebColumnType"/>
        /// </summary>
        public byte Type { get; set; }

        public string Name { get; set; }
    }

    public class MicroWebHomeAddress
    {
        public string CoverIconUrl { get; set; }

        public string Name { get; set; }

        public string Address { get; set; }

        public string Longitude { get; set; }

        public string Latitude { get; set; }
    }

    public class MicroWebHomeColumn
    {
        public long Id { get; set; }

        public string Name { get; set; }

        /// <summary>
        ///  <see cref="ETMS.Entity.Enum.EmMicroWebColumnType"/>
        /// </summary>
        public byte Type { get; set; }

        /// <summary>
        /// 样式
        ///  <see cref="ETMS.Entity.Enum.EmMicroWebStyle"/>
        /// </summary>
        public int ShowStyle { get; set; }

        /// <summary>
        /// <see cref="ETMS.Entity.Enum.EmBool"/>
        /// </summary>
        public byte IsShowYuYue { get; set; }

        /// <summary>
        /// 数据
        /// </summary>
        public List<MicroWebHomeArticle> Articles { get; set; }
    }

    /// <summary>
    /// 栏目数据
    /// </summary>
    public class MicroWebHomeArticle
    {
        public long Id { get; set; }

        public long ColumnId { get; set; }

        public string ArTitile { get; set; }

        public string ArCoverImgUrl { get; set; }

        public string ArSummary { get; set; }
    }
}
