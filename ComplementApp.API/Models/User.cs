using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace ComplementApp.API.Models
{
    [Table("TUser")]
    public class User
    {
        public int Id { get; set; }

        public string Username { get; set; }

        public byte[] PasswordHash { get; set; }

        public byte[] PasswordSalt { get; set; }

        public string Gender {get; set;}

        public DateTime DayOfBirth { get; set; }

        public DateTime LastActive { get; set; }

        public DateTime Created { get; set; }

        public string KnownAs {get; set;}

        public string Introduction { get; set; }

        public string LookingFor { get; set; }

        public string Interests { get; set; }

        public string  City { get; set; }

        public string Country { get; set; }

        public ICollection<Photo> Photos { get; set; }
    }
}