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
        public OutputActivity instance;
        List<string> items = new List<string>();
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            // Create your application here
            SetContentView(Resource.Layout.console_output);

            ListView listView = FindViewById<ListView>(Resource.Id.listView1);
            listView.Adapter = new OutputAdapter(this, items);

            instance = this;
        }
        public void Clear()
        {
            items.Clear();
        }
        public void Push(string sz)
        {
            items.Add(sz);
        }
        public void Pop()
        {
            if (items.Count>0)
            {
                items.RemoveAt(items.Count - 1);
            }            
        }
    }
}