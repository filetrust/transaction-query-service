using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Glasswall.Administration.K8.TransactionQueryService.Business.Store;
using Glasswall.Administration.K8.TransactionQueryService.Common.Enums;
using Glasswall.Administration.K8.TransactionQueryService.Common.Models.AnalysisReport;
using Glasswall.Administration.K8.TransactionQueryService.Common.Models.V1;
using Glasswall.Administration.K8.TransactionQueryService.Common.Serialisation;
using Glasswall.Administration.K8.TransactionQueryService.Common.Services;
using Microsoft.Extensions.Logging;
using EventId = Glasswall.Administration.K8.TransactionQueryService.Common.Enums.EventId;

namespace Glasswall.Administration.K8.TransactionQueryService.Business.Services
{
    public class TransactionService : ITransactionService
    {
        private readonly ILogger<ITransactionService> _logger;
        private readonly IEnumerable<IFileStore> _fileStores;
        private readonly IJsonSerialiser _jsonSerialiser;
        private readonly IXmlSerialiser _xmlSerialiser;

        public TransactionService(
            ILogger<ITransactionService> logger,
            IEnumerable<IFileStore> fileStores,
            IJsonSerialiser jsonSerialiser,
            IXmlSerialiser xmlSerialiser)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _fileStores = fileStores ?? throw new ArgumentNullException(nameof(fileStores));
            _jsonSerialiser = jsonSerialiser ?? throw new ArgumentNullException(nameof(jsonSerialiser));
            _xmlSerialiser = xmlSerialiser ?? throw new ArgumentNullException(nameof(xmlSerialiser));
        }

        public Task<Transactions> GetTransactionsAsync(GetTransactionsRequestV1 request, CancellationToken cancellationToken)
        {
            if (request == null) throw new ArgumentNullException(nameof(request));

            return InternalGetTransactionsAsync(request, cancellationToken);
        }

        public Task<TransactionDetail> GetDetailAsync(string fileDirectory, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(fileDirectory))
                throw new ArgumentException("Value must not be null or whitespace", nameof(fileDirectory));

