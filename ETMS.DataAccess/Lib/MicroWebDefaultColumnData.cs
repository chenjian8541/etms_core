using ETMS.Entity.Database.Source;
using ETMS.Entity.Enum;
using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.DataAccess.Lib
{
    public class MicroWebDefaultColumnData
    {
        private static List<EtMicroWebColumn> _microWebColumnDefault;

        public static List<EtMicroWebColumn> MicroWebColumnDefault
        {
            get
            {
                return _microWebColumnDefault;
            }
        }

        static MicroWebDefaultColumnData()
        {
            _microWebColumnDefault = new List<EtMicroWebColumn>();
            _microWebColumnDefault.Add(new EtMicroWebColumn()
            {
                Id = -99,
                IsDeleted = EmIsDeleted.Normal,
                IsShowInHome = EmBool.True,
                IsShowInMenu = EmBool.True,
                IsShowYuYue = EmBool.True,
                Name = "品牌介绍",
                OrderIndex = -99,
                ShowInMenuIcon = "system/material/microweb/img/1.png",
                ShowStyle = EmMicroWebStyle.Style1,
                Type = EmMicroWebColumnType.SinglePage,
                Status = EmMicroWebStatus.Enable,
                TenantId = 0
            });
            _microWebColumnDefault.Add(new EtMicroWebColumn()
            {
                Id = -98,
                IsDeleted = EmIsDeleted.Normal,
                IsShowInHome = EmBool.True,
                IsShowInMenu = EmBool.True,
                IsShowYuYue = EmBool.True,
                Name = "热门课程",
                OrderIndex = -98,
                ShowInMenuIcon = "system/material/microweb/img/2.png",
                ShowStyle = EmMicroWebStyle.Style2,
                Type = EmMicroWebColumnType.ListPage,
                Status = EmMicroWebStatus.Enable,
                TenantId = 0
            });
            _microWebColumnDefault.Add(new EtMicroWebColumn()
            {
                Id = -97,
                IsDeleted = EmIsDeleted.Normal,
                IsShowInHome = EmBool.True,
                IsShowInMenu = EmBool.True,
                IsShowYuYue = EmBool.True,
                Name = "师资团队",
                OrderIndex = -97,
                ShowInMenuIcon = "system/material/microweb/img/3.png",
                ShowStyle = EmMicroWebStyle.Style3,
                Type = EmMicroWebColumnType.ListPage,
                Status = EmMicroWebStatus.Enable,
                TenantId = 0
            });
            _microWebColumnDefault.Add(new EtMicroWebColumn()
            {
                Id = -96,
                IsDeleted = EmIsDeleted.Normal,
                IsShowInHome = EmBool.True,
                IsShowInMenu = EmBool.True,
                IsShowYuYue = EmBool.True,
                Name = "明星学员",
                OrderIndex = -96,
                ShowInMenuIcon = "system/material/microweb/img/4.png",
                ShowStyle = EmMicroWebStyle.Style4,
                Type = EmMicroWebColumnType.ListPage,
                Status = EmMicroWebStatus.Enable,
                TenantId = 0
            });
            _microWebColumnDefault.Add(new EtMicroWebColumn()
            {
                Id = -95,
                IsDeleted = EmIsDeleted.Normal,
                IsShowInHome = EmBool.True,
                IsShowInMenu = EmBool.True,
                IsShowYuYue = EmBool.True,
                Name = "校区环境",
                OrderIndex = -95,
                ShowInMenuIcon = "system/material/microweb/img/5.png",
                ShowStyle = EmMicroWebStyle.Style1,
                Type = EmMicroWebColumnType.SinglePage,
                Status = EmMicroWebStatus.Enable,
                TenantId = 0
            });
            _microWebColumnDefault.Add(new EtMicroWebColumn()
            {
                Id = -94,
                IsDeleted = EmIsDeleted.Normal,
                IsShowInHome = EmBool.True,
                IsShowInMenu = EmBool.True,
                IsShowYuYue = EmBool.True,
                Name = "活动风采",
                OrderIndex = -94,
                ShowInMenuIcon = "system/material/microweb/img/6.png",
                ShowStyle = EmMicroWebStyle.Style2,
                Type = EmMicroWebColumnType.ListPage,
                Status = EmMicroWebStatus.Enable,
                TenantId = 0
            });
            _microWebColumnDefault.Add(new EtMicroWebColumn()
            {
                Id = -93,
                IsDeleted = EmIsDeleted.Normal,
                IsShowInHome = EmBool.False,
                IsShowInMenu = EmBool.True,
                IsShowYuYue = EmBool.True,
                Name = "学习资料",
                OrderIndex = -93,
                ShowInMenuIcon = "system/material/microweb/img/7.png",
                ShowStyle = EmMicroWebStyle.Style2,
                Type = EmMicroWebColumnType.ListPage,
                Status = EmMicroWebStatus.Enable,
                TenantId = 0
            });
            _microWebColumnDefault.Add(new EtMicroWebColumn()
            {
                Id = -92,
                IsDeleted = EmIsDeleted.Normal,
                IsShowInHome = EmBool.False,
                IsShowInMenu = EmBool.True,
                IsShowYuYue = EmBool.True,
                Name = "新闻动态",
                OrderIndex = -92,
                ShowInMenuIcon = "system/material/microweb/img/8.png",
                ShowStyle = EmMicroWebStyle.Style5,
                Type = EmMicroWebColumnType.ListPage,
                Status = EmMicroWebStatus.Enable,
                TenantId = 0
            });
        }
    }
}
