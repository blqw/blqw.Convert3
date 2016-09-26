using blqw.IOC;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace blqw.Converts
{
    [ExportMetadata("Priority", 1)]
    class CJsonObject : CObject
    {
        protected override object ChangeType(ConvertContext context,string input, Type outputType, out bool success)
        {
            if (input != null && input.Length > 2)
            {
                switch (input[0])
                {
                    case '"':
                        if (input[input.Length - 1] != '"')
                            return base.ChangeType(context, input, outputType, out success);
                        break;
                    case '\'':
                        if (input[input.Length - 1] != '\'')
                            return base.ChangeType(context, input, outputType, out success);
                        break;
                    case '{':
                        if (input[input.Length - 1] != '}')
                            return base.ChangeType(context, input, outputType, out success);
                        break;
                    case '[':
                        if (input[input.Length - 1] != ']')
                            return base.ChangeType(context, input, outputType, out success);
                        break;
                    default:
                        return base.ChangeType(context, input, outputType, out success);
                }
                try
                {
                    success = true;
                    return ComponentServices.ToJsonObject(outputType, input);
                }
                catch (Exception ex)
                {
                    Error.Add(ex);
                }
            }
            return base.ChangeType(context, input, outputType, out success);
        }
    }
}
