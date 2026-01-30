using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UHSB_Bagalkot.Data.Models;
using UHSB_Bagalkot.Service.ViewModels.AvailabilityTools;
using UHSB_Bagalkot.Service.ViewModels.CartOrder;
using UHSB_Bagalkot.Service.ViewModels.Crop;
using UHSB_Bagalkot.Service.ViewModels.Product;
using UHSB_Bagalkot.Service.ViewModels.Sections;

namespace UHSB_Bagalkot.Service.Common
{
    public class AutoMapperConfig : Profile
    {
        public AutoMapperConfig()
        {
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
            CreateMap<UhsbProduct, ProductsVM>().ReverseMap();


            CreateMap<ProductVarietyVM, UhsbProductVariety>()
           .ForMember(d => d.CreatedDate, opt => opt.Ignore())
           .ForMember(d => d.ModifiedDate, opt => opt.Ignore())
           .ForMember(d => d.CreatedBy, opt => opt.Ignore())
           .ForMember(d => d.ModifiedBy, opt => opt.Ignore())
           .ForMember(d => d.VarietiesId, opt => opt.Ignore());
            CreateMap<UhsbProduct, Product_SP_VM>();


            CreateMap<Product_SP_VM, UhsbProduct>()
           .ForMember(d => d.CreatedDate, opt => opt.Ignore())
           .ForMember(d => d.ModifiedDate, opt => opt.Ignore())
           .ForMember(d => d.CreatedBy, opt => opt.Ignore())
           .ForMember(d => d.ModifiedBy, opt => opt.Ignore())
           .ForMember(d => d.ProductId, opt => opt.Ignore()); 

            CreateMap<UhsbProduct, Product_SP_VM>();

            CreateMap<UhsbCartMaster, CartMasterVM>();
            CreateMap<UhsbCartItem, CartItemVM>();

            CreateMap<UhsbOrderMaster, OrderMasterVM>();
            CreateMap<UhsbOrderItem, OrderItemVM> ();

        }
    }
}
