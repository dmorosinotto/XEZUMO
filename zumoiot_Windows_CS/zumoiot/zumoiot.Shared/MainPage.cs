using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.MobileServices;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using Quobject.SocketIoClientDotNet.Client;
using Newtonsoft.Json.Linq;

namespace zumoiot
{
    sealed partial class MainPage: Page
    {
        private IMobileServiceTable<IOTData> IOTData = App.MobileService.GetTable<IOTData>();
        private MobileServiceCollection<IOTData,IOTData> _items;
        private MobileServiceUser _curruser;
        
        private Socket io = IO.Socket(App.MobileService.ApplicationUri);
        public MainPage()
        {
            this.InitializeComponent();
            io.On("logiot",async msg => await RealtimeUpdate(msg)); 
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            await RefreshItems();
        }

        private async Task RefreshItems()
        {
            await asyncCallAndShowErr(async () =>
            {
                _items = await IOTData
                    .OrderByDescending(itm => itm.When)
                    .ToCollectionAsync();
                ListItems.ItemsSource = _items;
            }, "Error Reading");
        }

        private async Task RealtimeUpdate(object msg)
        {
            await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                if (msg.GetType().Name == "String") {
                    TextInput.Text = ">> " + msg.ToString();
                    //await new MessageDialog(msg.ToString(), "Notifica realtime socket.io").ShowAsync();
                } else {
                    IOTData data = ((JObject)msg).ToObject<IOTData>();
                    _items.Insert(0, data);
                }
            });
        }

        private Random rnd = new Random();
        private async void ButtonSave_Click(object sender, RoutedEventArgs e)
        {
            await asyncCallAndShowErr(async () => {
                var itm = new IOTData { DeviceLocation = TextInput.Text, Temperature = rnd.Next(10, 30), Umidity = rnd.NextDouble() * 100 };
                await IOTData.InsertAsync(itm); //L'interfaccia viene aggiornata dal RealTimeUpdate tramite Socket.IO
                TextInput.Text = "";
            },"Error Inserting");
        }

        private async void CheckBoxDelete_Checked(object sender, RoutedEventArgs e)
        {
            await asyncCallAndShowErr(async () =>
            {
                var itm = ((CheckBox)sender).DataContext as IOTData;
                await IOTData.DeleteAsync(itm);
                _items.Remove(itm);
            }, "Error Deleting");
        }

        private async void AppBarButtoAuth_Click(object sender, RoutedEventArgs e)
        {   //TODO LOGIN con AAD
            string message = null;
            try {
                if (_curruser == null) {
                    _curruser = await App.MobileService.LoginAsync(MobileServiceAuthenticationProvider.WindowsAzureActiveDirectory);
                    message = "Welcome " + _curruser.UserId;
                } else {
                    App.MobileService.Logout();
                    _curruser = null;
                    message = "You are Logged OFF!";
                }
            } catch (Exception ex) {
                message = "Error Auth:" + ex.Message; 
            }
            await new MessageDialog(message).ShowAsync();
            //Cambia l'icona in base allo stato del l'utente corrente
            AppBarButtonAuth.Label = (_curruser == null ? "Log IN" : "Log OUT");
            AppBarButtonAuth.Icon = new SymbolIcon((_curruser == null ? Symbol.Contact : Symbol.BlockContact));
            await RefreshItems();
        }

        private async void ButtonWipe_Click(object sender, RoutedEventArgs e)
        {
            if (_curruser == null) {
                await new MessageDialog("Devi essere Loggato per eseguire WIPEDATA!").ShowAsync();
            } else {
                try {
                    await App.MobileService.InvokeApiAsync("wipedata",System.Net.Http.HttpMethod.Delete,null);
                } catch {}
            }
        }

        private async Task<bool> asyncCallAndShowErr(Func<Task> doCall, string exTitle) {
            string errmsg = null; 
            try {
                await doCall(); 
            } catch (Exception ex) {
                errmsg = ex.Message;
            }
            if (errmsg != null) await new MessageDialog(errmsg, exTitle).ShowAsync();
            return (errmsg==null);
        }
    }
}
