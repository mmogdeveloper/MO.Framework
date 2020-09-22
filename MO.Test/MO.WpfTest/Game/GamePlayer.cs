using ProtoMessage;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace MO.WpfTest.Game
{
    public class GamePlayer
    {
        public Rectangle Rect_Player { get; }

        public MOPoint CurPoint { get; private set; }

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
            CurPoint = new MOPoint();
        }

        public MOPoint GetPoint()
        {
            return CurPoint;
        }

        public void SetPoint(MOPoint point)
        {
            CurPoint = point;
            Canvas.SetTop(Rect_Player, point.Y);
            Canvas.SetLeft(Rect_Player, point.X);
        }
    }
}
