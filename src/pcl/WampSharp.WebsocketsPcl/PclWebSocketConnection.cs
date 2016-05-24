using System;
using System.Threading.Tasks;
using WampSharp.Core.Listener;
using WampSharp.Core.Message;
using WampSharp.V2.Binding;
using WampSharp.V2.Fluent;
using Websockets;

namespace WampSharp.WebsocketsPcl
{
    public abstract class PclWebSocketConnection<TMessage> : AsyncWampConnection<TMessage>, IControlledWampConnection<TMessage>
    {
        protected IWebSocketConnection WebSocket;
        private readonly string _mUri;
        private bool _mIsConnected;

        protected PclWebSocketConnection(string uri)
        {
            _mUri = uri;
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
            WebSocket = null;
            _mIsConnected = false;
            RaiseConnectionClosed();
        }

        protected override bool IsConnected => _mIsConnected;

        protected abstract override Task SendAsync(WampMessage<object> message);

        public void Connect()
        {
            try
            {
                WebSocket = CreateWebSocket();
                WebSocket.Open(_mUri);
                _mIsConnected = true;
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
            WebSocket.Dispose();
        }
    }
    public class PclWebSocketTextConnection<TMessage> : PclWebSocketConnection<TMessage>
    {
        private readonly IWampTextBinding<TMessage> _mTextBinding;

        public PclWebSocketTextConnection(string uri, IWampTextBinding<TMessage> binding) : base(uri)
        {
            _mTextBinding = binding;
        }

        protected override void OnMessageReceived(string message)
        {
            try
            {
                var wampMessage = _mTextBinding.Parse(message);
                RaiseMessageArrived(wampMessage);
            }
            catch (Exception ex)
            {
                RaiseConnectionError(ex);
                WebSocket?.Dispose();
            }
        }

        protected override async Task SendAsync(WampMessage<object> message)
        {
            try
            {
                var frame = _mTextBinding.Format(message);
                WebSocket.Send(frame);
            }
            catch (Exception ex)
            {
                RaiseConnectionError(ex);

                WebSocket?.Dispose();

                throw;
            }
        }
    }

    public class PclWebSocketActivator : IWampConnectionActivator
    {
        private readonly string _mServerAddress;

        public PclWebSocketActivator(string serverAddress)
        {
            _mServerAddress = serverAddress;
        }

        public IControlledWampConnection<TMessage> Activate<TMessage>(IWampBinding<TMessage> binding)
        {
            Func<IControlledWampConnection<TMessage>> factory = () => GetConnectionFactory(binding);

            var result = new ReviveClientConnection<TMessage>(factory);

            return result;
        }

        private IControlledWampConnection<TMessage> GetConnectionFactory<TMessage>(IWampBinding<TMessage> binding)
        {
            var textBinding = binding as IWampTextBinding<TMessage>;

            if (textBinding != null)
            {
                return CreateTextConnection(textBinding);
            }

            throw new ArgumentException("binding is not of type IWampTextBinding<T>");
        }

        protected IControlledWampConnection<TMessage> CreateTextConnection<TMessage>(IWampTextBinding<TMessage> textBinding)
        {
            return new PclWebSocketTextConnection<TMessage>(_mServerAddress, textBinding);
        }
    }

    public static class PclWebSocketChannelFactoryExtensions
    {
        public static ChannelFactorySyntax.ITransportSyntax WebSocketTransport(this ChannelFactorySyntax.IRealmSyntax realmSyntax, string address)
        {
            var state = realmSyntax.State;

            state.ConnectionActivator = new PclWebSocketActivator(address);

            return state;
        }
    }
}
