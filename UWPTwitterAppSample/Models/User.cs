using CoreTweet;
using CoreTweet.Streaming;
using Prism.Mvvm;
using System;
using System.Collections.ObjectModel;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading.Tasks;
using Windows.Security.Credentials;
using Windows.Storage;

namespace UWPTwitterAppSample.Models
{
    class User : BindableBase
    {
        private Subject<StreamingMessage> stream = new Subject<StreamingMessage>();

        private Tokens tokens;

        public bool IsAuth
        {
            get { return this.tokens != null; }
        }

        private IObservable<StreamingMessage> Stream
        {
            get { return this.stream.AsObservable(); }
        }

        private readonly ObservableCollection<StatusMessage> statusMessages;

        private readonly ReadOnlyObservableCollection<StatusMessage> readOnlyStatusMessages;
        public ReadOnlyObservableCollection<StatusMessage> StatusMessages
        {
            get { return this.readOnlyStatusMessages; }
        }

        public string ScreenName
        {
            get { return (string)ApplicationData.Current.RoamingSettings.Values["User.ScreenName"]; }
            set { ApplicationData.Current.RoamingSettings.Values["User.ScreenName"] = value; }
        }

        public bool IsFirst
        {
            get { return string.IsNullOrWhiteSpace(this.ScreenName); }
        }

        public User()
        {
            this.statusMessages = new ObservableCollection<StatusMessage>();
            this.readOnlyStatusMessages = new ReadOnlyObservableCollection<StatusMessage>(this.statusMessages);

            this.Stream.OfType<StatusMessage>().Subscribe(x => this.statusMessages.Insert(0, x));
        }

        public void LoadToken()
        {
            if (this.IsFirst) { return; }

            try
            {
                var p = new PasswordVault();
                var accessToken = p.Retrieve("UWPTwitterAppSample.AccessToken", this.ScreenName);
                var accessTokenSecret = p.Retrieve("UWPTwitterAppSample.AccessTokenSecret", this.ScreenName);

                this.tokens = Tokens.Create(
                    Constants.ConsumerKey,
                    Constants.ConsumerSecret,
                    accessToken.Password,
                    accessTokenSecret.Password);

                this.tokens.Followers.ListAsync();
                this.OnPropertyChanged(() => this.IsAuth);
            }
            catch
            {
                // ignore
            }
        }

        public void SaveToken(Tokens tokens)
        {
            this.tokens = tokens;
            this.ScreenName = this.tokens.ScreenName;
            var p = new PasswordVault();
            p.Add(new PasswordCredential("UWPTwitterAppSample.AccessToken", this.ScreenName, this.tokens.AccessToken));
            p.Add(new PasswordCredential("UWPTwitterAppSample.AccessTokenSecret", this.ScreenName, this.tokens.AccessTokenSecret));
        }

        public void Connect()
        {

            this.tokens.Streaming.UserAsObservable()
                .Subscribe(x => this.stream.OnNext(x), ex => this.stream.OnError(ex), () => this.stream.OnCompleted());
        }

        public async Task TweetAsync(string tweet)
        {
            await this.tokens.Statuses.UpdateAsync(status => tweet);
        }

        public async Task RetweetAsync(StatusMessage statusMessage)
        {
            await this.tokens.Statuses.RetweetAsync(id => statusMessage.Status.Id);
        }
    }
}
