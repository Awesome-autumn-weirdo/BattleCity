using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace BattleCity
{
    public class PlayerTank : Tank
    {
        public bool IsNoBarrier(List<Tank> tanks)
        {
            for (int i = 1; i < tanks.Count; i++)
            {
                if (HasCollisionWithTank(tanks[i]))
                    return false;
            }
            return true;
        }

        private bool HasCollisionWithTank(Tank otherTank)
        {
            return NextRadius.IntersectsWith(otherTank.Radius) && !otherTank.IsDead;
        }

        public override void DrawItem(Rectangle pic)
        {
            SetTankImageAndNextRadius(pic);
            HandleInvalidMove(pic);
            UpdateTankAppearance(pic);
        }

        private void SetTankImageAndNextRadius(Rectangle pic) //Устанавливает изображение и следующий радиус танка в зависимости от текущего режима (Mode)
        {
            BitmapImage img;

            switch (Mode)
            {
                case 1:
                    img = new BitmapImage(new Uri("YR.png", UriKind.Relative));
                    NextRadius = new Rect(Canvas.GetLeft(pic) + 7, Canvas.GetTop(pic), 60, 60);
                    break;
                case 2:
                    img = new BitmapImage(new Uri("YL.png", UriKind.Relative));
                    NextRadius = new Rect(Canvas.GetLeft(pic) - 7, Canvas.GetTop(pic), 60, 60);
                    break;
                case 3:
                    img = new BitmapImage(new Uri("YUp.png", UriKind.Relative));
                    NextRadius = new Rect(Canvas.GetLeft(pic), Canvas.GetTop(pic) - 7, 60, 60);
                    break;
                case 4:
                    img = new BitmapImage(new Uri("YD.png", UriKind.Relative));
                    NextRadius = new Rect(Canvas.GetLeft(pic), Canvas.GetTop(pic) + 7, 60, 60);
                    break;
                default:
                    return;
            }

            ImageBrush image = new()
            { 
                ImageSource = img
            };
            pic.Fill = image;
            pic.Width = Width;
            pic.Height = Height;
            pic.Visibility = Visibility.Visible;
            Radius = new Rect((int)Canvas.GetLeft(pic), (int)Canvas.GetTop(pic), 60, 60);
        }

        private void HandleInvalidMove(Rectangle pic) //Обрабатывает недопустимые движения танка
        {
            if (Coordinates != null && !IsValidMove(Coordinates))
            {
                if (Coordinates.Item1 < -10)
                {
                    IsDead = true;
                    return;
                }

                int returnValue;
                if (Coordinates.Item1 > 730)
                {
                    returnValue = Coordinates.Item2;
                    Canvas.SetLeft(pic, 1120);
                    Canvas.SetTop(pic, returnValue);
                }
                else if (Coordinates.Item2 > 700)
                {
                    returnValue = Coordinates.Item1;
                    Canvas.SetLeft(pic, returnValue);
                    Canvas.SetTop(pic, 700);
                }
                else if (Coordinates.Item1 < 1)
                {
                    returnValue = Coordinates.Item2;
                    Canvas.SetLeft(pic, 1);
                    Canvas.SetTop(pic, returnValue);
                }
                else
                {
                    returnValue = Coordinates.Item1;
                    Canvas.SetLeft(pic, returnValue);
                    Canvas.SetTop(pic, 1);
                }
            }
        }

        private void UpdateTankAppearance(Rectangle pic)
        {
            Coordinates = new Tuple<int, int>((int)Canvas.GetLeft(pic), (int)Canvas.GetTop(pic));
        }
    }
}


