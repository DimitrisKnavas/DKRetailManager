using AutoMapper;
using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TRMDesktopUI.Models;
using TRMDesktopUILibrary.Api;
using TRMDesktopUILibrary.Helpers;
using TRMDesktopUILibrary.Models;

namespace TRMDesktopUI.ViewModels
{
    public class SalesViewModel : Screen
    {
        private BindingList<ProductDisplayModel> _products ;
        IProductEndpoint _productEndpoint;
        IConfigHelper _configHelper;
        ISaleEndpoint _saleEndpoint;
        IMapper _mapper;

        public SalesViewModel(IProductEndpoint productEndpoint, IConfigHelper configHelper,
            ISaleEndpoint saleEndpoint, IMapper mapper)
        {
            _productEndpoint = productEndpoint;
            _configHelper = configHelper;
            _saleEndpoint = saleEndpoint;
            _mapper = mapper;
        }

        protected override async void OnViewLoaded(object view)
        {
            base.OnViewLoaded(view);
            await LoadProducts();
        }

        private async Task LoadProducts()
        {
            var productList = await _productEndpoint.GetAll();
            var products = _mapper.Map<List<ProductDisplayModel>>(productList); //Automapper -> convert from ProductModel given by the API to ProductDisplayModel
            Products = new BindingList<ProductDisplayModel>(products);
        }

        public BindingList<ProductDisplayModel> Products
        {
            get { return _products; }
            set { _products = value;
                NotifyOfPropertyChange(() => Products);
            }
        }

        private ProductDisplayModel _selectedProduct;

        public ProductDisplayModel SelectedProduct
        {
            get { return _selectedProduct;; }
            set { _selectedProduct = value;
                  NotifyOfPropertyChange(() => SelectedProduct);
                  NotifyOfPropertyChange(() => CanAddToCart);
            }
        }

        private CartItemDisplayModel _selectedProductToRemove;

        public CartItemDisplayModel SelectedProductToRemove
        {
            get { return _selectedProductToRemove; }
            set { _selectedProductToRemove = value;
                  NotifyOfPropertyChange(() => SelectedProductToRemove);
                  NotifyOfPropertyChange(() => CanCheckOut);
                  NotifyOfPropertyChange(() => CanRemoveFromCart);
                }
        }



        private BindingList<CartItemDisplayModel> _cart = new BindingList<CartItemDisplayModel>();

        public BindingList<CartItemDisplayModel> Cart
        {
            get { return _cart; }
            set { _cart = value;
                NotifyOfPropertyChange(() => Cart);

            }
        }

        public string SubTotal
        {
            get
            {
                return CalculateSubTotal().ToString("C"); //passing the C convertes it to currency (2 decimal points)
            }
            
        }

        private decimal CalculateSubTotal()
        {
            decimal subTotal = 0;

            foreach (var item in Cart)
            {
                subTotal += (item.Product.RetailPrice * item.QuantityInCart);
            }

            return subTotal;
        }

        public string Tax
        {
            get
            {
                return CalcucalateTax().ToString("C");
            }

        }

        private decimal CalcucalateTax()
        {
            decimal taxAmount = 0;
            decimal taxRate = _configHelper.GetTaxRate()/100;

            taxAmount = Cart
                .Where(x => x.Product.IsTaxable)
                .Sum(x => x.Product.RetailPrice * x.QuantityInCart * taxRate);
            
            //foreach (var item in Cart)
            //{
            //    if (item.Product.IsTaxable)
            //    {
            //        taxAmount += (item.Product.RetailPrice * item.QuantityInCart * taxRate);
            //    }
            //}

            return taxAmount;
        }

        public string Total
        {
            get
            {
                decimal total = CalculateSubTotal() + CalcucalateTax();
                return total.ToString("C");
            }

        }



        private int _itemQuantity = 1;

        public int ItemQuantity
        {
            get { return _itemQuantity; }
            set { _itemQuantity = value;
                  NotifyOfPropertyChange(() => ItemQuantity);
                  NotifyOfPropertyChange(() => CanAddToCart);
                }
        }

        public void AddToCart()
        {
            CartItemDisplayModel existingItem = Cart.FirstOrDefault(x => x.Product == SelectedProduct);

            if(existingItem != null)
            {
                existingItem.QuantityInCart += ItemQuantity;
                ////HACK - There should be a better way of refreshing the cart display
                //Cart.Remove(existingItem);
                //Cart.Add(existingItem);
            }
            else
            {
                CartItemDisplayModel item = new CartItemDisplayModel
                {
                    Product = SelectedProduct,
                    QuantityInCart = ItemQuantity
                };
                Cart.Add(item);
            }

            SelectedProduct.QuantityInStock -= ItemQuantity;
            ItemQuantity = 1;
            NotifyOfPropertyChange(() => SubTotal);
            NotifyOfPropertyChange(() => Tax);
            NotifyOfPropertyChange(() => Total);
            NotifyOfPropertyChange(() => CanCheckOut);
        }

        public bool CanAddToCart
        {
            get
            {
                bool output = false;
                //Make sure something is selected
                //Make sure there is an item quantity
                if(ItemQuantity > 0 && SelectedProduct?.QuantityInStock >= ItemQuantity)
                {
                    output = true;
                }
                
                return output;
            }
        }

        public bool CanRemoveFromCart
        {
            get
            {
                bool output = false;
                if (SelectedProductToRemove != null)
                {
                    output = true;
                }
                return output;
            }
        }



        public void RemoveFromCart()
        {
            CartItemDisplayModel existingItem = Cart.FirstOrDefault(x => x.Product == SelectedProductToRemove.Product);

            
            existingItem.QuantityInCart -= ItemQuantity;
            //HACK - There should be a better way of refreshing the cart display
            Cart.Remove(existingItem);
            if (existingItem.QuantityInCart > 0)
            {
                Cart.Add(existingItem);
            }


            SelectedProduct.QuantityInStock += ItemQuantity;
            ItemQuantity = 1;
            NotifyOfPropertyChange(() => SubTotal);
            NotifyOfPropertyChange(() => Tax);
            NotifyOfPropertyChange(() => Total);
            NotifyOfPropertyChange(() => CanCheckOut);
        }

        public bool CanCheckOut
        {
            get
            {
                bool output = false;
                //Make sure something is selected
                if(Cart.Count > 0)
                {
                    output = true;
                }

                return output;
            }
        }


        public async Task CheckOut()
        {
            //Create a SaleModel and post to the API
            SaleModel sale = new SaleModel();
            foreach (var item in Cart)
            {
                sale.SaleDetails.Add(new SaleDetailModel
                {
                    ProductId = item.Product.Id,
                    Quantity = item.QuantityInCart
                });
            }

            await _saleEndpoint.PostSale(sale);
        }


    }
}
