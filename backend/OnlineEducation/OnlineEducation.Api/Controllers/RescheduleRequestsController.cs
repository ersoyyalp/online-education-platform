using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OnlineEducation.Application.DTOs;
using OnlineEducation.Application.Interfaces;

// ÖĞRENCİ – ERTELEME TALEBİ OLUŞTURMA
[ApiController]
[Route("api/participant/lesson-schedules/{lessonScheduleId:int}/reschedule-requests")]
[Authorize(Roles = "Participant")]
public class RescheduleRequestsController : BaseController
{
    private readonly ICreateRescheduleRequestCommand _createCommand;

    public RescheduleRequestsController(
        ICreateRescheduleRequestCommand createCommand)
    {
        _createCommand = createCommand;
    }

    /// <summary>
    /// Öğrenci ders için erteleme talebi oluşturur
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> Create(
        int lessonScheduleId,
        [FromBody] CreateRescheduleRequestDto request)
    {
        var participantId = GetParticipantId();

        request.ParticipantId = participantId;

        var id = await _createCommand.CreateAsync(
            lessonScheduleId,
            request);

        return CreatedAtAction(nameof(Create), new { id }, new { id });
    }
}
