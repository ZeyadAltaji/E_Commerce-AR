using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace E_CommerceAR.Domain.ModalsBase
{
   
        public partial class User : BaseEntity
        {
            [Required]
            public string UserName { get; set; } = string.Empty;
            [Required]
            public string Frist_Name { get; set; } = string.Empty;
            [Required]
            public string Last_Name { get; set; } = string.Empty;
            [Required]
            [EmailAddress]
            public string Email { get; set; } = string.Empty;
            [Required]
            [MinLength(6)]
            public byte[] Password { get; set; }
            [Required]
            [Compare("Password")]
            public byte[] ComfirmPassword { get; set; }
            public byte[] PasswordKey { get; set; }
            [Required]
            public string Phone1 { get; set; } = string.Empty;
            [Required]
            public string Phone2 { get; set; } = string.Empty;
            [Required]
            public string Address { get; set; } = string.Empty;
            public int Role { get; set; }

            [NotMapped]
            public IFormFile Image_userUrl { get; set; }
            public string Public_id { get; set; } = string.Empty;
        
    }
}
