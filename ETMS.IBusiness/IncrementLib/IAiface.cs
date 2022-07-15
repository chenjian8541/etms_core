using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.IBusiness.IncrementLib
{
    public interface IAiface : IBaseBLL
    {
        Task StudentDelete(long studentId);

        Task<Tuple<bool, string>> StudentInitFace(long studentId, string imageBase64);

        Task<bool> StudentClearFace(long studentId);

        Task<Tuple<long, string>> SearchPerson(string imageBase64);

        Task Gr0oupDelete(int tenantId);
    }
}
