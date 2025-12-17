using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Iepan_Flaviu_Lab2.Models
{
    public class Author
    {
        public int ID { get; set; }
        [Display(Name = "First name: ")]
        public string FirstName { get; set; } = default!;
        [Display(Name = "Last name: ")]
        public string LastName { get; set; } = default!;
        public ICollection<Book>? Books { get; set; }
    }
}