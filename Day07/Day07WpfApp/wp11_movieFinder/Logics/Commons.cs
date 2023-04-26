using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace wp11_MovieFinder.Logics
{
    public class Commons
    {

        public static readonly string connString = "Data Source=localhost;" +
                                                   "Initial Catalog=pknu;" +
                                                   "Persist Security Info=True;" +
                                                   "User ID=sa;" +
                                                   "Password=12345";
        public static async Task<MessageDialogResult> ShowMessageAsync(string title, string message,
            MessageDialogStyle style = MessageDialogStyle.Affirmative)
        {
            return await ((MetroWindow)Application.Current.MainWindow).ShowMessageAsync(title, message, style, null);
        }
    }
}