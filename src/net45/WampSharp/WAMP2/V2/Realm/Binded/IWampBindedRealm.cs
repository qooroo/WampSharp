﻿using WampSharp.V2.Authentication;
using WampSharp.V2.Core.Contracts;

namespace WampSharp.V2.Realm.Binded
{
    public interface IWampBindedRealm<TMessage>
    {
        IWampServer<TMessage> Server { get; }

        void Hello(long session, HelloDetails details, WelcomeDetails welcomeDetails);
        void Abort(long session, AbortDetails details, string reason);
        void Goodbye(long session, GoodbyeDetails details, string reason);
        void SessionLost(long sessionId);
    }
}