using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OnlineEducation.Application.DTOs;
using OnlineEducation.Application.Interfaces;

[Route("api/participant/lesson-requests")]
[Authorize(Roles = "Participant")]
public class LessonRequestController : BaseController
{
    private readonly ICreateLessonForParticipantRequestCommand _command;

    public LessonRequestController(
        ICreateLessonForParticipantRequestCommand command)
    {
        _command = command;
    }

    [HttpPost("request")]
    public async Task<IActionResult> Create(
        [FromBody] CreateLessonForParticipantRequestDto dto)
    {
        var participantId = GetParticipantId();

        await _command.CreateAsync(
            participantId,
            dto
        );

        return Ok();
    }
}
