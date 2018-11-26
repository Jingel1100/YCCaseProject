using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebstoreMVC.Models
{
    public class ListViewModel<T>
    {
        public List<T> ListItems { get; set; }

        public bool AllowEdit { get; set; }

        public int PageSize { get; set; }
        public int PageNumber { get; set; }
        public int PageCount { get; set; }

        public ListViewModel()
        {
            PageSize = 10;
            PageNumber = 1;

            ListItems = new List<T>();
        }
    }
}