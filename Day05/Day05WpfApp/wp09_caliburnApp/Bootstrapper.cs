using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using wp09_caliburnApp.ViewModels;

namespace wp09_caliburnApp
{
    public class Bootstrapper :BootstrapperBase
    {
        public Bootstrapper()
        {
            Initialize();   // 초기화
        }

        protected override async void OnStartup(object sender, StartupEventArgs e)
        {
            // base.OnStartup(sender, e);
            await DisplayRootViewForAsync<MainViewModel>();
        }
    }
}
