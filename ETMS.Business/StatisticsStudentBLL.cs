using ETMS.Entity.Enum;
using ETMS.Event.DataContract;
using ETMS.IBusiness;
using ETMS.IDataAccess;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using ETMS.Utility;
using ETMS.Entity.Dto.HisData.Request;
using ETMS.Entity.Common;
using ETMS.Entity.Dto.HisData.Output;
using ETMS.Entity.View.Persistence;

namespace ETMS.Business
{
    public class StatisticsStudentBLL : IStatisticsStudentBLL
    {
        private readonly IStatisticsStudentDAL _statisticsStudentDAL;

        private readonly IStatisticsStudentCountDAL _statisticsStudentCountDAL;

        private readonly IStatisticsStudentTrackCountDAL _statisticsStudentTrackCountDAL;

        private readonly IStudentSourceDAL _studentSourceDAL;

        private readonly IStatisticsStudentSourceDAL _statisticsStudentSourceDAL;

        public StatisticsStudentBLL(IStatisticsStudentDAL statisticsStudentDAL, IStatisticsStudentCountDAL statisticsStudentCountDAL,
            IStatisticsStudentTrackCountDAL statisticsStudentTrackCountDAL, IStudentSourceDAL studentSourceDAL, IStatisticsStudentSourceDAL statisticsStudentSourceDAL)
        {
            this._statisticsStudentDAL = statisticsStudentDAL;
            this._statisticsStudentCountDAL = statisticsStudentCountDAL;
            this._statisticsStudentTrackCountDAL = statisticsStudentTrackCountDAL;
            this._studentSourceDAL = studentSourceDAL;
            this._statisticsStudentSourceDAL = statisticsStudentSourceDAL;
        }

        public void InitTenantId(int tenantId)
        {
            this.InitDataAccess(tenantId, _statisticsStudentDAL, _statisticsStudentCountDAL, _statisticsStudentTrackCountDAL, _studentSourceDAL, _statisticsStudentSourceDAL);
        }

        public async Task StatisticsStudentCountConsumeEvent(StatisticsStudentCountEvent request)
        {
            if (request.OpType == StatisticsStudentOpType.Add)
            {
                await _statisticsStudentCountDAL.AddStudentCount(request.Time.Date, request.ChangeCount);
            }
            else
            {
                await _statisticsStudentCountDAL.DeductionStudentCount(request.Time.Date, request.ChangeCount);
            }
        }

        public async Task StatisticsStudentTrackCountConsumeEvent(StatisticsStudentTrackCountEvent request)
        {
            if (request.OpType == StatisticsStudentTrackCountOpType.Add)
            {
                await _statisticsStudentTrackCountDAL.AddStudentTrackCount(request.Time.Date, request.ChangeCount);
            }
            else
            {
                await _statisticsStudentTrackCountDAL.DeductionStudentTrackCount(request.Time.Date, request.ChangeCount);
            }
        }

        public async Task StatisticsStudentConsumeEvent(StatisticsStudentEvent request)
        {
            switch (request.OpType)
            {
                case EmStatisticsStudentType.StudentSource:
                    await UpdateStatisticsStudentSource(request.StatisticsDate);
                    break;
                case EmStatisticsStudentType.StudentType:
                    await UpdateStatisticsStudentType();
                    break;
            }
        }

        private async Task UpdateStatisticsStudentSource(DateTime time)
        {
            await _statisticsStudentSourceDAL.UpdateStatisticsStudentSource(time);
        }

        private async Task UpdateStatisticsStudentType()
        {
            var data = await _statisticsStudentDAL.GetStatisticsStudentType();
            var contentData = string.Empty;
            if (data != null && data.Any())
            {
                contentData = EtmsHelper.EtmsSerializeObject(data.ToList());
            }
            await _statisticsStudentDAL.SaveStatisticsStudent((int)EmStatisticsStudentType.StudentType, contentData);
        }

