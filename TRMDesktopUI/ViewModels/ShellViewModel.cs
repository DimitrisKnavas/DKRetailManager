using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Caliburn.Micro;
using TRMDesktopUI.EventModels;

namespace TRMDesktopUI.ViewModels
{
    public class ShellViewModel : Conductor<object>,IHandle<LogOnEvent>
    {
        
        private IEventAggregator _events;
        private SalesViewModel _salesVM;
        //private SimpleContainer _container;

        public ShellViewModel(IEventAggregator events, SalesViewModel salesVM)//, SimpleContainer container)//constructor injection
        {
            _events = events;
            
            _salesVM = salesVM;
            //_container = container;

            _events.Subscribe(this);
            
            ActivateItem(IoC.Get<LoginViewModel>()); //Do this instead of "ActivateItem(_loginVM) so that every time I have a new instance without the previous user data
            //with Inversion of Control(caliburn micro) i don't have to pass an instance of container
        }

        public void Handle(LogOnEvent message)
        {
            ActivateItem(_salesVM);
        }
    }
}
