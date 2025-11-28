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
    public async Task<ActionResult<GetTotalClassificationCountResult>> GetEmailStats(CancellationToken cancellationToken)
    {
        GetTotalClassificationCountResult result = await Mediator.Send(new GetTotalClassificationCountQuery(), cancellationToken);
        return Ok(result);
    }

    [HttpPost("clear")]
    public async Task<ActionResult<ClearClassificationCountsResult>> Clear(CancellationToken cancellationToken)
    {
        ClearClassificationCountsResult result = await Mediator.Send(new ClearClassificationCountsCommand(), cancellationToken);
        return Ok(result);
    }
}