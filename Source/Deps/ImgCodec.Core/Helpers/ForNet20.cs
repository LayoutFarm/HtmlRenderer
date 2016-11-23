using System;
using System.Collections;
using System.Collections.Generic;
 
namespace ImageTools
{   
    public static class IEnum
    {
        public static byte[] ToArrayByBitsLength(byte[] bytes, int bits)
        {
            //Contract.Requires<ArgumentNullException>(bytes != null, "Bytes cannot be null.");
            //Contract.Requires<ArgumentException>(bits > 0, "Bits must be greater than zero.");
            //Contract.Ensures(Contract.Result<byte[]>() != null);

            byte[] result = null;

            if (bits < 8)
            {
                result = new byte[bytes.Length * 8 / bits];

                int factor = (int)Math.Pow(2, bits) - 1;
                int mask = (0xFF >> (8 - bits));
                int resultOffset = 0;

                for (int i = 0; i < bytes.Length; i++)
                {
                    for (int shift = 0; shift < 8; shift += bits)
                    {
                        //Contract.Assume(resultOffset < result.Length);
                        //Contract.Assume(factor > 0);

                        int colorIndex = (((bytes[i]) >> (8 - bits - shift)) & mask) * (255 / factor);

                        result[resultOffset] = (byte)colorIndex;

                        resultOffset++;
                    }

                }
            }
            else
            {
                result = bytes;
            }

            return result;
        }
        public static bool Contains(int[] array, int value)
        {
            for (int i = array.Length - 1; i >= 0; --i)
            {
                if (array[i] == value)
                {
                    return true;
                }
            }
            return false;
        }

        public static bool Any<T>(IEnumerable<T> ienum, TestDel<T> test)
        {
            foreach (T t in ienum)
            {
                if (test(t))
                {
                    return true;
                }
            }
            return false;
        }
        public static T First<T>(IEnumerable<T> ienum, TestDel<T> test)
        {
            foreach (T t in ienum)
            {
                if (test(t))
                {
                    return t;
                }
            }
            return default(T);
        }
        public static int Sum(IEnumerable<short> ienum1)
        {
            int total = 0;
            foreach (short s in ienum1)
            {
                total += s;
            }
            return total;
        }
        public static int Max<T>(IEnumerable<T> ienum1, GetValueDel<T> test)
        {
            int max = int.MinValue;
            foreach (T t in ienum1)
            {
                int value = test(t);
                if (value > max)
                {
                    max = value;
                }
            }
            return max;
        }
    }
}
namespace System
{ 
    public delegate bool TestDel<T>(T d);
    public delegate int GetValueDel<T>(T d);
     
    public delegate R MyFunc<T1, T2, R>(T1 t1, T2 t2); 
}
namespace System.Runtime.CompilerServices
{
    public partial class ExtensionAttribute : Attribute { }
}