using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

public abstract class BaseController : ControllerBase
{
    protected int GetInstructorId()
    {
        var claim = User.Claims.FirstOrDefault(c => c.Type == "instructorId");
        if (claim == null)
            throw new UnauthorizedAccessException("InstructorId claim not found");

        return int.Parse(claim.Value);
    }

    protected int GetParticipantId()
    {
        var claim = User.Claims.FirstOrDefault(c => c.Type == "participantId");
        if (claim == null)
            throw new UnauthorizedAccessException("ParticipantId claim not found");

        return int.Parse(claim.Value);
    }
}
