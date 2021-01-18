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
            _actionAllList = new List<int>();
            _actionAllList.Add(1);
            _actionAllList.Add(2);
            _actionAllList.Add(4);
        }
    }
}
