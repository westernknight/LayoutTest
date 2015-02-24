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

namespace LayoutTest
{
    [Activity(Label = "My Activity")]
    public class OutputActivity : Activity
    {
        public static  OutputActivity instance;
        List<string> items = new List<string>();
        ListView listView;
        protected override void OnCreate(Bundle bundle)
        {
            instance = this;
            base.OnCreate(bundle);

            // Create your application here
            SetContentView(Resource.Layout.console_output);

            listView = FindViewById<ListView>(Resource.Id.listView1);
            listView.Adapter = new OutputAdapter(this, items);

            Button bt = FindViewById<Button>(Resource.Id.button1);
            bt.Click += (sender, e) => { Push(DateTime.Now.ToString()); };
        }
        public void Clear()
        {
            items.Clear();
        }
        public void Push(string sz)
        {

            items.Insert(0, sz);
            listView.Adapter = new OutputAdapter(this, items);
        }
        public void Pop()
        {
            if (items.Count>0)
            {
                items.RemoveAt(items.Count - 1);
            }
            listView.Adapter = new OutputAdapter(this, items);
        }
    }
}