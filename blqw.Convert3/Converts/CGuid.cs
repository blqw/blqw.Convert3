using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace blqw
{
    public class CGuid : SystemTypeConvertor<Guid>
    {
        public override Guid ChangeType(string input, Type outputType, out bool success)
        {
            if (input == null || input.Length == 0)
            {
                success = false;
                return Guid.Empty;
            }
            Guid result;
            success = Guid.TryParse(input, out result);
            return result;
        }

        public override Guid ChangeType(object input, Type outputType, out bool success)
        {
            var bs = input as byte[];
            if (bs != null && bs.Length == 16)
            {
                success = true;
                return new Guid(bs);
            }
            success = false;
            return default(Guid);
        }
        
    }
}
