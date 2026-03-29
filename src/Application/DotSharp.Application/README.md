# DotSharp.Application

In-process messaging for .NET — request/response and publish/subscribe with a composable pipeline.

## Installation
```bash
dotnet add package DotSharp.Application
```

## What's included

### Request/response
```csharp
public sealed record CreateOrderCommand(string CustomerName) : IRequest<Result<Guid>>;

public sealed class CreateOrderCommandHandler : IRequestHandler<CreateOrderCommand, Result<Guid>>
{
    public Task<Result<Guid>> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
    {
        return Task.FromResult(Result<Guid>.Success(Guid.NewGuid()));
    }
}
```

### Notifications
```csharp
public sealed record OrderCreatedNotification(Guid OrderId) : INotification;

public sealed class SendWelcomeEmailOnOrderCreated : INotificationHandler<OrderCreatedNotification>
{
    public Task Handle(OrderCreatedNotification notification, CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}
```

### Validation
```csharp
public sealed class CreateOrderCommandValidator : AbstractValidator<CreateOrderCommand>
{
    public CreateOrderCommandValidator()
    {
        RuleFor(x => x.CustomerName)
            .NotEmpty()
            .MinimumLength(3)
            .MaximumLength(120);
    }
}
```

If validation fails the handler is never called and a `Result.Failure` with validation details is returned.

> `ValidationBehavior` requires `TResult` to be `Result` or `Result<T>`.

### IMessageBus
```csharp
public sealed class OrdersAppService(IMessageBus bus)
{
    public async Task<Result<Guid>> CreateAsync(string customerName, CancellationToken ct)
    {
        Result<Guid> result = await bus.Send(new CreateOrderCommand(customerName), ct);
        if (result.IsFailure)
            return result;

        await bus.Publish(new OrderCreatedNotification(result.Value), ct);
        return result;
    }
}
```

### Registration
```csharp
builder.Services.AddDotSharpApplication(typeof(CreateOrderCommandHandler).Assembly);
```

Automatically registers `ISender`, `IPublisher`, `IMessageBus`, `ValidationBehavior`, and all `IRequestHandler<,>`, `INotificationHandler<>` and `IValidator<>` found in the provided assemblies.

## Design decisions

- No dependency on MediatR — the pipeline is implemented from scratch using `IServiceProvider` and reflection for generic dispatch.
- `ValidationBehavior` deduplicates validation errors across multiple validators for the same request.
- Notification handlers run concurrently via `Task.WhenAll` — order of execution is not guaranteed.
- Internal and public handlers are both discovered during assembly scanning.
