using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Games.Microservice.Domain.DTOS
{
    public class UserPurchaseDto
    {
        public Guid UserId { get; set; }
        public Guid GameId { get; set; }
        public Guid Category { get; set; }
    }
}
