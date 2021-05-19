using ETMS.Entity.EtmsManage.Common;

namespace ETMS.Entity.EtmsManage.Dto.User.Request
{
    public class UserEditRequest : AgentRequestBase
    {
        public long Id { get; set; }

        public int UserRoleId { get; set; }

        public string Name { get; set; }

        public string Phone { get; set; }

        public bool IsLock { get; set; }

        public string Remark { get; set; }

        public string Address { get; set; }

        public override string Validate()
        {
            if (Id <= 0)
            {
                return "数据校验不合法";
            }
            if (string.IsNullOrEmpty(Name))
            {
                return "名称不能为空";
            }
            Phone = Phone.Trim();
            if (string.IsNullOrEmpty(Phone))
            {
                return "手机号码不能为空";
            }
            return string.Empty;
        }
    }
}
