using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using EPICOS_API.Managers;
using EPICOS_API.Models.Entities;
using EPICOS_API.Models.Parameters;
using EPICOS_API.Models.Wrappers;
using EPICOS_API.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace EPICOS_API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class FloorController : ControllerBase
    {

        private FloorRepository floorRepository;
        private OfficeRepository _officeRepository;
        private IConfiguration _configuration;
        IWebHostEnvironment hosting;
        private readonly IHttpCallManager _httpCallManager;
        private string floorFilePath = "uploads\\img\\floors";

        public FloorController(IWebHostEnvironment host, IHttpCallManager httpCallManager, IConfiguration configuration)
        {
            _httpCallManager = httpCallManager;
            _officeRepository = new OfficeRepository(_httpCallManager);
            _configuration = configuration;
            this.floorRepository = new FloorRepository();

            hosting = host;
        }

        [HttpGet()]
        public IActionResult FloorGetAll([FromQuery] FloorFilter filters)
        {
            var response = this.floorRepository.FloorGetAll(filters);
            return Ok(response);
        }

        [HttpPost("upload")]
        public async Task<IActionResult> Upload()
        {
            var storageConnectionString = _configuration.GetValue<string>("StorageConnectionString");

            var _blobContainerName = _configuration.GetValue<string>("BlobContainerName");

            var blobUrl = _configuration.GetValue<string>("BlobUrl");

            BlobContainerClient container = new BlobContainerClient(storageConnectionString, _blobContainerName);

            if (!container.Exists())
            {
                container.Create();
            }

            try
            {
                var file = Request.Form.Files[0];

                if (file == null)
                {
                    return BadRequest("Could not upload files.");
                }
                else
                {
                    var fileName = GetRandomBlobName(file.FileName);


                    BlobClient blob = container.GetBlobClient(fileName);

                    var stream = file.OpenReadStream();

                    await blob.UploadAsync(stream, new BlobHttpHeaders
                    {
                        ContentType = file.ContentType
                    },
                    conditions: null);

                    return Ok(new { filename = fileName });
      
                }

            }
            catch (Exception e)
            {
                return BadRequest("Error encountered.");
            }
            //try {
            //    var formCollection = await Request.ReadFormAsync();
            //    var file = formCollection.Files.First();
            //    var uploads = Path.Combine("Resources", floorFilePath);
            //    if (!Directory.Exists(uploads))
            //    {
            //        Directory.CreateDirectory(uploads);
            //    }
            //    if (file.Length > 0)
            //    {
            //        var fileName = Guid.NewGuid().ToString().Replace("-", "") + Path.GetExtension(file.FileName);
            //        using (var fileStream = new FileStream(Path.Combine(uploads, fileName), FileMode.Create))
            //        {
            //            await file.CopyToAsync(fileStream);
            //        }
            //        return Ok(new {filename = fileName});
            //    }
            //     return StatusCode(404, new {message = "No file found"});
            //}catch(Exception ex) {
            //    return StatusCode(500, $"Internal server error: {ex}");
            //}
        }

        private string GetRandomBlobName(string filename)
        {
            string ext = Path.GetExtension(filename);
            string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(filename);

            return DateTime.Now.Ticks + "_" + fileNameWithoutExtension + ext;
        }

        [HttpPost()]
        public async Task<IActionResult> FloorCreate([FromBody] Floor parameters)
        {
            var response = new Response<Floor>();
                Site office = await _officeRepository.OfficeGetID(parameters.OfficeID);
            
                if(office == null){
                    response.Message = "Invalid Office ID";
                    response.Succeeded = false;
                    return StatusCode(404, response);
                }
                Response<Floor> floor = await this.floorRepository.CreateFloor(parameters);
                response.Message = floor.Message;
                response.Data = floor.Data;
                response.Succeeded = floor.Succeeded;
                return Ok(response);
       
        }

        [HttpPut("{Id}")]
        public async Task<IActionResult> FloorUpdate([FromBody] Floor parameters, int Id)
        {
            var response = new Response<Floor>();
            var checkRecord = this.floorRepository.FloorGetID(Id);
            if(checkRecord == null){
                response.Message = "Record not found";
                response.Succeeded = false;
                return StatusCode(404, response);
            }
            Site office = await _officeRepository.OfficeGetID(parameters.OfficeID);
        
            if(office == null){
                response.Message = "Invalid Office ID";
                response.Succeeded = false;
                return StatusCode(404, response);
            }
            Response<Floor> floor = await this.floorRepository.UpdateFloor(parameters, Id);
            response.Message = floor.Message;
            response.Data = floor.Data;
            response.Succeeded = true;
            return Ok(response);
        }

        [HttpDelete("{Id}")]
        public async Task<IActionResult> Delete(int Id)
        {
            var response = await this.floorRepository.FloorDelete(Id);
            return StatusCode(response.StatusCode, response);  
        }
    }
}