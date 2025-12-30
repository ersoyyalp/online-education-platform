using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OnlineEducation.Application.Interfaces;

[Route("api/participant/schedule")]
[Authorize(Roles = "Participant")]
public class ParticipantScheduleController : BaseController
{
    private readonly IParticipantScheduleQuery _scheduleQuery;

    public ParticipantScheduleController(IParticipantScheduleQuery scheduleQuery)
    {
        _scheduleQuery = scheduleQuery;
    }

    [HttpGet("weekly")]
    public async Task<IActionResult> GetWeeklySchedule(
        [FromQuery] int offset = 0)
    {
        var participantId = GetParticipantId();

        var result = await _scheduleQuery.GetWeeklyScheduleAsync(
            participantId,
            offset);

        return Ok(result);
    }
}
