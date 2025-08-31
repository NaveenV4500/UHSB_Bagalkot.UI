using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UHSB_Bagalkot.Service.Dto;

namespace UHSB_Bagalkot.Service.Interface
{
    public interface ICropProfileRepository
    {
        Task<CropFullDetailsDto> GetCropFullDetailsAsync(int cropId);
    }
}
