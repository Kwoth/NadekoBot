using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Karenkof.Models;

namespace Karenkof.Views
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class CurrencyOne : ContentPage
	{
        private string code2;
        private string inputTextCur;

		public CurrencyOne ()
		{
			InitializeComponent ();
            BindingContext = new ListViewDataModelViewModel();
        }

        public CurrencyOne (string param, string textcur)
        {
            InitializeComponent();
            BindingContext = new ListViewDataModelViewModel();
            code2 = param;
            inputTextCur = textcur;
        }

        private async void ListViewItemTapped(object sender, ItemTappedEventArgs e)
        {
            CurrencyList item = (CurrencyList)e.Item;
            string code1= item.Code.ToString();
            try
            {
                await Navigation.PushAsync(new HomePage(code1, code2, inputTextCur));
            }
            catch (InvalidOperationException ex)
            {
                Console.WriteLine(@"Kesalahan {0}", ex.Message);
                Console.WriteLine(@"sendcoodeee : {0}", code1);
                await Navigation.PopAsync(true);
            }
        }

        public class ListViewDataModelViewModel : BindableObject
        {
            private List<CurrencyList> lstItems;
            public ListViewDataModelViewModel()
            {
                lstItems = new List<CurrencyList>
                {
                    new CurrencyList {Code="AUD", Description="Australian Dollar" },
                    new CurrencyList {Code="BGN",Description="Bulgarian Lev" },
                    new CurrencyList {Code="BRL",Description="Brazilian Real" },
                    new CurrencyList {Code="CAD",Description="Canadian Dollar" },
                    new CurrencyList {Code="CHF",Description="Swiss Franc" },
                    new CurrencyList {Code="CNY",Description="Renminbi (Yuan)" },
                    new CurrencyList {Code="CZK",Description="Czech Koruna"},
                    new CurrencyList {Code="DKK",Description="Danish Krone" },
                    new CurrencyList {Code="EUR",Description="Euro" },
                    new CurrencyList {Code="GBP",Description="Pound Sterling" },
                    new CurrencyList {Code="HKD",Description="Hong Kong Dollar" },
                    new CurrencyList {Code="HRK",Description="Croatian Kuna" },
                    new CurrencyList {Code="HUF",Description="Forint" },
                    new CurrencyList {Code="IDR",Description="Indonesian Rupiah" },
                    new CurrencyList {Code="ILS",Description="New Israeli Sheqel" },
                    new CurrencyList {Code="INR",Description="Ngultrum Indian Rupee" },
                    new CurrencyList {Code="JPY",Description="Yen (Japan)" },
                    new CurrencyList {Code="KRW",Description="Won (South Korea)" },
                    new CurrencyList {Code="MXN",Description="Mexico Peso   " },
                    new CurrencyList {Code="MYR",Description="Malaysian Ringgit" },
                    new CurrencyList {Code="NZD",Description="New Zealand Dollar" },
                    new CurrencyList {Code="PHP",Description="Philippine Peso" },
                    new CurrencyList {Code="PLN",Description="Zloty (Poland)" },
                    new CurrencyList {Code="RON",Description="Romania New Leu" },
                    new CurrencyList {Code="RUB",Description="Rusian Ruble" },
                    new CurrencyList {Code="SEK",Description="Swedish Krona" },
                    new CurrencyList {Code="SGD",Description="Singapore Dollar" },
                    new CurrencyList {Code="THB",Description="Thai Baht" },
                    new CurrencyList {Code="TRY",Description="Yeni Türk Liras (YTL) on 1 January 2005 New Turkish Lira replaced Turkish Lira (TRL)" },
                    new CurrencyList {Code="USD",Description="US Dollar" },
                    new CurrencyList {Code="ZAR",Description="Rand Namibia Dollar" }
                };
            }
            public List<CurrencyList> ListItems
            {
                get { return lstItems; }
                set
                {
                    lstItems = value;
                    OnPropertyChanged("ListItems");
                }
            }
        }
    }
}