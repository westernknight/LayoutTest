using Android.Content;
using Android.Views;
using Android.Widget;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LayoutTest
{
    class MyListViewAdapter:BaseAdapter<Persion>
    {
        List<Persion> items;
        Context context;
        public MyListViewAdapter(Context c, List<Persion> l)
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
            TextView textName = row.FindViewById<TextView>(Resource.Id.textName);
            TextView textAge = row.FindViewById<TextView>(Resource.Id.textAge);
            TextView textGender = row.FindViewById<TextView>(Resource.Id.textGender);
            textName.Text = items[position].name;
            textAge.Text = items[position].age;
            textGender.Text = items[position].gender;
            return row;
        }
        public override Persion this[int position]
        {
            get { return items[position]; }
        }
    }
}
