using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Tradof.Data.SpecificationParams;
using Tradof.Project.Services.DTOs;
using Tradof.Project.Services.Interfaces;

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

	[HttpGet("AllStartedProjects")] // Corrected typo
	public async Task<IActionResult> GetStartedProjectsAsync(string companyId)
	{
		try
		{ 
			return Ok(await _projectService.GetStartedProjectsAsync(companyId));
		}
		catch(Exception ex)
		{
			return BadRequest(ex.Message);
		}
	}

	[HttpGet("AllInComingProjects")] // Corrected case
	public async Task<IActionResult> GetInComingProjectsAsync(string companyId)
	{
		try
        {
            return Ok(await _projectService.GetInComingProjectsAsync(companyId));
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
	public async Task<IActionResult> Create(string companyId, CreateProjectDto projectDto)
	{
		try
        {
            return Ok(await _projectService.CreateAsync(companyId, projectDto));
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
	}

	[HttpPut]
	//[Authorize]
	public async Task<IActionResult> Update(string companyId, UpdateProjectDto projectDto)
	{
		return Ok(await _projectService.UpdateAsync(companyId, projectDto));
	}

	[HttpDelete("{id}")]
	//[Authorize]
	public async Task<IActionResult> Delete(long id)
	{
		return Ok(await _projectService.DeleteAsync(id));
	}

	[HttpGet("GetProjectCardData")]
    public async Task<IActionResult> GetProjectCardData(long projectId)
    {
        return Ok(await _projectService.GetProjectCardData(projectId));
    }
}