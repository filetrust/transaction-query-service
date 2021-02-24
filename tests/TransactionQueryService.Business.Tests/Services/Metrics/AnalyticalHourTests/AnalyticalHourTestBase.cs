using Glasswall.Administration.K8.TransactionQueryService.Common.Models.Metrics;
using TestCommon;

namespace TransactionQueryService.Business.Tests.Services.Metrics.AnalyticalHourTests
{
    public class AnalyticalHourTestBase : UnitTestBase<AnalyticalHour>
    {
        public const string ExampleGwOutcome = "Replace";
        public const string ExampleNcfsOutcome = "Failed";
        public const string ExampleUnmanagedFileTypeActionFlag = "Unmodified";
        public const string ExampleUnmanagedBlockActionFlag = "Relay";

        public void SharedSetup()
        {
            ClassInTest = new AnalyticalHour();
        }
    }
}
