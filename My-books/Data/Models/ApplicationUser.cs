using Microsoft.AspNetCore.Identity;

namespace My_books.Data.Models
{
    public class ApplicationUser :IdentityUser
    {
        public string?  Custom { get; set; }

    }
}