            return InternalTryGetDetailAsync(fileDirectory, cancellationToken);
        }

        public async Task<TransactionAnalytics> GetTransactionAnalyticsAsync(DateTimeOffset fromDate, DateTimeOffset toDate, CancellationToken cancellationToken)
        {
            var response = new TransactionAnalytics();
            var analytics = new ConcurrentDictionary<DateTimeOffset, AnalyticalHour>();

            await foreach (var (transactionFile, _) in GetEachTransactionAndPath(fromDate, toDate, cancellationToken))
            {
                transactionFile.TryGetEvent(EventId.NewDocument, out var newDocumentEvent);
                transactionFile.TryGetEvent(EventId.RebuildCompleted, out var rebuildCompleted);
                transactionFile.TryGetEvent(EventId.NcfsCompletedEvent, out var ncfsEvent);
                
                if (!DateTimeOffset.TryParse(newDocumentEvent?.PropertyOrDefault("Timestamp"), out var timestamp)) continue;
                var hourTimestamp = new DateTimeOffset(new DateTime(timestamp.Year, timestamp.Month, timestamp.Day, timestamp.Hour, 0, 0));
                var gwOutcome = rebuildCompleted.PropertyOrDefault<GwOutcome>("GwOutcome");
                var ncfsOutcome = ncfsEvent.PropertyOrDefault<NcfsOutcome>("NCFSOutcome");

                analytics.AddOrUpdate(hourTimestamp,
                    new AnalyticalHour
                    {
                        Date = hourTimestamp,
                        Processed =  1, // gwOutcome == null && ncfsOutcome == null ? 0 : 1,
                        // Pending = gwOutcome == null && ncfsOutcome == null ? 1 : 0,
                        SentToNcfs = ncfsOutcome != null ? 1 : 0,
                        ProcessedByNcfs = new Dictionary<string, long>
                        {
                            [NcfsOutcome.Blocked.ToString()] = ncfsOutcome == NcfsOutcome.Blocked ? 1 : 0,
                            [NcfsOutcome.Relayed.ToString()] = ncfsOutcome == NcfsOutcome.Relayed ? 1 : 0,
                            [NcfsOutcome.Replaced.ToString()] = ncfsOutcome == NcfsOutcome.Replaced ? 1 : 0,
                        },
                        ProcessedByOutcome = new Dictionary<string, long>
                        {
                            [GwOutcome.Failed.ToString()] = gwOutcome == GwOutcome.Failed ? 1 : 0,
                            [GwOutcome.Replace.ToString()] = gwOutcome == GwOutcome.Replace ? 1 : 0,
                            [GwOutcome.Unmodified.ToString()] = gwOutcome == GwOutcome.Unmodified ? 1 : 0,
                        }
                    },
                    (key, val) =>
                    {
                        if (gwOutcome != null) val.ProcessedByOutcome[gwOutcome.Value.ToString()] += 1;
                        
                        if (ncfsOutcome != null)
                        {
                            val.ProcessedByNcfs[ncfsOutcome.Value.ToString()] += 1;
                            val.SentToNcfs += 1;
                        }
                        
                        //if (gwOutcome == null && ncfsOutcome == null)
                        //    val.Pending += 1;
                        // else
                        
                        val.Processed += 1;
                        return val;
                    });
            }

            response.TotalProcessed = analytics.Sum(f => f.Value.Processed);
            response.Data = analytics.Select(f => f.Value).OrderBy(f => f.Date).ToArray();
            return response;
        }

        private async Task<TransactionDetail> InternalTryGetDetailAsync(string fileDirectory, CancellationToken cancellationToken)
        {
            GWallInfo analysisReport = null;
            var status = DetailStatus.FileNotFound;

            foreach (var fileStore in _fileStores)
            {
                if (!await fileStore.ExistsAsync(fileDirectory, cancellationToken)) continue;

                var fullPath = $"{fileDirectory}/report.xml";

                using (var analysisReportStream = await fileStore.DownloadAsync(fullPath, cancellationToken))
                {
                    if (analysisReportStream == null)
                    {
                        status = DetailStatus.AnalysisReportNotFound;
                        break;
                    }

                    analysisReport = await _xmlSerialiser.Deserialize<GWallInfo>(analysisReportStream, Encoding.UTF8);
                    status = DetailStatus.Success;
                    break;
                }
            }

            return new TransactionDetail
            {
                Status = status,
                AnalysisReport = analysisReport
            };
        }

        private async Task<Transactions> InternalGetTransactionsAsync(
            GetTransactionsRequestV1 request,
            CancellationToken cancellationToken)
        {
            _logger.LogInformation("Searching file store ");

            var items = new List<TransactionFile>();

            await foreach (var (transactionFile, fileDirectory) in GetEachTransactionAndPath(request.Filter.TimestampRangeStart, request.Filter.TimestampRangeEnd, cancellationToken))
            {
                if (request.Filter.AllFileIdsFound) break;

                if (!transactionFile.TryParseEventDateWithFilter(request.Filter, out var timestamp)) continue;
                if (!transactionFile.TryParseFileIdWithFilter(request.Filter, out var fileId)) continue;
                if (!transactionFile.TryParseFileTypeWithFilter(request.Filter, out var fileType)) continue;
                if (!transactionFile.TryParseRiskWithFilter(request.Filter, out var risk)) continue;
                if (!transactionFile.TryParsePolicyIdWithFilter(request.Filter, out var policyId)) continue;

                items.Add(new TransactionFile
                {
                    ActivePolicyId = policyId,
                    DetectionFileType = fileType,
                    FileId = fileId,
                    Risk = risk,
                    Timestamp = timestamp,
                    Directory = fileDirectory
                });
            }

            return new Transactions
            {
                Files = items
            };
        }

        private async IAsyncEnumerable<(TransactionAdapationEventMetadataFile, string)> GetEachTransactionAndPath(DateTimeOffset? dateStart, DateTimeOffset? dateEnd, [EnumeratorCancellation]CancellationToken cancellationToken)
        {
            foreach (var store in _fileStores.AsParallel())
            {
                await foreach (var fileDirectory in store.ListAsync(new DatePathFilter(dateStart, dateEnd), cancellationToken))
                {
                    var (couldRead, file) = await TryReadTransactionEventFile(store, fileDirectory, cancellationToken);

                    if (couldRead) yield return (file, fileDirectory);
                }
            }
        }
        
        private async Task<(bool, TransactionAdapationEventMetadataFile)> TryReadTransactionEventFile(IFileStore store, string fileDirectory, CancellationToken cancellationToken)
        {
            try
            {
                using var ms = await store.DownloadAsync($"{fileDirectory}/metadata.json", cancellationToken);
                var eventFile = await _jsonSerialiser.Deserialize<TransactionAdapationEventMetadataFile>(ms, Encoding.UTF8);

                if (eventFile == null) throw new Exception($"{fileDirectory} - file could not be read.");

                return (true, eventFile);
            }
            catch (Exception ex)
            {
                _logger.LogError($"{fileDirectory} - Exception raised", ex);
            }

            return (false, null);
        }
    }
}