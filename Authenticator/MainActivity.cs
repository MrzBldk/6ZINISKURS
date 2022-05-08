using Android.App;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using AndroidX.AppCompat.App;
using AndroidX.RecyclerView.Widget;
using Authenticator.Adapters;
using Authenticator.Models;
using Google.Android.Material.FloatingActionButton;
using SQLite;
using System;
using System.IO;
using System.Threading.Tasks;
using TotpLibrary;

namespace Authenticator
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme.NoActionBar", MainLauncher = true)]
    public class MainActivity : AppCompatActivity
    {
        private SQLiteConnection _db;
        
        private CodeAdapater _adapter;

        private Button btnPopupCancel;
        private Button btnPopOk;
        private Dialog popupDialog;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            SetContentView(Resource.Layout.activity_main);

            string dbPath = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), "codes.db");
            _db = new SQLiteConnection(dbPath);
            _db.CreateTable<Code>();

            RecyclerView recyclerView = FindViewById<RecyclerView>(Resource.Id.recyclerView);
            LinearLayoutManager LayoutManager = new LinearLayoutManager(this);
            recyclerView.SetLayoutManager(LayoutManager);
            _adapter = new CodeAdapater(_db.Table<Code>());
            _adapter.ItemLongClick += OnItemLongClick;
            recyclerView.SetAdapter(_adapter);

            AndroidX.AppCompat.Widget.Toolbar toolbar = FindViewById<AndroidX.AppCompat.Widget.Toolbar>(Resource.Id.toolbar);
            SetSupportActionBar(toolbar);

            FloatingActionButton fab = FindViewById<FloatingActionButton>(Resource.Id.fab);
            fab.Click += FabOnClick;

            ChangedData();
        }

        private void ChangedData()
        {
            Task.Delay(1000).ContinueWith(t =>
            {
                _adapter.NotifyDataSetChanged();
                ChangedData();
            }, TaskScheduler.FromCurrentSynchronizationContext());
        }

        void OnItemLongClick(object sender, int position)
        {
            _db.Delete(_db.Table<Code>().ElementAt(position));
            Toast.MakeText(Application.Context, "Сode has been deleted!", ToastLength.Short).Show();
        }

        private void FabOnClick(object sender, EventArgs eventArgs)
        {
            popupDialog = new Dialog(this);
            popupDialog.SetContentView(Resource.Layout.Popup);
            popupDialog.Window.SetSoftInputMode(SoftInput.AdjustResize);
            popupDialog.Show();

            popupDialog.Window.SetLayout(ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.WrapContent);
            popupDialog.Window.SetBackgroundDrawableResource(Android.Resource.Color.Transparent);

            Spinner spinner = popupDialog.FindViewById<Spinner>(Resource.Id.spinner);
            var adapter = ArrayAdapter.CreateFromResource(this, Resource.Array.hash_array, Android.Resource.Layout.SimpleSpinnerItem);
            adapter.SetDropDownViewResource(Android.Resource.Layout.SimpleSpinnerDropDownItem);
            spinner.Adapter = adapter;

            btnPopupCancel = popupDialog.FindViewById<Button>(Resource.Id.btnCancel);
            btnPopOk = popupDialog.FindViewById<Button>(Resource.Id.btnOk);

            btnPopupCancel.Click += BtnPopupCancel_Click;
            btnPopOk.Click += BtnPopOk_Click;

        }
        private void BtnPopOk_Click(object sender, EventArgs e)
        {
            AutoCompleteTextView name = popupDialog.FindViewById<AutoCompleteTextView>(Resource.Id.autocomplete_name);
            AutoCompleteTextView key = popupDialog.FindViewById<AutoCompleteTextView>(Resource.Id.autocomplete_key);
            AutoCompleteTextView step = popupDialog.FindViewById<AutoCompleteTextView>(Resource.Id.autocomplete_step);
            AutoCompleteTextView length = popupDialog.FindViewById<AutoCompleteTextView>(Resource.Id.autocomplete_length);
            Spinner spinner = popupDialog.FindViewById<Spinner>(Resource.Id.spinner);

            try
            {
                Code code = new Code
                {
                    Name = name.Text,
                    SecretCode = Base32.ToBytes(key.Text),
                    TimeStep = TimeSpan.FromSeconds(int.Parse(step.Text)),
                    Algorithm = spinner.SelectedItem.ToString(),
                    Length = int.Parse(length.Text)
                };
                _db.Insert(code);
            }
            catch
            {
                Toast.MakeText(Application.Context, "Incorrect data", ToastLength.Short).Show();
            }

            popupDialog.Dismiss();
            popupDialog.Hide();
        }

        private void BtnPopupCancel_Click(object sender, EventArgs e)
        {
            popupDialog.Dismiss();
            popupDialog.Hide();
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }
    }
}
