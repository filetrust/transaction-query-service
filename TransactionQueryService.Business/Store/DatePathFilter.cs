using System;
using System.Collections.Generic;
using Glasswall.Administration.K8.TransactionQueryService.Common.Models.V1;
using Glasswall.Administration.K8.TransactionQueryService.Common.Services;

namespace Glasswall.Administration.K8.TransactionQueryService.Business.Store
{
    /// <summary>
    /// This class determines whether the recursion should continue based on the path / folder structure
    /// </summary>
    public class DatePathFilter : IPathFilter
    {
        // This assumes structure is for example from root '/[Year]/[Month]/[Day]/[Hour]/[FileId]/metadata.json'
        // This assumes structure is for example from root '/[Year]/[Month]/[Day]/[Hour]/[FileId]/report.xml'
        private const int NumberOfPartsBeforeFileDirectory = 4;

        private readonly DateTimeOffset _start;
        private readonly DateTimeOffset _end;

        public DatePathFilter(DateTimeOffset? start, DateTimeOffset? end)
        {
            _start = start ?? throw new ArgumentNullException(nameof(start));
            _end = end ?? throw new ArgumentNullException(nameof(end));
        }

        public PathAction DecideAction(string path)
        {
            path = EnsureValidPath(path);

            var parts = path.Split('/');

            if (ShouldRecurse(parts))
                return PathAction.Recurse;
                
            return parts.Length > NumberOfPartsBeforeFileDirectory
                    ? PathAction.Collect
                    : PathAction.Stop;
        }

        private static string EnsureValidPath(string path)
        {
            path ??= string.Empty;
            return path.TrimStart('/');
        }

        private bool ShouldRecurse(IReadOnlyList<string> parts)
        {
            switch (parts.Count)
            {
                case 1:
                    return YearInRange(_start, _end, parts);
                case 2:
                    return MonthFolderInRange(_start, _end, parts);
                case 3:
                    return DayOfMonthInRange(_start, _end, parts);
                case 4:
                    return HourOfDayInRange(_start, _end, parts);
                default:
                    return false;
            }
        }

        private static bool YearInRange(DateTimeOffset start, DateTimeOffset end, IReadOnlyList<string> folderParts)
        {
            if (!int.TryParse(folderParts[0], out var val)) return false;
            return val >= start.Year && val <= end.Year;
        }

        private static bool MonthFolderInRange(DateTimeOffset start, DateTimeOffset end, IReadOnlyList<string> folderParts)
        {
            if (!int.TryParse(folderParts[0], out var parsedYear)) return false;
            if (!int.TryParse(folderParts[1], out var parsedMonth)) return false;

            if (start.Year == end.Year) return parsedMonth >= start.Month && parsedMonth <= end.Month;
            if (parsedYear == start.Year) return parsedMonth >= start.Month;
            if (parsedYear == end.Year) return parsedMonth <= end.Month;
            return true;
        }

        private static bool DayOfMonthInRange(DateTimeOffset start, DateTimeOffset end, IReadOnlyList<string> folderParts)
        {
            if (!int.TryParse(folderParts[0], out var parsedYear)) return false;
            if (!int.TryParse(folderParts[1], out var parsedMonth)) return false;
            if (!int.TryParse(folderParts[2], out var parsedDay)) return false;

            var parsedDateExcludingTime = new DateTimeOffset(new DateTime(parsedYear, parsedMonth, parsedDay));
            var searchStartExcludingTime = new DateTimeOffset(new DateTime(start.Year, start.Month, start.Day));
            var searchEndExcludingTime = new DateTimeOffset(new DateTime(end.Year, end.Month, end.Day));
            return parsedDateExcludingTime >= searchStartExcludingTime && parsedDateExcludingTime <= searchEndExcludingTime;
        }

        private static bool HourOfDayInRange(DateTimeOffset start, DateTimeOffset end, IReadOnlyList<string> folderParts)
        {
            if (!int.TryParse(folderParts[0], out var parsedYear)) return false;
            if (!int.TryParse(folderParts[1], out var parsedMonth)) return false;
            if (!int.TryParse(folderParts[2], out var parsedDay)) return false;
            if (!int.TryParse(folderParts[3], out var parsedHour)) return false;

            var parsedDateIncludingTime = new DateTimeOffset(new DateTime(parsedYear, parsedMonth, parsedDay, parsedHour, 0, 0));
            var searchStartIncludingTime = new DateTimeOffset(new DateTime(start.Year, start.Month, start.Day, start.Hour, 0, 0));
            var searchEndIncludingTime = new DateTimeOffset(new DateTime(end.Year, end.Month, end.Day, end.Hour, 0, 0));
            return parsedDateIncludingTime >= searchStartIncludingTime && parsedDateIncludingTime <= searchEndIncludingTime;
        }
    }
}