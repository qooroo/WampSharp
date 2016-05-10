#if PCL
using System;
using System.Threading.Tasks;
using WampSharp.V2.Binding;
using WampSharp.Core.Message;

namespace WampSharp.WebsocketsPcl
{
    public class MessageWebSocketTextConnection<TMessage> : MessageWebSocketConnection<TMessage>
    {
        private readonly IWampTextBinding<TMessage> mTextBinding;

        public MessageWebSocketTextConnection(string uri, IWampTextBinding<TMessage> binding) :
            base(uri, binding)
        {
            mTextBinding = binding;
        }

        protected override void OnMessageReceived(string rawMessage)
        {
            try
            {
                var message = mTextBinding.Parse(rawMessage);

                RaiseMessageArrived(message);
            }
            catch (Exception ex)
            {
                RaiseConnectionError(ex);
                mWebSocket?.Dispose();
            }
        }

        protected override async Task SendAsync(WampMessage<object> message)
        {
            try
            {
                var frame = mTextBinding.Format(message);
                mWebSocket.Send(frame);
            }
            catch (Exception ex)
            {
                RaiseConnectionError(ex);
                mWebSocket?.Dispose();
                throw;
            }
        }
    }
}
#endif