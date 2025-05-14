using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Media.Control;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace PlayLook
{
    class SMTCSessionManager
    {
        private GlobalSystemMediaTransportControlsSession? session;
        private GlobalSystemMediaTransportControlsSessionManager manager = GlobalSystemMediaTransportControlsSessionManager.RequestAsync().GetAwaiter().GetResult();

        public PropContents propContents;
        public SMTCSessionManager()
        {
            propContents = new PropContents();
            session = manager.GetCurrentSession();
            if (session != null)
            {
                getMediaPropaties();
                getMediaStatus();
                session.PlaybackInfoChanged += (s, e) =>
                {
                    getMediaStatus();
                };
                session.MediaPropertiesChanged += (s, e) =>
                {
                    getMediaPropaties();
                };
            }
            manager.CurrentSessionChanged += (s, e) =>
            {
                session = manager.GetCurrentSession();
                if (session != null)
                {
                    getMediaPropaties();
                    getMediaStatus();
                    session.PlaybackInfoChanged += (s, e) =>
                    {
                        getMediaStatus();

                    };
                    session.MediaPropertiesChanged += (s, e) =>
                    {
                        getMediaPropaties();
                    };
                }
            };
        }
        private void getMediaPropaties()
        {
            GlobalSystemMediaTransportControlsSessionMediaProperties prop;
            try
            {
                prop = session.TryGetMediaPropertiesAsync().GetAwaiter().GetResult();
                propContents.Title.Value = prop.Title;
                if (prop.Artist != null)
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

        private void getMediaStatus()
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

        public async void TryControl()
        {
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
