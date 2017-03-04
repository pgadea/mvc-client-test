using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MVCClientTest.Models.ViewModels
{
    public class BookVM
    {
        public int Id { get; set; }

        public string Title { get; set; }

        public string Genre { get; set; }

        public DateTime PublishedOn { get; set; }

        public int NoofPages { get; set; }

        public string ISBN { get; set; }

        public string AuthorName { get; set; }

        public AuthorVM Author { get; set; }
    }
}
