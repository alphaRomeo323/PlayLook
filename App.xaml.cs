using System.Windows;
using MaterialDesignThemes.Wpf;

namespace PlayLook
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : System.Windows.Application
    {
        private PropView? prop;
        private SettingWindow? wnd;
        /// <summary>
        /// アプリケーションのエントリポイント。
        /// </summary>
        /// <param name="e"></param>
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            
            // テーマの初期設定を行う
            var paletteHelper = new PaletteHelper();
            var theme = paletteHelper.GetTheme();
            if (Settings.Default.BaseTheme =="Dark")
            {
                theme.SetBaseTheme(BaseTheme.Dark);
            }
            else
            {
                theme.SetBaseTheme(BaseTheme.Light);
            }
            paletteHelper.SetTheme(theme);

            // タスクトレイにアイコンを表示
            var icon = GetContentStream(new Uri("icon.ico", UriKind.Relative)).Stream;
            var menu = new System.Windows.Forms.ContextMenuStrip();
            menu.Items.Add("設定", null, onClick: Main_Click);
            menu.Items.Add("終了", null, onClick: Exit_Click);
            var notifyIcon = new System.Windows.Forms.NotifyIcon
            {
                Visible = true,
                Icon = new System.Drawing.Icon(icon),
                Text = "PlayLook",
                ContextMenuStrip = menu
            };
            notifyIcon.MouseClick += new System.Windows.Forms.MouseEventHandler(NotifyIcon_Click);

            // プロパティウィンドウの初期化
            prop = new PropView();
            if (Settings.Default.FirstStartup)
            {
                Settings.Default.FirstStartup = false;
                Settings.Default.WindowLeft = SystemParameters.PrimaryScreenWidth - prop.Width;
                Settings.Default.WindowTop = SystemParameters.PrimaryScreenHeight - prop.Height - 40;
                Settings.Default.Save();
            }
            prop.Left = Settings.Default.WindowLeft;
            prop.Top = Settings.Default.WindowTop;
            prop.Show();
            prop.Closing += (s, args) =>
            {
                args.Cancel = true;
                prop.Hide();
            };
        }

        /// <summary>
        /// タスクトレイアイコンのクリックイベント
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void NotifyIcon_Click(object? sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                prop?.Show();
            }
        }

        /// <summary>
        /// アプリケーションを終了するためのイベントハンドラ
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Exit_Click(object? sender, EventArgs e)
        {
            if (prop != null)
            {
                Settings.Default.WindowLeft = prop.Left;
                Settings.Default.WindowTop = prop.Top;
                Settings.Default.Save();
            }
            Shutdown();
        }

        /// <summary>
        /// 設定ウィンドウを表示するためのイベントハンドラ
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Main_Click(object? sender, EventArgs e)
        {
            if (wnd == null)
            {
                wnd = new SettingWindow();
                wnd.Closed += (s, args) =>
                {
                    wnd = null;
                };
            }
            wnd.Show();
        }
    }
}
