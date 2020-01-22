using ADO.Mapper.Interfaces;
using System.Collections.Generic;

namespace ADO.Mapper.Classes
{
    public class ADOPagedList<T> : List<T>, IPagedList<T>
    {
        #region fields
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; set; }
        public int TotalPages { get; set; }

        public bool HasPreviousPage
        {
            get { return (PageIndex > 0); }
        }
        public bool HasNextPage
        {
            get { return (PageIndex + 1 < TotalPages); }
        }
        #endregion
    }
}
