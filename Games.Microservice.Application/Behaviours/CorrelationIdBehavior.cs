using Games.Microservice.shared;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Games.Microservice.Application.Behaviours
{
    public class CorrelationBehavior<TRequest, TResponse>
    : IPipelineBehavior<TRequest, TResponse>
    {
        public async Task<TResponse> Handle(
            TRequest request,
            RequestHandlerDelegate<TResponse> next,
            CancellationToken cancellationToken)
        {
            _ = CorrelationContext.Current;
            return await next();
        }
    }

}
