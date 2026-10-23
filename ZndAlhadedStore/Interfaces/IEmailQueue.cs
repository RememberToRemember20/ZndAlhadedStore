namespace ZndAlhadedStore.Interfaces
{
    public record EmailMessage(string To, string Subject, string HtmlBody, string TextBody);

    public interface IEmailQueue
    {
        bool TryEnqueue(EmailMessage message);
    }
}
