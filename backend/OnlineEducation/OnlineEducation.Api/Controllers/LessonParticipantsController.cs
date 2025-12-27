using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OnlineEducation.Application.DTOs;
using OnlineEducation.Application.Interfaces;

// EĞİTMEN – DERS KATILIMCILARI
[ApiController]
[Route("api/instructor/lessons/{lessonId:int}/participants")]
[Authorize(Roles = "Instructor")]
public class LessonParticipantsController : BaseController
{
    private readonly IUpdateLessonParticipantsCommand _command;

    public LessonParticipantsController(IUpdateLessonParticipantsCommand command)
    {
        _command = command;
    }

    /// <summary>
    /// Ders katılımcılarını günceller
    /// </summary>
    [HttpPatch]
    public async Task<IActionResult> Update(
        int lessonId,
        [FromBody] UpdateLessonParticipantsRequestDto request)
    {
        var instructorId = GetInstructorId();

        var success = await _command.UpdateAsync(
            instructorId,
            lessonId,
            request);

        return success ? NoContent() : NotFound();
    }
}
