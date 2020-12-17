using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.IBusiness.IncrementLib
{
    public interface IAiface : IBaseBLL
    {
        Task StudentDelete(long studentId);

        Task<bool> StudentInitFace(long studentId, string faceGreyKeyUrl);

        Task<bool> StudentClearFace(long studentId);

        Task<long> SearchPerson(string imageBase64);
    }
}
