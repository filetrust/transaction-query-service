namespace Glasswall.Administration.K8.TransactionQueryService.Common.Enums
{
    public enum EventId
    {
        Unknown = 0x00,
        NewDocument = 0x10,
        FileTypeDetected = 0x20,
        UnmanagedFiletypeAction = 0x30,
        RebuildStarted = 0x40,
        BlockedFileTypeAction = 0x50,
        RebuildCompleted = 0x60,
        AnalysisCompleted = 0x70,
        NcfsStartedEvent = 0x80,
        NcfsCompletedEvent = 0x90
    }
}
