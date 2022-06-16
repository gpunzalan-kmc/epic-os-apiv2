using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using System.Threading.Tasks;
using Attributes;
using EpicOS.Helpers;
using EpicOS.Managers;
using EpicOS.Models.Entities;
using EpicOS.Models.Parameters;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Models.Entities;

namespace EpicOS.Controllers.API
{
    [Route("api/[controller]")]
    [ApiController]
    [ApiKeyAuth]
    public class DeviceController : ControllerBase
    {


        [HttpPost("/api/device/sensor-list", Name = "Device_SensorList")]
        [Consumes(MediaTypeNames.Application.Json)]
        public List<DeviceGetByMac> GetSensorsByHubMAC(Device parameter)
        {
            DeviceManager manager = new DeviceManager();
            List<DeviceGetByMac> devices = new List<DeviceGetByMac>();
            Hub hub = manager.HubGetByMAC(parameter.MAC);
            if (hub != null)
            {
                if (hub.IsActive)
                {
                    devices = manager.GetDevicesByHub(hub.ID);
                }
            }
            return devices;
        }

        [HttpPost("/api/device/telemery/insert", Name = "Device_Telemery_Insert_Batch")]
        [Consumes(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult CreateTelemeryByBatch(IEnumerable<Telemery> parameters)
        {
            
            DeviceManager manager = new DeviceManager();
            List<Result> results = new List<Result>();
    
            if (parameters.Count() > 0)
            {
                foreach (Telemery parameter in parameters)
                {
                    if (manager.IsActive(parameter.MAC, 1))
                    {
                        Workpoint point = manager.WorkpointGetByMAC(parameter.MAC);
                        parameter.WorkpointID = point.ID;
                        parameter.IsActive = true;
                        parameter.IsDeleted = false;
                        Result result = manager.TelemeryInsert(parameter);
                        if (result.ID > 0)
                        {
                            results.Add(result);
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

        [HttpPost("/api/device/log/insert", Name = "Device_Log_Insert_Batch")]
        [Consumes(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult CreateLog(IEnumerable<Log> parameters)
        {
            DeviceManager manager = new DeviceManager();
            List<Result> results = new List<Result>();
            if (parameters.Count() > 0)
            {
                foreach (Log parameter in parameters)
                {
                    if (manager.IsActive(parameter.MAC, 2))
                    {
                        Result result = manager.LogInsert(parameter);
                        if (result.ID > 0)
                        {
                            results.Add(result);
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
    }
}
