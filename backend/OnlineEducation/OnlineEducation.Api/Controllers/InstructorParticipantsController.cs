using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OnlineEducation.Application.Interfaces;

[ApiController]
[Route("api/instructor/participants")]
[Authorize(Roles = "Instructor")]
public class InstructorParticipantsController : BaseController
{
    private readonly IGetParticipantsQuery _query;

    public InstructorParticipantsController(IGetParticipantsQuery query)
    {
        _query = query;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var instructorId = GetInstructorId();
        var result = await _query.GetAllAsync(instructorId);
        return Ok(result);
    }
}
