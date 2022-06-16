using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EPICOS_API.Models;
using EPICOS_API.Models.Entities;
using EPICOS_API.Models.Filters;
using EPICOS_API.Models.Wrappers;
using EPICOS_API.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EPICOS_API.Controllers
{    
    
    [Authorize]
    [Route("api/users")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private UserRepository _userRepository =  new UserRepository();


        [HttpGet()]
        public IActionResult Get([FromQuery] UserFilter filters)
        {
            using (var context = new EpicOSContext())
            {
                var result = _userRepository.UserGetAll(filters);
                var response = new PageResponse<List<User>>(result, filters.Page, filters.Limit);
                return Ok(response);
            }
        }
        


        [HttpPost]
        public async Task<IActionResult> Create(User parameter)
        {
            var response = await _userRepository.Create(parameter);
            return StatusCode(response.StatusCode, response);
        }

        [HttpPut("{Id}")]
        public async Task<IActionResult> Update(User parameter, int Id)
        {
            var response = await _userRepository.Update(parameter, Id);
            return StatusCode(response.StatusCode, response);
        }

        [HttpDelete("{Id}")]
        public async Task<IActionResult> Delete(int Id)
        {

                var response = await _userRepository.Delete(Id);
                return StatusCode(response.StatusCode, response);
            
        }

    }
}