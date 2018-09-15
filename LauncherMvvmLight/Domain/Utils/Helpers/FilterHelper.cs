using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;

//GetEnumDescription(States.NewMexico);
//DropDownList stateDropDown = new DropDownList();
//foreach (States state in EnumToList<States>())
//{
//    stateDropDown.Items.Add(GetEnumDescription(state));
//}
//Sample: IEnumerable<Foo> distinctList = sourceList.DistinctBy(x => x.FooName);
namespace LauncherMvvmLight.Domain.Utils.Helpers
{
    public static class FilterHelper
    {
        public static IEnumerable<TSource> IEnumDistinctBy<TSource, TKey>(
            this IEnumerable<TSource> source,
            Func<TSource, TKey> keySelector)
        {
            var knownKeys = new HashSet<TKey>();
            return source.Where(element => knownKeys.Add(keySelector(element)));
        }

        public static IList<TSource> IListDistinctBy<TSource, TKey>(
           this IList<TSource> source,
           Func<TSource, TKey> keySelector)
        {
            var knownKeys = new HashSet<TKey>();
            return (source.Where(element => knownKeys.Add(keySelector(element)))).ToList();
        }

    }
}