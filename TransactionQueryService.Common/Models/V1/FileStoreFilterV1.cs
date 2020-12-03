using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Glasswall.Administration.K8.TransactionQueryService.Common.Enums;

namespace Glasswall.Administration.K8.TransactionQueryService.Common.Models.V1
{
    public class FileStoreFilterV1
    {
        [Required]
        public DateTimeOffset? TimestampRangeStart { get; set; }

        [Required]
        public DateTimeOffset? TimestampRangeEnd { get; set; }

        public IEnumerable<FileType>  FileTypes { get; set; }

        public IEnumerable<Risk> Risks { get; set; }

        public IEnumerable<Guid> PolicyIds { get; set; }

        /// <summary>
        /// This is a list so that we can remove as we find them to reduce checks (Should be unique)
        /// </summary>
        public List<Guid> FileIds { get; set; }

        public bool SearchFileIds => FileIds?.Any() ?? false;

        public bool SearchFileType => FileTypes?.Any() ?? false;

        public bool SearchRisk => Risks?.Any() ?? false;

        public bool SearchPolicyIds => PolicyIds?.Any() ?? false;

        public bool AllFileIdsFound { get; set; }
    }
}