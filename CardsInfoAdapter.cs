using Android.Content;
using Android.Views;
using Android.Widget;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LayoutTest
{
    class CardsInfoAdapter : BaseAdapter<string>
    {
        private List<string> items;
        private Context context;
        public CardsInfoAdapter(Context context, List<string> items)
        {
            this.items = items;
            this.context = context;
        }
        public override int Count
        {
            get { return items.Count; }
        }
        public override long GetItemId(int position)
        {
            return position;
        }
        public override string this[int position]
        {
            get { return items[position]; }
        }
        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            View row = convertView;
            if (row == null)
            {
                row = LayoutInflater.From(this.context).Inflate(Resource.Layout.cards_info_row_inflate, null, false);
            }
            TextView txt = row.FindViewById<TextView>(Resource.Id.cardsInfoRowInflateTextView1);
            txt.Text = items[position];
            return row;
        }
    }
}
