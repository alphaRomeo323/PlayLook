using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media.Animation;

namespace PlayLook
{
    /// <summary>
    /// PropView.xaml の相互作用ロジック
    /// </summary>
    public partial class PropView : Window
    {
        private SMTCSessionManager sessionManager; // SMTCセッションを管理するクラス
        private HTTPSessionManager httpSessionManager; // HTTPセッションを管理するクラス
        private Storyboard? storyBoard; // アニメーションを管理するクラス

        private const double TEXT_BLOCK_WIDTH = 480; // テキストブロックの幅(px)
        private const int ANIMATION_BASE_TIME = 7; // アニメーションの基本時間(s)
        private const double ANIMATION_SPEED = 40; // アニメーション速度(px/s)
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public PropView()
        {
            InitializeComponent();
            sessionManager = new SMTCSessionManager();
            httpSessionManager = new HTTPSessionManager(sessionManager.propContents);
            this.DataContext = sessionManager.propContents;
            
        }
        /// <summary>
        /// ウィンドウをドラッグするためのイベントハンドラ
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }

        /// <summary>
        /// ウィンドウを閉じるためのイベントハンドラ
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Collapse_Click(object sender, RoutedEventArgs e)
        {
            this.Hide();
        }

        /// <summary>
        /// SMTCを通じて再生・一時停止を行うためのイベントハンドラ
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Control_Click(object sender, RoutedEventArgs e)
        {
            sessionManager.TryControl();
        }

        /// <summary>
        /// テキストの長さに応じてアニメーションを行うためのイベントハンドラ
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private async void TargetUpdatedHandler(object sender, DataTransferEventArgs args)
        {
            await Task.Delay(100);
            if (storyBoard != null)
            {
                storyBoard.Stop(this);
                storyBoard.Children.Clear();
            }
            if (current.ActualWidth > TEXT_BLOCK_WIDTH)
            {
                double animationLengsth = TEXT_BLOCK_WIDTH - current.ActualWidth;
                int animationTime = (int)Math.Ceiling(-animationLengsth / ANIMATION_SPEED);
                DoubleAnimationUsingKeyFrames transformAnimation = new DoubleAnimationUsingKeyFrames();
                transformAnimation.Duration = TimeSpan.FromSeconds(2 * ANIMATION_BASE_TIME + animationTime);
                transformAnimation.KeyFrames.Add(
                    new LinearDoubleKeyFrame(0, KeyTime.FromTimeSpan(TimeSpan.FromSeconds(0))));
                transformAnimation.KeyFrames.Add(
                    new LinearDoubleKeyFrame(0, KeyTime.FromTimeSpan(TimeSpan.FromSeconds(ANIMATION_BASE_TIME))));
                transformAnimation.KeyFrames.Add(
                    new LinearDoubleKeyFrame(animationLengsth, KeyTime.FromTimeSpan(TimeSpan.FromSeconds(ANIMATION_BASE_TIME + animationTime))));
                transformAnimation.KeyFrames.Add(
                    new LinearDoubleKeyFrame(animationLengsth, KeyTime.FromTimeSpan(TimeSpan.FromSeconds(2 * ANIMATION_BASE_TIME + animationTime))));

                Storyboard.SetTarget(transformAnimation, current);
                Storyboard.SetTargetProperty(transformAnimation, new PropertyPath(Canvas.LeftProperty));
                storyBoard = new Storyboard() { AutoReverse = true, RepeatBehavior = RepeatBehavior.Forever };
                storyBoard.Children.Add(transformAnimation);
                storyBoard.Begin(this, true);

            }
        }
        
        

    }
}
