using System.Windows.Controls;
using System.Windows.Shapes;

namespace BattleCity
{
    public class Bullet : GameItem
    {
        public int reload;
        public Tuple<int, int> Coordinates { get; set; }
        private Tuple<int, int> direction;
        public int Speed { get; set; }

        public Bullet()
        {
            reload = 0;
            Speed = 6;
            Width = 23;
            Height = 23;
            direction = new Tuple<int, int>(0, 0);
            Coordinates = new Tuple<int, int>(-50, -50);
        }

        public void Reloading()
        {
            reload++;
        }

        public bool IsVisible(Tuple<int, int> coord)
        {
            if (coord.Item1 > 1140 || coord.Item2 > 740 || coord.Item1 < 1 || coord.Item2 < 0)
            {
                return false;
            }
            else
                return true;
        }

        public void Destroy()
        {
            Reloading();
            Coordinates = new Tuple<int, int>(-50, -50);
            direction = new Tuple<int, int>(0, 0);
        }

        public void Movement()
        {
            if (Coordinates.Item1 > -10)
                Coordinates = new Tuple<int, int>(Coordinates.Item1 + direction.Item1 * Speed, Coordinates.Item2 + direction.Item2 * Speed);
        }

        public void Spawn(Tuple<int, int> dir, Tuple<int, int> p1)
        {
            direction = dir;
            Coordinates = new Tuple<int, int>(p1.Item1, p1.Item2);
        }

        public override void DrawItem(Rectangle pic)
        {
            Canvas.SetLeft(pic, Coordinates.Item1);
            Canvas.SetTop(pic, Coordinates.Item2);
        }
    }
}
