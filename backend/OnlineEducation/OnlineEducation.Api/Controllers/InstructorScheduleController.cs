using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OnlineEducation.Application.DTOs;
using OnlineEducation.Application.Interfaces;

[Route("api/instructor/schedule")]
[Authorize(Roles = "Instructor")]
public class InstructorScheduleController : BaseController
{
    private readonly IInstructorScheduleQuery _scheduleQuery;
    private readonly ICreateLessonCommand _createLessonCommand;
    private readonly IUpdateLessonCommand _updateLessonCommand;
    private readonly IDeleteLessonCommand _deleteLessonCommand;

    public InstructorScheduleController(
        IInstructorScheduleQuery scheduleQuery,
        ICreateLessonCommand createLessonCommand,
        IUpdateLessonCommand updateLessonCommand,
        IDeleteLessonCommand deleteLessonCommand)
    {
        _scheduleQuery = scheduleQuery;
        _createLessonCommand = createLessonCommand;
        _updateLessonCommand = updateLessonCommand;
        _deleteLessonCommand = deleteLessonCommand;
    }

    [HttpGet("weekly")]
    public async Task<IActionResult> GetWeeklySchedule(
        [FromQuery] int offset = 0)
    {
        var instructorId = GetInstructorId();

        var result = await _scheduleQuery.GetWeeklyScheduleAsync(
            instructorId,
            offset);

        return Ok(result);
    }

    [HttpPost("lessons")]
    public async Task<IActionResult> CreateLesson(
      [FromBody] CreateLessonRequestDto request)
    {
        var instructorId = GetInstructorId();

        var lessonId = await _createLessonCommand.CreateAsync(
            instructorId,
            request);

        return CreatedAtAction(
            nameof(CreateLesson),
            new { lessonId },
            new { lessonId });
    }

    [HttpPut("{lessonId:int}")]
    public async Task<IActionResult> UpdateLesson(
        int lessonId,
        [FromBody] UpdateLessonRequestDto request)
    {
        var instructorId = GetInstructorId();

        try
        {
            var success = await _updateLessonCommand.UpdateAsync(
                instructorId,
                lessonId,
                request);

            if (!success)
                return NotFound("Ders bulunamadı veya yetkiniz yok.");

            return NoContent();
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { message = ex.Message });
        }
    }

    [HttpDelete("{lessonId:int}")]
    public async Task<IActionResult> DeleteLesson(int lessonId)
    {
        var instructorId = GetInstructorId();

        var deleted = await _deleteLessonCommand.DeleteAsync(
            instructorId,
            lessonId);

        if (!deleted)
            return NotFound("Ders bulunamadı veya yetkiniz yok.");

        return NoContent();
    }


}
