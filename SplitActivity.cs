using System;
using Android.App;
using Android.OS;
using Android.Runtime;
using Android.Views;
using AndroidX.AppCompat.Widget;
using AndroidX.AppCompat.App;
using Google.Android.Material.FloatingActionButton;
using Google.Android.Material.Snackbar;
using Android.Widget;
using Android.Text;

namespace SplitBill
{
    [Activity(Label = "SplitBill")]
    public class SplitActivity : AppCompatActivity
    {
        double amount;
        TextView toPay;
        EditText percentage;
        EditText tipAmount;
        Spinner spinner;
        SeekBar seeker;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.activity_split);

            toPay = FindViewById<TextView>(Resource.Id.txtTotal);
            amount = Intent.GetDoubleExtra("Amount", 0);
            ChangeAmountToPay(toPay, amount.ToString());

            percentage = FindViewById<EditText>(Resource.Id.inputPercentage);
            tipAmount = FindViewById<EditText>(Resource.Id.inputTipAmount);
            spinner = FindViewById<Spinner>(Resource.Id.spinner1);
            seeker = FindViewById<SeekBar>(Resource.Id.seekBar1);

            InitializeTip(percentage, tipAmount, seeker, amount);

            var adapter = ArrayAdapter.CreateFromResource(
                    this, Resource.Array.people_array, Android.Resource.Layout.SimpleSpinnerItem);

            adapter.SetDropDownViewResource(Android.Resource.Layout.SimpleSpinnerDropDownItem);
            spinner.Adapter = adapter;


            seeker.ProgressChanged += OnSeekerProgressChanged;

            tipAmount.AfterTextChanged += OnTipTextChanged;

            percentage.AfterTextChanged += OnPercentageTextChanged;

            spinner.ItemSelected += (sender, e) =>
            {
                var isValid = double.TryParse(tipAmount.Text, out double tip);
                if (isValid)
                {
                    ChangeAmountToPay(toPay, GetAmountToPay(amount, tip, GetSpinnerValue(spinner)));
                }
            };
        }

        private void OnPercentageTextChanged(object sender, AfterTextChangedEventArgs e)
        {
            tipAmount.AfterTextChanged -= OnTipTextChanged;
            var isValid = Int32.TryParse(percentage.Text.NormalizePercentage(), out int tip);
            if (isValid)
            {
                ChangeTipText(tip.ToString(), tipAmount);
                ChangeSeekerProgress(seeker, Int32.Parse(tip.ToString()));

            }
            tipAmount.AfterTextChanged += OnTipTextChanged;
        }

        private void OnTipTextChanged(object sender, AfterTextChangedEventArgs e)
        {
            percentage.AfterTextChanged -= OnPercentageTextChanged;
            var isValid = double.TryParse(tipAmount.Text, out double tip);
            if (isValid)
            {
                var percent = GetPercentageValue(amount, tip);
                ChangePercentageText(percent.ToString() + "%", percentage);
                ChangeSeekerProgress(seeker, percent);
                ChangeAmountToPay(toPay, GetAmountToPay(amount, Percent(amount, percent), GetSpinnerValue(spinner)));
            }
            percentage.AfterTextChanged += OnPercentageTextChanged;
        }

        private void OnSeekerProgressChanged(object sender, SeekBar.ProgressChangedEventArgs e)
        {
            if (e.FromUser)
            {
                ChangeTipText(Percent(amount, seeker.Progress).ToString(), tipAmount);
                ChangePercentageText(seeker.Progress.ToString() + '%', percentage);
            }
        }

        private void InitializeTip(EditText percentage, EditText tipAmount, SeekBar seeker, double total)
        {
            percentage.Text = Resources.GetString(Resource.String.default_percentage) + "%";
            seeker.Progress = Int32.Parse(percentage.Text.NormalizePercentage());
            tipAmount.Text = Percent(total, seeker.Progress).ToString();
        }

        private int GetSpinnerValue(Spinner spinner)
        {
            return Int32.Parse(spinner.SelectedItem.ToString());
        }

        private string GetAmountToPay(double amount, double tip, int people)
        {
            var result = (amount + tip) / people;
            return result.ToString();
        }
        private void ChangeAmountToPay(TextView total, string value)
        {
            total.Text = value;
        }
        private void ChangeSeekerProgress(SeekBar seeker, int progress)
        {
            seeker.Progress = progress;
        }

        private void ChangeTipText(string tip, EditText tipAmount)
        {
            tipAmount.Text = tip;
        }

        private void ChangePercentageText(string percent, EditText percentage)
        {
            percentage.Text = percent;
        }

        private int GetPercentageValue(double total, double value)
        {
            return (int)((value * 100) / total);
        }
        private double Percent(double total, double percent)
        {
            return (percent * total) / 100;
        }


    }
}