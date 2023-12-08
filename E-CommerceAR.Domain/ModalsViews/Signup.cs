using Google.Cloud.Firestore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_CommerceAR.Domain.ModalsViews
{
    [FirestoreData]
    public partial class Signup
    {

        [FirestoreProperty("firstName")]
        [Required(ErrorMessage = "his field is required")]
        public string firstName { get; set; } = string.Empty;
        [FirestoreProperty("lastName")]
        [Required(ErrorMessage = "his field is required")]
        public string lastName { get; set; } = string.Empty;
        [FirestoreProperty("email")]
        [Required(ErrorMessage = "his field is required")]
        [EmailAddress]
        public string email { get; set; } = string.Empty;
        [FirestoreProperty("password")]
        [Required(ErrorMessage = "his field is required")]
        public string password { get; set; } = string.Empty;
        [FirestoreProperty("Role")]
        public int Role { get; set; }
        [FirestoreProperty("ISActive")]
        public bool IsActive { get; set; }
        [FirestoreProperty("IsDeleted")]
        public bool IsDeleted { get; set; }
    }
}
