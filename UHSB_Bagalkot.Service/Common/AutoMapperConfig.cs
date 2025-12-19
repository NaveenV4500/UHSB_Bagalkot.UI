using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UHSB_Bagalkot.Data.Models;
using UHSB_Bagalkot.Service.ViewModels.AvailabilityTools;
using UHSB_Bagalkot.Service.ViewModels.Crop;
using UHSB_Bagalkot.Service.ViewModels.Sections;

namespace UHSB_Bagalkot.Service.Common
{
    public class AutoMapperConfig:Profile
    { 
        public AutoMapperConfig() {
            CreateMap<UhsbCategory, CategoryGridVM>().ReverseMap();
            CreateMap<UhsbCategory, RequestCategoryDetailsVM>().ReverseMap();


            CreateMap<UhsbCrop, CropGridVM>().ReverseMap();
            CreateMap<UhsbCrop, CropDetailsVM>().ReverseMap();  
            CreateMap<UhsbItemDeail, RequestItemDetailsVM>().ReverseMap();  
            CreateMap<UhsbItemDeail, ItemDetailsVM>().ReverseMap(); 
            CreateMap<UhsbSection, UhsbSectionVM>().ReverseMap();  
            CreateMap<UhsbImageFile, UhsbImageFileGridVM>().ReverseMap();
            CreateMap<UhsbSection, RequestSectionDetailsVM>().ReverseMap();
            CreateMap<UhsbSection, SectionsGridVM>().ReverseMap();
            CreateMap<UhsbSeedPlantingCenterMaster, UhsbSeedPlantingCenterMasterVM>().ReverseMap();
            CreateMap<UhsbAvailabilityToolsDetail, AvailabilityToolsDetailsVM>().ReverseMap();

            
        }
    }
}
