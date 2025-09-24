using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UHSB_Bagalkot.Data.Models;
using UHSB_Bagalkot.Service.Dto;
using UHSB_Bagalkot.Service.Interface;
using UHSB_Bagalkot.Service.ViewModels;
using UHSB_Bagalkot.Service.ViewModels.CropProfile;

namespace UHSB_Bagalkot.Service.Repositories
{
    public class CropProfileRepository : CommonConnection, ICropProfileRepository
    {
        private readonly IMapper _mapper;

        public CropProfileRepository(Uhsb2025Context context) : base(context)
        {

        }

        public IEnumerable<UhsbCategory> GetCategoryItems()
        {
            return _context.UhsbCategories.ToList();
        }

        public IEnumerable<UhsbCrop> GetCropDetailsAsync(int categoryId = 0)
        {
            return _context.UhsbCrops.Where(c => c.CategoryId == categoryId).ToList();
        }

        public IEnumerable<UhsbSection> GetSectionsByCropId(int cropId)
        {
            return _context.UhsbSections
                .Where(s => s.CropId == cropId)
                .ToList();
        }

        public IEnumerable<UhsbSubSection> GetSubSectionsBySectionId(int sectionId)
        {
            return _context.UhsbSubSections
                .Where(ss => ss.SectionId == sectionId)
                .ToList();
        }

        public async Task<IEnumerable<UhsbItemDeail>> GetItemsBySubSectionIdAsync(int subSectionId)
        {
            var items = await _context.UhsbItemDeails
                .Where(i => i.SubSectionId == subSectionId).ToListAsync();
            return items;
        }




        //public async Task<IEnumerable<ItemDto>> GetItemsBySubSectionIdAsync(int subSectionId)
        //{
        //    var items = await _context.UhsbItemDeails
        //        .Where(i => i.SubSectionId == subSectionId)
        //        .Select(i => new ItemDto
        //        {
        //            ItemId = i.ItemId,
        //            Name = i.Name,
        //            ImageUrl = i.ImageUrl,
        //            Images = i.UhsbItemImages
        //                      .Select(img => new ItemImageDto
        //                      {
        //                          ImageId = img.ImageId,
        //                          ImageUrl = img.ImageUrl,
        //                          Description = img.Description
        //                      }).ToList()
        //        }).ToListAsync();

        //    return items;
        //}


        public async Task<List<ItemDto>> GetContentByItemIdAsyncOld(int itemId)
        {
            return await _context.ItemContents
                .Where(c => c.ItemId == itemId)
                .Select(c => new ItemDto
                {
                    ContentId = c.ContentId,
                    ItemId = c.ItemId,
                    Title = c.Title,
                    Article = c.Article,
                    Images = _context.UhsbItemImages
                                .Where(i => i.ItemId == c.ItemId)
                                .Select(i => new ItemImageDto
                                {
                                    ImageId = i.ImageId,
                                    ItemId = i.ItemId,
                                    ImageUrl = i.ImageUrl,
                                    Description = i.Description
                                })
                                .ToList()
                })
                .ToListAsync();
        }

        public async Task<List<ItemDto>> GetContentByItemIdAsync(int itemId)
        {
            return await _context.ItemContents
                .Where(c => c.ItemId == itemId)
                .Select(c => new ItemDto
                {
                    ContentId = c.ContentId,
                    ItemId = c.ItemId,
                    Title = c.Title,
                    Article = c.Article,
                    Images = _context.UhsbItemImages
                                .Where(i => i.ItemId == c.ItemId)
                                .Select(i => new ItemImageDto
                                {
                                    ImageId = i.ImageId,
                                    ItemId = i.ItemId,
                                    ImageUrl = "/InwardsInvoices/TempFiles/" + i.ImageUrl, // relative path
                                    Description = i.Description
                                }).ToList()
                }).ToListAsync();
        }


        public async Task<IEnumerable<UhsbItemQnA>> GetByItemIdAsync(int itemId)
        {
            if (itemId == 0)
                return await _context.UhsbItemQnAs
                .OrderByDescending(q => q.CreatedDate)
                .ToListAsync();

            return await _context.UhsbItemQnAs
                .Where(q => q.ItemId == itemId)
                .OrderByDescending(q => q.CreatedDate)
                .ToListAsync();
        }

