using System;
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

                var eventFile = await DownloadFile(store, fileDirectory, cancellationToken);

                if (!eventFile.TryParseEventDateWithFilter(filter, out var timestamp)) continue;
                if (!eventFile.TryParseFileIdWithFilter(filter, out var fileId)) continue;
                if (!eventFile.TryParseFileTypeWithFilter(filter, out var fileType)) continue;
                if (!eventFile.TryParseRiskWithFilter(filter, out var risk)) continue;
                if (!eventFile.TryParsePolicyIdWithFilter(filter, out var policyId)) continue;

                yield return new GetTransactionsResponseV1File
                {
                    ActivePolicyId = policyId,
                    DetectionFileType = fileType,
                    FileId = fileId,
                    Risk = risk,
                    Timestamp = timestamp,
                    Directory = fileDirectory
                };
            }
        }

        private async Task<TransactionAdapationEventMetadataFile> DownloadFile(IFileStore store, string fileDirectory, CancellationToken cancellationToken)
        {
            using var ms = await store.DownloadAsync($"{fileDirectory}/metadata.json", cancellationToken);
            return await _jsonSerialiser.Deserialize<TransactionAdapationEventMetadataFile>(ms, Encoding.UTF8);
        }
    }
}