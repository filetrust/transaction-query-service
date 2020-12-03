using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Glasswall.Administration.K8.TransactionQueryService.Common.Enums;

namespace Glasswall.Administration.K8.TransactionQueryService.Business.Store
{
    /// <summary>
    /// Represents the model sent by each step in the adaption process
    /// </summary>
    public class TransactionAdaptionEventModel
    {
        public Dictionary<string, string> Properties { get; set; }

        [ExcludeFromCodeCoverage]
        public static TransactionAdaptionEventModel NewDocumentEvent(
            Guid? policyId = null,
            RequestMode? mode = null,
            Guid? fileId = null,
            DateTimeOffset? timestamp = null)
        {
            var model = Create(EventId.NewDocument, fileId, timestamp);
            model.Properties.Add("PolicyId", policyId.GetValueOrDefault(Guid.NewGuid()).ToString());
            model.Properties.Add("RequestMode", ((int)mode.GetValueOrDefault(RequestMode.Response)).ToString());
            return model;
        }

        [ExcludeFromCodeCoverage]
        public static TransactionAdaptionEventModel FileTypeDetectedEvent(
            FileType fileType,
            Guid? fileId = null,
            DateTimeOffset? timestamp = null)
        {
            var model = Create(EventId.FileTypeDetected, fileId, timestamp);
            model.Properties.Add("FileType", ((int)fileType).ToString());
            return model;
        }

        [ExcludeFromCodeCoverage]
        public static TransactionAdaptionEventModel RebuildEventStarting(
            Guid? fileId = null,
            DateTimeOffset? timestamp = null)
        {
            var model = Create(EventId.RebuildStarted, fileId, timestamp);
            return model;
        }

        [ExcludeFromCodeCoverage]
        public static TransactionAdaptionEventModel RebuildCompletedEvent(
            GwOutcome gwOutcome,
            Guid? fileId = null,
            DateTimeOffset? timestamp = null)
        {
            var model = Create(EventId.RebuildCompleted, fileId, timestamp);
            model.Properties.Add("GwOutcome", gwOutcome.ToString());
            return model;
        }

        [ExcludeFromCodeCoverage]
        public static TransactionAdaptionEventModel AnalysisCompletedEvent(
            Guid? fileId = null,
            DateTimeOffset? timestamp = null)
        {
            var model = Create(EventId.AnalysisCompleted, fileId, timestamp);
            return model;
        }

        [ExcludeFromCodeCoverage]
        public static TransactionAdaptionEventModel NcfsStartedEvent(
            Guid? fileId = null,
            DateTimeOffset? timestamp = null)
        {
            var model = Create(EventId.NcfsStartedEvent, fileId, timestamp);
            return model;
        }

        [ExcludeFromCodeCoverage]
        public static TransactionAdaptionEventModel NcfsCompletedEvent(
            NcfsOutcome ncfsOutcome,
            Guid? fileId = null,
            DateTimeOffset? timestamp = null)
        {
            var model = Create(EventId.NcfsCompletedEvent, fileId, timestamp);
            model.Properties.Add("NCFSOutcome", ncfsOutcome.ToString());
            return model;
        }

        [ExcludeFromCodeCoverage]
        private static TransactionAdaptionEventModel Create(EventId eventId, Guid? fileId, DateTimeOffset? timestamp)
        {
            return new TransactionAdaptionEventModel
            {
                Properties = new Dictionary<string, string>
                {
                    ["FileId"] = fileId?.ToString() ?? Guid.NewGuid().ToString(),
                    ["EventId"] = ((int)eventId).ToString(),
                    ["Timestamp"] = timestamp.GetValueOrDefault(DateTimeOffset.UtcNow).ToString("O")
                }
            };
        }
    }
}