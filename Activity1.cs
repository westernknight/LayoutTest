using System;

using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using System.Collections.Generic;
using Android.Net;
using Android.Net.Wifi;
using Java.Net;
using System.Net;
using System.Net.Sockets;

namespace LayoutTest
{
    [Activity(Label = "LayoutTest", MainLauncher = true, Icon = "@drawable/icon")]
    public class Activity1 : Activity
    {
        int count = 1;
        ListView listView;
        List<Persion> items = new List<Persion>();
        Button checkConnectButton;
        TextView checkConnectTextView;
        TextView debugTextView;
        LinearLayout serverInfo;
        LinearLayout cardsInfo;
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            ActionBar.SetDisplayShowHomeEnabled(false);
            ActionBar.SetDisplayShowTitleEnabled(false);
            ActionBar.SetCustomView(Resource.Layout.action_bar);
            ActionBar.SetDisplayShowCustomEnabled(true);


            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Main);

            AndroidEnvironment.UnhandledExceptionRaiser += (sender, args) =>
            {
                Console.WriteLine("[exception begin]=========================================");
                Console.WriteLine(args.Exception);
                Console.WriteLine("[exception end]===========================================");
            };

            items.Add(new Persion() { name = "jams", age = "22", gender = "man" });
            items.Add(new Persion() { name = "mary", age = "22", gender = "lady" });


            // Get our button from the layout resource,
            // and attach an event to it
            checkConnectButton = FindViewById<Button>(Resource.Id.button1);
            checkConnectTextView = FindViewById<TextView>(Resource.Id.textView1);
            debugTextView = FindViewById<TextView>(Resource.Id.debugTextView);
            serverInfo = FindViewById<LinearLayout>(Resource.Id.linearLayout1);
            cardsInfo = FindViewById<LinearLayout>(Resource.Id.linearLayout2);

            checkConnectButton.Click += (sender, e) =>
                {
                    if (IsNeworkConnect())
                    {
                        WifiManager wifiManager = (WifiManager)GetSystemService(Service.WifiService);
                        int ip = wifiManager.ConnectionInfo.IpAddress;



                        IPHostEntry host;
                        string localIP = "?";
                        host = Dns.GetHostEntry(Dns.GetHostName());
                        foreach (IPAddress ipa in host.AddressList)
                        {
                            if (ipa.AddressFamily == AddressFamily.InterNetwork)
                            {
                                localIP = ipa.ToString();
                                Console.WriteLine(localIP);
                            }
                        }
                        checkConnectTextView.Text = "True " + localIP;
                    }
                    else
                    {
                        checkConnectTextView.Text = "False";
                    }
                };

            SocketServer sc = new SocketServer();
            sc.msg_callback += (msg) => { debugTextView.Text += msg; };
            sc.Start(5656);

            serverInfo.Click += (sender, e) => { Console.WriteLine("serverInfo"); };

            cardsInfo.Click += (sender, e) => { Console.WriteLine("cardsInfo"); };

            //listView = FindViewById<ListView>(Resource.Id.listView1);
            //listView.Adapter = new MyListViewAdapter(this, items);
            //listView.ItemClick += (sender, e) => { Console.WriteLine(e.Position); };


        }

        bool IsNeworkConnect()
        {
            ConnectivityManager connectivityManager = (ConnectivityManager)GetSystemService(Context.ConnectivityService);
            if (connectivityManager != null)
            {
                NetworkInfo mWifi = connectivityManager.GetNetworkInfo(ConnectivityType.Wifi);
                if (mWifi.IsConnected)
                {
                    return true;
                }
                NetworkInfo mMobile = connectivityManager.GetNetworkInfo(ConnectivityType.Mobile);
                if (mMobile.IsConnected)
                {
                    return true;
                }

            }
            return false;

        }
    }
}

