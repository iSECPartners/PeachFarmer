using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PeachFarmerTest
{
    static class Util
    {
        public static bool ListsEqual<T>(List<T> list1, List<T> list2)
        {
            return ArraysEqual(list1.ToArray(), list2.ToArray());
        }

        public static bool ArraysEqual<T>(T[] array1, T[] array2)
        {
            if (array1.Length != array2.Length)
            {
                return false;
            }

            for (int i = 0; i < array1.Length; i++)
            {
                if (!array1[i].Equals(array2[i]))
                {
                    return false;
                }
            }

            return true;
        }
    }
}
