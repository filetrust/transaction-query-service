using System;
using Microsoft.AspNetCore.Mvc;

namespace Glasswall.Administration.K8.TransactionQueryService.Authentication
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class BasicAuthAttribute : TypeFilterAttribute
    {
        public BasicAuthAttribute() : base(typeof(BasicAuthFilter))
        {
        }
    }
}