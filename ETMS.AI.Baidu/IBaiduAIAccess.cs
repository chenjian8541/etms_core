using ETMS.AI.Baidu.Dto.Output;

namespace ETMS.AI.Baidu
{
    public interface IBaiduAIAccess
    {
        Task<LocationOutput> GetLocationInfo(string url);
    }
}