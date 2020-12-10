using System;
using System.Diagnostics.CodeAnalysis;

namespace Glasswall.Administration.K8.TransactionQueryService.Common.Configuration.Validation.Errors
{
    [ExcludeFromCodeCoverage]
    public class ConfigurationParserError
    {
        public ConfigurationParserError(string paramName, string reason)
        {
            ParamName = paramName ?? throw new ArgumentNullException(nameof(paramName));
            Reason = reason ?? throw new ArgumentNullException(nameof(reason));
        }

        public string ParamName { get; }

        public string Reason { get; }
    }
}