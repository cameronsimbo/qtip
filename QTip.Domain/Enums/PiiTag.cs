using System.ComponentModel;

namespace QTip.Domain.Enums;

public enum PiiTag
{
    [Description("pii.email")]
    PiiEmail,

    [Description("finance.iban")]
    FinanceIban,

    [Description("pii.phone")]
    PiiPhone,

    [Description("security.token")]
    SecurityToken
}