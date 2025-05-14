using System.Configuration;
using System.Data;
using System.Security.Cryptography.X509Certificates;
using System.Windows;

namespace PlayLook
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : System.Windows.Application
    {
        private PropView prop;

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            var icon = GetResourceStream(new Uri("Playlook.ico", UriKind.Relative)).Stream;
            var menu = new System.Windows.Forms.ContextMenuStrip();
            menu.Items.Add("設定", null, Main_Click);
            menu.Items.Add("終了", null, Exit_Click);
            var notifyIcon = new System.Windows.Forms.NotifyIcon
            {
                Visible = true,
                Icon = new System.Drawing.Icon(icon),
                Text = "PlayLook",
                ContextMenuStrip = menu
            };
            notifyIcon.MouseClick += new System.Windows.Forms.MouseEventHandler(NotifyIcon_Click);

            prop = new PropView();
            var dispWidth = SystemParameters.PrimaryScreenWidth;
            var appWidth = prop.Width;
            var dispHeight = SystemParameters.PrimaryScreenHeight;
            var appHeight = prop.Height;
            prop.Left = dispWidth - appWidth;
            prop.Top = dispHeight - appHeight - 40;
            prop.Show();
            prop.Closing += (s, args) =>
            {
                args.Cancel = true;
                prop.Hide();
            };
        }

        private void NotifyIcon_Click(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                
                prop.Show();
            }
        }

        private void Exit_Click(object sender, EventArgs e)
        {
            Shutdown();
        }

        private void Main_Click(object sender, EventArgs e)
        {
            var wnd = new MainWindow();
            wnd.Show();
        }
    }

}
