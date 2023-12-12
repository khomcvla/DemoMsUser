namespace DemoMsUser.Common.Enums
{
    [Flags]
    public enum PrivacyEnum
    {
        None        = 0b_0000_0000, //   0
        Friend      = 0b_0000_0001, //   1
        CloseFriend = 0b_0000_0010, //   2
        Family      = 0b_0000_0100, //   4
        Everyone    = 0b_1000_0000  // 128
    }
}
