using MediatR;
using Microsoft.AspNetCore.Mvc;
using QTip.Application.Features.Submissions;
using QTip.Application.Features.Submissions.Models;

namespace QTip.Api.Controllers;

[Route("api/[controller]")]
public sealed class SubmissionsController : BaseApiController
{
    public SubmissionsController(IMediator mediator)
        : base(mediator)
    {
    }

    [HttpPost]
    public async Task<ActionResult<SubmitTextResult>> Submit(
        [FromBody] SubmitTextRequest request,
        CancellationToken cancellationToken)
    {
        if (request is null)
        {
            return BadRequest(new { Message = "Request body is required." });
        }

        SubmitTextCommand command = new SubmitTextCommand(request.Text);
        SubmitTextResult result = await Mediator.Send(command, cancellationToken);

        return Ok(result);
    }
}