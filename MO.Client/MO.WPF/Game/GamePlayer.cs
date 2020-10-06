using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace MO.WpfTest.Game
{
    public class GamePlayer
    {
        public Rectangle Rect_Player { get; }

        public float X { get; private set; }

        public float Y { get; private set; }

        public long UserId { get; }

        public GamePlayer(long userId)
        {
            UserId = userId;
            Rect_Player = new Rectangle();
            Rect_Player.Width = 20;
            Rect_Player.Height = 20;
            Rect_Player.Fill = Brushes.Gray;
            Canvas.SetLeft(Rect_Player, 0);
            Canvas.SetTop(Rect_Player, 0);
            Canvas.SetRight(Rect_Player, 0);
            Canvas.SetBottom(Rect_Player, 0);
        }

        public (float, float) GetPoint()
        {
            return (X, Y);
        }

        public void SetPoint(float x, float y)
        {
            X = x;
            Y = y;
            Canvas.SetLeft(Rect_Player, x);
            Canvas.SetTop(Rect_Player, y);
        }

        public void MoveTo(float x, float y, float step)
        {
            //_storyboard.Stop();
            //创建X轴方向动画

            Canvas.SetLeft(Rect_Player, x);
            Canvas.SetTop(Rect_Player, y);
        }
    }
}
