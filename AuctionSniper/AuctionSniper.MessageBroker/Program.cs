using Topshelf;

namespace AuctionSniper.MessageBroker
{
    class Program
    {
        static void Main()
        {
            HostFactory.Run(host =>
            {
                host.Service<MessageBroker>(service =>
                {
                    service.ConstructUsing(name => new MessageBroker(ConfigurationSettings.InBoundChannelName));
                    service.WhenStarted(consumer => consumer.Start());
                    service.WhenContinued(consumer => consumer.Start());
                    service.WhenPaused(consumer => consumer.Pause());
                    service.WhenStopped(consumer => consumer.Stop());
                });
                host.RunAsLocalService();
                host.SetDisplayName("Fake XMPP Message Broker");
                host.SetDescription("Routes messages between XMPP clients");
                host.SetServiceName("Fake.XMPPBroker");
            });
        }
    }
}
