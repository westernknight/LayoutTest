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
using System.IO;
using System.ComponentModel;
using System.Threading;
using LitJson;
using System.Collections;


namespace LayoutTest
{
    [Activity(Label = "LayoutTest", MainLauncher = true, Icon = "@drawable/icon")]
    public class Activity1 : TabActivity
    {
        public static Activity1 instance;
        public bool gameStart = false;//OutputActivity控制
        public SocketServer sockerServer= new SocketServer();          
        BackgroundWorker checkConnection = new BackgroundWorker();
        protected override void OnCreate(Bundle bundle)
        {
            instance = this;
            base.OnCreate(bundle);

            ActionBar.SetDisplayShowHomeEnabled(false);
            ActionBar.SetDisplayShowTitleEnabled(false);
            ActionBar.SetCustomView(Resource.Layout.action_bar);
            ActionBar.SetDisplayShowCustomEnabled(true);


            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.layout1);
            TextView ipShow = FindViewById<TextView>(Resource.Id.textView1);
            ipShow.Text = "wait for wifi connection...";
            checkConnection.DoWork += (sender, e) => 
            {
                while (true)
                {
                    if (IsNeworkConnect() == true)
                    {
                        IPHostEntry host;
                        string localIP = "?";   
                        host = Dns.GetHostEntry(Dns.GetHostName());
                        foreach (IPAddress ipa in host.AddressList)
                        {
                            if (ipa.AddressFamily == AddressFamily.InterNetwork)
                            {
                                localIP = ipa.ToString();

                            }
                        }
                        sockerServer.Start(5656);
                        //ipShow.Text = localIP;
                        RunOnUiThread(() => { ipShow.Text = localIP + " (" + sockerServer.socketList.Count+ ")"; });
                       
                    }
                    else
                    {
                        
                        RunOnUiThread(() => { ipShow.Text = "wait for wifi connection..."; });  
                    }
                    Thread.Sleep(1000);                    
                }
                
            };
            checkConnection.RunWorkerAsync();




            AndroidEnvironment.UnhandledExceptionRaiser += (sender, args) =>
            {
                Console.WriteLine("[exception begin]=========================================");
                Console.WriteLine(args.Exception);
                Console.WriteLine("[exception end]===========================================");
            };


            CreateTab(typeof(CardsInfoActivity), "tag1", "cardsinfo");
            //CreateTab(typeof(SettingActivity), "tag2", "Setting");
            CreateTab(typeof(SessionsActivity), "tag3", "Sessions");
            CreateTab(typeof(OutputActivity), "tag4", "Output");

            Console.WriteLine("Activity Create");
           
            // Get our button from the layout resource,
            // and attach an event to it


            //serverInfo = FindViewById<LinearLayout>(Resource.Id.linearLayout1);
            //cardsInfo = FindViewById<LinearLayout>(Resource.Id.linearLayout2);

            

            //             serverInfo.Click += (sender, e) =>
            //             {
            //                 Console.WriteLine("serverInfo");
            //                 SetContentView(Resource.Layout.Main);
            //                 
            //                 InitServerInfoPage();
            //             };

            //             cardsInfo.Click += (sender, e) =>
            //             {
            //                 Console.WriteLine("cardsInfo");
            //                 SetContentView(Resource.Layout.cardslist);
            //                 InitCardsInfoPage();
            //                
            //             };




            //listView = FindViewById<ListView>(Resource.Id.listView1);
            //listView.Adapter = new MyListViewAdapter(this, items);
            //listView.ItemClick += (sender, e) => { Console.WriteLine(e.Position); };



        }

        private void CreateTab(Type activityType, string tag, string label)
        {
            var intent = new Intent(this, activityType);
            intent.AddFlags(ActivityFlags.NewTask);

            var spec = TabHost.NewTabSpec(tag);
            spec.SetIndicator(label);
            spec.SetContent(intent);

            TabHost.AddTab(spec);
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
                    return false;
                }

            }
            return false;

        }
    }
}

