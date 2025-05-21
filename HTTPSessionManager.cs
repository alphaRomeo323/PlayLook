using System.Text;
using System.Text.Json;
using Windows.Media.Control;
using System.Net.Http;
using System.Text.Encodings.Web;
using System.Text.Unicode;

namespace PlayLook
{
    /// <summary>
    /// HTTPセッションを管理するクラス
    /// </summary>
    class HTTPSessionManager
    {
        private PropContents propContents;
        private bool cooldown = false;
        int errorCount = 0;
        const int MAX_ERROR_COUNT = 5;
        const int REPEAT_TIME = 5 * 60 * 1000; // 5分
        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="p">すでに初期化されたPropContents</param>
        public HTTPSessionManager(PropContents p)
        {
            propContents = p;
            // プロパティの変更を監視する
            p.Title.Subscribe((s) =>
            {
                PropChangeHandler();
            });
            p.Status.Subscribe((s) =>
            {
                PropChangeHandler();
            });
            //接続維持のために定期的にHTTP接続を行う
            System.Timers.Timer timer = new(REPEAT_TIME);
            timer.Elapsed += (s, e) =>
            {
                if (Settings.Default.HTTPConection)
                {
                    PropChangeHandler();
                }
            };
            timer.AutoReset = true;
            timer.Enabled = true;
            timer.Start();
        }

        /// <summary>
        /// プロパティの変更を監視し、HTTP接続を行う
        /// </summary>
        private void PropChangeHandler()
        {
            if (!Settings.Default.HTTPConection)
            {
                return;
            }
            if (cooldown)
            {
                return;
            }
            cooldown = true;
            Task.Run(async () =>
            {
                bool success = await PostToServer();
                await Task.Delay(1000);
                cooldown = false;
                if (!success)
                {
                    Console.WriteLine("HTTP接続に失敗しました。");
                    errorCount++;
                    if (errorCount >= MAX_ERROR_COUNT)
                    {
                        Settings.Default.HTTPConection = false;
                        Settings.Default.Save();
                        System.Windows.MessageBox.Show("HTTP接続が5回失敗したため、HTTP接続を無効にしました。", "エラー", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
                    }
                }
                else
                {
                    errorCount = 0;
                }
            });
        }

        /// <summary>
        /// HTTPサーバーにPOSTリクエストを送信する
        /// </summary>
        /// <returns>HTTPサーバーからのステータスコードが2xxなら true</returns>
        private async Task<bool> PostToServer()
        {
            string url = Settings.Default.PostDest;
            using (var client = new HttpClient())
            {
                
                var dic = new Dictionary<string, string>();
                dic.Add("title", propContents.Title.Value);
                dic.Add("artist", propContents.Artist.Value);
                dic.Add("album", propContents.Album.Value);
                switch (propContents.Status.Value)
                {
                    case GlobalSystemMediaTransportControlsSessionPlaybackStatus.Playing:
                        dic.Add("state", "1");
                        break;
                    case GlobalSystemMediaTransportControlsSessionPlaybackStatus.Paused:
                        dic.Add("state", "2");
                        break;
                    default:
                        dic.Add("state", "0");
                        break;
                }
                var content = new StringContent(JsonSerializer.Serialize(dic, GetOption()), Encoding.UTF8, "application/json");
                var response = await client.PostAsync(url, content);
                return response.IsSuccessStatusCode;
            }
        }

        /// <summary>
        /// JSONシリアライザのオプションを生成する
        /// </summary>
        /// <returns></returns>
        private static JsonSerializerOptions GetOption()
        {
            var options = new JsonSerializerOptions
            {
                Encoder = JavaScriptEncoder.Create(UnicodeRanges.All),
                WriteIndented = true,
            };
            return options;
        }
    }
}
