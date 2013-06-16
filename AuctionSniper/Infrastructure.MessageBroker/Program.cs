using Topshelf;

namespace AuctionSniper.MessageBroker
{
    class Program
    {
        static void Main()
        {
            HostFactory.Run(host =>
            {
                host.Service<Infrastructure.MessageBroker.MessageBroker>(service =>
                {
                    service.ConstructUsing(name => new Infrastructure.MessageBroker.MessageBroker(ConfigurationSettings.InBoundChannelName));
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
