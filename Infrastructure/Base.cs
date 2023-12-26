using System.Windows;
using System.Windows.Shapes;

namespace BattleCity
{
    public class Base : GameItem
    {
        public bool IsDestroyed { get; set; }
        public Base()
        {
            IsDestroyed = false;
            Width = 64;
            Height = 59;
        }
        public override void DrawItem(Rectangle pic)
        {
            pic.Width = Width;
            pic.Height = Height;
            pic.Visibility = Visibility.Visible;
        }
    }
}