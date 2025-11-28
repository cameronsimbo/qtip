using MediatR;
using Microsoft.AspNetCore.Mvc;
using QTip.Application.Features.Statistics;
using QTip.Application.Features.Statistics.Models;

namespace QTip.Api.Controllers;

[Route("api/[controller]")]
public sealed class StatsController : BaseApiController
{
    public StatsController(IMediator mediator)
        : base(mediator)
    {
    }

    [HttpGet("emails")]
    public async Task<ActionResult<TotalPiiEmailsResponse>> GetEmailStats(CancellationToken cancellationToken)
    {
        GetTotalPiiEmailCountResult result = await Mediator.Send(new GetTotalPiiEmailCountQuery(), cancellationToken);

        TotalPiiEmailsResponse response = new TotalPiiEmailsResponse
        {
            TotalPiiEmails = result.TotalPiiEmails
        };

        return Ok(response);
    }
}