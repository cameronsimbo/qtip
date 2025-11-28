using MediatR;
using Microsoft.AspNetCore.Mvc;
using QTip.Application.Features.Statistics;

namespace QTip.Api.Controllers;

[Route("api/[controller]")]
public sealed class StatsController : BaseApiController
{
    public StatsController(IMediator mediator)
        : base(mediator)
    {
    }

    [HttpGet("emails")]
    public async Task<ActionResult<GetTotalPiiEmailCountResult>> GetEmailStats(CancellationToken cancellationToken)
    {
        GetTotalPiiEmailCountResult result = await Mediator.Send(new GetTotalPiiEmailCountQuery(), cancellationToken);
        return Ok(result);
    }
}