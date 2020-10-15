﻿using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.Enum.EtmsManage
{
    public struct EmSysAgentSmsLogChangeType
    {
        /// <summary>
        /// 增加
        /// </summary>
        public const int Add = 0;

        /// <summary>
        /// 扣减
        /// </summary>
        public const int Deduction = 1;

        public static string GetSysAgentSmsLogChangeTypeDesc(int type)
        {
            if (type == Add)
            {
                return "增加";
            }
            return "扣减";
        }

        public static int GetChangeCountDesc(int changeCount, int type)
        {
            return type == EmSysAgentSmsLogChangeType.Add ? changeCount : -changeCount;
        }
    }
}
