using Games.Microservice.Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Games.Microservice.Domain.Events
{
    public record GamePurchaseRequestedEvent(
        Guid GameId,
        Guid UserId,
        CurrencyAmount Price
    );

}
