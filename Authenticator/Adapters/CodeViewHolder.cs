using Android.Views;
using Android.Widget;
using AndroidX.RecyclerView.Widget;
using System;

namespace Authenticator.Adapters
{
    class CodeViewHolder : RecyclerView.ViewHolder
    {
        public TextView Code { get; private set; }
        public TextView Name { get; private set; }
        public ProgressBar ProgressBar { get; private set; }

        public CodeViewHolder(View itemView, Action<int> listener) : base(itemView)
        {

            Code = itemView.FindViewById<TextView>(Resource.Id.textView);
            Name = itemView.FindViewById<TextView>(Resource.Id.textViewname);
            ProgressBar = itemView.FindViewById<ProgressBar>(Resource.Id.progressBar);

            itemView.LongClick += (sender, e) => listener(LayoutPosition);
        }
    }
}