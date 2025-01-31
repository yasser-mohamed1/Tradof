using Microsoft.AspNetCore.Mvc;


namespace Tradof.Admin.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PropsalController() : ControllerBase
    {
        //[Authorize]
        //[HttpGet]
        //public async Task<IActionResult> GetAll([FromQuery] ProjectSpecParams specParams)
        //{
        //    return Ok(await _projectService.GetAllAsync(specParams));
        //}
        //[HttpGet("{id}")]
        //public async Task<IActionResult> GetById(long id)
        //{
        //    var project = await _projectService.GetByIdAsync(id);
        //    return project != null ? Ok(project) : NotFound();
        //}

        //[HttpPost]
        //[Authorize]
        //public async Task<IActionResult> Create(CreateProjectDto projectDto)
        //{
        //    return Ok(await _projectService.CreateAsync(projectDto));
        //}

        //[HttpPut]
        //public async Task<IActionResult> Update(UpdateProjectDto projectDto)
        //{
        //    return Ok(await _projectService.UpdateAsync(projectDto));

        //}

        //[HttpDelete("{id}")]
        //public async Task<IActionResult> Delete(long id)
        //{
        //    return Ok(await _projectService.DeleteAsync(id));

        //}
    }
}