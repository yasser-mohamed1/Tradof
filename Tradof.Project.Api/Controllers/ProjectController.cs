using Microsoft.AspNetCore.Mvc;

namespace Tradof.Admin.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProjectController() : ControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            return Ok();
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(long id)
        {
            return Ok();

        }

        [HttpPost]
        public async Task<IActionResult> Create()
        {
            return Ok();

        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(long id)
        {
            return Ok();

        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(long id)
        {
            return Ok();

        }
    }
}