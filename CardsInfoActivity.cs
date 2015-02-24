using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System.IO;

namespace LayoutTest
{
    [Activity(Label = "My Activity")]
    public class CardsInfoActivity : Activity
    {
        List<string> pileOne = new List<string>();
        List<string> pileTwo = new List<string>();
        public ListView listView1;
        public ListView listView2;
        public static CardsInfoActivity instance;
        protected override void OnCreate(Bundle bundle)
        {
            instance = this;
            base.OnCreate(bundle);

            // Create your application here
            SetContentView(Resource.Layout.cards_info);

            Button button = FindViewById<Button>(Resource.Id.button1);

            button.Click += delegate
            {
                var imageIntent = new Intent();
                imageIntent.SetType("text/xml");
                imageIntent.SetAction(Intent.ActionGetContent);
                StartActivityForResult(
                    Intent.CreateChooser(imageIntent, "Select xml"), 0);
            };
            

            //Console.WriteLine(File.ReadAllText(data.Data.Path));
            //CardsConfig cc = new CardsConfig(File.OpenRead(data.Data.Path));
            CardsConfig cc = new CardsConfig(File.OpenRead("/storage/emulated/0/kk.xml"));
            Console.WriteLine(cc.GetPilesCount());
            Console.WriteLine("GetPilesCount ==============");
            pileOne = cc.GetPile(0);
            pileTwo = cc.GetPile(1);


            ListView listView1 = FindViewById<ListView>(Resource.Id.listView1);
            ListView listView2 = FindViewById<ListView>(Resource.Id.listView2);

 
            listView1.Adapter = new CardsInfoAdapter(this, pileOne);             
            listView2.Adapter = new CardsInfoAdapter(this, pileTwo);

            //Console.WriteLine(((CardsInfoAdapter)(listView1.Adapter)).GetCardCount());
            //Console.WriteLine("Checked ==============");
        }

        protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);

            if (resultCode == Result.Ok)
            {
               
                Console.WriteLine(File.ReadAllText(data.Data.Path));
                //CardsConfig cc = new CardsConfig(File.OpenRead(data.Data.Path));
                CardsConfig cc = new CardsConfig(File.OpenRead("/storage/emulated/0/kk.xml"));
                Console.WriteLine(cc.GetPilesCount());
                Console.WriteLine("GetPilesCount ==============");
                pileOne = cc.GetPile(0);
                pileTwo = cc.GetPile(1);


                ListView listView1 = FindViewById<ListView>(Resource.Id.listView1);
                ListView listView2 = FindViewById<ListView>(Resource.Id.listView2);

                listView1.Adapter = new CardsInfoAdapter(this, pileOne);
                listView2.Adapter = new CardsInfoAdapter(this, pileTwo);

            }
        }
    }
}