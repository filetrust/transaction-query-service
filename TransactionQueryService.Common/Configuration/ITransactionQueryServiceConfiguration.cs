using System;

namespace Glasswall.Administration.K8.TransactionQueryService.Common.Configuration
{
    public interface ITransactionQueryServiceConfiguration
    {
        string Username { get; set; }
        string Password { get; }
        string TokenSecret { get; }
    }

    public class TransactionQueryServiceConfiguration : ITransactionQueryServiceConfiguration
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public string TokenSecret { get; set; } = Guid.NewGuid().ToString();
    }
}