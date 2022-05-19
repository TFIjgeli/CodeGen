using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace CodeGen.Domain.Common
{
    public class Pagination<TEntity>
    {

        [JsonIgnore]
        private IEnumerable<TEntity> QueriedList { get; set; }
        [JsonIgnore]
        private int PageSize { get; set; }
        private int CurrentPage { get; set; }
        [JsonIgnore]
        private int TotalItems { get; set; }


        public Pagination()
        {

        }
        public Pagination(IEnumerable<TEntity> queriedList, int totalItems, int currentPage, int pageSize)
        {
            QueriedList = queriedList;
            CurrentPage = currentPage;
            PageSize = pageSize;
            //QueryCreated = queryCreated;
            TotalItems = totalItems;
        }



        public List<TEntity> List
        {
            //get; set;
            get { return QueriedList.ToList(); }
        }

        //public IEnumerable<int> PageIndices
        //{
        //    get { return TotalItems == 0 ? Enumerable.Empty<int>() : GetPageRange(); }
        //}
        public string PageSummary
        {
            get { return TotalItems == 0 ? "No results found" : $"Showing {FirstShownEntry} to {LastShownEntry} of {TotalItems} {EntryLabel}"; }
        }

        public string EntryLabel
        {
            get { return TotalItems > 1 ? "entries" : "entry"; }
        }

        public int PageCount
        {
            get { return (int)Math.Ceiling((decimal)TotalItems / PageSize); }
        }

        public int FirstShownEntry
        {
            get { return (CurrentPage * PageSize) - PageSize + 1; }
        }

        public int LastShownEntry
        {
            get { return CurrentPage * PageSize > TotalItems ? TotalItems : CurrentPage * PageSize; }
        }



        //private IEnumerable<int> GetPageRange()
        //{
        //    List<int> range = new List<int>();
        //    int maxRangeSize = 10;

        //    // Start of List
        //    if (CurrentPage <= (int)Math.Ceiling((decimal)maxRangeSize / 2) || PageCount < maxRangeSize)
        //    {
        //        for (int i = 1; i <= maxRangeSize; i++)
        //        {
        //            range.Add(i);
        //            if (i == PageCount) break;
        //        }
        //    }
        //    // End of List
        //    else if (CurrentPage + (int)Math.Ceiling((decimal)maxRangeSize / 2) > PageCount)
        //    {
        //        for (int i = PageCount - maxRangeSize + 1; i <= PageCount; i++)
        //        {
        //            range.Add(i);
        //        }
        //    }
        //    // Middle of list
        //    else
        //    {
        //        int startIndex = (int)Math.Floor((decimal)maxRangeSize / 2);
        //        if (maxRangeSize % 2 == 0) startIndex--;
        //        for (int i = CurrentPage - startIndex; i <= CurrentPage + (int)Math.Floor((decimal)maxRangeSize / 2); i++)
        //        {
        //            range.Add(i);
        //        }
        //    }

        //    return range.ToArray();
        //}
    }
}
