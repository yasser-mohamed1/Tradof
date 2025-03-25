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

        [HttpGet("AllStartedProjects")]
        public async Task<IActionResult> GetStartedProjectsAsync(
        [FromQuery] string companyId,
        [FromQuery] int pageIndex = 1,
        [FromQuery] int pageSize = 10)
        { 
            try
            {
                return Ok(await _projectService.GetStartedProjectsAsync(companyId, pageIndex, pageSize));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("AllInComingProjects")]
        public async Task<IActionResult> GetInComingProjectsAsync()
        {
            try
            {
                return Ok(await _projectService.GetInComingProjectsAsync());
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
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
        public async Task<IActionResult> GetProjectsCountByMonth(int year, int month)
        {
            return Ok(await _projectService.GetProjectsCountByMonth(year, month));
        }

        [HttpGet("statistics")]
        public async Task<IActionResult> GetProjectsStatistics()
        {
            Tuple<int, int, int> statistics = await _projectService.ProjectsStatistics();
            return Ok(new
            {
                active = statistics.Item1,
                inProgress = statistics.Item2,
                accepted = statistics.Item3
            });
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Create(CreateProjectDto projectDto)
        {
            try
            {
                return Ok(await _projectService.CreateAsync(projectDto));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut]
        [Authorize]
        public async Task<IActionResult> Update(UpdateProjectDto projectDto)
        {
            return Ok(await _projectService.UpdateAsync(projectDto));
        }

        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> Delete(long id)
        {
            return Ok(await _projectService.DeleteAsync(id));
        }

        [HttpGet("GetProjectCardData")]
        public async Task<IActionResult> GetProjectCardData(long projectId)
        {
            return Ok(await _projectService.GetProjectCardData(projectId));
        }

        [HttpGet("current-projects")]
        public async Task<IActionResult> GetCurrentProjectsByCompanyId(
        [FromQuery] string companyId,
        [FromQuery] int pageIndex = 1,
        [FromQuery] int pageSize = 10)
        {
            try
            {
                var result = await _projectService.GetCurrentProjectsByCompanyIdAsync(companyId, pageIndex, pageSize);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("current-projects/freelancer")]
        public async Task<IActionResult> GetCurrentProjectsByFreelancerId(
        [FromQuery] string freelancerId,
        [FromQuery] int pageIndex = 1,
        [FromQuery] int pageSize = 10)
        {
            try
            {
                var result = await _projectService.GetCurrentProjectsByFreelancerIdAsync(freelancerId, pageIndex, pageSize);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}