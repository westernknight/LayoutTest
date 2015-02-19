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
        protected override void OnCreate(Bundle bundle)
        {
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

            ListView listView1 = FindViewById<ListView>(Resource.Id.listView1);
            ListView listView2 = FindViewById<ListView>(Resource.Id.listView2);

            List<string> content = new List<string>();
            content.Add("a1");
            content.Add("s1");

            content.Add("a1");
            content.Add("s1");
            content.Add("a1");
            content.Add("s1");
            content.Add("a1");
            content.Add("a1");
            content.Add("a1");
            content.Add("a1");
            content.Add("a1");
            content.Add("a1");
            content.Add("s1");
            content.Add("a1");
            content.Add("s1");
            content.Add("s1");
            content.Add("s1");
            content.Add("s1");
            content.Add("s1");
            content.Add("s1");
            ArrayAdapter<string> items = new ArrayAdapter<string>(this, Android.Resource.Layout.SimpleExpandableListItem1, content);



            listView1.Adapter = new CardsInfoAdapter(this, content);

        }

        protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);

            if (resultCode == Result.Ok)
            {
                Console.WriteLine(File.ReadAllText(data.Data.Path));
                CardsConfig cc = new CardsConfig(File.OpenRead(data.Data.Path));
                Console.WriteLine(cc.GetPilesCount());
                Console.WriteLine("GetPilesCount ==============");
            }
        }
    }
}