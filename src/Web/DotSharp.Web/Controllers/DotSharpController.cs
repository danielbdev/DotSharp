using DotSharp.Application.Abstractions.Messaging;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

namespace DotSharp.Web.Controllers;

/// <summary>
/// Base controller for DotSharp Web APIs providing lazy-resolved access
/// to <see cref="IMessageBus"/> for dispatching commands and queries.
/// </summary>
[ApiController]
public class DotSharpController : ControllerBase
{
    private IMessageBus _messageBus = null!;

    /// <summary>
    /// Gets the <see cref="IMessageBus"/> resolved lazily from the current request's
    /// service provider on first access.
    /// </summary>
    protected IMessageBus MessageBus => _messageBus ??= HttpContext.RequestServices.GetRequiredService<IMessageBus>();
}
