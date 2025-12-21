using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Games.Microservice.Domain.ValueObjects
{
    public record Money(decimal Amount, string Currency)
    {
        public static Money BRL(decimal value)
            => new(value, "BRL");
    }
}
