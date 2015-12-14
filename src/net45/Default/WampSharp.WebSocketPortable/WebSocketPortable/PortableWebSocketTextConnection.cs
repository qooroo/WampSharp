#if PCL
using System;
using System.Threading.Tasks;
using WampSharp.Core.Message;
using WampSharp.V2.Binding;
using WebSocket.Portable.Interfaces;

namespace WampSharp.WebsocketPortable
{
    public class PortableWebSocketTextConnection<TMessage> : PortableWebSocketConnection<TMessage>
    {
        private readonly IWampTextBinding<TMessage> mTextBinding;

        public PortableWebSocketTextConnection(string uri, IWampTextBinding<TMessage> binding) : 
            base(uri, binding)
        {
            mTextBinding = binding;
        }

        protected override void OnMessageReceived(IWebSocketMessage msg)
        {
            try
            {
                WampMessage<TMessage> message = mTextBinding.Parse(msg.ToString());

                RaiseMessageArrived(message);
            }
            catch (Exception ex)
            {
                RaiseConnectionError(ex);

                if (mWebSocket != null)
                {
                    mWebSocket.Dispose();                    
                }
            }
        }

        protected override async Task SendAsync(WampMessage<object> message)
        {
            try
            {
                string frame = mTextBinding.Format(message);

                await mWebSocket.SendAsync(frame);
            }
            catch (Exception ex)
            {
                RaiseConnectionError(ex);

                if (mWebSocket != null)
                {
                    mWebSocket.Dispose();
                }
                
                throw;
            }
        }
    }
}
#endif