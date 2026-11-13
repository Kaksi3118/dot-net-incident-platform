using MediatR;
using OpsRelay.Domain.Common;

namespace OpsRelay.Application.Common.Events;

public sealed record DomainEventNotification<TDomainEvent>(TDomainEvent DomainEvent)
    : INotification
    where TDomainEvent : IDomainEvent;
