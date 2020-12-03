using System;
using System.Collections.Generic;
using System.Linq;
using Glasswall.Administration.K8.TransactionQueryService.Business.Enums;
using Glasswall.Administration.K8.TransactionQueryService.Common.Enums;
using Glasswall.Administration.K8.TransactionQueryService.Common.Models.V1;
// ReSharper disable ConvertIfStatementToReturnStatement
// ReSharper disable InvertIf
// ReSharper disable SwitchStatementHandlesSomeKnownEnumValuesWithDefault

namespace Glasswall.Administration.K8.TransactionQueryService.Business.Store
{
    /// <summary>
    /// Structure of the file contained in the azure file store
    /// </summary>
    public class TransactionAdapationEventMetadataFile
    {
        private readonly Dictionary<EventId, TransactionAdaptionEventModel> _eventLookup = new Dictionary<EventId, TransactionAdaptionEventModel>();

        public IEnumerable<TransactionAdaptionEventModel> Events { get; set; }

        /// <summary>
        /// Determines whether the file contains the event and sets the output parameter to the event
        /// </summary>
        /// <param name="eventId">Event to look for</param>
        /// <param name="adaptionEvent">The output parameter</param>
        /// <returns>True if it was found, false otherwise</returns>
        public bool TryGetEvent(EventId eventId, out TransactionAdaptionEventModel adaptionEvent)
        {
            if (_eventLookup.ContainsKey(eventId))
            {
                adaptionEvent = _eventLookup[eventId];
            }
            else
            {
                adaptionEvent = Events.EventOrDefault(eventId);
                _eventLookup.Add(eventId, adaptionEvent);
            }

            return adaptionEvent != null;
        }

        public bool TryParseEventDateWithFilter(FileStoreFilterV1 filter, out DateTimeOffset timestamp)
        {
            TryGetEvent(EventId.NewDocument, out var newDocumentEvent);

            if (!DateTimeOffset.TryParse(newDocumentEvent.PropertyOrDefault("Timestamp"), out timestamp)) return false;

            return timestamp >= filter.TimestampRangeStart && timestamp <= filter.TimestampRangeEnd;
        }

        public bool TryParseFileIdWithFilter(FileStoreFilterV1 filter, out Guid fileId)
        {
            TryGetEvent(EventId.NewDocument, out var newDocumentEvent);

            if (!Guid.TryParse(newDocumentEvent.PropertyOrDefault("FileId"), out fileId)) return false;
            
            if (filter.SearchFileIds)
            {
                if (!filter.FileIds.Contains(fileId)) return false;
                filter.FileIds.Remove(fileId);
                if (!filter.FileIds.Any()) filter.AllFileIdsFound = true;
            }

            return true;
        }

        public bool TryParseFileTypeWithFilter(FileStoreFilterV1 filter, out FileType fileType)
        {
            TryGetEvent(EventId.FileTypeDetected, out var fileTypeDetectedEvent);

            fileType = fileTypeDetectedEvent.PropertyOrDefault<FileType>("FileType") ?? FileType.Unknown;

            if (filter.SearchFileType) return filter.FileTypes.Contains(fileType);
            return true;
        }

        public bool TryParseRiskWithFilter(FileStoreFilterV1 filter, out Risk risk)
        {
            TryGetEvent(EventId.RebuildCompleted, out var rebuildEvent);
            TryGetEvent(EventId.NcfsCompletedEvent, out var ncfsEvent);

            var gwOutcome = rebuildEvent.PropertyOrDefault<GwOutcome>("GwOutcome") ?? GwOutcome.Unknown;
            var ncfsOutcome = ncfsEvent.PropertyOrDefault<NcfsOutcome>("NCFSOutcome") ?? NcfsOutcome.Unknown;

            risk = RiskAssessment.DetermineRisk(ncfsOutcome, gwOutcome);

            if (filter.SearchRisk) return filter.Risks.Contains(risk);

            return true;
        }

        public bool TryParsePolicyIdWithFilter(FileStoreFilterV1 filter, out Guid policyId)
        {
            TryGetEvent(EventId.NewDocument, out var newDocumentEvent);

            Guid.TryParse(newDocumentEvent.PropertyOrDefault("PolicyId"), out policyId);

            if (filter.SearchPolicyIds) return filter.PolicyIds.Contains(policyId);

            return true;
        }
    }
}