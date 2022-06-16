using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EPICOS_API.Attributes;
using EPICOS_API.Models;
using EPICOS_API.Models.Entities;
using EPICOS_API.Models.Filters;
using EPICOS_API.Models.Wrappers;
using EPICOS_API.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace EPICOS_API.Controllers
{
    [ApiKey]
    [Route("api/telemery")]
    [ApiController]
    public class RaspberryController : ControllerBase
    {
        private TelemeryRepository _telemeryRepository = new TelemeryRepository();



        [HttpGet("sensor-list")]
        public List<RaspWorkPoint> GetSensorsByHubMAC([FromQuery] string MAC)
        {
            // DeviceManager manager = new DeviceManager();
            List<RaspWorkPoint> devices = new List<RaspWorkPoint>();
            Hub hub = _telemeryRepository.HubByMAC(MAC);
            if (hub != null)
            {
                devices = _telemeryRepository.WorkPointByHub(hub.ID);
            }
            return devices;
        }


        [HttpPost("insert")]
        public async Task<IActionResult> CreateTelemeryByBatch(IEnumerable<Telemery> parameters)
        {
            List<Result> results = new List<Result>();
            if (parameters.Count() > 0)
            {
                foreach (Telemery parameter in parameters)
                {
                    if (_telemeryRepository.IsActive(parameter.MAC, 1))
                    {
                   
                        Workpoint point = _telemeryRepository.WorkPointByMAC(parameter.MAC);
                        if(point != null){
                            parameter.HubID = point.HubID;
                            parameter.WorkpointID = point.ID;
                            parameter.IsActive = true;
                            parameter.IsDeleted = false;
                            Result result = await _telemeryRepository.TelemeryInsert(parameter, point);

                            if (result.ID > 0)
                            {
                                results.Add(result);
                            }
                        }
                    }
                }
                if (results.Count() > 0)
                {
                    return Ok(results);
                }
            }
            return BadRequest();
        }



        [HttpPost("log/insert")]
        public async Task<IActionResult> CreateLog(IEnumerable<Log> parameters)
        {
            List<Result> results = new List<Result>();
            if (parameters.Count() > 0)
            {
                foreach (Log parameter in parameters)
                {
                    if (_telemeryRepository.IsActive(parameter.MAC, 2))
                    {
                        // Workpoint point = _telemeryRepository.WorkPointByMAC(parameter.MAC);
                        // if(point != null){
                        //     parameter.WorkpointID = point.ID;
                        //     parameter.IsActive = true;
                        //     parameter.IsDeleted = false;
                        Result result = await _telemeryRepository.LogInsert(parameter);
                        if (result.ID > 0)
                        {
                            results.Add(result);
                        }
                        // }
                    }
                }
                if (results.Count() > 0)
                {
                    return Ok(results);
                }
            }
            return BadRequest();
        }
    }
}