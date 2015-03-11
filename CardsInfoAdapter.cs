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
        private Dictionary<int, Switch> switches = new Dictionary<int, Switch>();
        private Context context;

        public CardsInfoAdapter(Context context, List<string> items)
        {
            this.items = items;
            this.context = context;
            for (int i = 0; i < items.Count; i++)
            {

            }
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
                //row = LayoutInflater.From(this.context).Inflate(Resource.Layout.cards_info_row_inflate, null, false);
                row = LayoutInflater.From(this.context).Inflate(Resource.Layout.cards_info_row_inflate, null, false);
            }

            Switch sw = row.FindViewById<Switch>(Resource.Id.toggleButton1);
            sw.Text = items[position];
            if (switches.ContainsKey(position) == false)
            {
                switches.Add(position, sw);
            }           
            
            return row;
        }
        public bool GetCardChecked(int id)
        {
            if (switches.ContainsKey(id))
            {
                return switches[id].Checked;
            }

            return false;

        }
        public List<string> GetCards()
        {
            return items;
        }
    }
}
