#if PCL
using System;
using WampSharp.Core.Listener;
using WampSharp.V2.Binding;
using WampSharp.WebsocketPortable;

namespace WampSharp.V2.Fluent
{
    internal class PortableWebSocketActivator : IWampConnectionActivator
    {
        private readonly string mServerAddress;

        public PortableWebSocketActivator(string serverAddress)
        {
            mServerAddress = serverAddress;
        }

        public IControlledWampConnection<TMessage> Activate<TMessage>(IWampBinding<TMessage> binding)
        {
            Func<IControlledWampConnection<TMessage>> factory = 
                () => GetConnectionFactory(binding);

            ReviveClientConnection<TMessage> result = 
                new ReviveClientConnection<TMessage>(factory);

            return result;
        }

        private IControlledWampConnection<TMessage> GetConnectionFactory<TMessage>(IWampBinding<TMessage> binding)
        {
            IWampTextBinding<TMessage> textBinding = binding as IWampTextBinding<TMessage>;

            if (textBinding != null)
            {
                return CreateTextConnection(textBinding);
            }

            throw new Exception();
        }

        protected IControlledWampConnection<TMessage> CreateTextConnection<TMessage>(IWampTextBinding<TMessage> textBinding)
        {
            return new PortableWebSocketTextConnection<TMessage>(mServerAddress, textBinding);
        }
    }
}

#endif