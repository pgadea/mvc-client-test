using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MVCClientTest.Models.Helpers
{
    public class PageInfo
    {
        public int PageNo { get; set; }

        public int PageSize { get; set; }

        public int TotalBooks { get; set; }

        public int TotalPages { get; set; }

        public string PrevPageLink { get; set; }

        public string NextPageLink { get; set; }
    }
}
