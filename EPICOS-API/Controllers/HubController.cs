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
    [Route("api/[controller]")]
    [ApiController]
    public class HubController : ControllerBase
    {
        private readonly IHttpCallManager _httpCallManager;
        private DeviceRepository _deviceRepository = new DeviceRepository();
        private FloorRepository _floorRepository = new FloorRepository();
        private OfficeRepository _officeRepository;

        public HubController(IHttpCallManager httpCallManager)
        {
            _httpCallManager = httpCallManager;
            _officeRepository = new OfficeRepository(_httpCallManager);
        }

        [HttpGet()]
        public async Task<IActionResult> HubGetAll([FromQuery] HubFilter filters)
        {
            var query = await _deviceRepository.HubGetAll(filters);
            var response = new PageResponse<List<Hub>>(query, filters.Page, filters.Limit);
            return Ok(response);
        }

        [HttpPost()]
        public async Task<IActionResult> HubCreate([FromBody] Hub hub)
        {
            var response = new Response<Hub>();
            var floors = _floorRepository.FloorGetID(hub.FloorID);
            var validateMac = _deviceRepository.HubValidateMAC(hub);
            var validateIp = _deviceRepository.HubValidateIP(hub);
            Site office = await _officeRepository.OfficeGetID(hub.OfficeID);
            if(floors == null){
                response.StatusCode = 404;
                response.Message = "Invalid Floor ID";
                response.Succeeded = false;
                return StatusCode(404, response);
            }else if(office == null){
                response.StatusCode = 404;
                response.Message = "Invalid Office ID";
                response.Succeeded = false;
                return StatusCode(404, response);
            }else if(validateMac != null){
                response.Message = "Duplicate Mac address";
                response.Succeeded = false;
                return StatusCode(403, response);
            }else if(validateIp != null){
                response.Message = "Duplicate IP address";
                response.Succeeded = false;
                return StatusCode(403, response);
            }else {
                hub.MAC = hub.MAC.ToUpper();
               var res = await _deviceRepository.HubCreate(hub);
               response.Data = res.Data;
               response.Message = res.Message;
               response.Succeeded = res.Succeeded;
            }
            return Ok(response);
        }

        [HttpPut("{Id}")]
        public async Task<IActionResult> HubUpdate([FromBody] Hub hub, int Id)
        {
            var response = new Response<Hub>();
            var checkRecord = _deviceRepository.HubGetID(Id);
            if(checkRecord == null){
                response.StatusCode = 404;
                response.Message = "Hub not found";
                response.Succeeded = false;
                return StatusCode(404, response);
            }else {
                if(hub.IPaddress != checkRecord.IPaddress){
                    var validateIp = _deviceRepository.HubValidateIP(hub);
                    if(validateIp != null){
                        response.Message = "Duplicate Ip address";
                        response.Succeeded = false;
                        return StatusCode(403, response);
                    }
                }
                if(hub.MAC.ToLower() != checkRecord.MAC.ToLower()){
                    var validateMac = _deviceRepository.HubValidateMAC(hub);
                    if(validateMac != null){
                        response.Message = "Duplicate Mac address";
                        response.Succeeded = false;
                        return StatusCode(403, response);
                    }
                }
                var floors = _floorRepository.FloorGetID(hub.FloorID);
                Site office = await _officeRepository.OfficeGetID(hub.OfficeID);
                
                if(floors == null){
                    response.StatusCode = 404;
                    response.Message = "Invalid Floor ID";
                    response.Succeeded = false;
                    return StatusCode(404, response);
                }else if(office == null){
                    response.StatusCode = 404;
                    response.Message = "Invalid Office ID";
                    response.Succeeded = false;
                    return StatusCode(404, response);
                }else {
                    hub.MAC = hub.MAC.ToUpper();
                    var result = await _deviceRepository.HubUpdate(hub, Id);
                    response.Data = result.Data;
                    response.Message = result.Message;
                    response.Succeeded = result.Succeeded;
                    return Ok(response);
                }
                
            }
        }
        
        [HttpDelete("{Id}")]
        public async Task<IActionResult> Delete(int Id)
        {
            var response = await _deviceRepository.HubDelete(Id);
            return StatusCode(response.StatusCode, response);  
        }
    }
}