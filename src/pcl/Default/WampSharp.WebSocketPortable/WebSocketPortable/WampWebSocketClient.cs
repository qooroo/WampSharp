using WebSocket.Portable;

namespace WampSharp.WebsocketPortable
{
    public class WampWebSocketClient : WebSocketClientBase<WebSocket.Portable.WebSocket>
    {
        public WampWebSocketClient(string subProtocol) : base(subProtocol)
        {
        }
    }
}