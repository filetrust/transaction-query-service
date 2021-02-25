using System;
using System.Collections.Generic;
using System.Linq;

namespace Glasswall.Administration.K8.TransactionQueryService.Common.Models.Metrics
{
    public class AnalyticalHour
    {
        public long Processed { get; set; }

        public long SentToNcfs { get; set; }

        public DateTimeOffset Date { get; set; }

        public Dictionary<string, long> ProcessedByOutcome { get; set; }

        public Dictionary<string, long> ProcessedByNcfs { get; set; }

        public static AnalyticalHour Initial(
            DateTimeOffset hourTimestamp,
            string gwOutcome,
            string ncfsOutcome,
            string unmanagedFileTypeActionFlag,
            string blockedFileTypeActionFlag)
        {
            var hour = new AnalyticalHour
            {
                Date = hourTimestamp,
                ProcessedByNcfs = new Dictionary<string, long>(),
                ProcessedByOutcome = new Dictionary<string, long>()
            };

            hour.Update(gwOutcome, ncfsOutcome, unmanagedFileTypeActionFlag, blockedFileTypeActionFlag);
            return hour;
        }

        public void Update(
            string gwOutcome, 
            string ncfsOutcome, 
            string unmanagedFileTypeActionFlag, 
            string blockedFileTypeActionFlag)
        {
            if (!string.IsNullOrWhiteSpace(unmanagedFileTypeActionFlag))
                SafeIncrement(ProcessedByOutcome, unmanagedFileTypeActionFlag);
            else if (!string.IsNullOrWhiteSpace(blockedFileTypeActionFlag))
                SafeIncrement(ProcessedByOutcome, blockedFileTypeActionFlag);
            else
                SafeIncrement(ProcessedByOutcome, gwOutcome);

            SafeIncrement(ProcessedByNcfs, ncfsOutcome);

            if (!string.IsNullOrWhiteSpace(ncfsOutcome))
                SentToNcfs += 1;

            Processed += 1;
        }
        
        private static void SafeIncrement<TKey>(IDictionary<string, long> dict, TKey key)
        {
            var asString = key?.ToString();

            if (string.IsNullOrWhiteSpace(asString)) return;

            asString = asString.First().ToString().ToUpper() + asString.Substring(1);

            if (dict.ContainsKey(asString))
                dict[asString] += 1;
            else
                dict.Add(asString, 1);
        }
    }
}