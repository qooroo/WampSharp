﻿using System;
using WampSharp.PubSub.Server;

namespace WampSharp.Api
{
    public interface IWampHost : IDisposable
    {
        void Open();
        void HostService(object instance);

        IWampTopicContainer TopicContainer { get; }
    }
}