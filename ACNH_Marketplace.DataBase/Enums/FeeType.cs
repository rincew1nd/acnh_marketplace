using System.ComponentModel;

namespace ACNH_Marketplace.DataBase.Enums
{
    public enum FeeType : int
    {
        Unknown = 0,
        [Description("Bells")]
        Money,
        [Description("Nook Miles Tickets")]
        NMT,
        [Description("Star Fragment")]
        StarFragment,
        [Description("Recipe")]
        Recipe,
        [Description("Resource")]
        Resource
    }
}
