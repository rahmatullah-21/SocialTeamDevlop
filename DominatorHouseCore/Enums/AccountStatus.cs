using System.ComponentModel;

namespace DominatorHouseCore.Enums
{
    public enum AccountStatus
    {
        //New Comment
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
        [Description("LangKeyTryingToLogin")]TryingToLogin,
        [Description("LangKeyAddPhoneNumberToYourAccount")]AddPhoneNumberToYourAccount,
        [Description("LangKeyTooManyAttemptsOnPhoneVerification")]
        TooManyAttemptsOnPhoneVerification,
        [Description("LangKeyRetypeEmail")]
        ReTypeEmail,
        [Description("LangKeyRetypePhoneNumber")]
        ReTypePhoneNumber,
        [Description("LangKeyProfileSuspended")]
        ProfileSuspended,
        [Description("LangKeyTwoFactorLoginAttempt")]
        TwoFactorLoginAttempt,
        [Description("LangKeyTooManyAttemptsOnSignIn")]
        TooManyAttemptsOnSignIn

    }
}