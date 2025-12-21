using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Games.Microservice.Domain.Events
{
    public record PaymentConfirmedEvent(
       Guid GameId,
       Guid UserId
   );

}
