public partial class Popup : BaseMasterPage
{
    public string pageLabel
    {
        set { Label1.Text = value; }
    }
    public override string PageTimer { get; set; }

    public override string ForumUrl { get; set; }
    public override string ForumTitle { get; set; }
}
