using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;

namespace MyAddin_Test
{
    class EnumUtil
    {
        public static bool checkInEnumValues(string value, Type enumType)
        {
            try
            {
                Enum.Parse(enumType, value);
                return true;
            } catch(Exception)
            {
                return false;
            }
        }
    }
}
