using System.ComponentModel;


namespace SnitzConfig
{
    /// <summary>
    /// Grid pager display type
    /// </summary>
    public enum PagerType
    {
        [Description("Pager With DropdownList")]
        Dropdown,
        [Description("Pager with Buttons")]
        Button,
        [Description("Pager using Hyperlinks")]
        Text,
        [Description("Pager using LinkButtons")]
        Lnkbutton
    };

    /// <summary>
    /// Forum Subscription Level
    /// </summary>
    public enum SubscriptionLevel
    {
        [Description("No Subscriptions")]
        None = 0,
        [Description("Subscribe to Whole Board")]
        Board,
        [Description("Subscribe by Category")]
        Category,
        [Description("Subscribe by Forum")]
        Forum,
        [Description("Subscribe by Topic")]
        Topic
    }

    /// <summary>
    /// Forum Rank display type
    /// </summary>
    public enum RankType
    {
        None = 0,
        RankOnly,
        StarsOnly,
        Both
    }
}
