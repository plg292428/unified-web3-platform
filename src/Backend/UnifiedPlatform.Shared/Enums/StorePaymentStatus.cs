namespace UnifiedPlatform.Shared.Enums
{
    public enum StorePaymentStatus
    {
        PendingSignature = 0,
        AwaitingOnChainConfirmation = 1,
        Confirmed = 2,
        Failed = 3,
        Cancelled = 4
    }
}
