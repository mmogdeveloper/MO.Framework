using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace MO.WpfTest.Game
{
    public class GamePlayer
    {
        public Rectangle Rect_Player { get; }

        public Int64 X { get; private set; }

        public Int64 Y { get; private set; }

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

        public (Int64, Int64) GetPoint()
        {
            return (X, Y);
        }

        public void SetPoint(Int64 x, Int64 y)
        {
            X = x;
            Y = y;
            Canvas.SetLeft(Rect_Player, x);
            Canvas.SetTop(Rect_Player, y);
        }
    }
}
