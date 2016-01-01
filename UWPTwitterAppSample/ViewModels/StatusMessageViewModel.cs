using Prism.Commands;
using Prism.Windows.AppModel;
using Prism.Windows.Mvvm;
using Prism.Windows.Navigation;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reactive.Linq;
using UWPTwitterAppSample.Models;
using Windows.Security.Authentication.Web;
using Windows.UI.Xaml.Navigation;
using CoreTweet.Streaming;

namespace UWPTwitterAppSample.ViewModels
{
    public class StatusMessageViewModel : ViewModelBase
    {
        public StatusMessage Model { get; private set; }

        public ReactiveProperty<string> ProfileImage { get; private set; }

        public ReactiveProperty<string> ScreenName { get; private set; }

        public ReactiveProperty<string> Text { get; private set; }

        public StatusMessageViewModel()
        {
        }

        public StatusMessageViewModel(StatusMessage statusMessage)
        {
            this.Model = statusMessage;

            this.ProfileImage = ReactiveProperty.FromObject(
                statusMessage.Status.User,
                x => x.ProfileImageUrlHttps);
            this.ScreenName = ReactiveProperty.FromObject(
                statusMessage.Status.User,
                x => x.ScreenName);
            this.Text = ReactiveProperty.FromObject(
                statusMessage.Status,
                x => x.Text);
        }

    }
}
