using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Iepan_Flaviu_Lab2.Models
{
    public class Book
    {
        public int ID { get; set; }
        public string Title { get; set; } = default!;
        public decimal Price { get; set; }

        // Foreign key to the new Author entity
        public int? AuthorID { get; set; }
        public Author? AuthorRef{ get; set; }

        public int? GenreID { get; set; }
        public Genre? Genre { get; set; }
        public ICollection<Order>? Orders { get; set; }
    }
}
