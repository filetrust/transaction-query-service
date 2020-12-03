namespace Glasswall.Administration.K8.TransactionQueryService.Common.Configuration
{
    public interface IConfigurationParser
    {
        TConfiguration Parse<TConfiguration>() where TConfiguration : new();
    }
}