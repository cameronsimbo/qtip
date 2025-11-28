using MediatR;
using Microsoft.AspNetCore.Mvc;
using QTip.Application.Features.Submissions;

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
        [FromBody] SubmitTextCommand command,
        CancellationToken cancellationToken)
    {
        SubmitTextResult result = await Mediator.Send(command, cancellationToken);

        return Ok(result);
    }
}