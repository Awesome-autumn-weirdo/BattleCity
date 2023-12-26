using System.Windows;
using System.Windows.Controls;
using System.Windows.Shapes;

namespace BattleCity
{
    public class Tank : GameItem
    {
        public Rect NextRadius { get; set; }
        public Rect Radius { get; set; }
        public bool IsDead { get; set; }
        public Tuple<int, int> Coordinates { get; set; }        
        public Bullet Bullet { get; set; }
        public int Mode { get; set; }
        public int Speed { get; set; }
        public Tank()
        {
            IsDead = false;
            Bullet = new Bullet();
            Speed = 2;
            Mode = 3;
            Width = 60;
            Height = 60;
            Coordinates = Tuple.Create(0, 0);
        }
        public Tuple<int, int> GetFirePosition(Rectangle pic)
        {
            Tuple<int, int> dir;
            if (Mode == 1)
                dir = new Tuple<int, int>((int)(Canvas.GetLeft(pic) + pic.Width / 2 + 40), (int)(Canvas.GetTop(pic) + pic.Width / 2 - 10));
            else if (Mode == 2)
                dir = new Tuple<int, int>((int)(Canvas.GetLeft(pic) - pic.Width / 2), (int)(Canvas.GetTop(pic) + pic.Width / 2 - 10));
            else if (Mode == 3)
                dir = new Tuple<int, int>((int)(Canvas.GetLeft(pic) + pic.Width / 2 - 10), (int)(Canvas.GetTop(pic) - pic.Width / 2));
            else
                dir = new Tuple<int, int>((int)(Canvas.GetLeft(pic) + pic.Width / 2 - 10), (int)(Canvas.GetTop(pic) + pic.Width / 2 + 40));
            return dir;
        }
        public Tuple<int, int> GetDirection()
        {
            Tuple<int, int> dir;
            if (Mode == 1)
                dir = new Tuple<int, int>(1, 0);
            else if (Mode == 2)
                dir = new Tuple<int, int>(-1, 0);
            else if (Mode == 3)
                dir = new Tuple<int, int>(0, -1);
            else
                dir = new Tuple<int, int>(0, 1);
            return dir;
        }
        public bool IsValidMove(Tuple<int, int> coord)
        {
            if (coord.Item1 > 1120 || coord.Item2 > 700 || coord.Item1 < 1 || coord.Item2 < 1)
            {
                return false;
            }
            else
                return true;
        }

        public bool IsComputerTank() // показывает, что танк вражеский
        {
            return this is ComputerTank;
        }

        public override void DrawItem(Rectangle pic)
        {
        }
    }
}
