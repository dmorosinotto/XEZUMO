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
        private MobileServiceCollection<TodoItem, TodoItem> items;
        private IMobileServiceTable<TodoItem> todoTable = App.MobileService.GetTable<TodoItem>();
        private Socket io = IO.Socket(App.MobileService.ApplicationUri);

        private struct Idata
        {
            public string id;
            public string hwid;
            public double temp;
            public double umid;
            public DateTime when;
            public string ToString()
            {
#if WINDOWS_PHONE_APP
                return string.Format("T: {1,2}° - U: {2,3}% | {0}", hwid, temp, umid);
#else //WINDOWS_APP
                return string.Format("Temp: {1,2}° , Umid: {2,3}% | {0}", hwid, temp, umid);
#endif
            }
        }

        public MainPage()
        {
            this.InitializeComponent();
            io.On("logiot", async msg =>
            {
                await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                {
                    if (msg.GetType().Name == "String")
                    {
                        TextInput.Text = ">> " + msg.ToString();
                    }
                    else
                    {
                        Idata data = ((JObject)msg).ToObject<Idata>();
                        items.Insert(0, new TodoItem() { Text = data.ToString(), Complete = false, Id = data.id });
                    }
                    //await new MessageDialog(msg.ToString(), "Notifica realtime socket.io").ShowAsync();
                });
            });
        }

        private async Task InsertTodoItem(TodoItem todoItem)
        {
            // This code inserts a new TodoItem into the database. When the operation completes
            // and Mobile Services has assigned an Id, the item is added to the CollectionView
            await todoTable.InsertAsync(todoItem);
            items.Add(todoItem);
        }

        private async Task RefreshTodoItems()
        {
            MobileServiceInvalidOperationException exception = null;
            try
            {
                // This code refreshes the entries in the list view by querying the TodoItems table.
                // The query excludes completed TodoItems
                items = await todoTable
                    .Where(todoItem => todoItem.Complete == false)
                    .ToCollectionAsync();
            }
            catch (MobileServiceInvalidOperationException e)
            {
                exception = e;
            }

            if (exception != null)
            {
                await new MessageDialog(exception.Message, "Error loading items").ShowAsync();
            }
            else
            {
                ListItems.ItemsSource = items;
                this.ButtonSave.IsEnabled = true;
            }
        }

        private async Task UpdateCheckedTodoItem(TodoItem item)
        {
            // This code takes a freshly completed TodoItem and updates the database. When the MobileService 
            // responds, the item is removed from the list 
            await todoTable.UpdateAsync(item);
            items.Remove(item);
            ListItems.Focus(Windows.UI.Xaml.FocusState.Unfocused);
        }

        private async void ButtonRefresh_Click(object sender, RoutedEventArgs e)
        {
            await RefreshTodoItems();
        }

        private async void ButtonSave_Click(object sender, RoutedEventArgs e)
        {
            var todoItem = new TodoItem { Text = TextInput.Text };
            await InsertTodoItem(todoItem);
        }

        private async void CheckBoxComplete_Checked(object sender, RoutedEventArgs e)
        {
            CheckBox cb = (CheckBox)sender;
            TodoItem item = cb.DataContext as TodoItem;
            await UpdateCheckedTodoItem(item);
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            await RefreshTodoItems();
        }
    }
}
