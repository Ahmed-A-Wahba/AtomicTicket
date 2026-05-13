using MediatR;

namespace AtomicTicket.Application.Abstractions.Messaging;

public interface IQuery<out TResponse> : IRequest<TResponse> { }