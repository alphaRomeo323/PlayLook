using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using static System.Windows.Forms.LinkLabel;

namespace PlayLook
{
    /// <summary>
    /// PropView.xaml の相互作用ロジック
    /// </summary>
    public partial class PropView : Window
    {
        private SMTCSessionManager sessionManager;
        private Storyboard storyBoard;
        public PropView()
        {
            InitializeComponent();
            sessionManager = new SMTCSessionManager();
//storyBoard = (Storyboard?)FindResource("OverFlowTitleAnimation");
            this.DataContext = sessionManager.propContents;
        }
        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }
        
        private void Collapse_Click(object sender, RoutedEventArgs e)
        {
            this.Hide();
        }

        private void Control_Click(object sender, RoutedEventArgs e)
        {
            sessionManager.TryControl();
        }

        private async void TargetUpdatedHandler(object sender, DataTransferEventArgs args)
        {
            await Task.Delay(1000);
            if (storyBoard != null)
            {
                storyBoard.Stop(this);
            }
            if (current.ActualWidth > 480)
            {
                DoubleAnimationUsingKeyFrames transformAnimation =
                     new DoubleAnimationUsingKeyFrames();
                transformAnimation.Duration = TimeSpan.FromSeconds(30);
                transformAnimation.KeyFrames.Add(
                    new LinearDoubleKeyFrame(0, KeyTime.FromTimeSpan(TimeSpan.FromSeconds(10))));
                transformAnimation.KeyFrames.Add(
                    new LinearDoubleKeyFrame(480 - current.ActualWidth, KeyTime.FromTimeSpan(TimeSpan.FromSeconds(20))));
                transformAnimation.KeyFrames.Add(
                    new LinearDoubleKeyFrame(480 - current.ActualWidth, KeyTime.FromTimeSpan(TimeSpan.FromSeconds(30))));

                Storyboard.SetTarget(transformAnimation, current);
                Storyboard.SetTargetProperty(transformAnimation, new PropertyPath(Canvas.LeftProperty));
                storyBoard = new Storyboard() { AutoReverse = true, RepeatBehavior = RepeatBehavior.Forever };
                storyBoard.Children.Add(transformAnimation);
                storyBoard.Begin(this, true);

            }
        }
        
        

    }
}
