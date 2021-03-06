﻿using System;
using AuctionSniper.Domain;
using AuctionSniper.XMPP;

namespace AuctionSniper.UI.Wpf
{
    public class AppMain
    {
        private readonly SniperWindow _ui;

        private const int ArgHostName = 0;
        private const int ArgUserName = 1;

        private Action _closeAction;

        private readonly SniperPortfolio _portfolio = new SniperPortfolio();

        public static void Run(string[] args)
        {                                    
            var hostName = args[ArgHostName];
            var userName = args[ArgUserName];

            var auctionHouse = new XMPPAuctionHouse(hostName, userName);
            
            var appMain = new AppMain();
            appMain.AddRequestListenerFor(auctionHouse);
            appMain.OnUiClosing(auctionHouse.Disconnect);
        }

        private void OnUiClosing(Action closeAction)
        {
            _closeAction = closeAction;
        }

        public AppMain()
        {
            _ui = new SniperWindow(_portfolio);
            _ui.Show();
            _ui.Closing += UiClosing;            
        }

        void UiClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if(_closeAction!=null) 
                _closeAction.Invoke();
        }

        private void AddRequestListenerFor(XMPPAuctionHouse auctionHouse)
        {
            var launcher = new SniperLauncher(auctionHouse, _portfolio);

            _ui.SetUserRequestListener(launcher.JoinAuction);
        }        
    }
}