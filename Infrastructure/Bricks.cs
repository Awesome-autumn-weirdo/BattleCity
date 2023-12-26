using System.Windows;
using System.Windows.Shapes;

namespace BattleCity
{
    public class Bricks : GameItem
    {
        public Bricks()
        {
            Width = 65;
            Height = 64;
        }
        public override void DrawItem(Rectangle pic)
        {
            pic.Width = Width;
            pic.Height = Height;
            pic.Visibility = Visibility.Visible;
        }
    }
}
