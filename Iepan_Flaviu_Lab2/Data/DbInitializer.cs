using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Iepan_Flaviu_Lab2.Models;

namespace Iepan_Flaviu_Lab2.Data
{
    public class DbInitializer
    {
        public static void Initialize(IServiceProvider serviceProvider)
        {
            using var context = new LibraryContext(
                serviceProvider.GetRequiredService<DbContextOptions<LibraryContext>>());

            // If there's existing data, do nothing
            if (context.Book.Any() || context.Genre.Any() || context.Author.Any() || context.Customer.Any())
            {
                return;
            }

            // Seed genres
            var roman = new Genre { Name = "Roman" };
            var nuvela = new Genre { Name = "Nuvela" };
            var poezie = new Genre { Name = "Poezie" };
            context.Genre.AddRange(roman, nuvela, poezie);
            context.SaveChanges();

            // Seed authors
            var sadoveanu = new Author { FirstName = "Mihail", LastName = "Sadoveanu" };
            var calinescu = new Author { FirstName = "George", LastName = "Calinescu" };
            var eliade = new Author { FirstName = "Mircea", LastName = "Eliade" };
            context.Author.AddRange(sadoveanu, calinescu, eliade);
            context.SaveChanges();

            // Seed books referencing genre and author IDs
            context.Book.AddRange(
                new Book { Title = "Baltagul", Price = 22m, GenreID = roman.ID, AuthorID = sadoveanu.ID },
                new Book { Title = "Enigma Otiliei", Price = 18m, GenreID = nuvela.ID, AuthorID = calinescu.ID },
                new Book { Title = "Maytrei", Price = 27m, GenreID = poezie.ID, AuthorID = eliade.ID }
            );

            // Seed customers
            context.Customer.AddRange(
                new Customer
                {
                    Name = "Popescu Marcela",
                    Adress = "Str. Plopilor, nr. 24",
                    BirthDate = DateTime.Parse("1979-09-01")
                },
                new Customer
                {
                    Name = "Mihailescu Cornel",
                    Adress = "Str. Bucuresti, nr. 45, ap. 2",
                    BirthDate = DateTime.Parse("1969-07-08")
                }
            );

            context.SaveChanges();
        }
    }
}