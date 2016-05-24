using System;
using System.Threading.Tasks;
using WampSharp.Core.Listener;
using WampSharp.Core.Message;
using WampSharp.V2.Binding;
using Websockets;

namespace WampSharp.WebsocketsPcl
{
    public abstract class PclWebSocketConnection<TMessage> : AsyncWampConnection<TMessage>, IControlledWampConnection<TMessage>
    {
        protected IWebSocketConnection mWebSocket;
        private readonly string mUri;
        private readonly IWampBinding<TMessage> mBinding;
        private bool mIsConnected;

        protected PclWebSocketConnection(string uri, IWampBinding<TMessage> binding)
        {
            mUri = uri;
            mBinding = binding;
        }

        private IWebSocketConnection CreateWebSocket()
        {
            var socket = WebSocketFactory.Create();
            
            socket.OnMessage += OnMessageReceived;
            socket.OnClosed += OnClosed;
            return socket;
        }

        protected abstract void OnMessageReceived(string message);

        protected void OnClosed()
        {
            mWebSocket = null;
            mIsConnected = false;
            RaiseConnectionClosed();
        }

        protected override bool IsConnected => mIsConnected;

        protected abstract override Task SendAsync(WampMessage<object> message);

        public void Connect()
        {
            try
            {
                mWebSocket = CreateWebSocket();
                mWebSocket.Open(mUri);
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
    public class PclWebSocketTextConnection
    {
    }
    public class PclWebSocketActivator
    {
    }
    public class PclWebSocketChannelFactoryExtensions
    {
    }
}
