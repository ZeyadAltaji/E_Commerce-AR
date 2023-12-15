using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Google.Cloud.Firestore;

namespace E_CommerceAR.Domain.ModalsBase
{
    [FirestoreData]

        public partial class Users : BaseEntity
        {
            [Required]
            [FirestoreProperty("UserName")]

            public string UserName { get; set; } = string.Empty;
            [Required]
            [FirestoreProperty("firstName")]

            public string Frist_Name { get; set; } = string.Empty;
            [Required]
            [FirestoreProperty("lastName")]

            public string Last_Name { get; set; } = string.Empty;
            [Required]
            [EmailAddress]
            [FirestoreProperty("email")]

            public string Email { get; set; } = string.Empty;
            [Required]
            [MinLength(6)]
            [FirestoreProperty("password")]

            public string Password { get; set; }
            [Required]
            [Compare("Password")]

            public string ComfirmPassword { get; set; }
            [FirestoreProperty("Role")]
            public int Role { get; set; }

            [FirestoreProperty("ISActive")]
            public bool IsActive { get; set; }

            [FirestoreProperty("IsDeleted")]
            public bool IsDeleted { get; set; }
        public string PasswordKey { get; set; }
            [Required]
            public string Phone1 { get; set; } = string.Empty;
            [Required]
            public string Phone2 { get; set; } = string.Empty;
            [Required]
            public string Address { get; set; } = string.Empty;
 
            [NotMapped]
            public IFormFile Image_userUrl { get; set; }
            public string Public_id { get; set; } = string.Empty;
        
    }
}
