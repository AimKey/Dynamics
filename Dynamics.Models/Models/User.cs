﻿using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;

namespace Dynamics.Models.Models
{
    public class User
    {
        
        public Guid UserID { get; set; }
        [Required]
        [Display(Name = "Username")]
        public string UserFullName { get; set; }
        [DataType(DataType.Date)]
        [Display(Name = "Date of Birth")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateOnly? UserDOB { get; set; }
        [DataType(DataType.EmailAddress)]
        [Required]
        [Display(Name = "Email Address")]
        public string UserEmail { get; set; }
        [DataType(DataType.PhoneNumber)]
        [Display(Name = "Phone Number")]
        public string? UserPhoneNumber { get; set; }
        [ValidateNever]
        public string? UserAddress { get; set; }
        [ValidateNever]
        public string? UserAvatar { get; set; }
        public string? UserDescription { get; set; }
        [ValidateNever]
        public virtual ICollection<Report> ReportsMade { get; set; }
        public virtual ICollection<Award> Award { get; set; }
        public virtual ICollection<Request> Request { get; set; }
        public virtual ICollection<ProjectMember> ProjectMember { get; set; }
        public virtual ICollection<OrganizationMember> OrganizationMember { get; set; }
        public virtual ICollection<UserToOrganizationTransactionHistory> UserToOrganizationTransactions { get; set; }
        public virtual ICollection<UserToProjectTransactionHistory> UserToProjectTransactions { get; set; }
    }
}
