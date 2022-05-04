using CodeGen.Domain.Common;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CodeGen.Application.Configurations.Validations
{
    public class ValidationBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
        where TResponse : CQRSResponse, new()
    {
        private readonly IValidationHandler<TRequest> validationHandler;

        public ValidationBehaviour()
        {

        }

        public ValidationBehaviour(IValidationHandler<TRequest> validationHandler)
        {
            this.validationHandler = validationHandler;
        }

        public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<TResponse> next)
        {
            if (validationHandler == null)
            {
                return await next();
            }

            var result = await validationHandler.Validate(request);
            if (!result.IsSuccessful)
            {
                return new TResponse { Message = result.Error, StatusCode = HttpStatusCode.BadRequest, Error = true };
            }

            return await next();
        }
    }
}
