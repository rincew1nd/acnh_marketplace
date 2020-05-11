namespace ACNH_Marketplace.Telegram.Enums
{
    public enum UserStateEnum : int
    {
        Default,
        Error,

        //Registration
        Welcome,
        EnteringIGName,
        EnteringIslandName,
        ConfirmRegistration,

        //Host turnip
        HostTurnipExchange,
    }
}
