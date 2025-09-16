using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks; 
namespace UHSB_Bagalkot.Service.Dto
{
    public class ItemImageDto
    {
        public int ImageId { get; set; }
        public int ItemId { get; set; }
        public string ImageUrl { get; set; }
        public string Description { get; set; }
    }

    public class ItemDto
    {
        public int ContentId { get; set; }
        public int ItemId { get; set; }
        public string Title { get; set; }
        public string Article { get; set; }
        public List<ItemImageDto> Images { get; set; }
    }


    public class CropFullDetailsDto
    {
        public CropDto Crop { get; set; }
        public IEnumerable<CropDetailDto> CropDetails { get; set; }
        public IEnumerable<ArticleDto> Articles { get; set; }
    }

    public  class CropDto
    {
        public int CropId { get; set; }

        public int CategoryId { get; set; }

        public string CropName { get; set; } = null!;

        public string? Description { get; set; }

        public string? ImageUrl { get; set; }
    }
    public class CropDetailDto
    {
        public int DetailId { get; set; }

        public int CropId { get; set; }

        public string DetailType { get; set; } = null!;

        public string? Title { get; set; }

        public string? Description { get; set; }
    }
    public class ArticleDto
    {
        public int ArticleId { get; set; }
        public int? CropId { get; set; }
        public  int? CropDetailID { get; set; }
        public string Title { get; set; } = null!;
        public string? Content { get; set; }
        public string? ImageUrl { get; set; }
        public IEnumerable<ArticleItemDto> ArticleItems { get; set; }

    }
    public class ArticleItemDto
    {
        public int ArticleItemId { get; set; }
        public int? ArticleId { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public string ImageUrl { get; set; }
    }



    //DD
    public class DropdownItemDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
}
