#if PCL
using System;
using System.Threading.Tasks;
using WampSharp.Core.Listener;
using WampSharp.Core.Message;
using WampSharp.V2.Binding;
using WebSocket.Portable.Interfaces;

namespace WampSharp.WebsocketPortable
{
    public abstract class PortableWebSocketConnection<TMessage> : AsyncWampConnection<TMessage>,
        IControlledWampConnection<TMessage>
    {
        protected WampWebSocketClient mWebSocket;
        private readonly string mUri;
        private readonly IWampBinding<TMessage> mBinding;
        private bool mIsConnected;

        protected PortableWebSocketConnection(string uri,
            IWampBinding<TMessage> binding)
        {
            mUri = uri;
            mBinding = binding;
        }

        private WampWebSocketClient CreateWebSocket()
        {
            var client = new WampWebSocketClient(mBinding.Name);
            client.MessageReceived += OnMessageReceived;
            client.Closed += OnClosed;
            return client;
        }

        protected void OnClosed()
        {
            mWebSocket = null;
            mIsConnected = false;
            RaiseConnectionClosed();
        }

        protected abstract void OnMessageReceived(IWebSocketMessage msg);

        protected override bool IsConnected
        {
            get
            {
                return mIsConnected;
            }
        }

        protected abstract override Task SendAsync(WampMessage<object> message);

        public async void Connect()
        {
            try
            {
                mWebSocket = CreateWebSocket();
                var uri = new Uri(mUri);
                await mWebSocket.OpenAsync(uri.AbsoluteUri, uri.Port);
                mIsConnected = true;
                RaiseConnectionOpen();
            }
            catch (Exception ex)
            {                
                RaiseConnectionError(ex);
                RaiseConnectionClosed();
            }
        }

        protected override void Dispose()
        {
            mWebSocket.Dispose();
        }
    }
}
#endif