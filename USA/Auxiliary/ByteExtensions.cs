using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UPD.Auxiliary
{
    public static class ByteExtensions
    {
        public static bool BitState(this byte b, int position)
        {
            if (position < 0 || position > 7)
                throw new ArgumentException("Номер позиции бита должен быть от 0 до 7");
            return (b & (1 << position)) >> position == 1;
        }
    }
}
