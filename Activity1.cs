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

namespace LayoutTest
{
    [Activity(Label = "LayoutTest", MainLauncher = true, Icon = "@drawable/icon")]
    public class Activity1 : TabActivity
    {
        int count = 1;
        ListView listView;
        List<Persion> items = new List<Persion>();

        TextView debugTextView;
        LinearLayout serverInfo;
        LinearLayout cardsInfo;
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            //ActionBar.SetDisplayShowHomeEnabled(false);
            //ActionBar.SetDisplayShowTitleEnabled(false);
            //ActionBar.SetCustomView(Resource.Layout.action_bar);
            //ActionBar.SetDisplayShowCustomEnabled(true);


            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.layout1);


            AndroidEnvironment.UnhandledExceptionRaiser += (sender, args) =>
            {
                Console.WriteLine("[exception begin]=========================================");
                Console.WriteLine(args.Exception);
                Console.WriteLine("[exception end]===========================================");
            };

            items.Add(new Persion() { name = "jams", age = "22", gender = "man" });
            items.Add(new Persion() { name = "mary", age = "22", gender = "lady" });

            CreateTab(typeof(MyScheduleActivity), "tag1", "schedule");
            CreateTab(typeof(SessionsActivity), "tag2", "sessions");
            CreateTab(typeof(SpeakersActivity), "tag3", "seakers");
            
            // Get our button from the layout resource,
            // and attach an event to it


            //serverInfo = FindViewById<LinearLayout>(Resource.Id.linearLayout1);
            //cardsInfo = FindViewById<LinearLayout>(Resource.Id.linearLayout2);


            //InitServerInfoPage();
            // SocketServer sc = new SocketServer();            
            //sc.Start(5656);

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
        protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);

            if (resultCode == Result.Ok)
            {
                Console.WriteLine(data.Action);
                Console.WriteLine("data.Action ==============");
                Console.WriteLine(data.DataString);
                Console.WriteLine("data.DataString ==============");
                Console.WriteLine(data.Data.Path);
                Console.WriteLine("data.Data.Path ==============");
                Console.WriteLine(File.ReadAllText(data.Data.Path));
                CardsConfig cc = new CardsConfig(File.OpenRead(data.Data.Path));
                Console.WriteLine(cc.GetPilesCount());
                Console.WriteLine("GetPilesCount ==============");
            }
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
        void InitServerInfoPage()
        {
            TextView checkConnectTextView = FindViewById<TextView>(Resource.Id.textView1);
            Button checkConnectButton = FindViewById<Button>(Resource.Id.button1);
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

        }
        void InitCardsInfoPage()
        {
            Button button = FindViewById<Button>(Resource.Id.myButton);

            button.Click += delegate
            {
                var imageIntent = new Intent();
                imageIntent.SetType("text/xml");
                imageIntent.SetAction(Intent.ActionGetContent);
                StartActivityForResult(
                    Intent.CreateChooser(imageIntent, "Select xml"), 0);
            };

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

