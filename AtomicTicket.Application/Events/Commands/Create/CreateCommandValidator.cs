using FluentValidation;

namespace AtomicTicket.Application.Events.Commands.Create;

public sealed class CreateEventCommandValidator : AbstractValidator<CreateEventCommand>
{
    public CreateEventCommandValidator()
    {
        RuleFor(c => c.Title)
            .NotEmpty()
                .WithErrorCode("Event.TitleRequired")
                .WithMessage("Event title cannot be empty.")
            .MaximumLength(200)
                .WithErrorCode("Event.TitleTooLong")
                .WithMessage("Event title cannot exceed 200 characters.");

        RuleFor(c => c.Description)
            .NotEmpty()
                .WithErrorCode("Event.DescriptionRequired")
                .WithMessage("Event description cannot be empty.")
            .MaximumLength(2000)
                .WithErrorCode("Event.DescriptionTooLong")
                .WithMessage("Event description cannot exceed 2000 characters.");

        RuleFor(c => c.VenueName)
            .NotEmpty()
                .WithErrorCode("Venue.NameRequired")
                .WithMessage("Venue name cannot be empty.")
            .MaximumLength(200)
                .WithErrorCode("Venue.NameTooLong")
                .WithMessage("Venue name cannot exceed 200 characters.");

        RuleFor(c => c.VenueAddress)
            .NotEmpty()
                .WithErrorCode("Venue.AddressRequired")
                .WithMessage("Venue address cannot be empty.")
            .MaximumLength(500)
                .WithErrorCode("Venue.AddressTooLong")
                .WithMessage("Venue address cannot exceed 500 characters.");

        RuleFor(c => c.VenueCapacity)
            .GreaterThan(0)
                .WithErrorCode("Venue.InvalidCapacity")
                .WithMessage("Venue capacity must be greater than zero.");

        RuleFor(c => c.Date)
            .GreaterThan(DateTimeOffset.UtcNow)
                .WithErrorCode("Event.InvalidDate")
                .WithMessage("Event date must be in the future.");
    }
}