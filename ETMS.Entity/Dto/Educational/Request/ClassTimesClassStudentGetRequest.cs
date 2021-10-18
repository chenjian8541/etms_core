using ETMS.Entity.Common;

namespace ETMS.Entity.Dto.Educational.Request
{
    public class ClassTimesClassStudentGetRequest : RequestBase
    {
        public long CId { get; set; }

        /// <summary>
        /// <see cref="ETMS.Entity.Enum.ClassTimesGetStudentType"/>
        /// </summary>
        public int Type { get; set; }

        public override string Validate()
        {
            if (CId <= 0)
            {
                return "请求数据格式错误";
            }
            return string.Empty;
        }
    }
}
