using System;

namespace Svr.Utils.Models
{
    public class PageViewModel
    {
        /// <summary>
        /// текущая страница
        /// </summary>
        public int PageNumber { get; private set; }
        /// <summary>
        /// всего страниц
        /// </summary>
        public int TotalPages { get; private set; }
        #region конструктор
        public PageViewModel(int count, int pageNumber, int pageSize)
        {
            PageNumber = pageNumber;
            TotalPages = (int)Math.Ceiling(count / (double)pageSize);
        }
        #endregion
        /// <summary>
        /// можно назад?
        /// </summary>
        public bool HasPreviousPage
        { get { return (PageNumber > 1); } }
        /// <summary>
        /// можно вперед?
        /// </summary>
        public bool HasNextPage
        { get { return (PageNumber < TotalPages); } }

        //public int TotalItems { get; set; }
        //public int ItemsPerPage { get; set; }
        //public int ActualPage { get; set; }

    }
}
