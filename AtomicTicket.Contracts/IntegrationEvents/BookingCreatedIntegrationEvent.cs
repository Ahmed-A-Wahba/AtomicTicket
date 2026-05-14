using AtomicTicket.SharedKernel.Primitives;

namespace AtomicTicket.Contracts.IntegrationEvents;

public sealed record BookingCreatedIntegrationEvent
(
) : IIntegrationEvent;
