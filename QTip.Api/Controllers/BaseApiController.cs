using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace QTip.Api.Controllers;

[ApiController]
public abstract class BaseApiController : ControllerBase
{
    protected IMediator Mediator { get; }

    protected BaseApiController(IMediator mediator)
    {
        Mediator = mediator;
    }
}