        public async Task<UhsbItemQnA> AddAsync(UhsbItemQnAVM qnaVm)
        {
            var qna = new UhsbItemQnA
            {
                ItemId = qnaVm.ItemId,
                Question = qnaVm.Question,
                Answer = qnaVm.Answer ?? string.Empty,
                CreatedDate = DateTime.UtcNow,
                Datastatus = qnaVm.Datastatus,
                ImageUrl = qnaVm.ImageUrl,
                UserId = qnaVm.UserID
            };

            // Add and save
            _context.UhsbItemQnAs.Add(qna);
            await _context.SaveChangesAsync();

            return qna;
        }


        //public async Task<CropFullDetailsDto> GetCropFullDetailsAsync(int cropId)
        //{
        //    // 1️⃣ Get Crop basic info
        //    var crop = await _context.UhsbCrops
        //        .Where(c => c.CropId == cropId)
        //        .Select(c => new CropDto
        //        {
        //            CropId = c.CropId,
        //            CategoryId = c.CategoryId,
        //            CropName = c.Name,
        //            Description = c.Description,
        //            ImageUrl = c.ImageUrl
        //        })
        //        .FirstOrDefaultAsync();

        //    if (crop == null) return null;

        //    // 2️⃣ Get Sections & SubSections
        //    var sections = await _context.UhsbSections
        //        .Where(s => s.CropId == cropId)
        //        .Select(s => new SectionDto
        //        {
        //            SectionId = s.SectionId,
        //            CropId = s.CropId,
        //            Name = s.Name,
        //            Description = s.Description,
        //            ImageUrl = s.ImageUrl,
        //            SubSections = _context.UhsbSubSections
        //                .Where(ss => ss.SectionId == s.SectionId)
        //                .Select(ss => new SubSectionDto
        //                {
        //                    SubSectionId = ss.SubSectionId,
        //                    SectionId = ss.SectionId,
        //                    Name = ss.Name,
        //                    Description = ss.Description,
        //                    ImageUrl = ss.ImageUrl,
        //                    Items = _context.UhsbItemDeails
        //                        .Where(i => i.SubSectionId == ss.SubSectionId)
        //                        .Select(i => new ItemDto
        //                        {
        //                            ItemId = i.ItemId,
        //                            SubSectionId = i.SubSectionId,
        //                            Name = i.Name,
        //                            ImageUrl = i.ImageUrl,
        //                            ItemImages = _context.UhsbItemImages
        //                                .Where(img => img.ItemId == i.ItemId)
        //                                .Select(img => new ItemImageDto
        //                                {
        //                                    ImageId = img.ImageId,
        //                                    ImageUrl = img.ImageUrl,
        //                                    Description = img.Description
        //                                }).ToList(),
        //                            Contents = _context.ItemContents
        //                                .Where(c => c.ItemId == i.ItemId)
        //                                .Select(c => new ItemContentDto
        //                                {
        //                                    ContentId = c.ContentId,
        //                                    Title = c.Title,
        //                                    Article = c.Article,
        //                                    CreatedDate = c.CreatedDate
        //                                }).ToList(),
        //                            QnAs = _context.UhsbItemQnAs
        //                                .Where(q => q.ItemId == i.ItemId)
        //                                .Select(q => new ItemQnADto
        //                                {
        //                                    QnAId = q.QnAId,
        //                                    Question = q.Question,
        //                                    Answer = q.Answer,
        //                                    CreatedDate = q.CreatedDate
        //                                }).ToList()
        //                        }).ToList()
        //                }).ToList()
        //        })
        //        .ToListAsync();

        //    return new CropFullDetailsDto
        //    {
        //        Crop = crop,
        //        Sections = sections
        //    };
        //}


        public async Task AddCropFullDetailsAsync(CropFullDetailsDto cropDetails)
        {
            if (cropDetails == null)
                throw new ArgumentNullException(nameof(cropDetails));

            //var cropEntity = new Crop
            //{
            //    CropName = cropDetails.CropName,
            //    CategoryVM = cropDetails.CategoryVM,
            //    Variety = cropDetails.Variety,
            //    PlantingDate = cropDetails.PlantingDate,
            //    HarvestDate = cropDetails.HarvestDate,
            //    Notes = cropDetails.Notes
            //    // add other properties here
            //};

            //_context.CropFullDetails.Add(cropEntity);

            await _context.SaveChangesAsync();
        }

        public async Task<List<DropdownItemDto>> GetCategoryForDD()
        {
            return await _context.UhsbCategories
                .OrderBy(c => c.Name)
                .Select(c => new DropdownItemDto
                {
                    Id = c.CategoryId,
                    Name = c.Name
                })
                .ToListAsync();
        }

        public async Task<List<FarmersProfile>> GetAllFarmers()
        {
            return _context.FarmersProfiles
                .OrderByDescending(f => f.CreatedDate)
                .ToList();
        }

       

    }
}
