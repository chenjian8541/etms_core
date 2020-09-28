using ETMS.Entity.Database.Source;
using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.Config
{
    public interface ICacheSimpleDataContract<T> where T : Entity<long>
    {
        List<T> Entitys { get; set; }
    }
}
