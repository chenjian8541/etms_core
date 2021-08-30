using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.IDataAccess
{
    public interface IComDAL: IBaseDAL
    {
        Task ExecuteSql(string sql);
    }
}
