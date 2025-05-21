using Reactive.Bindings;
using System.Reactive.Linq;
using Windows.Media.Control;

namespace PlayLook
{
    /// <summary>
    /// プロパティの内容を管理するクラス
    /// </summary>
    public class PropContents
    {
        
        public ReactiveProperty<string> Title { get; private set; }
        public ReactiveProperty<string> Artist { get; private set; }
        public ReactiveProperty<string> Album { get; private set; }
        public ReactiveProperty<GlobalSystemMediaTransportControlsSessionPlaybackStatus> Status { get; private set; }
        public ReactiveProperty<string> Current { get; }
        public ReactiveProperty<string> Icon { get; }
        private string defaltString = "No media playing";
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public PropContents()
        {
            Title = new ReactiveProperty<string>("");
            Artist = new ReactiveProperty<string>("Unknown");
            Album = new ReactiveProperty<string>("");
            Status = new ReactiveProperty<GlobalSystemMediaTransportControlsSessionPlaybackStatus>(GlobalSystemMediaTransportControlsSessionPlaybackStatus.Closed);
            Current = new ReactiveProperty<string>("No media playing");
            Icon = new ReactiveProperty<string>("VolumeOff");
            Current = Title.CombineLatest(Artist, Album, Status, GenerateCurrentStatus).ToReactiveProperty<string>();
        }

        /// <summary>
        /// PropViewに表示する情報を更新する
        /// </summary>
        /// <param name="title"></param>
        /// <param name="artist"></param>
        /// <param name="album"></param>
        /// <param name="status"></param>
        /// <returns></returns>
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
                    return defaltString;

            }
        }

        /// <summary>
        /// PropViewに表示するテキストを生成する
        /// </summary>
        /// <param name="title"></param>
        /// <param name="artist"></param>
        /// <param name="album"></param>
        /// <returns></returns>
        private string SetCurrentStatus(string title, string artist, string album)
        {
            string currentStatus = "";
            if ( !( artist == "Unknown" || Settings.Default.OnlyTitle ) )
            {
                currentStatus += $"{artist} - ";
            }
            if (title != "")
            {
                currentStatus += $"{title}";
            }
            else { return defaltString; }
            if ( !( album == "" || Settings.Default.OnlyTitle) )
                {
                currentStatus += $" (from {album})";
            }
            return currentStatus;
        }
    }
}
