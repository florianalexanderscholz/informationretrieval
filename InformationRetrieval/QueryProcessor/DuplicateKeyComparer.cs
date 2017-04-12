using System;
using System.Collections.Generic;

namespace InformationRetrieval.QueryProcessor
{
    /// <summary>
    /// from http://stackoverflow.com/questions/5716423/c-sharp-sortable-collection-which-allows-duplicate-keys
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    public class DuplicateKeyComparer<TKey>
        :
            IComparer<TKey> where TKey : IComparable
    {
        #region IComparer<TKey> Members

        public int Compare(TKey x, TKey y)
        {
            int result = x.CompareTo(y);

            if (result == 0)
                return 1;   // Handle equality as beeing greater
            else
                return result;
        }

        #endregion
    }
}