        public async Task<ResponseBase> GetStatisticsStudentCount(GetStatisticsStudentCountRequest request)
        {
            var currentDate = request.StartOt.Value;
            var endDate = request.EndOt.Value;
            var statisticsStudentCount = await _statisticsStudentCountDAL.GetStatisticsStudentCount(currentDate, endDate);
            var echartsBar = new EchartsBar<int>();
            while (currentDate <= endDate)
            {
                var myStatisticsStudentCount = statisticsStudentCount.FirstOrDefault(p => p.Ot == currentDate);
                echartsBar.XData.Add(currentDate.ToString("MM-dd"));
                echartsBar.MyData.Add(myStatisticsStudentCount == null ? 0 : myStatisticsStudentCount.AddStudentCount);
                currentDate = currentDate.AddDays(1);
            }
            return ResponseBase.Success(echartsBar);
        }

        public async Task<ResponseBase> GetStatisticsStudentTrackCount(GetStatisticsStudentTrackCountRequest request)
        {
            var currentDate = request.StartOt.Value;
            var endDate = request.EndOt.Value;
            var statisticsStudentTrackCount = await _statisticsStudentTrackCountDAL.GetStatisticsStudentTrackCount(currentDate, endDate);
            var echartsBar = new EchartsBar<int>();
            while (currentDate <= endDate)
            {
                var myStatisticsStudentTrackCount = statisticsStudentTrackCount.FirstOrDefault(p => p.Ot == currentDate);
                echartsBar.XData.Add(currentDate.ToString("MM-dd"));
                echartsBar.MyData.Add(myStatisticsStudentTrackCount == null ? 0 : myStatisticsStudentTrackCount.TrackCount);
                currentDate = currentDate.AddDays(1);
            }
            return ResponseBase.Success(echartsBar);
        }

        public async Task<ResponseBase> GetStatisticsStudentSource(GetStatisticsStudentRequest request)
        {
            var statisticsStudentDataStudentSource = await _statisticsStudentSourceDAL.GetStatisticsStudentSource(request.StartOt.Value, request.EndOt.Value);
            var studentSourceAll = await _studentSourceDAL.GetAllStudentSource();
            var echartsPieStudentSource = new EchartsPie<int>();
            const string otherTagName = "其他";
            var echartsPieDataStudentSourceOther = new EchartsPieData<int>() { Name = otherTagName, Value = 0 };
            foreach (var p in statisticsStudentDataStudentSource)
            {
                if (p.SourceId == null)
                {
                    echartsPieDataStudentSourceOther.Value += p.TotalCount;
                    continue;
                }
                var mySource = studentSourceAll.FirstOrDefault(j => j.Id == p.SourceId.Value);
                if (mySource == null)
                {
                    echartsPieDataStudentSourceOther.Value += p.TotalCount;
                    continue;
                }
                echartsPieStudentSource.LegendData.Add(mySource.Name);
                echartsPieStudentSource.MyData.Add(new EchartsPieData<int>()
                {
                    Name = mySource.Name,
                    Value = p.TotalCount
                });
            }
            if (echartsPieDataStudentSourceOther.Value > 0)
            {
                echartsPieStudentSource.LegendData.Add(otherTagName);
                echartsPieStudentSource.MyData.Add(echartsPieDataStudentSourceOther);
            }
            return ResponseBase.Success(echartsPieStudentSource);
        }

        public async Task<ResponseBase> GetStatisticsStudentType(GetStatisticsStudentRequest request)
        {
            var statisticsStudentDataStudentType = await _statisticsStudentDAL.GetStatisticsStudent((int)EmStatisticsStudentType.StudentType);
            var statisticsStudentType = new List<StatisticsStudentType>();
            if (statisticsStudentDataStudentType != null && !string.IsNullOrEmpty(statisticsStudentDataStudentType.ContentData))
            {
                statisticsStudentType = EtmsHelper.EtmsDeserializeObject<List<StatisticsStudentType>>(statisticsStudentDataStudentType.ContentData);
            }
            var echartsPieStudentType = new EchartsPie<int>();
            var allType = EmStudentType.GetAllStudentType();
            foreach (var p in allType)
            {
                var tempName = EmStudentType.GetStudentTypeDesc(p);
                var myData = statisticsStudentType.FirstOrDefault(j => j.StudentType == p);
                if (myData == null)
                {
                    continue;
                }
                echartsPieStudentType.LegendData.Add(tempName);
                echartsPieStudentType.MyData.Add(new EchartsPieData<int>()
                {
                    Name = tempName,
                    Value = myData.TotalCount
                });
            }
            return ResponseBase.Success(echartsPieStudentType);
        }
    }
}
