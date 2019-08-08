using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Caliburn.Micro;
using TRMDesktopUI.EventModels;
using TRMDesktopUILibrary.Models;

namespace TRMDesktopUI.ViewModels
{
    public class ShellViewModel : Conductor<object>,IHandle<LogOnEvent>
    {
        
        private IEventAggregator _events;
        private SalesViewModel _salesVM;
        private ILoggedInUserModel _user;
        //private SimpleContainer _container;

        public ShellViewModel(IEventAggregator events, ILoggedInUserModel user,
            SalesViewModel salesVM)//, SimpleContainer container)//constructor injection
        {
            _events = events;
            
            _salesVM = salesVM;
            //_container = container;

            _user = user;

            _events.Subscribe(this);
            
            ActivateItem(IoC.Get<LoginViewModel>()); //Do this instead of "ActivateItem(_loginVM) so that every time I have a new instance without the previous user data
            //with Inversion of Control(caliburn micro) i don't have to pass an instance of container
        }

        public void Handle(LogOnEvent message)
        {
            ActivateItem(_salesVM);
            NotifyOfPropertyChange(() => IsLoggedIn);
        }

        public bool IsLoggedIn
        {
            get
            {
                bool output = false;

                if(!string.IsNullOrWhiteSpace(_user.Token))
                {
                    output = true;
                }

                return output;
            }
        }

        public void LogOut()
        {
            _user.LogOffUser();
            ActivateItem(IoC.Get<LoginViewModel>());
            NotifyOfPropertyChange(() => IsLoggedIn);
        }

        public void ExitApplication()
        {
            TryClose(); // Closes wpf,api still runs(tried this.ExitApplication but throws exception)
        }
    }
}
