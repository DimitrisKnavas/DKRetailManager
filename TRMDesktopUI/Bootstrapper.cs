using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using TRMDesktopUI.Helpers;
using TRMDesktopUI.ViewModels;
using TRMDesktopUILibrary.Api;
using TRMDesktopUILibrary.Models;

namespace TRMDesktopUI
{
    public class Bootstrapper : BootstrapperBase
    {
        //handle the instantiation of almost all classes (dependency injection)
        private SimpleContainer _container = new SimpleContainer();

        public Bootstrapper()
        {
            Initialize();

            ConventionManager.AddElementConvention<PasswordBox>(
            PasswordBoxHelper.BoundPasswordProperty,
            "Password",
            "PasswordChanged");
        }

        //where the instantiation happens....where does the container know what to connect to what
        protected override void Configure()
        {
            _container.Instance(_container)
                .PerRequest<IProductEndpoint, ProductEndpoint>();//_container holds an instance of itself to pass when a simplecontainer is asked

            //Singleton:create one instance of the class for the life of an application
            _container
                .Singleton<IWindowManager, WindowManager>()      //ties the interface to the implementation
                .Singleton<IEventAggregator, EventAggregator>()
                .Singleton<ILoggedInUserModel, LoggedInUserModel>()
                .Singleton<IAPIHelper, APIHelper>();

            //handle viewmodels to the views
            GetType().Assembly.GetTypes()    //get every type in the entire application
               .Where(type => type.IsClass)  //limit that where the type is a class..only class types
               .Where(type => type.Name.EndsWith("ViewModel"))  //limit that where the name of the class ends with viewmodel
               .ToList()   // we need to turn Ienum to list in order to use foreach
               .ForEach(viewModelType => _container.RegisterPerRequest(viewModelType, viewModelType.ToString(), viewModelType));
        }

        protected override void OnStartup(object sender, StartupEventArgs e)
        {
            DisplayRootViewFor<ShellViewModel>();
        }

        protected override object GetInstance(Type service, string key)
        {
            return _container.GetInstance(service, key);
        }

        protected override IEnumerable<object> GetAllInstances(Type service)
        {
            return _container.GetAllInstances(service);
        }

        protected override void BuildUp(object instance)
        {
            _container.BuildUp(instance);
        }
    }
}
