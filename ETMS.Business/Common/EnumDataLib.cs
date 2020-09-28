using ETMS.Entity.Enum;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace ETMS.Business.Common
{
    internal static class EnumDataLib
    {
        private static List<EnumItem> _userOperationTypeDesc;

        private static object LockUserOperationTypeObj = new object();

        internal static List<EnumItem> GetUserOperationTypeDesc
        {
            get
            {
                if (_userOperationTypeDesc == null)
                {
                    lock (LockUserOperationTypeObj)
                    {
                        if (_userOperationTypeDesc == null)
                        {
                            _userOperationTypeDesc = new List<EnumItem>();
                            foreach (var value in Enum.GetValues(typeof(EmUserOperationType)))
                            {
                                var desc = value.GetType().GetField(value.ToString()).GetCustomAttributes(typeof(DescriptionAttribute), true)[0] as DescriptionAttribute;
                                _userOperationTypeDesc.Add(new EnumItem()
                                {
                                    Value = Convert.ToInt32(value),
                                    Label = desc.Description
                                });
                            }
                            return _userOperationTypeDesc;
                        }
                    }
                }
                return _userOperationTypeDesc;
            }
        }

        private static List<EnumItem> _studentOperationTypeDesc;

        private static object LockStudentOperationTypeObj = new object();

        internal static List<EnumItem> GetStudentOperationTypeDesc
        {
            get
            {
                if (_studentOperationTypeDesc == null)
                {
                    lock (LockStudentOperationTypeObj)
                    {
                        if (_studentOperationTypeDesc == null)
                        {
                            _studentOperationTypeDesc = new List<EnumItem>();
                            foreach (var value in Enum.GetValues(typeof(EmStudentOperationLogType)))
                            {
                                var desc = value.GetType().GetField(value.ToString()).GetCustomAttributes(typeof(DescriptionAttribute), true)[0] as DescriptionAttribute;
                                _studentOperationTypeDesc.Add(new EnumItem()
                                {
                                    Value = Convert.ToInt32(value),
                                    Label = desc.Description
                                });
                            }
                            return _studentOperationTypeDesc;
                        }
                    }
                }
                return _studentOperationTypeDesc;
            }
        }
    }

    public class EnumItem
    {
        public int Value { get; set; }

        public string Label { get; set; }
    }
}
