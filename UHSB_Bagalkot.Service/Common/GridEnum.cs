using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UHSB_Bagalkot.Service.Common
{
    public class GridEnum
    {
        public enum DataTypeEnum
        {
            Numeric = 1,
            String = 2,
            Enum = 3,
            DateTime = 4,
            Boolean = 5,
            Decimal = 6
        }

        public enum FilterTypeEnum
        {
            Contains = 1,
            Equal = 2,
            GreaterThan = 3,
            LessThan = 4,
            StartsWith = 5,
            EndsWith = 6,
            InBetween = 7
        }
         
        
        public enum GSTR1ErrorLogEnum
        {
            Select = 0,
            [DropDownAttribute(typeof(int))]
            ErrNumber = 1,
            [DropDownAttribute(typeof(int))]
            ErrSeverity = 2,
            [DropDownAttribute(typeof(string))]
            ErrorProcedure = 3,
            [DropDownAttribute(typeof(int))]
            ErrLine = 4,
            [DropDownAttribute(typeof(string))]
            ErrMessage = 5
        }


        public enum InwardLogViewEnum
        {
            Select = 0,
            [DropDownAttribute(typeof(string))]
            BranchName = 1,
            [DropDownAttribute(typeof(string))]
            BranchCode = 2,
            [DropDownAttribute(typeof(string))]
            ServicingBranchGSTIN = 3,
            [DropDownAttribute(typeof(string))]
            InvoiceNumber = 4,
            [DropDownAttribute(typeof(DateTime?))]
            InvoiceDate = 5,
            [DropDownAttribute(typeof(string))]
            VendorName = 6,
            [DropDownAttribute(typeof(string))]
            VendorCode = 7,
            [DropDownAttribute(typeof(string))]
            BankStateName = 8,
            [DropDownAttribute(typeof(string))]
            VendorGSTIN = 9,
            [DropDownAttribute(typeof(string))]
            CBSUniqueReferenceNumber = 10,
            [DropDownAttribute(typeof(string))]
            GcNumber = 11,
            [DropDownAttribute(typeof(decimal?))]
            TotalTaxableAmount = 12,
            [DropDownAttribute(typeof(decimal?))]
            TotalAmount = 13,
            [DropDownAttribute(typeof(AuditStatusEnum))]
            AuditStatus = 14,
        }

        public enum AuditStatusEnum
        {
            [DropDownAttribute(typeof(int))]
            Deleted = 6,
        }

        
        public enum IMSnvoiceDataEnum
        {
            Select = 0,

            [DropDownAttribute(typeof(string))]
            InvoiceNumber = 1,

            [DropDownAttribute(typeof(string))]
            GSTINOfSupplier = 2,

            [DropDownAttribute(typeof(DateTime?))]
            InvoiceDate = 3,

            [DropDownAttribute(typeof(decimal?))]
            InvoiceValue = 4,

            [DropDownAttribute(typeof(decimal?))]
            TotalTaxableValue = 5,

            [DropDownAttribute(typeof(decimal?))]
            IntegratedTax = 6,

            [DropDownAttribute(typeof(decimal?))]
            CentralTax = 7,

            [DropDownAttribute(typeof(decimal?))]
            StateOrUTTax = 8,

            [DropDownAttribute(typeof(decimal?))]
            Cess = 9,

            [DropDownAttribute(typeof(string))]
            OriginalInvoiceNumber = 10,

            [DropDownAttribute(typeof(DateTime?))]
            OriginalInvoiceDate = 11
        }
        public enum ReferenceEnum
        {
            Select = 0,
            [DropDownAttribute(typeof(string))]
            Description = 1,
            [DropDownAttribute(typeof(string))]
            UserName = 2,
            [DropDownAttribute(typeof(string))]
            BranchName = 3,
            [DropDownAttribute(typeof(string))]
            DepartmentName = 4,

        }
        public enum FTPDocumentsLogs
        {
            Select = 0,
            [DropDownAttribute(typeof(string))]
            Description = 1,
            [DropDownAttribute(typeof(string))]
            DepartmentName = 2,
            [DropDownAttribute(typeof(string))]
            UserName = 3,
            [DropDownAttribute(typeof(string))]
            BranchName = 4,
            [DropDownAttribute(typeof(DateTime?))]
            UploadedDate = 5,
            [DropDownAttribute(typeof(string))]
            DeletedByUserName = 6,
            [DropDownAttribute(typeof(string))]
            UploadedByUserName = 7,
            [DropDownAttribute(typeof(string))]
            DeletedRemarks = 8,
            [DropDownAttribute(typeof(DateTime?))]
            DeletedDate = 9,
            [DropDownAttribute(typeof(byte))]
            DatastatusName = 10,
        }
        public enum AvailabilityToolsFilterBy
        {
            Select = 0,

            // Main searchable fields
            [DropDownAttribute(typeof(string))]
            AvailToolNameKnd = 1,          // ಸಸಿಯ ಹೆಸರು (ಕನ್ನಡ)

            [DropDownAttribute(typeof(string))]
            RecordHeadKnd = 2,             // ವರ್ಗ (ಸಸಿ, ಪರಿಕರ, ಗೊಬ್ಬರ...)

            [DropDownAttribute(typeof(string))]
            AvailToolNameEng = 3,          // English name (optional)

            [DropDownAttribute(typeof(string))]
            Unit = 4,                      // ಘಟಕ (piece, kg, etc.)

            [DropDownAttribute(typeof(decimal?))]
            Price = 5,                     // ಬೆಲೆ (for range filter)

            [DropDownAttribute(typeof(int?))]
            Quantity = 6,                  // ಲಭ್ಯ ಪ್ರಮಾಣ

            [DropDownAttribute(typeof(string))]
            Remarks = 7,                   // ಟಿಪ್ಪಣಿ

            [DropDownAttribute(typeof(DateTime?))]
            CreatedDate = 8,               // ದಿನಾಂಕ

            [DropDownAttribute(typeof(DateTime?))]
            AvailabilityDate = 9,          // ಲಭ್ಯತೆ ದಿನಾಂಕ

            [DropDownAttribute(typeof(string))]
            CenterNameKnd = 10,            // ಕೇಂದ್ರದ ಹೆಸರು
        }

        public enum UserMasterColumns
        {
            Select = 0,

            [DropDownAttribute(typeof(string))]
            UserName = 1,

            [DropDownAttribute(typeof(string))]
            PhoneNumber = 2,

            [DropDownAttribute(typeof(bool))]
            IsActive = 3,

            [DropDownAttribute(typeof(DateTime))]
            CreatedDate = 4,

            [DropDownAttribute(typeof(DateTime?))]
            UpdatedDate = 5,

            [DropDownAttribute(typeof(string))]
            Village = 6,

            [DropDownAttribute(typeof(decimal?))]
            LandSize = 7,

            [DropDownAttribute(typeof(string))]
            PasswordHash = 8,

            [DropDownAttribute(typeof(byte?))]
            DistrictsId = 9,

            [DropDownAttribute(typeof(DateTime?))]
            ModifiedDate = 10,

            [DropDownAttribute(typeof(byte?))]
            ModifiedBy = 11,

            [DropDownAttribute(typeof(byte?))]
            CreatedBy = 12,

            [DropDownAttribute(typeof(int))]
            RoleType = 13
        }

    }

    [AttributeUsage(AttributeTargets.Field, Inherited = false, AllowMultiple = false)]
    public class DropDownAttribute : Attribute
    {
        public DropDownAttribute(Type type)
        {

        }
    }
}
