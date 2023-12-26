using System.Windows;
using System.Windows.Controls;
using System.IO;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using System.Text.Json;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace BattleCity
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly DispatcherTimer timer1;
        private readonly List<Point> startPos = [];

        private int animationMode = 3;
        private bool isPressedKey = false;
        private List<Rectangle> tanks = [];
        private List<Rectangle> bricks = [];
        private List<Rectangle> walls = [];
        private List<Rectangle> bases = [];
        private Bricks brick = new();
        private BlockWall wall = new();
        private readonly Base Base = new();
        private PlayerTank player = new();
        private List<Tank> all = [];
        private readonly string TanksControls = "TanksControls.json";
        private readonly string BricksContorls = "BricksControls.json";
        private readonly string WallsControls = "WallsControls.json";
        private readonly string BaseControl = "BaseControl.json";
        private readonly string PlayerTankInfo = "PlayerTankInfo.json";
        private readonly string EnemyTanksInfo = "EnemyTankInfo.json";
        readonly List<Rectangle> bullets = [];

        public MainWindow() 
        {
            InitializeComponent();

            foreach (var item in GameField.Children)
            {
                if (item is not Rectangle rect) continue;

                ImageBrush imageControl = new();

                if (rect.Width == 65)
                {
                    imageControl.ImageSource = new BitmapImage(new Uri("brick.png", UriKind.Relative));
                }
                else if (rect.Width == 64 && rect.Height != 59)
                {
                    imageControl.ImageSource = new BitmapImage(new Uri("wall.png", UriKind.Relative));
                }
                else if (rect.Height == 59)
                {
                    imageControl.ImageSource = new BitmapImage(new Uri("base.png", UriKind.Relative));
                }
                else if (rect == player1)
                {
                    imageControl.ImageSource = new BitmapImage(new Uri("YUp.png", UriKind.Relative));
                    imageControl.ViewportUnits = BrushMappingMode.RelativeToBoundingBox;
                }
                else
                {
                    imageControl.ImageSource = new BitmapImage(new Uri("WD.png", UriKind.Relative));
                    imageControl.ViewportUnits = BrushMappingMode.RelativeToBoundingBox;
                }

                imageControl.ViewboxUnits = BrushMappingMode.RelativeToBoundingBox;
                rect.Fill = imageControl;
            }

            startPos.Add(new Point(Canvas.GetLeft(player1), Canvas.GetTop(player1)));
            startPos.Add(new Point(Canvas.GetLeft(computer1), Canvas.GetTop(computer1)));
            startPos.Add(new Point(Canvas.GetLeft(computer2), Canvas.GetTop(computer2)));
            startPos.Add(new Point(Canvas.GetLeft(computer3), Canvas.GetTop(computer3)));

            if (File.ReadAllText(TanksControls).Length > 0)
                ReDraw();
            else
            {
                DrawMap();
                player = new PlayerTank();
                all.Add(player);
                ComputerTank enemy;
                enemy = new ComputerTank()
                {
                    Mode = 1
                };
                all.Add(enemy);
                enemy = new ComputerTank()
                {
                    Mode = 1
                };
                all.Add(enemy);
                enemy = new ComputerTank();
                all.Add(enemy);
                tanks.Add(player1);
                tanks.Add(computer1);
                tanks.Add(computer2);
                tanks.Add(computer3);
                for (int i = 0; i < all.Count; i++)
                {
                    BitmapImage texture = new(new Uri("Bullet.png", UriKind.Relative));
                    ImageBrush textureImage = new()
                    {
                        ImageSource = texture
                    };
                    bullets.Add(new Rectangle());
                    bullets[i].Width = 23;
                    bullets[i].Height = 23;
                    bullets[i].Fill = textureImage;
                    GameField.Children.Add(bullets[i]);
                }
            }

            Closing += Form_FormClosing;
            KeyDown += Keyboard;
            KeyUp += FreeKey;


            timer1 = new DispatcherTimer()
            {
                Interval = TimeSpan.FromMilliseconds(15)
            };

            timer1.Tick += new EventHandler(Update);
            new Thread(() =>
            {
                timer1.Start();
            }).Start();

            Loaded += (object sender, RoutedEventArgs e) => // для быстрой отрисовки объектов
            {
                RenderOptions.SetBitmapScalingMode(this, BitmapScalingMode.NearestNeighbor);
                RenderOptions.SetEdgeMode(this, EdgeMode.Unspecified);
                RenderOptions.ProcessRenderMode = System.Windows.Interop.RenderMode.SoftwareOnly;
            };

            PlayerTankAnimation();
            EnemyTankAnimation();            
        }
        private static Point GetEmptySpace(List<Rectangle> tanks) // алгоритм для корректного возраждения вражеского танка
        {
            Point tup = new(1, 1);
            bool flag = true;
            bool conflict;
            while (flag)
            {
                conflict = false;
                foreach (Rectangle tank in tanks)
                {
                    Rect collision = new(Canvas.GetLeft(tank), Canvas.GetTop(tank), tank.Width, tank.Height);
                    if (collision.IntersectsWith(new Rect(tup, new Size(60, 60))))
                    {
                        tup = new Point(tup.X + 60, tup.Y);
                        conflict = true;
                    }
                }
                if (!conflict)
                    flag = false;
            }
            return tup;
        }
        private void Keyboard(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.D:
                case Key.A:
                case Key.W:
                case Key.S:
                    HandleMovementKey(e.Key);
                    break;
                case Key.Space:
                    HandleSpaceKey();
                    break;
            }
        }

        private void HandleMovementKey(Key key)
        {
            animationMode = GetAnimationMode(key);
            isPressedKey = true;
        }

        private static int GetAnimationMode(Key key)
        {
            return key switch
            {
                Key.D => 1,
                Key.A => 2,
                Key.W => 3,
                Key.S => 4,
                _ => 0,
            };
        }

        private void HandleSpaceKey()
        {
            if (player.Bullet.Coordinates.Item1 < 0)
                player.Bullet.Spawn(player.GetDirection(), player.GetFirePosition(player1));
        }

        private void FreeKey(object sender, KeyEventArgs e) // функция, которая отслеживает, нажата ли клавиша
        {
            isPressedKey = false;
        }
        public void EnemyTankAnimation() // отрисовка вражеских танков
        {
            ((ComputerTank)all[1]).DrawItem(computer1, bullets[1]);
            ((ComputerTank)all[2]).DrawItem(computer2, bullets[2]);
            ((ComputerTank)all[3]).DrawItem(computer3, bullets[3]);
        }
        public void PlayerTankAnimation() // отрисовка игрока
        {
            player.DrawItem(player1);
        }
        private void EnemyTankMovement() // алгоритм движения вражеских танков
        {
            if (((ComputerTank)all[1]).IsStoped && !((ComputerTank)all[1]).IsDead)
                ((ComputerTank)all[1]).Movement(computer1, GameField.Children);
            else if (!all[1].IsDead)
                ((ComputerTank)all[1]).Movement(computer1);

            if (((ComputerTank)all[2]).IsStoped && !((ComputerTank)all[2]).IsDead)
                ((ComputerTank)all[2]).Movement(computer2, GameField.Children);
            else if (!all[2].IsDead)
                ((ComputerTank)all[2]).Movement(computer2);

            if (((ComputerTank)all[3]).IsStoped && !((ComputerTank)all[3]).IsDead)
                ((ComputerTank)all[3]).Movement(computer3, GameField.Children);
            else if (!all[3].IsDead)
                ((ComputerTank)all[3]).Movement(computer3);
        }
        private void Form_FormClosing(object? sender, System.ComponentModel.CancelEventArgs e)
        {
            SaveListToJSON(tanks, TanksControls);
            SaveListToJSON(bricks, BricksContorls);
            SaveListToJSON(walls, WallsControls);
            SaveListToJSON(bases, BaseControl);

            player = (PlayerTank)all[0];
            string json5 = JsonSerializer.Serialize(player);

            List<ComputerTank> comp = [];
            for (int i = 1; i <= 3; i++)
            {
                comp.Add((ComputerTank)all[i]);
            }
            string json6 = JsonSerializer.Serialize(comp);

            File.WriteAllText(PlayerTankInfo, json5);
            File.WriteAllText(EnemyTanksInfo, json6);
        }

        private static void SaveListToJSON(List<Rectangle> rectangles, string fileName)
        {
            List<Point> adder = [];
            foreach (Rectangle pic in rectangles)
            {
                adder.Add(new Point((int)Canvas.GetLeft(pic), (int)Canvas.GetTop(pic)));
            }
            string json = JsonSerializer.Serialize(adder);
            File.WriteAllText(fileName, json);
        }

        public void DrawMap() // отрисовка карты
        {
            brick = new Bricks();
            wall = new BlockWall();
            foreach (var item in GameField.Children)
            {
                if (item is not Rectangle rect) continue;

                if (!rect.IsVisible)
                {
                    rect.Visibility = Visibility.Visible;
                }

                if (rect.Width == 65)
                {
                    brick.DrawItem(rect);
                    bricks.Add(rect);
                }

                if (rect.Width == 64 && rect.Height != 59)
                {
                    wall.DrawItem(rect);
                    walls.Add(rect);
                }

                if (rect.Height == 59)
                {
                    Base.DrawItem(rect);
                    bases.Add(rect);
                }
            }
        }
        private void ReDraw() // зарузка последней сохраненной игры
        {
            string json1 = File.ReadAllText(TanksControls);
            string json2 = File.ReadAllText(BricksContorls);
            string json3 = File.ReadAllText(WallsControls);
            string json4 = File.ReadAllText(BaseControl);
            string json5 = File.ReadAllText(PlayerTankInfo);
            string json6 = File.ReadAllText(EnemyTanksInfo);

            List<Point>? tankscon = json1.Length > 0 ? JsonSerializer.Deserialize<List<Point>>(json1) : null;
            List<Point>? brickscon = json2.Length > 0 ? JsonSerializer.Deserialize<List<Point>>(json2) : null;
            List<Point>? wallscon = json3.Length > 0 ? JsonSerializer.Deserialize<List<Point>>(json3) : null;
            List<Point>? basecon = json4.Length > 0 ? JsonSerializer.Deserialize<List<Point>>(json4) : null;

            if (json5.Length > 0)
                all.Add(JsonSerializer.Deserialize<PlayerTank>(json5)!); 
            if (json6.Length > 0)
                all.AddRange(JsonSerializer.Deserialize<List<ComputerTank>>(json6)!);

            if (tankscon != null && tankscon.Count > 0)
            {
                Canvas.SetLeft(player1, tankscon[0].X);
                Canvas.SetTop(player1, tankscon[0].Y);
            }
            
            if (tankscon != null && tankscon.Count > 0)
            {
                Canvas.SetLeft(computer1, tankscon[1].X);
                Canvas.SetTop(computer1, tankscon[1].Y);
            }
            
            if (tankscon != null && tankscon.Count > 0)
            {
                Canvas.SetLeft(computer2, tankscon[2].X);
                Canvas.SetTop(computer2, tankscon[2].Y);
            }
        
            if (tankscon != null && tankscon.Count > 0)
            {
                Canvas.SetLeft(computer3, tankscon[3].X);
                Canvas.SetTop(computer3, tankscon[3].Y);
            }

            tanks.Add(player1);
            tanks.Add(computer1);
            tanks.Add(computer2);
            tanks.Add(computer3);
            player = (PlayerTank)all[0];            
            player.Bullet = new Bullet();
            all[1].Bullet = new Bullet();
            all[2].Bullet = new Bullet();
            all[3].Bullet = new Bullet();

            foreach (var item in GameField.Children)
            {
                if (item is not Rectangle rectangleItem) continue;

                var position = new Point(Canvas.GetLeft(rectangleItem), Canvas.GetTop(rectangleItem));

                if (rectangleItem.Width == 65 && brickscon != null && brickscon.Contains(position))
                {
                    brick.DrawItem(rectangleItem);
                    bricks.Add(rectangleItem);
                }
                else if (rectangleItem.Width == 64 && rectangleItem.Height != 59 && wallscon != null && wallscon.Contains(position))
                {
                    wall.DrawItem(rectangleItem);
                    walls.Add(rectangleItem);
                }
                else if (rectangleItem.Height == 59 && basecon != null && basecon.Contains(position))
                {
                    Base.DrawItem(rectangleItem);
                    bases.Add(rectangleItem);
                }
                else if (rectangleItem.Height != 60)
                {
                    rectangleItem.Visibility = Visibility.Hidden;
                }
            }

            for (int i = 0; i < all.Count; i++)
            {
                BitmapImage texture = new(new Uri("Bullet.png", UriKind.Relative));
                ImageBrush textureImage = new()
                { 
                    ImageSource = texture 
                };
                bullets.Add(new Rectangle());
                bullets[i].Width = 23;
                bullets[i].Height = 23;
                bullets[i].Fill = textureImage;
                GameField.Children.Add(bullets[i]);
            }
        }
        private void Start() // функция для перезапуска при поражении
        {
            Base.IsDestroyed = false;
            Canvas.SetLeft(player1, startPos[0].X);
            Canvas.SetTop(player1, startPos[0].Y);
            Canvas.SetLeft(computer1, startPos[1].X);
            Canvas.SetTop(computer1, startPos[1].Y);
            Canvas.SetLeft(computer2, startPos[2].X);
            Canvas.SetTop(computer2, startPos[2].Y);
            Canvas.SetLeft(computer3, startPos[3].X);
            Canvas.SetTop(computer3, startPos[3].Y);

            tanks = [];
            bricks = [];
            walls = [];
            bases = [];
            all = new List<Tank>();
            DrawMap();
            Canvas.SetLeft(bullets[0], -60);
            Canvas.SetTop(bullets[0], -60);
            player = new PlayerTank();
            all.Add(player);
            ComputerTank enemy;
            enemy = new ComputerTank()
            { 
                Mode = 1
            };
            all.Add(enemy);
            enemy = new ComputerTank()
            {
                Mode = 1
            };
            all.Add(enemy);
            enemy = new ComputerTank();
            all.Add(enemy);
            tanks.Add(player1);
            tanks.Add(computer1);
            tanks.Add(computer2);
            tanks.Add(computer3);
            PlayerTankAnimation();
            EnemyTankAnimation();
        }
        private void Update(object? sender, EventArgs e) // обновление формы (анимация объектов)
        {
            InvalidateVisual();

            if (Base.IsDestroyed || player.IsDead) // проверяем, можно ли продолжать игру
            {
                Start();
                return;
            }
            player.Mode = animationMode;
            if (isPressedKey && player.IsValidMove(player.Coordinates)) // фиксируем нажатую пользователем кнопку для движения танка
            {
                switch (animationMode)
                {
                    case 1:
                        if (player.IsNoBarrier(all))
                        {
                            Canvas.SetLeft(player1, Canvas.GetLeft(player1) + player.Speed);
                            Canvas.SetTop(player1, Canvas.GetTop(player1));
                        }
                        break;
                    case 2:
                        if (player.IsNoBarrier(all))
                        {
                            Canvas.SetLeft(player1, Canvas.GetLeft(player1) - player.Speed);
                            Canvas.SetTop(player1, Canvas.GetTop(player1));
                        }
                        break;
                    case 3:
                        if (player.IsNoBarrier(all))
                        {
                            Canvas.SetLeft(player1, Canvas.GetLeft(player1));
                            Canvas.SetTop(player1, Canvas.GetTop(player1) - player.Speed);
                        }
                        break;
                    case 4:
                        if (player.IsNoBarrier(all))
                        {
                            Canvas.SetLeft(player1, Canvas.GetLeft(player1));
                            Canvas.SetTop(player1, Canvas.GetTop(player1) + player.Speed);
                        }
                        break;
                }
            }
            for (int i = 0; i < tanks.Count; i++) // проверка столкновений танков с внутриигровыми объектами
            {
                Rect tankCollision = new(Canvas.GetLeft(tanks[i]), Canvas.GetTop(tanks[i]), tanks[i].Width, tanks[i].Height);
                int mode;
                if (tanks[i] == computer1)
                    mode = all[1].Mode;
                else if (tanks[i] == computer2)
                    mode = all[2].Mode;
                else if (tanks[i] == computer3)
                    mode = all[3].Mode;
                else
                    mode = player.Mode;
                foreach (var br in bricks)
                {
                    Rect collision = new(Canvas.GetLeft(br), Canvas.GetTop(br), br.Width, br.Height);

                    if (collision.IntersectsWith(tankCollision))
                    {
                        switch (mode)
                        {
                            case 1:
                                Canvas.SetLeft(tanks[i], Canvas.GetLeft(tanks[i]) - 2);
                                Canvas.SetTop(tanks[i], Canvas.GetTop(tanks[i]));
                                break;
                            case 2:
                                Canvas.SetLeft(tanks[i], Canvas.GetLeft(tanks[i]) + 2);
                                Canvas.SetTop(tanks[i], Canvas.GetTop(tanks[i]));
                                break;
                            case 3:
                                Canvas.SetLeft(tanks[i], Canvas.GetLeft(tanks[i]));
                                Canvas.SetTop(tanks[i], Canvas.GetTop(tanks[i]) + 2);
                                break;
                            case 4:
                                Canvas.SetLeft(tanks[i], Canvas.GetLeft(tanks[i]));
                                Canvas.SetTop(tanks[i], Canvas.GetTop(tanks[i]) - 2);
                                break;
                        }

                        if (all[i] is ComputerTank computerTank)
                        {
                            computerTank.IsStoped = true;
                        }

                        break;
                    }
                }

                foreach (var wl in walls)
                {
                    Rect collision = new(Canvas.GetLeft(wl), Canvas.GetTop(wl), wl.Width, wl.Height);

                    if (collision.IntersectsWith(tankCollision))
                    {
                        switch (mode)
                        {
                            case 1:
                                Canvas.SetLeft(tanks[i], Canvas.GetLeft(tanks[i]) - 2);
                                Canvas.SetTop(tanks[i], Canvas.GetTop(tanks[i]));
                                break;
                            case 2:
                                Canvas.SetLeft(tanks[i], Canvas.GetLeft(tanks[i]) + 2);
                                Canvas.SetTop(tanks[i], Canvas.GetTop(tanks[i]));
                                break;
                            case 3:
                                Canvas.SetLeft(tanks[i], Canvas.GetLeft(tanks[i]));
                                Canvas.SetTop(tanks[i], Canvas.GetTop(tanks[i]) + 2);
                                break;
                            case 4:
                                Canvas.SetLeft(tanks[i], Canvas.GetLeft(tanks[i]));
                                Canvas.SetTop(tanks[i], Canvas.GetTop(tanks[i]) - 2);
                                break;
                        }

                        if (all[i] is ComputerTank computerTank)
                        {
                            computerTank.IsStoped = true;
                        }

                        break;
                    }
                }
            }

            foreach (var t in all)
            {
                if (t is ComputerTank computerTank && computerTank.IsDead && computerTank.RestoreTime > 399)
                {
                    computerTank.RestoreCoord = GetEmptySpace(tanks);
                }
            }

            for (int i = 0; i < tanks.Count; i++) // регистрация столкновения между танками
            {
                Rect comparison;

                for (int j = 0; j < tanks.Count; j++)
                {
                    if (all[j].IsDead || all[i].IsDead)
                        break;

                    if (all[j] is ComputerTank computerTank)
                    {
                        comparison = computerTank.NextRadius;
                    }
                    else
                    {
                        comparison = all[j].Radius;
                    }

                    if (tanks[i] == tanks[j])
                        continue;
                    else if (!all[i].NextRadius.IntersectsWith(comparison))
                        continue;
                    else if (all[i] is ComputerTank iComputerTank)
                    {
                        iComputerTank.IsStoped = true;

                        if (all[j] is ComputerTank jComputerTank)
                        {
                            jComputerTank.IsStoped = true;
                        }
                    }
                    else if (all[j] is ComputerTank jComputerTank)
                    {
                        jComputerTank.IsStoped = true;
                    }

                    break;
                }
            }

            foreach (Tank item in all) // регистрация попадания снаряда 
            {
                if (item.Bullet.Coordinates != null && item.Bullet.Coordinates.Item1 > 0)
                {
                    if (item is PlayerTank)
                    {
                        item.Bullet.DrawItem(bullets[0]);
                        item.Bullet.Movement();
                    }
                    foreach (Rectangle brick in bricks)
                    {
                        Rect collision = new(Canvas.GetLeft(brick), Canvas.GetTop(brick), brick.Width, brick.Height);
                        if (collision.IntersectsWith(new Rect(new Point(item.Bullet.Coordinates.Item1, item.Bullet.Coordinates.Item2), new Size(23, 23))))
                        {
                            item.Bullet.Destroy();
                            brick.Visibility = Visibility.Hidden;
                            bricks.Remove(brick);
                            break;
                        }
                    }
                    foreach (Rectangle wall in walls)
                    {
                        Rect collision = new(Canvas.GetLeft(wall), Canvas.GetTop(wall), wall.Width, wall.Height);
                        if (collision.IntersectsWith(new Rect(new Point(item.Bullet.Coordinates.Item1, item.Bullet.Coordinates.Item2), new Size(23, 23))))
                        {
                            item.Bullet.Destroy();
                            break;
                        }
                    }
                    foreach (Rectangle b in bases)
                    {                        
                        Rect collision = new(Canvas.GetLeft(b), Canvas.GetTop(b), b.Width, b.Height);
                        if (collision.IntersectsWith(new Rect(new Point(item.Bullet.Coordinates.Item1, item.Bullet.Coordinates.Item2), new Size(23, 23))))
                        {
                            item.Bullet.Destroy();
                            b.Visibility = Visibility.Hidden;
                            bases.Remove(b);
                            Base.IsDestroyed = true;
                            break;
                        }
                    }
                    foreach (Rectangle en in tanks)
                    {
                        Rect collision = new(Canvas.GetLeft(en), Canvas.GetTop(en), en.Width, en.Height);
                        if (collision.IntersectsWith(new Rect(new Point(item.Bullet.Coordinates.Item1, item.Bullet.Coordinates.Item2), new Size(23, 23))))
                        {
                            if ((en == computer1 || en == computer2 || en == computer3) && item is ComputerTank)
                            {
                                item.Bullet.Destroy();
                                break;
                            }
                            item.Bullet.Destroy();
                            Canvas.SetLeft(bullets[0], -60);
                            Canvas.SetTop(bullets[0], -60);
                            Canvas.SetLeft(en, -60);
                            Canvas.SetTop(en, -60);
                            break;
                        }
                    }
                    if (!item.Bullet.IsVisible(item.Bullet.Coordinates))
                    {
                        item.Bullet.Destroy();
                        if (item is PlayerTank)
                        {
                            Canvas.SetLeft(bullets[0], -60);
                            Canvas.SetTop(bullets[0], -60);
                        }
                    }
                }
            }
            EnemyTankAnimation();
            EnemyTankMovement();
            PlayerTankAnimation();
        }
    }
}