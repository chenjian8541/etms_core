using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.AiFace.Dto.Baidu.Output
{
    public class SearchOutput 
    {
        public string face_token { get; set; }

        public List<SearchUser> user_list { get; set; }
    }

    public class SearchUser
    {
        public string group_id { get; set; }

        public string user_id { get; set; }

        public string user_info { get; set; }

        public float score { get; set; }
    }
}
