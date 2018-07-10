using System.ComponentModel;

namespace DominatorHouseCore.Enums
{
    public enum AccountStatus
    {
        [Description("LangKeySuccess")]Success,
        [Description("LangKeyFailed")]Failed,
        [Description("LangKeyInvalidCredentials")]InvalidCredentials,
        [Description("LangKeyProxyNotWorking")]ProxyNotWorking,
        [Description("LangKeyEmailVerification")]EmailVerification,
        [Description("LangKeyPhoneVerification")]PhoneVerification,
        [Description("LangKeyNeedsVerification")]NeedsVerification,
        [Description("LangKeyPermanentlyBlocked")]PermanentlyBlocked,
        [Description("LangKeyTemporarilyBlocked")]TemporarilyBlocked,
        [Description("LangKeyNotChecked")]NotChecked,
        [Description("LangKeyNotChecked")]TryingToLogin

    }
}