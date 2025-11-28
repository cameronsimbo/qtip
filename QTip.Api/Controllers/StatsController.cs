using MediatR;
using Microsoft.AspNetCore.Mvc;
using QTip.Application.Features.Statistics;

namespace QTip.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class StatsController : ControllerBase
{
    private readonly IMediator _mediator;

    public StatsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    public sealed class TotalPiiEmailsResponse
    {
        public long TotalPiiEmails { get; set; }
    }

    [HttpGet("emails")]
    public async Task<ActionResult<TotalPiiEmailsResponse>> GetEmailStats(CancellationToken cancellationToken)
    {
        GetTotalPiiEmailCountResult result = await _mediator.Send(new GetTotalPiiEmailCountQuery(), cancellationToken);

        TotalPiiEmailsResponse response = new TotalPiiEmailsResponse
        {
            TotalPiiEmails = result.TotalPiiEmails
        };

        return Ok(response);
    }
}


