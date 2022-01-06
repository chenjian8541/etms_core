using ETMS.Entity.Common;
using ETMS.Entity.Database.Source;
using ETMS.Entity.View.LibMedia;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.IDataAccess
{
    public interface ILibMediaDAL : IBaseDAL
    {
        Task AddImage(EtLibImages entity);

        Task DelImage(long id);

        Task AddAudio(EtLibAudios entity);

        Task DelAudio(long id);

        Task<Tuple<IEnumerable<EtLibImages>, int>> GetPagingImg(IPagingRequest request);

        Task<Tuple<IEnumerable<EtLibAudios>, int>> GetPagingAudio(IPagingRequest request);

        Task<IEnumerable<LibImageView>> GetImages(int type);

        Task<IEnumerable<LibAudioView>> GetAudios(int type);
    }
}
