using System.Windows.Shapes;

namespace BattleCity
{
    public abstract class GameItem
    {
        public abstract void DrawItem(Rectangle pic);
        public int Width { get; set; }
        public int Height { get; set; }
    }
}

