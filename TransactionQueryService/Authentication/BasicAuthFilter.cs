using System;
using System.Diagnostics.CodeAnalysis;
using System.Net.Http.Headers;
using System.Text;
using Glasswall.Administration.K8.TransactionQueryService.Common.Configuration;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Server.HttpSys;
using Microsoft.Extensions.DependencyInjection;

namespace Glasswall.Administration.K8.TransactionQueryService.Authentication
{
    [ExcludeFromCodeCoverage]
    public class BasicAuthFilter : Attribute, IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            if (!TryGetAndValidateCredentials(context))
                context.Result = new UnauthorizedResult();
        }

        private static bool TryGetAndValidateCredentials(ActionContext context)
        {
            if (!AuthenticationHeaderValue.TryParse(context.HttpContext.Request.Headers["Authorization"], out var header)) return false;
            if (!header.Scheme.Equals(AuthenticationSchemes.Basic.ToString(), StringComparison.OrdinalIgnoreCase)) return false;

            var credentials = Encoding.UTF8
                .GetString(Convert.FromBase64String(header.Parameter ?? string.Empty))
                .Split(":");

            if (credentials.Length != 2)
                return false;

            var username = credentials[0];
            var password = credentials[1];

            var config = context.HttpContext.RequestServices
                .GetRequiredService<ITransactionQueryServiceConfiguration>();

            return config.Username == username && config.Password == password;
        }
    }
}