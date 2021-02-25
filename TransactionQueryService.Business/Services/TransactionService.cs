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
using Glasswall.Administration.K8.TransactionQueryService.Common.Models;
using Glasswall.Administration.K8.TransactionQueryService.Common.Models.AnalysisReport;
using Glasswall.Administration.K8.TransactionQueryService.Common.Models.Metrics;
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
                transactionFile.TryGetEvent(EventId.UnmanagedFiletypeAction, out var unmanagedFiletypeAction);
                transactionFile.TryGetEvent(EventId.BlockedFileTypeAction, out var blockedFileTypeAction);

                if (!DateTimeOffset.TryParse(newDocumentEvent?.PropertyOrDefault("Timestamp"), out var timestamp)) continue;
                var hourTimestamp = new DateTimeOffset(new DateTime(timestamp.Year, timestamp.Month, timestamp.Day, timestamp.Hour, 0, 0));
                var gwOutcome = rebuildCompleted.PropertyOrDefault("GwOutcome");
                var ncfsOutcome = ncfsEvent.PropertyOrDefault("NCFSOutcome");
                var unmanagedFileTypeActionFlag = unmanagedFiletypeAction.PropertyOrDefault("Action");
                var blockedFileTypeActionFlag = blockedFileTypeAction.PropertyOrDefault("Action");

                analytics.AddOrUpdate(hourTimestamp, AnalyticalHour.Initial(hourTimestamp, gwOutcome, ncfsOutcome, unmanagedFileTypeActionFlag, blockedFileTypeActionFlag),
                    (key, val) => { val.Update(gwOutcome, ncfsOutcome, unmanagedFileTypeActionFlag, blockedFileTypeActionFlag); return val; });
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
                _logger.LogError(0, ex, $"{fileDirectory} - Exception raised");
            }

            return (false, null);
        }
    }
}