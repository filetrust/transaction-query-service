using System.Collections.Generic;
using Glasswall.Administration.K8.TransactionQueryService.Common.Configuration.Validation.Errors;

namespace Glasswall.Administration.K8.TransactionQueryService.Common.Configuration.Validation
{
    public interface IConfigurationItemValidator
    {
        bool TryParse(string key, string rawValue, List<ConfigurationParserError> validationErrors, out object parsed);
    }
}