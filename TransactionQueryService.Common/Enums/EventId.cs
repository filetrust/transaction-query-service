namespace Glasswall.Administration.K8.TransactionQueryService.Common.Enums
{
    public enum EventId
    {
        Unknown = 0x00,
        NewDocument = 0x10, // 16
        FileTypeDetected = 0x20, // 32
        UnmanagedFiletypeAction = 0x30, // 48
        RebuildStarted = 0x40, // 64
        BlockedFileTypeAction = 0x50, // 80
        RebuildCompleted = 0x60, // 96
        AnalysisCompleted = 0x70, // 112
        NcfsStartedEvent = 0x80, // 128
        NcfsCompletedEvent = 0x90 // 144
    }
}
