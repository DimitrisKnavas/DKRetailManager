using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TRMDesktopUI.ViewModels
{
    public class SalesViewModel : Screen
    {
        private BindingList<string> _products ;

        public BindingList<string> Products
        {
            get { return _products; }
            set { _products = value;
                NotifyOfPropertyChange(() => Products);
            }
        }

        private BindingList<string> _cart;

        public BindingList<string> Cart
        {
            get { return _cart; }
            set { _cart = value;
                NotifyOfPropertyChange(() => Products);

            }
        }

        public string SubTotal
        {
            get
            {
                return "$0.00";
            }
            
        }

        public string Tax
        {
            get
            {
                return "$0.00";
            }

        }

        public string Total
        {
            get
            {
                return "$0.00";
            }

        }



        private string _itemQuantity;

        public string ItemQuantity
        {
            get { return _itemQuantity; }
            set { _itemQuantity = value;
                NotifyOfPropertyChange(() => Products);
                }
        }

        public void AddToCart()
        {

        }

        public bool CanAddToCart
        {
            get
            {
                bool output = false;
                //Make sure something is selected
                
                return output;
            }
        }


        public void RemoveFromCart()
        {

        }

        public bool CanCheckOut
        {
            get
            {
                bool output = false;
                //Make sure something is selected

                return output;
            }
        }


        public void CheckOut()
        {

        }


    }
}
