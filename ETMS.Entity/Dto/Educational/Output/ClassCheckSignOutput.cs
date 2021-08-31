using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Entity.Dto.Educational.Output
{
    public class ClassCheckSignOutput
    {
        public ClassCheckSignOutput(bool isFinish)
        {
            this.IsFinish = isFinish;
        }

        public bool IsFinish { get; set; }

        /// <summary>
        /// 0:表示时间重复
        /// </summary>
        public int ErrType { get; set; }
    }
}
