using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace blqw
{
    [System.ComponentModel.Composition.Export(typeof(IConvertor))]
    public class CGuid : SystemTypeConvertor<Guid>
    {
        protected override bool Try(object input, out Guid result)
        {
            var bs = input as byte[];
            if (bs != null && bs.Length == 16)
            {
                result = new Guid(bs);
                return true;
            }
            result = default(Guid);
            return false;
        }

        protected override bool Try(string input, out Guid result)
        {
            if (input == null || input.Length == 0)
            {
                result = Guid.Empty;
                return false;
            }

            if (Guid.TryParse(input, out result))
            {
                return true;
            }
            result = Guid.Empty;
            return false;
        }
    }
}
