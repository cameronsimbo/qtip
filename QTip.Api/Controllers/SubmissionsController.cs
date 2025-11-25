using MediatR;
using Microsoft.AspNetCore.Mvc;
using QTip.Application.Features.Submissions;

namespace QTip.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class SubmissionsController : ControllerBase
{
    private readonly IMediator _mediator;

    public SubmissionsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    public sealed class SubmitTextRequest
    {
        public string Text { get; set; } = string.Empty;
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

        var command = new SubmitTextCommand(request.Text);
        var result = await _mediator.Send(command, cancellationToken);

        return Ok(result);
    }
}


