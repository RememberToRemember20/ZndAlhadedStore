using System.Threading.Channels;
using ZndAlhadedStore.Interfaces;

namespace ZndAlhadedStore.Implment
{
    public class EmailQueue : IEmailQueue
    {
        private readonly Channel<EmailMessage> _channel = Channel.CreateBounded<EmailMessage>(
            new BoundedChannelOptions(500) { FullMode = BoundedChannelFullMode.Wait, SingleReader = true });

        public ChannelReader<EmailMessage> Reader => _channel.Reader;

        public bool TryEnqueue(EmailMessage message) => _channel.Writer.TryWrite(message);
    }
}
