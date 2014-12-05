using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.MobileServices;
using Newtonsoft.Json.Linq;

// http://go.microsoft.com/fwlink/?LinkId=290986&clcid=0x409

namespace zumoiot
{
    internal class zumoiotPush
    {
        const string toast = @"<toast><visual><binding template=""ToastText01""><text id=""1"">{0}</text></binding></visual></toast>";
        public async static void UploadChannel()
        {
            //TODO PUSH: Unificato registrazione (invio Channel) all'Hub
            var channel = await Windows.Networking.PushNotifications.PushNotificationChannelManager.CreatePushNotificationChannelForApplicationAsync();
            string[] tags = { "" }; string template;
            try
            {
#if WINDOWS_PHONE_APP
            tags[0] = "WP";
            template = String.Format(toast,"{'T: ' + $(tmin, 2) + ' / ' + $(tmax, 2) + ' U: ' + $(umed, 5) + '% | ' + .(name, 10)}");
#else //WINDOWS_APP
                tags[0] = "WIN";
                template = String.Format(toast, "{'A ' + $(name) + ' Temp min: ' + $(tmin) + ' max: ' + $(tmax) + ' - Umid: ' + $(umed) + '%'}");
#endif
                //await App.MobileService.GetPush().UnregisterAllAsync(channel.Uri);  //Questo serve per deregistare le notifiche
                //Registro sia in nativo (per usare WNS)
                await App.MobileService.GetPush().RegisterNativeAsync(channel.Uri, tags);
                // ma anche il Template (per usare Notification Hub)
                await App.MobileService.GetPush().RegisterTemplateAsync(channel.Uri, template, "infos", tags);

                //await App.MobileService.InvokeApiAsync("notifyAllUsers",
                //    new JObject(new JProperty("toast", "Sample Toast")));
            }
            catch (Exception exception)
            {
                HandleRegisterException(exception);
            }
        }

        private async static void HandleRegisterException(Exception exception)
        {
            await new Windows.UI.Popups.MessageDialog(exception.ToString(), "Push Registration Error").ShowAsync();
        }
    }
}
