using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using blqw.Convert3Component;

namespace blqw.Converts
{
    class CJsonObject : CObject
    {
        public override uint Priority
        {
            get
            {
                return base.Priority + 1;
            }
        }
        
        protected override object ChangeType(string input, Type outputType, out bool success)
        {
            if (input != null && input.Length > 2)
            {
                switch (input[0])
                {
                    case '"':
                        if (input[input.Length - 1] != '"')
                            return base.ChangeType(input, outputType, out success);
                        break;
                    case '\'':
                        if (input[input.Length - 1] != '\'')
                            return base.ChangeType(input, outputType, out success);
                        break;
                    case '{':
                        if (input[input.Length - 1] != '}')
                            return base.ChangeType(input, outputType, out success);
                        break;
                    case '[':
                        if (input[input.Length - 1] != ']')
                            return base.ChangeType(input, outputType, out success);
                        break;
                    default:
                        return base.ChangeType(input, outputType, out success);
                }
                try
                {
                    success = true;
                    return Component.ToJsonObject(outputType, input);
                }
                catch (Exception ex)
                {
                    Error.Add(ex);
                }
            }
            return base.ChangeType(input, outputType, out success);
        }
    }
}
