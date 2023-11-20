﻿using System.Linq;
using IdentityServer4.Validation.Contexts;
using IdentityServer4.Validation;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Host.Infrastructure.Validators
{
    public class ParameterizedScopeTokenRequestValidator : ICustomTokenRequestValidator
    {
        public Task ValidateAsync(CustomTokenRequestValidationContext context)
        {
            var transaction = context.Result.ValidatedRequest.ValidatedResources.ParsedScopes.FirstOrDefault(x => x.ParsedName == "transaction");
            if (transaction?.ParsedParameter != null)
            {
                context.Result.ValidatedRequest.ClientClaims.Add(new Claim(transaction.ParsedName, transaction.ParsedParameter));
            }

            return Task.CompletedTask;
        }
    }
}
