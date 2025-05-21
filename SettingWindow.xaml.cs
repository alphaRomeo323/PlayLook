using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using MaterialDesignThemes.Wpf;

namespace PlayLook
{
    /// <summary>
    /// SettingWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class SettingWindow : Window
    {
        private bool first = true;
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public SettingWindow()
        {
            InitializeComponent();
            this.DataContext = Settings.Default;
            if(Settings.Default.BaseTheme == "Dark")
            {
                ThemeListBox.SelectedIndex = 1;
            }else
            {
                ThemeListBox.SelectedIndex = 0;
            }
        }

        /// <summary>
        /// テキストボックスのフォーカスを外す
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (Keyboard.FocusedElement is DependencyObject focusedElement)
            {
                FocusManager.SetFocusedElement(FocusManager.GetFocusScope(focusedElement), null);
            }
            Focus();
        }
        /// <summary>
        /// テーマの変更
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Theme_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (first)
            {
                first = false;
                return;
            }
            var Item = ((System.Windows.Controls.ListBox)sender).SelectedItem;
            Settings.Default.BaseTheme = ((System.Windows.Controls.ListBoxItem)Item).Content.ToString();
            Settings.Default.Save();
            var paletteHelper = new PaletteHelper();
            var theme = paletteHelper.GetTheme();
            if (Settings.Default.BaseTheme == "Dark")
            {
                theme.SetBaseTheme(BaseTheme.Dark);
            }
            else
            {
                theme.SetBaseTheme(BaseTheme.Light);
            }
            paletteHelper.SetTheme(theme);
        }
        /// <summary>
        /// ウィンドウを閉じる際に設定を保存
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Settings.Default.Save();
        }

    }

}
