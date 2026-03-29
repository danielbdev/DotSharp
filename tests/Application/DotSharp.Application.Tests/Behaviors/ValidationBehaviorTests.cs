using DotSharp.Application.Abstractions.Handlers;
using DotSharp.Application.Abstractions.Messaging;
using DotSharp.Application.Behaviors;
using DotSharp.Results;
using FluentAssertions;
using FluentValidation;
using Xunit;

namespace DotSharp.Application.Tests.Behaviors;

public sealed class ValidationBehaviorTests
{
    #region Test doubles

    private sealed record CreateOrderCommand(string CustomerName) : IRequest<Result<Guid>>;

    private sealed record NoResultCommand(string Value) : IRequest<string>;

    private sealed class CreateOrderCommandValidator : AbstractValidator<CreateOrderCommand>
    {
        public CreateOrderCommandValidator()
        {
            RuleFor(x => x.CustomerName)
                .NotEmpty()
                .MinimumLength(3)
                .MaximumLength(120);
        }
    }

    private sealed class CreateOrderCommandHandler : IRequestHandler<CreateOrderCommand, Result<Guid>>
    {
        public Task<Result<Guid>> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
            => Task.FromResult(Result<Guid>.Success(Guid.NewGuid()));
    }

    #endregion

    #region No validators

    [Fact]
    public async Task Handle_WhenNoValidators_CallsNext()
    {
        ValidationBehavior<CreateOrderCommand, Result<Guid>> behavior = new ValidationBehavior<CreateOrderCommand, Result<Guid>>([]);
        CreateOrderCommand command = new CreateOrderCommand("John");
        bool nextCalled = false;

        await behavior.Handle(command, () =>
        {
            nextCalled = true;
            return Task.FromResult(Result<Guid>.Success(Guid.NewGuid()));
        }, CancellationToken.None);

        nextCalled.Should().BeTrue();
    }

    #endregion

    #region Validation success

    [Fact]
    public async Task Handle_WhenValidationPasses_CallsNext()
    {
        ValidationBehavior<CreateOrderCommand, Result<Guid>> behavior = new ValidationBehavior<CreateOrderCommand, Result<Guid>>(
            [new CreateOrderCommandValidator()]);
        CreateOrderCommand command = new CreateOrderCommand("John");
        bool nextCalled = false;

        await behavior.Handle(command, () =>
        {
            nextCalled = true;
            return Task.FromResult(Result<Guid>.Success(Guid.NewGuid()));
        }, CancellationToken.None);

        nextCalled.Should().BeTrue();
    }

    #endregion

    #region Validation failure

    [Fact]
    public async Task Handle_WhenValidationFails_ReturnsFailureResult()
    {
        ValidationBehavior<CreateOrderCommand, Result<Guid>> behavior = new ValidationBehavior<CreateOrderCommand, Result<Guid>>(
            [new CreateOrderCommandValidator()]);
        CreateOrderCommand command = new CreateOrderCommand("");

        Result<Guid> result = await behavior.Handle(command,
            () => Task.FromResult(Result<Guid>.Success(Guid.NewGuid())),
            CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error!.Code.Should().Be(ErrorCodes.Validation);
    }

    [Fact]
    public async Task Handle_WhenValidationFails_ReturnsErrorWithDetails()
    {
        ValidationBehavior<CreateOrderCommand, Result<Guid>> behavior = new ValidationBehavior<CreateOrderCommand, Result<Guid>>(
            [new CreateOrderCommandValidator()]);
        CreateOrderCommand command = new CreateOrderCommand("");

        Result<Guid> result = await behavior.Handle(command,
            () => Task.FromResult(Result<Guid>.Success(Guid.NewGuid())),
            CancellationToken.None);

        result.Error!.Details.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task Handle_WhenValidationFails_DeduplicatesErrors()
    {
        ValidationBehavior<CreateOrderCommand, Result<Guid>> behavior = new ValidationBehavior<CreateOrderCommand, Result<Guid>>(
            [new CreateOrderCommandValidator(), new CreateOrderCommandValidator()]);
        CreateOrderCommand command = new CreateOrderCommand("");

        Result<Guid> result = await behavior.Handle(command,
            () => Task.FromResult(Result<Guid>.Success(Guid.NewGuid())),
            CancellationToken.None);

        IReadOnlyList<ValidationError> details = result.Error!.Details!;
        details.Should().OnlyHaveUniqueItems();
    }

    [Fact]
    public async Task Handle_WhenValidationFails_DoesNotCallNext()
    {
        ValidationBehavior<CreateOrderCommand, Result<Guid>> behavior = new ValidationBehavior<CreateOrderCommand, Result<Guid>>(
            [new CreateOrderCommandValidator()]);
        CreateOrderCommand command = new CreateOrderCommand("");
        bool nextCalled = false;

        await behavior.Handle(command, () =>
        {
            nextCalled = true;
            return Task.FromResult(Result<Guid>.Success(Guid.NewGuid()));
        }, CancellationToken.None);

        nextCalled.Should().BeFalse();
    }

    #endregion

    #region Result type validation

    [Fact]
    public async Task Handle_WhenResultIsNotResultType_ThrowsInvalidOperationException()
    {
        InlineValidator<NoResultCommand> validator = new InlineValidator<NoResultCommand>();
        validator.RuleFor(x => x.Value).NotEmpty();

        ValidationBehavior<NoResultCommand, string> behavior = new ValidationBehavior<NoResultCommand, string>([validator]);
        NoResultCommand command = new NoResultCommand("");

        Func<Task> act = () => behavior.Handle(command,
            () => Task.FromResult(string.Empty),
            CancellationToken.None);

        await act.Should().ThrowAsync<InvalidOperationException>();
    }

    #endregion

    #region Result (non-generic)

    [Fact]
    public async Task Handle_WhenResultIsNonGeneric_ReturnsFailureResult()
    {
        ValidationBehavior<CreateOrderCommand, Result> behavior = new ValidationBehavior<CreateOrderCommand, Result>(
            [new CreateOrderCommandValidator()]);
        CreateOrderCommand command = new CreateOrderCommand("");

        Result result = await behavior.Handle(command,
            () => Task.FromResult(Result.Success()),
            CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error!.Code.Should().Be(ErrorCodes.Validation);
    }

    #endregion
}
