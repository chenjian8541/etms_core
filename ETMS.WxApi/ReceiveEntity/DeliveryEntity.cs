using System;
using System.Collections.Generic;
using System.Linq;
namespace WxApi.ReceiveEntity
{
    /// <summary>
    /// 快递公司列表
    /// </summary>
    public class DeliveryEntity
    {
        private static Dictionary<string, string> _deliverylist;
        public static Dictionary<string, string> Deliverylist
        {
            get
            {
                //判断值是否为空，如果不为空，则直接返回。否则从资源文件中读取。
                return new Dictionary<string, string>();
            }
        }
    }
}
