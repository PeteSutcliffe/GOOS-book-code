﻿using System;
using System.Messaging;
using MSMQMessage = System.Messaging.Message;

namespace AuctionSniper.XMPP
{
    public class Connection
    {
        private readonly string _userName;

        private readonly string _inputChannelName;

        private readonly MessageQueue _brokerChannel;
        private MessageQueue _inputChannel;

        private ChatManager _chatManager;

        public Connection(string hostName, string userName)
        {
            _userName = userName;
            _inputChannelName = GetChannelName(userName);
            _brokerChannel = new MessageQueue(GetChannelName(hostName));
        }

        public string User
        {
            get { return _userName; }
        }

        private static string GetChannelName(string userName)
        {
            if (userName.Contains("private$"))
                return userName;

            return string.Format(@".\private$\{0}", userName);
        }

        private static string RemoveChannelInfo(string channelName)
        {
            return channelName.Replace(@".\private$\", "");
        }

        public void Connect()
        {
            _inputChannel = InitializeQueue(_inputChannelName);
            _inputChannel.MessageReadPropertyFilter.SetAll();
            _inputChannel.ReceiveCompleted += OnMessageReceived;
            _inputChannel.BeginReceive();
            _inputChannel.Formatter = new XmlMessageFormatter(new[] { typeof(string) });
            _chatManager = new ChatManager(this);
        }

        private void OnMessageReceived(object sender, ReceiveCompletedEventArgs results)
        {
            var message = _inputChannel.EndReceive(results.AsyncResult);            
            var extension = message.DeserializeExtension();

            switch (extension.MessageType)
            {
                case MessageInfo.MessageTypes.CreateChat:
                    GetChatManager().ReceiveChat(RemoveChannelInfo(extension.MessageFrom));
                    break;
                case MessageInfo.MessageTypes.Message:
                    GetChatManager().MessageReceived(new Message(message.Body.ToString()), RemoveChannelInfo(extension.MessageFrom));
                    break;
            }

            _inputChannel.BeginReceive();
        }

        private MessageQueue InitializeQueue(string inputChannelName)
        {
            var queue = MessageQueue.Exists(inputChannelName)
                       ? new MessageQueue(inputChannelName)
                       : MessageQueue.Create(inputChannelName);
            queue.Purge();
            return queue;
        }

        public ChatManager GetChatManager()
        {
            if (_chatManager == null)
            {
                throw new InvalidOperationException("Not connected");
            }

            return _chatManager;
        }

        public void SendMessage(Message message, string recipient)
        {
            var msmqMessage = new MSMQMessage(message.Body);
            var extension = new MessageInfo
                                {
                                    MessageFrom = _inputChannelName,
                                    MessageTo = GetChannelName(recipient),
                                    MessageType = MessageInfo.MessageTypes.Message
                                };

            var extensionBytes = extension.Serialize();

            msmqMessage.Extension = extensionBytes;
            _brokerChannel.Send(msmqMessage);
        }

        internal void SendCreateMessage(string recipient)
        {
            var msmqMessage = new MSMQMessage("Create");
            var extension = new MessageInfo
            {
                MessageFrom = _inputChannelName,
                MessageTo = GetChannelName(recipient),
                MessageType = MessageInfo.MessageTypes.CreateChat                
            };

            var extensionBytes = extension.Serialize();

            msmqMessage.Extension = extensionBytes;
            _brokerChannel.Send(msmqMessage);
        }

        public void Disconnect()
        {
            _inputChannel.Close();
            _inputChannel = null;
            _chatManager = null;
        }
    }
}
