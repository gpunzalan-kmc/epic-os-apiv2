using System.Collections.Generic;
using System.Linq;
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
    [Route("api/workpoint")]
    [ApiController]
    public class WorkPointController: ControllerBase
    {
        private DeviceRepository _deviceRepository = new DeviceRepository();
        private FloorRepository _floorRepository = new FloorRepository();
        private OfficeRepository _officeRepository;
        private readonly IHttpCallManager _httpCallManager;

        public WorkPointController(IHttpCallManager httpCallManager)
        {
            _httpCallManager = httpCallManager;
            _officeRepository = new OfficeRepository(_httpCallManager);
        }

        [HttpGet()]
        public IActionResult WorkPointGetAll([FromQuery] WorkPointFilter filters)
        {
            var query = _deviceRepository.WorkPointGetAll(filters);
            var response = new PageResponse<List<Workpoint>>(query, filters.Page, filters.Limit);
            var count = _deviceRepository.PaginationWorkPointCount(filters);
            response.TotalRecords = count;
            return Ok(response);
        }

        [HttpPost()]
        public async Task<IActionResult> WorkPointCreate([FromBody] Workpoint workpoints)
        {
            var response = new Response<Workpoint>();
            var hubs = _deviceRepository.HubGetID(workpoints.HubID);
            var floors = _floorRepository.FloorGetID(workpoints.FloorID);
            var validateMac = _deviceRepository.WorkPointValidateMAC(workpoints);
            var validateIp = _deviceRepository.WorkPointValidateIP(workpoints);
            Site office = await _officeRepository.OfficeGetID(workpoints.OfficeID);
            if(hubs == null){
                response.Message = "Invalid Hub ID";
                response.Succeeded = false;
                return StatusCode(404, response);
            }else if(floors == null){
                response.Message = "Invalid Floor ID";
                response.Succeeded = false;
                return StatusCode(404, response);
            }else if(office == null){
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
                workpoints.MAC = workpoints.MAC.ToUpper();
               var res = await _deviceRepository.WorkPointCreate(workpoints);
               response.Data = res.Data;
               response.Message = res.Message;
               response.Succeeded = res.Succeeded;
            }
            return Ok(response);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> WorkPointUpdate([FromBody] Workpoint workpoints, int Id)
        {
            var response = new Response<Workpoint>();
            var sensor = _deviceRepository.WorkpointGetID(Id);
            if(sensor == null){
                response.Message = "Sensor not found";
                response.Succeeded = false;
                return StatusCode(404, response);
            }else {
                var hubs = _deviceRepository.HubGetID(workpoints.HubID);
                var floors = _floorRepository.FloorGetID(workpoints.FloorID);
                if(workpoints.IPaddress != sensor.IPaddress){
                    var validateIp = _deviceRepository.WorkPointValidateIP(workpoints);
                    if(validateIp != null){
                        response.Message = "Duplicate Ip address";
                        response.Succeeded = false;
                        return StatusCode(403, response);
                    }
                }
                if(workpoints.MAC.ToLower() != sensor.MAC.ToLower()){
                    var validateMac = _deviceRepository.WorkPointValidateMAC(workpoints);
                    if(validateMac != null){
                        response.Message = "Duplicate Mac address";
                        response.Succeeded = false;
                        return StatusCode(403, response);
                    }
                }
                Site office = await _officeRepository.OfficeGetID(workpoints.OfficeID);
                if(hubs == null){
                    response.Message = "Invalid Hub ID";
                    response.Succeeded = false;
                    return StatusCode(404, response);
                }else if(floors == null){
                    response.Message = "Invalid Floor ID";
                    response.Succeeded = false;
                    return StatusCode(404, response);
                }else if(office == null){
                    response.Message = "Invalid Office ID";
                    response.Succeeded = false;
                    return StatusCode(404, response);
                }
                else {
                    workpoints.MAC = workpoints.MAC.ToUpper();
                    var result = await _deviceRepository.WorkPointUpdate(workpoints, Id);
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
            var response = await _deviceRepository.WorkPointDelete(Id);
            return StatusCode(response.StatusCode, response);  
        }
    }
}