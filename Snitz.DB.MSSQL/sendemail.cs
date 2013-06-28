using System.Net;
using System.Net.Mail;
using SnitzConfig;
using SnitzData;

/// <summary>
/// Summary description for sendemail
/// </summary>
public class snitzEmail
{
    private static MailAddress _toUser;
    private static MailAddress _replyTo;
    private static MailAddress _fromUser;
    private static string _subject;
    private static string _msgBody;

    public MailAddress toUser
    {
        set
        {
            _toUser = value;
        }
    }
    public string fromUser
    {
        set
        {

            if (value.Contains("Administrator"))
            {
                _fromUser = new MailAddress(Config.AdminEmail, Config.ForumTitle);
                _replyTo = new MailAddress(Config.AdminEmail, Config.ForumTitle);
            }
            else
            {
                Member mu = Util.GetMember(value);
                _replyTo = new MailAddress(mu.Email, mu.Name);
                _fromUser = new MailAddress(Config.AdminEmail, Config.ForumTitle);
            }
        }
    }
    public string subject
    {
        set
        {
            _subject = value;
        }
    }
    public string msgBody
    {
        set
        {
            _msgBody = value;
        }
    }

    public void send()
    {

        MailMessage message = new MailMessage {IsBodyHtml = true, From = _fromUser, ReplyTo = _replyTo};
        message.To.Add(_toUser);
        message.Subject = _subject;
        message.Body = "<html><body>" + _msgBody + "</body></html>";
        SmtpClient client = new SmtpClient {Host = Config.EmailHost, Port = Config.EmailPort};
        if (Config.EmailAuthenticate)
        {
            NetworkCredential credential = new NetworkCredential(Config.EmailAuthUser, Config.EmailAuthPwd);
            client.Credentials = credential;
        }
        client.Send(message);
    }
}
