using ETMS.Entity.Temp.CloudFileClean;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.IDataAccess.SysOp
{
    public interface ICloudFileCleanDAL : IBaseDAL
    {
        Task<IEnumerable<HomeworkDetailView>> GetActiveHomeworkDetail(long homeworkId);

        Task<Tuple<IEnumerable<MicroWebColumnArticleView>, int>> GetMicroWebColumnArticlePaging(long columnId, int pageSize, int pageCurrent);

        Task<bool> ExistStudentAvatar(string key);

        Task<bool> ExistUserAvatar(string key);

        Task<bool> ExistStudentTrack(string key);

        Task<bool> ExistEvaluateStudent(string key);

        Task<bool> ExistGift(string key);

        Task<bool> ExistStudentLeave(string key);

        Task<bool> ExistActiveWxMessage(string key);

        Task<bool> ExistStudentHomework(string key);

        Task<bool> ExistStudentHomeworkAnswer(string key);

        Task<bool> ExistActiveGrowthRecord(string key);

        Task<bool> ExistMicroWebColumn(string key);

        Task<bool> ExistMicroWebColumnArticle(string key);

        Task<bool> ExistMicroWebColumnArticleContent(string key);

        Task<bool> ExistMallGoods(string key);

        Task<bool> ExistMallGoodsContent(string key);

        Task<bool> ExistShareTemplate(string key);

        Task<bool> ExistAlbum(string key);

        Task<bool> ExistAlbumImg(string key);

        Task<bool> ExistAlbumAudio(string key);

        Task<bool> ExistFaceKey(string key);
    }
}
