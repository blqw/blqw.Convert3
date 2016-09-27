using System;

namespace blqw.Converts
{
    internal sealed class CGuid : SystemTypeConvertor<Guid>
    {
        protected override Guid ChangeType(ConvertContext context, string input, Type outputType, out bool success)
        {
            if (input.Length == 0)
            {
                success = false;
                return Guid.Empty;
            }
            Guid result;
            success = Guid.TryParse(input, out result);
            return result;
        }

        protected override Guid ChangeTypeImpl(ConvertContext context, object input, Type outputType, out bool success)
        {
            var bytes = input as byte[];
            if (bytes?.Length == 16)
            {
                success = true;
                return new Guid(bytes);
            }
            if (input is decimal)
            {
                var arr = decimal.GetBits((decimal) input);
                bytes = new byte[16];
                Buffer.BlockCopy(arr, 0, bytes, 0, 16);
                success = true;
                return new Guid(bytes);
            }
            success = false;
            return default(Guid);
        }
    }
}