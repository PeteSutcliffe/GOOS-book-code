using System.Configuration;

namespace AuctionSniper.MessageBroker
{
    public static class ConfigurationSettings
    {
        static ConfigurationSettings()
        {
            InBoundChannelName = @".\private$\broker_channel"; //ConfigurationManager.AppSettings["InBoundChannelName"];
        }

        public static string InBoundChannelName { get; set; }
    }
}
