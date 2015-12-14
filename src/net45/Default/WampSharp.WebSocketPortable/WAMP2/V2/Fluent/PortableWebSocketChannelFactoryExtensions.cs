#if PCL
namespace WampSharp.V2.Fluent
{
    public static class PortableWebSocketChannelFactoryExtensions
    {
        public static ChannelFactorySyntax.ITransportSyntax WebSocketTransport(this ChannelFactorySyntax.IRealmSyntax realmSyntax, string address)
        {
            ChannelState state = realmSyntax.State;

            state.ConnectionActivator = new PortableWebSocketActivator(address);

            return state;
        }
    }
}

#endif