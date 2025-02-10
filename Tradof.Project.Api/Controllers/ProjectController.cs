using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Tradof.Data.SpecificationParams;
using Tradof.Project.Services.DTOs;
using Tradof.Project.Services.Interfaces;

namespace Tradof.Project.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProjectController(IProjectService _projectService) : ControllerBase
    {
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] ProjectSpecParams specParams)
        {
            return Ok(await _projectService.GetAllAsync(specParams));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(long id)
        {
            var project = await _projectService.GetByIdAsync(id);
            return project != null ? Ok(project) : NotFound();
        }
        [HttpPut("SendReviewRequest")]
        public async Task<IActionResult> SendReviewRequest(long id)
        {
            return Ok(await _projectService.SendReviewRequest(id));
        }
        [HttpPut("MarkAsFinished")]
        public async Task<IActionResult> MarkAsFinished(long id)
        {
            return Ok(await _projectService.MarkAsFinished(id));

        }
        [HttpGet("countByMonth")]
        public async Task<IActionResult> GetProjectsCountByMonth(long id, int year, int month)
        {
            return Ok(await _projectService.GetProjectsCountByMonth(id, year, month));
        }
        [HttpGet("statistics")]
        public async Task<IActionResult> GetProjectsStatistics(long id)
        {
            Tuple<int, int, int> statistics = await _projectService.ProjectsStatistics(id);
            return Ok(new
            {
                active = statistics.Item1,
                inProgress = statistics.Item2,
                accepted = statistics.Item3
            });
        }
        [HttpPost]
        //[Authorize]
        public async Task<IActionResult> Create(long companyId,CreateProjectDto projectDto)
        {
            return Ok(await _projectService.CreateAsync(companyId,projectDto));
        }

        [HttpPut]
        public async Task<IActionResult> Update(long companyId ,UpdateProjectDto projectDto)
        {
            return Ok(await _projectService.UpdateAsync(companyId,projectDto));
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(long id)
        {
            return Ok(await _projectService.DeleteAsync(id));

        }
    }
}