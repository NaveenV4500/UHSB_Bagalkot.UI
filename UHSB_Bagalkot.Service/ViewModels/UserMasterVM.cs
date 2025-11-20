using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.InteropServices.JavaScript;
using System.Text;
using System.Threading.Tasks;
using UHSB_Bagalkot.Service.Common;

namespace UHSB_Bagalkot.Service.ViewModels
{
    public class UserMasterVM
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public string PhoneNumber { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string? Village { get; set; }
        public decimal? LandSize { get; set; }
        public string PasswordHash { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public byte? ModifiedBy { get; set; }
        public byte? CreatedBy { get; set; }
        [Required]
        [Range(1, byte.MaxValue, ErrorMessage = "Role Type is required.")]
        [Display(Name = "Role Type")]
        public int RoleType { get; set; }
        public Dictionary<int, string> RoleTypeList { get; set; }
        [Required]
        [Range(1, byte.MaxValue, ErrorMessage = "District is required.")]
        [Display(Name = "Select District ")]
        public short DistrictsId { get; set; }
        public string DistrictsName { get; set; }
        public Dictionary<int, string> DistrictsList { get; set; }
        public string RoleTypeName { get { return EnumHelper<CommonEnum.UserRoleType>.GetName(RoleType); } }
        public string? Address { get; set; }
        public int? EmployeeId { get; set; }
        public string? EmailId { get; set; }

    }
    public class UserMasterRequestmobile
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public string PhoneNumber { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string? Village { get; set; }
        public decimal? LandSize { get; set; }
        public string PasswordHash { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public byte? ModifiedBy { get; set; }
        public byte? CreatedBy { get; set; } 
        public int RoleType { get; set; } 
        public short DistrictsId { get; set; }
        public string? Address { get; set; }
        public int? EmployeeId { get; set; }
        public string? EmailId { get; set; }

    }
}
