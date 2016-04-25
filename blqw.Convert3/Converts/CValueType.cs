using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace blqw.Converts
{
    public class CValueType : AdvancedConvertor<ValueType>
    {
        protected override ValueType ChangeType(string input, Type outputType, out bool success)
        {
            Error.CastFail("无法为值类型(struct)提供转换");
            success = false;
            return null;
        }

        protected override ValueType ChangeType(object input, Type outputType, out bool success)
        {
            Error.CastFail("无法为值类型(struct)提供转换");
            success = false;
            return null;
        }
    }
}
