using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.DataAccess.Lib
{
    public class PermissionDataH5
    {
        private static List<int> _pageAllList;

        private static List<int> _actionAllList;

        public static List<int> PageAllList
        {
            get
            {
                return _pageAllList;
            }
        }

        public static List<int> ActionAllList
        {
            get
            {
                return _actionAllList;
            }
        }

        static PermissionDataH5()
        {
            _pageAllList = new List<int>();
            _pageAllList.Add(2);
            _pageAllList.Add(67);
            _pageAllList.Add(23);
            _pageAllList.Add(19);
            _pageAllList.Add(34);
            _pageAllList.Add(76);
            _pageAllList.Add(70);
            _pageAllList.Add(63);

            _actionAllList = new List<int>();
            _actionAllList.Add(1);
            _actionAllList.Add(2);
            _actionAllList.Add(4);

            _actionAllList.Add(7);  //添加跟进记录
            _actionAllList.Add(38); //撤销上课记录
            _actionAllList.Add(46); //修改上课记录
            _actionAllList.Add(24); //编辑课次
            _actionAllList.Add(25); //删除课次
        }
    }
}
