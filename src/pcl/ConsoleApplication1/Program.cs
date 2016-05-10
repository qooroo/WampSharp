using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WampSharp.V2;
using WampSharp.V2.Client;

namespace ConsoleApplication1
{
    class Program
    {
        static void Main(string[] args)
        {
            Websockets.Net.WebsocketConnection.Link();

            DefaultWampChannelFactory factory =
                new DefaultWampChannelFactory();

            const string serverAddress = "ws://web-dev.adaptivecluster.com:8080/ws";

            IWampChannel channel =
                factory.CreateJsonChannel(serverAddress, "com.weareadaptive.reactivetrader");

            channel.Open().Wait(5000);

            IWampRealmProxy realmProxy = channel.RealmProxy;

            int received = 0;
            IDisposable subscription = null;

            subscription =
                realmProxy.Services.GetSubject<dynamic>("prices")
                     .Subscribe(x =>
                     {
                         Console.WriteLine("Got Event: " + x);

                         received++;

                         if (received > 5)
                         {
                             Console.WriteLine("Closing ..");
                             subscription.Dispose();
                         }
                     });

            Console.ReadLine();
        }
    }
}
