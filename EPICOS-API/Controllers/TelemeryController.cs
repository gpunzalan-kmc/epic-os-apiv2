using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EPICOS_API.Attributes;
using EPICOS_API.Models.Entities;
using EPICOS_API.Models.Filters;
using EPICOS_API.Models.Wrappers;
using EPICOS_API.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EPICOS_API.Controllers
{
    [Authorize]
    [Route("api/telemery")]
    [ApiController]
    public class TelemeryController : ControllerBase
    {
        private TelemeryRepository _telemeryRepository = new TelemeryRepository();

        private RaspberryRepository _raspberryRepository = new RaspberryRepository();
        
        [HttpGet()]
        public IActionResult RequestTelemery([FromQuery] TelemeryFilter filter)
        {
            var query = _telemeryRepository.TelemeryGetAll(filter);
            return Ok(query);
        }

        
        [HttpGet("log")]
        public IActionResult GetLog([FromQuery] LogFilter parameters)
        {
            var result = _raspberryRepository.LogGetAll(parameters);
            var response = new PageResponse<List<Log>>(result, parameters.Page, parameters.Limit);
            response.TotalRecords = _raspberryRepository.LogCount(parameters);
            return Ok(response);
        }
    }
}