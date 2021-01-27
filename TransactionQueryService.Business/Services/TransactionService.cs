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

        public Task<GetTransactionsResponseV1> GetTransactionsAsync(GetTransactionsRequestV1 request, CancellationToken cancellationToken)
        {
            if (request == null) throw new ArgumentNullException(nameof(request));

            return InternalGetTransactionsAsync(request, cancellationToken);
        }

        public Task<GetDetailResponseV1> GetDetailAsync(string fileDirectory, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(fileDirectory))
                throw new ArgumentException("Value must not be null or whitespace", nameof(fileDirectory));

            return InternalTryGetDetailAsync(fileDirectory, cancellationToken);
        }

        public async IAsyncEnumerable<DateTimeOffset> GetHourTimestampsOfFiles(DateTimeOffset fromDate, DateTimeOffset toDate, [EnumeratorCancellation]CancellationToken cancellationToken)
        {
            foreach (var fileStore in _fileStores.AsParallel())
            {
                await foreach (var fileDirectory in fileStore.ListAsync(new DatePathFilter(new FileStoreFilterV1 { TimestampRangeStart = fromDate, TimestampRangeEnd = toDate }), cancellationToken))
                {
                    if (!TryParseHourlyDateFromPath(fileDirectory.Replace(MountingInfo.MountLocation, ""), out var dateExcludingTime))
                        continue;

                    yield return dateExcludingTime;
                }
            }
        }

        private static bool TryParseHourlyDateFromPath(string path, out DateTimeOffset parsed)
        {
            var pathSplit = path.Split('/');
            parsed = DateTimeOffset.MaxValue;

            if (!int.TryParse(pathSplit[1], out var parsedYear)) return false;
            if (!int.TryParse(pathSplit[2], out var parsedMonth)) return false;
            if (!int.TryParse(pathSplit[3], out var parsedDay)) return false;
            if (!int.TryParse(pathSplit[4], out var parsedHour)) return false;

            parsed = new DateTimeOffset(new DateTime(parsedYear, parsedMonth, parsedDay, parsedHour, 0, 0));
            return true;
        }

        private async Task<GetDetailResponseV1> InternalTryGetDetailAsync(string fileDirectory, CancellationToken cancellationToken)
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

            return new GetDetailResponseV1
            {
                Status = status,
                AnalysisReport = analysisReport
            };
        }

        private async Task<GetTransactionsResponseV1> InternalGetTransactionsAsync(
            GetTransactionsRequestV1 request,
            CancellationToken cancellationToken)
        {
            _logger.LogInformation("Searching file store ");

            var items = new List<GetTransactionsResponseV1File>();

            foreach (var share in _fileStores.AsParallel())
            {
                await foreach (var item in HandleShare(share, request.Filter, cancellationToken))
                {
                    items.Add(item);
                }
            }

            return new GetTransactionsResponseV1
            {
                Files = items
            };
        }

        private async IAsyncEnumerable<GetTransactionsResponseV1File> HandleShare(
            IFileStore store,
            FileStoreFilterV1 filter,
            [EnumeratorCancellation] CancellationToken cancellationToken)
        {
            await foreach (var fileDirectory in store.ListAsync(new DatePathFilter(filter), cancellationToken))
            {
                if (filter.AllFileIdsFound) yield break;

                var successAndResponse = await TryGetTransactionsResponseV1File(store, filter, fileDirectory, cancellationToken);

                if (successAndResponse.Item1)
                    yield return successAndResponse.Item2;
            }
        }

        private async Task<(bool, GetTransactionsResponseV1File)> TryGetTransactionsResponseV1File(IFileStore store, FileStoreFilterV1 filter, string fileDirectory, CancellationToken cancellationToken)
        {
            try
            {
                var eventFile = await DownloadFile(store, fileDirectory, cancellationToken);

                if (eventFile == null)
                    throw new Exception("Unable to download metadata.json");

                if (!eventFile.TryParseEventDateWithFilter(filter, out var timestamp)) return (false, null);
                if (!eventFile.TryParseFileIdWithFilter(filter, out var fileId)) return (false, null);
                if (!eventFile.TryParseFileTypeWithFilter(filter, out var fileType)) return (false, null); 
                if (!eventFile.TryParseRiskWithFilter(filter, out var risk)) return (false, null); 
                if (!eventFile.TryParsePolicyIdWithFilter(filter, out var policyId)) return (false, null);

                return (true, new GetTransactionsResponseV1File
                {
                    ActivePolicyId = policyId,
                    DetectionFileType = fileType,
                    FileId = fileId,
                    Risk = risk,
                    Timestamp = timestamp,
                    Directory = fileDirectory
                });
            }
            catch (Exception ex)
            {
                _logger.LogError($"{fileDirectory} - Exception raised", ex);
            }

            return (false, null);
        }

        private async Task<TransactionAdapationEventMetadataFile> DownloadFile(IFileStore store, string fileDirectory, CancellationToken cancellationToken)
        {
            using var ms = await store.DownloadAsync($"{fileDirectory}/metadata.json", cancellationToken);
            return await _jsonSerialiser.Deserialize<TransactionAdapationEventMetadataFile>(ms, Encoding.UTF8);
        }
    }
}