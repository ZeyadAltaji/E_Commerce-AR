﻿using Google.Cloud.Firestore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_CommerceAR.Domain.ModalsBase
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
        [FirestoreProperty("ComfirmPassword")]
        [Required]
        [Compare("password")]
        public string ComfirmPassword { get; set; }
        [FirestoreProperty("MobileNo")]
        [Required]
        public string MobileNo { get; set; } = string.Empty;
        [FirestoreProperty("Address")]
        [Required]
        public string Address { get; set; } = string.Empty;
        [FirestoreProperty("Role")]
        public int Role { get; set; }
        [FirestoreProperty("ISActive")]
        public bool ISActive { get; set; }
        [FirestoreProperty("IsDeleted")]
        public bool ISDeleted { get; set; }
        [FirestoreProperty("Unapproved")]
        public bool Unapproved { get; set; }
    }
}
