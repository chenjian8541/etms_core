using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.Enum
{
    /// <summary>
    /// 点名记录操作类型
    /// </summary>
    public struct EmClassRecordOperationType
    {
        /// <summary>
        /// 撤销点名记录
        /// </summary>
        public const byte UndoClassRecord = 0;

        /// <summary>
        /// 修改点名记录
        /// </summary>
        public const byte ModifyStudentClassRecord = 1;

        public static string GetClassRecordOperationTypeDesc(int b)
        {
            if (b == UndoClassRecord)
            {
                return "撤销点名记录";
            }
            return "修改点名记录";
        }
    }
}
