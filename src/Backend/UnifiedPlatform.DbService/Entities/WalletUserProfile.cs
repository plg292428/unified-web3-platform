using System;

namespace UnifiedPlatform.DbService.Entities
{
    public partial class WalletUserProfile
    {
        public long WalletUserProfileId { get; set; }

        public int Uid { get; set; }

        public string ProviderType { get; set; } = null!;

        public string? ProviderName { get; set; }

        public string WalletAddress { get; set; } = null!;

        public string? AddressLabel { get; set; }

        public int? PreferredChainId { get; set; }

        public DateTime CreateTime { get; set; }

        public DateTime? LastUsedTime { get; set; }

        public bool IsPrimary { get; set; }

        public virtual User UidNavigation { get; set; } = null!;
    }
}
