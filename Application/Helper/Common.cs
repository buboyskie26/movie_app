using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;

namespace Application.Helper
{
    public static class Common
    {
        public static T MostCommon<T>(this IEnumerable<T> list)
        {
            var most = (from i in list
                        group i by i into grp
                        orderby grp.Count() descending
                        select grp.Key).FirstOrDefault();
            // To use
            /*var most = list.MostCommon();*/
            return most;
        }
        public static IEnumerable<T> Mode<T>(this IEnumerable<T> input)
        {
            var dict = input.ToLookup(x => x);

            if (dict.Count == 0)
                return Enumerable.Empty<T>();

            var maxCount = dict.Max(x => x.Count());
            return dict.Where(x => x.Count() == maxCount).Select(x => x.Key);
            // uSe case
            /*var modes = { }.Mode().ToArray(); //returns { }*/
        }


    }
}
