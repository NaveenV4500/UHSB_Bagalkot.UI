using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using UHSB_Bagalkot.Data;
using UHSB_Bagalkot.Service.Interface;
using UHSB_Bagalkot.Service.Repositories;

namespace UHSB_Bagalkot.UI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Microsoft.AspNetCore.Authorization.Authorize]
    public class FarmerProfilesController : ControllerBase
    {
        private readonly farmerRepository _repo;

        public FarmerProfilesController(farmerRepository repo)
        {
            _repo = repo;
        }

        //[HttpGet]
        //public async Task<ActionResult<IEnumerable<FarmerProfile>>> GetFarmerProfiles()
        //{
        //    var farmers = await _repo.GetFarmerProfilesAsync();
        //    return Ok(farmers);
        //}

        //[HttpPost]
        //public async Task<ActionResult<FarmerProfile>> SaveFarmerProfile([FromBody] FarmerProfile farmerProfile)
        //{
        //    if (farmerProfile == null)
        //        return BadRequest();

        //    if (await _repo.FarmerMobileExistsAsync(farmerProfile.Mobile))
        //        return Conflict("Mobile number already exists.");

        //    var createdFarmer = await _repo.AddFarmerProfileAsync(farmerProfile);
        //    return CreatedAtAction(nameof(GetFarmerProfiles), new { id = createdFarmer.FarmerId }, createdFarmer);
        //}
    }
}
