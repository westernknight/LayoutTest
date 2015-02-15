using Android.App;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LayoutTest
{
    class DialogCreate:DialogFragment
    {
        public override Android.Views.View OnCreateView(Android.Views.LayoutInflater inflater, Android.Views.ViewGroup container, Android.OS.Bundle savedInstanceState)
        {
             base.OnCreateView(inflater, container, savedInstanceState);


            var view = inflater.Inflate(Resource.Layout.dialog_create, container,false);
            return view;
        }
    }
}
