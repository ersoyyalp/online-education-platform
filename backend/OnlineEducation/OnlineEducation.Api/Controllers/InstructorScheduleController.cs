using Microsoft.AspNetCore.Mvc;
using OnlineEducation.Application.Interfaces;

namespace OnlineEducation.Api.Controllers;

[ApiController]
[Route("api/instructors/{instructorId:int}/schedule")]
public class InstructorScheduleController : ControllerBase
{
    private readonly IInstructorScheduleQuery _scheduleQuery;

    public InstructorScheduleController(IInstructorScheduleQuery scheduleQuery)
    {
        _scheduleQuery = scheduleQuery;
    }

    [HttpGet("weekly")]
    public async Task<IActionResult> GetWeeklySchedule(
        int instructorId,
        [FromQuery] int offset = 0)
    {
        var today = DateTime.Today;

        // Pazartesi bazlı hafta hesaplama
        int diff = (7 + (today.DayOfWeek - DayOfWeek.Monday)) % 7;
        var weekStart = today.AddDays(-diff).Date;
        var weekEnd = weekStart.AddDays(7).AddSeconds(-1);

        // Offset (hafta kaydırma)
        weekStart = weekStart.AddDays(offset * 7);
        weekEnd = weekEnd.AddDays(offset * 7);

        var result = await _scheduleQuery.GetWeeklyScheduleAsync(
            instructorId,
            weekStart,
            weekEnd);

        return Ok(result);
    }

}
