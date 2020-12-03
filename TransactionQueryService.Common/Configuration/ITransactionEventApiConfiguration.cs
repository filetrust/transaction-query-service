namespace Glasswall.Administration.K8.TransactionQueryService.Common.Configuration
{
    public interface ITransactionQueryServiceConfiguration
    {
        string TransactionStoreConnectionStringCsv { get; }
        string ShareName { get; }
    }
}