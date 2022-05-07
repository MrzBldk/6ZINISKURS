using Android.Views;
using AndroidX.RecyclerView.Widget;
using Authenticator.Models;
using Authenticator.Services;
using SQLite;
using System;

namespace Authenticator.Adapters
{
    class CodeAdapater : RecyclerView.Adapter
    {
        private readonly TableQuery<Code> _list;
        public event EventHandler<int> ItemLongClick;

        public CodeAdapater(TableQuery<Code> codes)
        {
            _list = codes;
        }

        public override int ItemCount => _list.Count();

        private void OnLongClick(int position)
        {
            if (ItemLongClick != null)
                ItemLongClick(this, position);
        }

        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            CodeViewHolder vh = holder as CodeViewHolder;
            Code code = _list.ElementAt(position);
            var generator = new TOTP(() => DateTimeOffset.UtcNow, code.TimeStep, code.Algorithm, code.Length);
            vh.Code.Text = generator.Generate(code.SecretCode);
            vh.Name.Text = code.Name;
            vh.ProgressBar.Max = (int)code.TimeStep.TotalSeconds;
            vh.ProgressBar.Progress = (int)code.TimeStep.TotalSeconds - (int)DateTimeOffset.UtcNow.ToUniversalTime().ToUnixTimeSeconds() % (int)code.TimeStep.TotalSeconds;
        }

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            View itemView = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.CodeView, parent, false);
            var vh = new CodeViewHolder(itemView, OnLongClick);
            return vh;
        }
    }
}