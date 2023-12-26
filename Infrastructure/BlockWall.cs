using System.Windows;
using System.Windows.Shapes;

namespace BattleCity
{
    public class BlockWall : GameItem
    {
        public BlockWall()
        {
            Width = 64;
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
