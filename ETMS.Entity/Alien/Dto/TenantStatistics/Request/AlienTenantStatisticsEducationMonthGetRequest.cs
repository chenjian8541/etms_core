using ETMS.Entity.Alien.Common;

namespace ETMS.Entity.Alien.Dto.TenantStatistics.Request
{
    public class AlienTenantStatisticsEducationMonthGetRequest : AlienTenantRequestBase
    {
        public int Year { get; set; }

        public int Month { get; set; }

        public override string Validate()
        {
            if (Year <= 0 || Month <= 0)
            {
                return "请选择月份";
            }
            return base.Validate();
        }
    }
}
