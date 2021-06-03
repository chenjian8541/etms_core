using ETMS.Entity.Common;
using ETMS.Entity.Database.Source;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.IDataAccess.MicroWeb
{
    public interface IMicroWebColumnArticleDAL : IBaseDAL
    {
        Task<EtMicroWebColumnArticle> GetMicroWebColumnArticle(long id);

        Task<bool> AddMicroWebColumnArticle(EtMicroWebColumnArticle entity);

        Task<bool> EditMicroWebColumnArticle(EtMicroWebColumnArticle entity);

        Task<bool> DelMicroWebColumnArticle(long id);

        Task<bool> ChangeMicroWebColumnArticleStatus(long id, byte newStatus);

        Task<bool> AddMicroWebColumnArticleReadCount(long id, int addCount);

        Task<Tuple<IEnumerable<EtMicroWebColumnArticle>, int>> GetPaging(RequestPagingBase request);

        Task<EtMicroWebColumnArticle> GetMicroWebColumnSinglePageArticle(long columnId);

        Task<bool> SaveMicroWebColumnSinglePageArticle(EtMicroWebColumnArticle entity);
    }
}
