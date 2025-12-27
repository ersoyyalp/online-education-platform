using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OnlineEducation.Application.DTOs;
using OnlineEducation.Application.Interfaces;

// EĞİTMEN – ERTELEME TALEPLERİ
[ApiController]
[Route("api/instructor/reschedule-requests")]
[Authorize(Roles = "Instructor")]
public class InstructorRescheduleRequestsController : BaseController
{
    private readonly IGetRescheduleRequestsQuery _query;
    private readonly IDecideRescheduleRequestCommand _decideCommand;

    public InstructorRescheduleRequestsController(
        IGetRescheduleRequestsQuery query,
        IDecideRescheduleRequestCommand decideCommand)
    {
        _query = query;
        _decideCommand = decideCommand;
    }

    /// <summary>
    /// Eğitmenin bekleyen erteleme talepleri
    /// </summary>
    [HttpGet("pending")]
    public async Task<IActionResult> GetPending()
    {
        var instructorId = GetInstructorId();
        return Ok(await _query.GetPendingAsync(instructorId));
    }

    /// <summary>
    /// Eğitmen erteleme talebini onaylar / reddeder
    /// </summary>
    [HttpPost("{requestId:int}/decision")]
    public async Task<IActionResult> Decide(
        int requestId,
        [FromBody] DecideRescheduleRequestDto request)
    {
        var instructorId = GetInstructorId();

        await _decideCommand.DecideAsync(instructorId, requestId, request);
        return NoContent();
    }
}
