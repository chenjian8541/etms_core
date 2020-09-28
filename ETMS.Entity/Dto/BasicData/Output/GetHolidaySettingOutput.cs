using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.Dto.BasicData.Output
{
    public class GetHolidaySettingOutput
    {
        public List<HolidaySettingOutputViewOutput> Holidays { get; set; }
    }

    public class HolidaySettingOutputViewOutput
    {

        public long CId { get; set; }

        public string StartTimeDesc { get; set; }

        public string EndTimeDesc { get; set; }

        public string Remark { get; set; }
    }
}
