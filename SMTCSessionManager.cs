using Windows.Media.Control;

namespace PlayLook
{
    /// <summary>
    /// GlobalSystemMediaTransportControlsSessionを保持するクラス
    /// </summary>
    class SMTCSessionManager
    {
        private GlobalSystemMediaTransportControlsSession? session;
        private GlobalSystemMediaTransportControlsSessionManager manager = GlobalSystemMediaTransportControlsSessionManager.RequestAsync().GetAwaiter().GetResult();

        public PropContents propContents;
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public SMTCSessionManager()
        {
            propContents = new PropContents();
            session = manager.GetCurrentSession();
            if (session != null)
            {
                getMediaPropaties(session);
                getMediaStatus(session);
                session.PlaybackInfoChanged += (s, e) =>
                {
                    getMediaStatus(s);
                };
                session.MediaPropertiesChanged += (s, e) =>
                {
                    getMediaPropaties(s);
                };
            }
            manager.CurrentSessionChanged += (s, e) =>
            {
                session = manager.GetCurrentSession();
                if (session != null)
                {
                    getMediaPropaties(session);
                    getMediaStatus(session);
                    session.PlaybackInfoChanged += (s, e) =>
                    {
                        getMediaStatus(s);

                    };
                    session.MediaPropertiesChanged += (s, e) =>
                    {
                        getMediaPropaties(s);
                    };
                }
            };
        }

        /// <summary>
        /// メディアのプロパティを取得する
        /// </summary>
        /// <param name="session">イベントから渡されるSession</param>
        private void getMediaPropaties(GlobalSystemMediaTransportControlsSession session)
        {
            GlobalSystemMediaTransportControlsSessionMediaProperties prop;
            try
            {
                prop = session.TryGetMediaPropertiesAsync().GetAwaiter().GetResult();
                propContents.Title.Value = prop.Title;
                if (prop.Artist == null || prop.Artist == "")
                {
                    propContents.Artist.Value = "Unknown";
                }
                else
                {
                    propContents.Artist.Value = prop.Artist;
                }
                if (prop.AlbumTitle != null)
                {
                    propContents.Album.Value = prop.AlbumTitle;
                }
            }
            catch
            {
                propContents.Title.Value = "";
                propContents.Artist.Value = "Unknown";
                propContents.Album.Value = "";
                propContents.Status.Value = 0;
            }

        }

        /// <summary>
        /// メディアのステータスを取得する
        /// </summary>
        /// <param name="session">イベントから渡されるSession</param>
        private void getMediaStatus(GlobalSystemMediaTransportControlsSession session)
        {
            try
            {
                propContents.Status.Value = session.GetPlaybackInfo().PlaybackStatus;
            }
            catch
            {
                propContents.Status.Value = 0;
            }
            switch (propContents.Status.Value)
            {
                case GlobalSystemMediaTransportControlsSessionPlaybackStatus.Playing:
                case GlobalSystemMediaTransportControlsSessionPlaybackStatus.Paused:
                case GlobalSystemMediaTransportControlsSessionPlaybackStatus.Stopped:
                    break;
                default:
                    propContents.Title.Value = "";
                    propContents.Artist.Value = "Unknown";
                    propContents.Album.Value = "";
                    break;
            }

        }

        /// <summary>
        /// 再生・一時停止を試行する
        /// </summary>
        public async void TryControl()
        {
            if(session == null)
            {
                return;
            }
            switch (propContents.Status.Value)
            {
                case GlobalSystemMediaTransportControlsSessionPlaybackStatus.Playing:
                    try
                    {
                        await session.TryPauseAsync();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                    return;
                case GlobalSystemMediaTransportControlsSessionPlaybackStatus.Paused:
                case GlobalSystemMediaTransportControlsSessionPlaybackStatus.Stopped:
                    try
                    {
                        await session.TryPlayAsync();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                    return;
                default:
                    return;
            }


        }
    }
}
