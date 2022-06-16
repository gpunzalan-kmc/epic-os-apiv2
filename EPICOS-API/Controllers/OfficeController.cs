using System.Collections.Generic;
using System.Threading.Tasks;
using EPICOS_API.Managers;
using EPICOS_API.Models.Entities;
using EPICOS_API.Models.Filters;
using EPICOS_API.Models.Wrappers;
using EPICOS_API.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EPICOS_API.Controllers
{
    [Authorize]
    [Route("api/office")]
    [ApiController]
    public class OfficeController : ControllerBase
    {
        private readonly IHttpCallManager _httpCallManager;
        private OfficeRepository _officeRepository;
  
        public OfficeController(IHttpCallManager httpCallManager)
        {
            _httpCallManager = httpCallManager;
            _officeRepository = new OfficeRepository(_httpCallManager);
        }


        [HttpGet()]
        public async Task<IActionResult> OfficeGetall([FromQuery] OfficeFilter filters)
        {
            ExternalPaginationResponse<Site> response = await _officeRepository.OfficeGetall(filters);

            if(response.Data.Length > 0){
                List<Site> results = new List<Site>();
                foreach (Site i in response.Data)
                {
                    Site office = new Site();
                    office.BuildingID = i.BuildingID;
                    office.Name = i.Name;
                    office.Line1 = i.Line1;
                    office.HubCount = await _officeRepository.HubCount(i.BuildingID);
                    office.WorkPointCount = await _officeRepository.WorkPointCount(i.BuildingID);
                    office.WorkPointCount = await _officeRepository.WorkPointCount(i.BuildingID);
                    office.FloorCount = await _officeRepository.FloorCount(i.BuildingID);
                    results.Add(office);
                }
                var result = new PageResponse<List<Site>>(results, 1, 100);
                result.TotalRecords = 100;
                return Ok(result);
            }
            return StatusCode(404, "No Record");

        }


        [HttpGet("{id}")]
        public async Task<IActionResult> OfficeGetID(int Id)
        {
            Site response = await _officeRepository.OfficeGetID(Id);
     
   
                Site results = new Site();

            results.BuildingID = response.BuildingID;
            results.Name = response.Name;
            results.Line1 = response.Line1;
            results.HubCount = await _officeRepository.HubCount(response.BuildingID);
            results.WorkPointCount = await _officeRepository.WorkPointCount(response.BuildingID);
            results.WorkPointCount = await _officeRepository.WorkPointCount(response.BuildingID);
            results.FloorCount = await _officeRepository.FloorCount(response.BuildingID);
   
                
  
       
                return Ok(results);
            
 
        }

        // [HttpPost()]
        // public async Task<IActionResult> OfficeCreate([FromBody] Office parameters)
        // {
        //     var response = await this.officeRepositry.Create(parameters);
        //     return Ok(response);
        // }

        // [HttpPut("{Id}")]
        // public async Task<IActionResult> OfficeUpdate([FromBody] Office parameters, int Id)
        // {
        //     var response = await this.officeRepositry.Update(parameters, Id);
        //     return Ok(response);
        // }
    }
}