using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UHSB_Bagalkot.Service.Common
{
    public class CommonEnum
    {
        public enum MonthsName
        {
            [Display(Name = " -- Select-- ")]
            Select = 0,
            [Display(Name = "January")]
            Jan = 1,
            [Display(Name = "February")]
            Feb = 2,
            [Display(Name = "March")]
            Mar = 3,
            [Display(Name = "April")]
            Apr = 4,
            [Display(Name = "May")]
            May = 5,
            [Display(Name = "June")]
            Jun = 6,
            [Display(Name = "July")]
            Jul = 7,
            [Display(Name = "August")]
            Aug = 8,
            [Display(Name = "September")]
            Sep = 9,
            [Display(Name = "October")]
            Oct = 10,
            [Display(Name = "November")]
            Nov = 11,
            [Display(Name = "December")]
            Dec = 12
        }

        public enum YesNoBoth
        {
            [Display(Name = "All", Order = 1)]
            All = 0,
            [Display(Name = "Yes", Order = 2)]
            Yes = 1,
            [Display(Name = "No")]
            No = 2,
        }
        public enum FileTypes
        {
            [Display(Name = "Category")]
            Category = 1,
            [Display(Name = "Crops")]
            Crops = 2,
            [Display(Name = "Sections")]
            Sections = 3,
            [Display(Name = "Items")]
            Items = 4,
            [Display(Name = "ItemContent")]
            ItemContent = 5,
        }
        public enum RCMType
        {
            [Display(Name = " -- Select-- ", Order = 1)]
            Select = 0,
            [Display(Name = "Arbitral Tribunal")]
            ArtibitralTribunal = 1,
            [Display(Name = "Goods Transport Agency (GTA)")]
            GoodsTransportAgency = 2,
            [Display(Name = "Individual Direct Selling Agents (DSAs) other than a body corporate, partnership or limited liability partnership firm to bank or NBFCs")]
            IndividualDirectSellingAgentsDSAs = 13,
            [Display(Name = "Insurance agent  business")]
            InsuranceAgentBusiness = 3,
            [Display(Name = "Legal Services, Directly or Indirectly")]
            LeagalServicesDirectlyorIndirectly = 4,
            [Display(Name = "Located in a Non-taxable territory or Service by Unregistred Persons")]
            LocatedinNonTaxableterritoryorServiceByUnregistredPersons = 5,
            [Display(Name = "Priority Sector Lending Certificates(PSLCs)")]
            PrioritySectorLendingCertificates = 16,
            [Display(Name = "Radio taxi or Passenger Transport Services provided through electronic commerce operator")]
            RadioSTaxiOrPassengerTransportServicesProvidedThroughElectronicCommerceOperator = 6,
            [Display(Name = "Recovery Agent Service")]
            RecoveryAgentService = 7,
            [Display(Name = "Renting of Motor Vehicle to a Body Corporate")]
            RentingMotorVehicleToBodyCorporate = 17,
            [Display(Name = "Security Services (other than body corporate)")]
            SecurityServicesOtherThanBodyCorporate = 15,
            [Display(Name = "Services by government or local authority")]
            ServicesByGovernmentOrLocalAuthority = 8,
            [Display(Name = "Services by director of a company or a body corporate")]
            ServicesByDirectorOfCompanyOrBodyCorporate = 9,
            [Display(Name = "Specified Goods")]
            SpecifiedGoods = 14,
            [Display(Name = "Sponsorship Services")]
            SponsorshipServices = 12,
            [Display(Name = "Transfer or permitting the use or enjoyment of a copyright original to literary, dramatic, musical or artistic works")]
            TransferSOrPermittingUseOrEnjoymentOfCopyrightOriginalToLiteraryDramaticMusicalOrArtisticWorks = 10,
            [Display(Name = "Transportation of goods by a vessel from a place outside India up to the customs station of clearance in India")]
            TransportationOfGoodsByVesselFromPlaceOutsideIndiaUptotheCustomsStationOfClearanceInIndia = 11,
            [Display(Name = "Renting Of Residential Dwelling")]
            RentingOfResidentialDwelling = 18,
            [Display(Name = "Renting Of Residential Dwelling (PMO)")]
            RentingOfResidentialDwellingPMO = 19,
            [Display(Name = "Rent to unregistered vendors")]
            Renttounregisteredvendors = 20,
            [Display(Name = "Rent paid to Govt or Local authorities")]
            RentpaidtoGovtorLocalauthorities = 21,
        }
        public enum UserRoleType
        {
            [Display(Name = "-- Select --", Order = 1)]
            Select = 0,

            [Display(Name = "Admin")]
            Admin = 1,

            [Display(Name = "Farmer")]
            Farmer = 2,

            [Display(Name = "Scientist")]
            Scientist = 3,

            [Display(Name = "Input Dealer")]
            InputDealer = 4,

            [Display(Name = "Student")]
            Student = 5,

            [Display(Name = "Others")]
            Others = 6
        }
        public enum RecordHeadType
        {
            [Display(Name = "-- Select --", Order = 1)]
            Select = 0, 
            [Display(Name = "Center Variates")]
            Centervariates = 1
        }

        public enum UserRoleTypeforSearch
        {
            [Display(Name = " -- Select-- ", Order = 1)]
            Select = 0,
            [Display(Name = "Maker")]
            Maker = 1,
            [Display(Name = "Checker")]
            Checker = 2,
            [Display(Name = "Branch Admin")]
            BranchAdmin = 3,
            [Display(Name = "Manager")]
            Manager = 5
        }

        public enum UnitType
        {
            Select = 0,
            Nos = 2,
            Packets = 3
        }

        public static void WriteLog(string message)
        {
            try
            {
                string logPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Error_Logs");
                if (!Directory.Exists(logPath))
                    Directory.CreateDirectory(logPath);

                string logFile = Path.Combine(logPath, $"Login_{DateTime.Now:yyyyMMdd}.txt");
                string logMessage = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss} - {message}{Environment.NewLine}";
                System.IO.File.AppendAllText(logFile, logMessage);
            }
            catch
            {
                 
            }
        }

    }

  
}
