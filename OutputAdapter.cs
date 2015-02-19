using Android.Content;
using Android.Views;
using Android.Widget;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LayoutTest
{
    class OutputAdapter:BaseAdapter<string>
    {
         List<string> items;
        Context context;
        public OutputAdapter(Context c, List<string> l)
        {
            context = c;
            items = l;
        }
        public override int Count
        {
            get { return items.Count; }
        }
        public override long GetItemId(int position)
        {
            return position;
        }
        public override Android.Views.View GetView(int position, Android.Views.View convertView, Android.Views.ViewGroup parent)
        {
            View row = convertView;
            if (row == null)
            {
                row = LayoutInflater.From(context).Inflate(Resource.Layout.layout_row, null);
            }
            TextView textName = row.FindViewById<TextView>(Resource.Id.textView1);

            textName.Text = items[position];
            return row;
        }
        public override string this[int position]
        {
            get { return items[position]; }
        }
    }
}
