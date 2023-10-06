using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tazmania.Entities
{
    public class User
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; } = null!;

        [Required]
        public string Surname { get; set; } = null!;

        [Required, EmailAddress]
        public string Mail { get; set; } = null!;

        [Required]
        public string PasswordHash { get; set; } = null!;

        public bool NotifyMail { get; set; }
    }
}
