using System;
using System.Collections.Generic;
using System.Messaging;
using AuctionSniper.XMPP;

namespace Infrastructure.MessageBroker
{
    internal class MessageBroker 
    {
        private readonly MessageQueue _inputChannel;
        private bool _isRunning;
        private readonly IDictionary<string, MessageQueue> _routingTable = new Dictionary<string, MessageQueue>();  

        public MessageBroker(string inputChannelName)
        {
            _inputChannel = InitializeQueue(inputChannelName);

            _inputChannel.MessageReadPropertyFilter.SetAll();         
 
            _inputChannel.ReceiveCompleted += Route;
        }


        public void Start()
        {
            _isRunning = true;
            Receive();
            Console.WriteLine("Service started");
        }


        public void Pause()
        {
            _isRunning = false;
            Console.WriteLine("Service paused");
        }

        public void Stop()
        {
            _isRunning = false;
            _inputChannel.Close();
            Console.WriteLine("Service stopped");
        }

        public MessageQueue InitializeQueue(string channelName)
        {
            var channel = !MessageQueue.Exists(channelName) ? MessageQueue.Create(channelName) : new MessageQueue(channelName);
            channel.Formatter = new XmlMessageFormatter(new[] {typeof (string)});
            channel.Purge();
            return channel;
        }

        private void Route(object source, ReceiveCompletedEventArgs result)
        {
            try
            {
                var queue = (MessageQueue) source;
                var message = queue.EndReceive(result.AsyncResult);                

                var extension = message.DeserializeExtension();

                Console.WriteLine("message received, from: {0}, recipient: {1}, message type: {2}, message body: {3}", 
                    extension.MessageFrom, extension.MessageTo, extension.MessageType, message.Body);

                var targetQueue = GetTargetQueue(extension.MessageTo);
                targetQueue.Send(message);

                Receive();

            }
            catch (MessageQueueException mqe)
            {
                Console.WriteLine("{0} {1}", mqe.Message, mqe.MessageQueueErrorCode);
            }

            Receive();
        }        

        private MessageQueue GetTargetQueue(string messageTo)
        {
            MessageQueue queue;
            if (!_routingTable.TryGetValue(messageTo, out queue))
            {
                try
                {
                    queue = new MessageQueue(messageTo);
                }
                catch (Exception)
                {
                    Console.WriteLine("Recipient queue does not exist");
                }                
                _routingTable.Add(messageTo, queue);
            }

            return queue;
        }

        private void Receive()
        {
            if (_isRunning)
            {
                _inputChannel.BeginReceive();
            }
        }
    }
}