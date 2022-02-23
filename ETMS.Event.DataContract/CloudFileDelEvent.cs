using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Event.DataContract
{
    public class CloudFileDelEvent : Event
    {
        public CloudFileDelEvent(int tenantId) : base(tenantId)
        {
        }

        public int SceneType { get; set; }

        public long RelatedId { get; set; }
    }

    public struct CloudFileScenes
    {
        public const int ActiveHomework = 0;

        public const int MicroWebColumnArticle = 1;
    }
}
