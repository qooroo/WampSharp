using System;
using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using WampSharp.V2;
using WampSharp.V2.Client;

namespace App1
{
    [Activity(Label = "App1", MainLauncher = true, Icon = "@drawable/icon")]
    public class MainActivity : Activity
    {
        int count = 1;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Main);

            // Get our button from the layout resource,
            // and attach an event to it
            Button button = FindViewById<Button>(Resource.Id.MyButton);

            button.Click += delegate { button.Text = string.Format("{0} clicks!", count++); };

            Websockets.Droid.WebsocketConnection.Link();

            var factory = new DefaultWampChannelFactory();

            const string serverAddress = "ws://web-dev.adaptivecluster.com:8080/ws";

            var channel = factory.CreateJsonChannel(serverAddress, "com.weareadaptive.reactivetrader");

            channel.Open().Wait(5000);

            var realmProxy = channel.RealmProxy;

            var received = 0;
            IDisposable subscription = null;

            subscription =
                realmProxy.Services.GetSubject<dynamic>("prices")
                     .Subscribe(x =>
                     {
                         received++;

                         if (received > 5)
                         {
                             subscription.Dispose();
                         }
                     });

            Console.ReadLine();
        }
    }
}

