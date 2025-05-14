using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Reactive.Bindings;
using System.Reactive.Linq;
using System.Reactive.Joins;
using Windows.Media.Control;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace PlayLook
{
    public class PropContents
    {
        public ReactiveProperty<string> Title { get; private set; }
        public ReactiveProperty<string> Artist { get; private set; }
        public ReactiveProperty<string> Album { get; private set; }
        public ReactiveProperty<GlobalSystemMediaTransportControlsSessionPlaybackStatus> Status { get; private set; }
        public ReactiveProperty<string> Current { get; }
        public ReactiveProperty<string> Icon { get; }

        public PropContents()
        {
            Title = new ReactiveProperty<string>("");
            Artist = new ReactiveProperty<string>("Unknown");
            Album = new ReactiveProperty<string>("");
            Status = new ReactiveProperty<GlobalSystemMediaTransportControlsSessionPlaybackStatus>(GlobalSystemMediaTransportControlsSessionPlaybackStatus.Closed);
            Current = new ReactiveProperty<string>("No media playing");
            Icon = new ReactiveProperty<string>("VolumeOff");
            Current = Title.CombineLatest(Artist, Album, Status, (title, artist, album, status) => GenerateCurrentStatus(title, artist, album, status)).ToReactiveProperty();
        }
        private string GenerateCurrentStatus(string title, string artist, string album, GlobalSystemMediaTransportControlsSessionPlaybackStatus status)
        {
            switch (status)
            {
                case GlobalSystemMediaTransportControlsSessionPlaybackStatus.Playing:
                    Icon.Value = "Play";
                    return SetCurrentStatus(title, artist, album);
                case GlobalSystemMediaTransportControlsSessionPlaybackStatus.Paused:
                    Icon.Value = "Pause";
                    return SetCurrentStatus(title, artist, album);
                case GlobalSystemMediaTransportControlsSessionPlaybackStatus.Stopped:
                    Icon.Value = "Stop";
                    return SetCurrentStatus(title, artist, album);
                default:
                    Icon.Value = "VolumeOff";
                    return "No media playing";

            }
        }
        private string SetCurrentStatus(string title, string artist, string album)
        {
            string currentStatus = "";
            if (artist != "Unknown")
            {
                currentStatus += $"{artist} - ";
            }
            if (title != "")
            {
                currentStatus += $"{title}";
            }
            else { return "No media playing"; }
            if (album != "")
            {
                currentStatus += $" (from {album})";
            }
            return currentStatus;
        }
    }
}
