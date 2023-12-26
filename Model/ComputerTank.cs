using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace BattleCity
{
    public class ComputerTank : Tank
    {
        private int delta = 0;
        public int TimeToRotate { get; set; } //время для поворота танка
        public int RestoreTime { get; set; }
        public bool IsStoped { get; set; }
        public Point RestoreCoord { get; set; }
        private void Rotate()
        {
            Random rand = new();
            if (delta > 300)
            {
                Mode = rand.Next(1, 5);
                delta = 0;
            }
        }
        public void Movement(Rectangle pic, UIElementCollection forms)
        {          
            int Right = 10000;
            int Left = 10000;
            int Up = 10000;
            int Down = 10000;
            int counter = 0;
            foreach (UIElement form in forms)
            {
                if (form is Rectangle rect)
                {
                    Rect collision = new(Canvas.GetLeft(rect), Canvas.GetTop(rect), rect.ActualWidth, rect.ActualHeight);

                    if (rect != pic)
                    {
                        while (!collision.IntersectsWith(new Rect(Canvas.GetLeft(pic) + counter, Canvas.GetTop(pic), 60, 60)) && IsValidMove(new Tuple<int, int>((int)(Canvas.GetLeft(pic) + counter), (int)Canvas.GetTop(pic))))
                            counter += 2;

                        if (counter < Right && counter != 0)
                        {
                            Right = counter;
                            counter = 0;
                        }
                        else
                        {
                            counter = 0;
                        }

                        while (!collision.IntersectsWith(new Rect(Canvas.GetLeft(pic) - counter, Canvas.GetTop(pic), 60, 60)) && IsValidMove(new Tuple<int, int>((int)(Canvas.GetLeft(pic) - counter), (int)Canvas.GetTop(pic))))
                            counter += 2;

                        if (counter < Left && counter != 0)
                        {
                            Left = counter;
                            counter = 0;
                        }
                        else
                        {
                            counter = 0;
                        }

                        while (!collision.IntersectsWith(new Rect(Canvas.GetLeft(pic), Canvas.GetTop(pic) - counter, 60, 60)) && IsValidMove(new Tuple<int, int>((int)Canvas.GetLeft(pic), (int)(Canvas.GetTop(pic) - counter))))
                            counter += 2;

                        if (counter < Up && counter != 0)
                        {
                            Up = counter;
                            counter = 0;
                        }
                        else
                        {
                            counter = 0;
                        }

                        while (!collision.IntersectsWith(new Rect(Canvas.GetLeft(pic), Canvas.GetTop(pic) + counter, 60, 60)) && IsValidMove(new Tuple<int, int>((int)Canvas.GetLeft(pic), (int)(Canvas.GetTop(pic) + counter))))
                            counter += 2;

                        if (counter < Down && counter != 0)
                        {
                            Down = counter;
                            counter = 0;
                        }
                        else
                        {
                            counter = 0;
                        }
                    }
                }
            }

            List<int> search = [ Right, Left, Up, Down ];

            if (Right == search.Max())
            {
                Mode = 1;
                delta = 0;
            }
            else if (Left == search.Max())
            {
                Mode = 2;
                delta = 0;
            }
            else if (Up == search.Max())
            {
                Mode = 3;
                delta = 0;
            }
            else
            {
                Mode = 4;
                delta = 0;
            }
            IsStoped = false;
        }
        public void Movement(Rectangle pic)
        {
            if (!IsStoped && !IsDead)
            {
                Speed = 2;
                if (Mode == 1)
                    Canvas.SetLeft(pic, Canvas.GetLeft(pic) + Speed);
                else if (Mode == 2)
                    Canvas.SetLeft(pic, Canvas.GetLeft(pic) - Speed);
                else if (Mode == 3)
                    Canvas.SetTop(pic, Canvas.GetTop(pic) - Speed);
                else if (Mode == 4)
                    Canvas.SetTop(pic, Canvas.GetTop(pic) + Speed);
            }
        }
        private void Restore()
        {
            if (!IsDead)
            {
                RestoreTime = 0;
            }
            else
            {
                RestoreTime++;
                if (RestoreTime > 400)
                {
                    Coordinates = new Tuple<int, int>(1, 1);
                    IsDead = false;
                }
            }
        }
        public void DrawItem(Rectangle pic, Rectangle bul)
        {
            delta++;
            Rotate();
            if (Bullet.Coordinates.Item1 < 0)
            {
                if (Bullet.reload > 100)
                {
                    Bullet.Spawn(GetDirection(), GetFirePosition(pic));
                    Bullet.reload = 0;
                }
                else
                {
                    Bullet.Reloading();
                    Canvas.SetLeft(bul, -60);
                    Canvas.SetTop(bul, -60);
                }
            }
            else
            {                
                Bullet.Reloading();
                Bullet.DrawItem(bul);
                Bullet.Movement();
            }
            if (RestoreTime > 0 && !IsDead)
            {
                Canvas.SetLeft(pic, RestoreCoord.X);
                Canvas.SetTop(pic, RestoreCoord.Y);
                RestoreTime = 0;
            }
            if (IsStoped)
                Speed = 0;
            else
                Speed = 2;
            if (Canvas.GetLeft(pic) == -60)
            {
                IsDead = true;
                Restore();
                return;
            }
            Coordinates = new Tuple<int, int>((int)Canvas.GetLeft(pic), (int)Canvas.GetTop(pic));
            if (IsDead)
                Restore();
            BitmapImage img;
            switch (Mode)
            {
                case 1:
                    img = new BitmapImage(new Uri("WR.png", UriKind.Relative));
                    NextRadius = new Rect(Canvas.GetLeft(pic) + 7, Canvas.GetTop(pic), 60, 60);
                    break;
                case 2:
                    img = new BitmapImage(new Uri("WL.png", UriKind.Relative));
                    NextRadius = new Rect(Canvas.GetLeft(pic) - 7, Canvas.GetTop(pic), 60, 60);
                    break;
                case 3:
                    img = new BitmapImage(new Uri("WUp.png", UriKind.Relative));
                    NextRadius = new Rect(Canvas.GetLeft(pic), Canvas.GetTop(pic) - 7, 60, 60);
                    break;
                case 4:
                    img = new BitmapImage(new Uri("WD.png", UriKind.Relative));
                    NextRadius = new Rect(Canvas.GetLeft(pic), Canvas.GetTop(pic) + 7, 60, 60);
                    break;
                default:
                    return;
            }
            if (Coordinates != null)
            {
                if (!IsValidMove(Coordinates))
                {
                    int returnValue;
                    if (Coordinates.Item1 > 1120)
                    {
                        returnValue = Coordinates.Item2;
                        Canvas.SetLeft(pic, 1120);
                        Canvas.SetTop(pic, returnValue);
                        IsStoped = true;
                    }
                    else if (Coordinates.Item2 > 700)
                    {
                        returnValue = Coordinates.Item1;
                        Canvas.SetLeft(pic, returnValue);
                        Canvas.SetTop(pic, 700);
                        IsStoped = true;
                    }
                    else if (Coordinates.Item1 < 0)
                    {
                        returnValue = Coordinates.Item2;
                        Canvas.SetLeft(pic, 1);
                        Canvas.SetTop(pic, returnValue);
                        IsStoped = true;
                    }
                    else if (Coordinates.Item2 < 0)
                    {
                        returnValue = Coordinates.Item1;
                        Canvas.SetLeft(pic, returnValue);
                        Canvas.SetTop(pic, 1);
                        IsStoped = true;
                    }
                }
            }
            ImageBrush image = new()
            { 
                ImageSource = img
            };
            pic.Fill = image;
            pic.Width = Width;
            pic.Height = Height;
            pic.Visibility = Visibility.Visible;
            Radius = new Rect(Canvas.GetLeft(pic), Canvas.GetTop(pic), 60, 60);
        }
    }
}
