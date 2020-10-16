using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using Microsoft.Win32;

namespace WpfApp1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Fluent.RibbonWindow
    {

        DispatcherTimer timer = new DispatcherTimer();

        public MainWindow()
        {
            InitializeComponent();
        }

        const int gameRows = 3;
        const int gameCols = 3;

        const int Width_Shape = 100;
        const int Height_Shape = 100;

        const int Ribbon_Height = 120;

        const int ImageWidth = 95;
        const int ImageHeight = 95;


        const int StartX = 50; // leftOffset
        const int StartY = 50; // topOffset

        const int StartXpreImage = 470;
        const int StartYpreImage = 50;
        const int previewImageWitdth = 600;
        const int previewImageHeight = 300;

        List<Image> croppedImages = null;
        Tuple<int, int> emptyImage = null;
        
        Image[,] _images;

        class Level
        {
            public string Name { get; set; }
            public int Value { get; set; } // giay

        }

        BindingList<Level> _level = null;



        private void RibbonWindow_Loaded(object sender, RoutedEventArgs e)
        {

            _images = new Image[gameRows, gameCols];
            // bingding level 
            var lines = File.ReadAllLines("level.txt"); // doc du lieu muc do
            if (lines.Length > 0)
            {
                _level = new BindingList<Level>();
                foreach (var line in lines)
                {
                    var token = line.Split(new string[] { "-" }, StringSplitOptions.None);
                    var level = new Level()
                    {
                        Name = token[0],
                        Value = int.Parse(token[1])
                    };
                    _level.Add(level);
                }

            }

            levelComboBox.ItemsSource = _level;

            drawShape();

        }

        private void drawShape()
        {
            // Ve hinh chu nhat bao quanh cac o
            var line1 = new Line();
            line1.StrokeThickness = 2;
            line1.Stroke = new SolidColorBrush(Colors.Brown);
            uiCanvas.Children.Add(line1);
            line1.X1 = StartX;
            line1.Y1 = StartY;

            line1.X2 = StartX;
            line1.Y2 = StartY + gameRows * Height_Shape;

            var line2 = new Line();
            line2.StrokeThickness = 2;
            line2.Stroke = new SolidColorBrush(Colors.Brown);
            uiCanvas.Children.Add(line2);
            line2.X1 = StartX + gameCols * Width_Shape;
            line2.Y1 = StartY;

            line2.X2 = StartX + gameCols * Width_Shape;
            line2.Y2 = StartY + gameRows * Height_Shape;

            var line3 = new Line();
            line3.StrokeThickness = 2;
            line3.Stroke = new SolidColorBrush(Colors.Brown);
            uiCanvas.Children.Add(line3);
            line3.X1 = StartX;
            line3.Y1 = StartY;

            line3.X2 = StartX + gameCols * Width_Shape;
            line3.Y2 = StartY;

            var line4 = new Line();
            line4.StrokeThickness = 2;
            line4.Stroke = new SolidColorBrush(Colors.Brown);
            uiCanvas.Children.Add(line4);
            line4.X1 = StartX;
            line4.Y1 = StartY + gameRows * Height_Shape;

            line4.X2 = StartX + gameCols * Width_Shape;
            line4.Y2 = StartY + gameRows * Height_Shape;

            // ve cac o ben trong: chia lam 9 o

            Line[] LineCols = new Line[gameCols];
            for (int j = 0; j < gameCols - 1; j++)
            {
                LineCols[j] = new Line();
                LineCols[j].StrokeThickness = 1;
                LineCols[j].Stroke = new SolidColorBrush(Colors.Brown);
                uiCanvas.Children.Add(LineCols[j]);
                LineCols[j].X1 = StartX + (j + 1) * Width_Shape;
                LineCols[j].Y1 = StartY;

                LineCols[j].X2 = StartX + (j + 1) * Width_Shape;
                LineCols[j].Y2 = StartY + gameRows * Height_Shape;
            }

            Line[] LineRows = new Line[gameRows];

            for (int i = 0; i < gameRows - 1; i++)
            {
                LineRows[i] = new Line();
                LineRows[i].StrokeThickness = 1;
                LineRows[i].Stroke = new SolidColorBrush(Colors.Brown);
                uiCanvas.Children.Add(LineRows[i]);
                LineRows[i].X1 = StartX;
                LineRows[i].Y1 = StartY + (i + 1) * Height_Shape;

                LineRows[i].X2 = StartX + gameCols * Width_Shape;
                LineRows[i].Y2 = StartY + (i + 1) * Height_Shape;
            }

            // ve khung cho previewImage
            var lineImages1 = new Line();
            lineImages1.StrokeThickness = 2;
            lineImages1.Stroke = new SolidColorBrush(Colors.Red);
            uiCanvas.Children.Add(lineImages1);
            lineImages1.X1 = StartXpreImage;
            lineImages1.Y1 = StartYpreImage;

            lineImages1.X2 = StartXpreImage;
            lineImages1.Y2 = StartYpreImage + previewImageHeight;

            var lineImages2 = new Line();
            lineImages2.StrokeThickness = 2;
            lineImages2.Stroke = new SolidColorBrush(Colors.Red);
            uiCanvas.Children.Add(lineImages2);
            lineImages2.X1 = StartXpreImage + previewImageWitdth;
            lineImages2.Y1 = StartYpreImage;

            lineImages2.X2 = StartXpreImage + previewImageWitdth;
            lineImages2.Y2 = StartYpreImage + previewImageHeight;

            var lineImages3 = new Line();
            lineImages3.StrokeThickness = 2;
            lineImages3.Stroke = new SolidColorBrush(Colors.Red);
            uiCanvas.Children.Add(lineImages3);
            lineImages3.X1 = StartXpreImage;
            lineImages3.Y1 = StartYpreImage;

            lineImages3.X2 = StartXpreImage + previewImageWitdth;
            lineImages3.Y2 = StartYpreImage;

            var lineImages4 = new Line();
            lineImages4.StrokeThickness = 2;
            lineImages4.Stroke = new SolidColorBrush(Colors.Red);
            uiCanvas.Children.Add(lineImages4);
            lineImages4.X1 = StartXpreImage;
            lineImages4.Y1 = StartYpreImage + previewImageHeight;

            lineImages4.X2 = StartXpreImage + previewImageWitdth;
            lineImages4.Y2 = StartYpreImage + previewImageHeight;



        }

        private void PreviewImage_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            var animation = new DoubleAnimation();
            animation.From = StartXpreImage - 25;
            animation.To = StartXpreImage + 25;
            animation.Duration = new Duration(TimeSpan.FromSeconds(1));
            animation.AutoReverse = true;
            animation.RepeatBehavior = RepeatBehavior.Forever;


            var story = new Storyboard();
            story.Children.Add(animation);
            Storyboard.SetTargetName(animation, previewImage.Name);
            Storyboard.SetTargetProperty(animation, new PropertyPath(Canvas.LeftProperty));
            story.Begin(this);
        }

        private void MenuItem_LoadGame_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Tinh nang dang phat trien");
        }

        private void MenuItem_NewGame_Click(object sender, RoutedEventArgs e)
        {
            int idx = levelComboBox.SelectedIndex;
            if (idx < 0)
            {
                MessageBox.Show("Please choose level to play game");
            }
            else if (croppedImages == null)
            {
                MessageBox.Show("Please choose picture to play game");
            }
            else
            {

                timer.Interval = TimeSpan.FromSeconds(1);
                timer.Tick += timer_Tick;

                timer.Start();

                iTime = _level[idx].Value; // gan thoi gian choi theo level

               // iTime = 10;
                _gameStart = true;

            }

        }

        private void timer_Tick(object sender, EventArgs e)
        {
            if (!_gameOver)
            {
                lblProgressStatus.Content = TimeSpan.FromSeconds(iTime).ToString(@"mm\:ss");
                iTime--;

            }

            if (iTime < 0) // checkWin
            {
                timer.Stop();
                _gameOver = true;
                MessageBox.Show("Game over!");
            }

        }

        private void BtnChoose_Click(object sender, RoutedEventArgs e)
        {

            if (!_gameStart)
            {

                var screen = new OpenFileDialog();
                // rang` buoc input phai la dinh dang hinh anh
                screen.Filter = "Image files (*.jpg, *.jpeg, *.jpe, *.jfif, *.png) | *.jpg; *.jpeg; *.jpe; *.jfif; *.png"; 

                if (screen.ShowDialog() == true)
                {
                    var source = new BitmapImage(new Uri(screen.FileName, UriKind.Absolute));

                    previewImage.Width = previewImageWitdth;
                    previewImage.Height = previewImageHeight;
                    previewImage.Source = source;

                    Canvas.SetLeft(previewImage, StartXpreImage);
                    Canvas.SetTop(previewImage, StartYpreImage);


                    // TH khong thich anh vua chon, chon lai anh khac !
                    if (croppedImages?.Count > 0)
                    {// da co hinh anh truoc do

                        for (int i = 0; i < croppedImages.Count; i++)
                        {
                            if (_images[i / gameCols, i % gameCols] != null)
                            {
                                uiCanvas.Children.Remove(_images[i / gameCols, i % gameCols]);
                            }

                        }

                    }

                    Debug.WriteLine($"{source.Width} - {source.Height}");

                    croppedImages = new List<Image>();

                    // cat hinh thanh 9 manh
                    for (int i = 0; i < gameRows; i++)
                    {
                        for (int j = 0; j < gameCols; j++)
                        {
                            if (!((i == 2) && (j == 2)))
                            {

                                // calc scale in Pixels for CroppedBitmap
                                var img = previewImage.Source as BitmapSource;


                                var scaleWidth = (img.PixelWidth) / 3;
                                var scaleHeight = (img.PixelHeight) / 3;

                                var rcFrom = new Int32Rect()
                                {
                                    X = j * scaleWidth,
                                    Y = i * scaleHeight,
                                    Width = scaleWidth,
                                    Height = scaleHeight
                                };

                                // x , y , width, height
                                //  var rect = new Int32Rect(j * width, i * height, width, height);
                                var cropBitmap = new CroppedBitmap(source, rcFrom);

                                var cropImage = new Image();
                                cropImage.Stretch = Stretch.Fill;
                                cropImage.Width = ImageWidth;
                                cropImage.Height = ImageHeight;
                                cropImage.Source = cropBitmap;

                                croppedImages.Add(cropImage); // 

                            }
                        }
                    }

                    // Shuffle

                    int t;
                    List<int> Indexes = new List<int>(new int[] { 0, 1, 2, 3, 4, 5, 6, 7, 9 });
                    List<Image> newCroppedImages = new List<Image>();
                    Random r = new Random(); // tao ra the hien moi

                    while (Indexes.Count > 0)
                    {
                        t = Indexes[r.Next(0, Indexes.Count)];
                        if (t != 9)
                        {
                            var img = new Image();
                            img.Stretch = Stretch.Fill;
                            img.Width = ImageWidth;
                            img.Height = ImageHeight;
                            img.Source = croppedImages[t].Source;

                            newCroppedImages.Add(img);
                        }

                        Indexes.Remove(t);
                    }

                    // Random
                    int m, n;
                    for (int i = 0; i < newCroppedImages.Count; i++)
                    {
                        m = i / gameCols;
                        n = i % gameCols;

                        _images[m, n] = newCroppedImages[i];
                        //_a[m, n] = 1; // danh dau o da co hinh anh
                        uiCanvas.Children.Add(newCroppedImages[i]);
                        Canvas.SetLeft(newCroppedImages[i], StartX + n * (Width_Shape + 2));
                        Canvas.SetTop(newCroppedImages[i], StartY + m * (Height_Shape + 2));
                        //cropImage.MouseLeftButtonDown += CropImage_MouseLeftButtonDown;
                        //cropImage.PreviewMouseLeftButtonUp += CropImage_PreviewMouseLeftButtonUp;
                        //cropImage.Tag = new Tuple<int, int>(i, j); 
                        newCroppedImages[i].MouseLeftButtonDown += MainWindow_MouseLeftButtonDown;
                        newCroppedImages[i].PreviewMouseLeftButtonUp += MainWindow_PreviewMouseLeftButtonUp;
                        newCroppedImages[i].Tag = new Tuple<int, int>(m, n);


                    }
                    emptyImage = new Tuple<int, int>(2, 2); // empty



                }
            }
        }

        bool _isDragging = false;
        Image _selectedBitmap = null;
        Point _lastPosition;
        bool _isMove = true;
        bool _checkEmptyBro = false;
        bool _gameOver = false;
        int iTime = 0;
        bool _gameStart = false;

        int[] temp_i = new int[] { -1, 0, 0, 1 };
        int[] temp_j = new int[] { 0, -1, 1, 0 };

        private void MainWindow_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {

            if (_gameStart && !_gameOver)
            {
                _isDragging = false;
                var Position = e.GetPosition(this);

                var (m, n) = _selectedBitmap.Tag as Tuple<int, int>;


                for (int t = 0; t < temp_i.Length; t++) // kiem tra 4 vi tri xung quanh Image duoc chon
                {
                    var idx_i = temp_i[t] + m;
                    var idx_j = temp_j[t] + n;

                    if ((idx_i >= 0 && idx_i <= 2) && (idx_j >= 0 && idx_j <= 2)) // cac vi tri ton tai trong mang
                    {
                        if (idx_i == emptyImage.Item1 && idx_j == emptyImage.Item2)
                            _checkEmptyBro = true;
                    }
                }


                if (_isMove && _checkEmptyBro)
                {

                    int x = (int)(Position.X - StartX) / (Width_Shape + 2) * (Width_Shape + 2) + StartX;
                    int y = (int)(Position.Y - StartY - Ribbon_Height) / (Height_Shape + 2) * (Height_Shape + 2) + StartY;

                    if (x == StartX + emptyImage.Item2 * (Width_Shape + 2) && (y == StartY + emptyImage.Item1 * (Height_Shape + 2))) // di chuyen vao o Empty
                    {
                        Canvas.SetLeft(_selectedBitmap, x);
                        Canvas.SetTop(_selectedBitmap, y);
                        int i = emptyImage.Item1;
                        int j = emptyImage.Item2;
                        _selectedBitmap.Tag = new Tuple<int, int>(i, j);
                        _images[i, j] = _selectedBitmap;

                        emptyImage = new Tuple<int, int>(m, n);

                        // MessageBox.Show($"{m}-{n}"); // checkWin tai day

                        _gameOver = CheckWin() == true ? true: false;

                        if (_gameOver && iTime > 0)
                        {
                            MessageBox.Show("YouWin");
                        }


                    }
                    else // Di chuyen vao o da co Image => tro lai ve tri cu
                    {
                        Canvas.SetLeft(_selectedBitmap, StartX + n * (Width_Shape + 2));
                        Canvas.SetTop(_selectedBitmap, StartY + m * (Height_Shape + 2));
                    }
                    _checkEmptyBro = false; // cap nhat lai bien check

                }
                else
                {
                    Canvas.SetLeft(_selectedBitmap, StartX + n * (Width_Shape + 2));
                    Canvas.SetTop(_selectedBitmap, StartY + m * (Height_Shape + 2));
                }
            }


        }

        private bool CheckWin()
        {
            int i;
            for(i=0;i<croppedImages.Count;i++)
            {
                if(croppedImages[i] != _images[i / gameCols, i % gameCols]) // 
                {                
                    break;
                }
            }
            if (i == croppedImages.Count) return true;

            return false;
        }

        private void MainWindow_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if(_gameStart && !_gameOver)
            {
                _isDragging = true;
                _selectedBitmap = sender as Image;
                var (m, n) = _selectedBitmap.Tag as Tuple<int, int>;
                _lastPosition = e.GetPosition(this);
            }

        }

        private void RibbonWindow_MouseMove(object sender, MouseEventArgs e)
        {

                var position = e.GetPosition(this);

                int i = ((int)position.Y - StartY - Ribbon_Height) / Height_Shape;
                int j = ((int)position.X - StartX) / Width_Shape;

                this.Title = $"{position.X} - {position.Y} , a[{i}][{j}]";

                if (_isDragging)
                {

                    var dx = position.X - _lastPosition.X;
                    var dy = position.Y - _lastPosition.Y;

                    var lastLeft = Canvas.GetLeft(_selectedBitmap);
                    var lastTop = Canvas.GetTop(_selectedBitmap);

                    Canvas.SetLeft(_selectedBitmap, lastLeft + dx);
                    Canvas.SetTop(_selectedBitmap, lastTop + dy);
                    _lastPosition = position;
                    if ((i < 0 || i > 2) || (j < 0 || j > 2)) // TH manh hinh bi keo ra ben ngoai Shape
                    {
                        _isMove = false;
                    }
                    else // TH nam trong Shape -> cho phep di chuyen
                    {
                        _isMove = true;
                    }
                }
            
        }

        private void BtnUp_Click(object sender, RoutedEventArgs e)
        {

            if (_gameStart && !_gameOver)
            {
                var m = emptyImage.Item1;
                var n = emptyImage.Item2;

                var idx_i = 1 + m;
                var idx_j = 0 + n;

                if (m != gameRows - 1)
                {
                    Canvas.SetLeft(_images[idx_i, idx_j], StartX + n * (Width_Shape + 2));
                    Canvas.SetTop(_images[idx_i, idx_j], StartY + m * (Height_Shape + 2));

                    _images[m, n] = _images[idx_i, idx_j];

                    _images[idx_i, idx_j].Tag = new Tuple<int, int>(m, n);
                    emptyImage = new Tuple<int, int>(idx_i, idx_j);// cap nhat emptyImage

                    _gameOver = CheckWin() == true ? true : false;

                    if (_gameOver && iTime > 0)
                    {
                        MessageBox.Show("YouWin");
                    }
                }
            }

        }

        private void BtnDown_Click(object sender, RoutedEventArgs e)
        {
            if (_gameStart && !_gameOver)
            {
                var m = emptyImage.Item1;
                var n = emptyImage.Item2;

                var idx_i = -1 + m;
                var idx_j = 0 + n;

                if (m != 0)
                {
                    Canvas.SetLeft(_images[idx_i, idx_j], StartX + n * (Width_Shape + 2));
                    Canvas.SetTop(_images[idx_i, idx_j], StartY + m * (Height_Shape + 2));

                    _images[m, n] = _images[idx_i, idx_j];

                    _images[idx_i, idx_j].Tag = new Tuple<int, int>(m, n);
                    emptyImage = new Tuple<int, int>(idx_i, idx_j);// cap nhat emptyImage

                    _gameOver = CheckWin() == true ? true : false;

                    if (_gameOver && iTime > 0)
                    {
                        MessageBox.Show("YouWin");
                    }


                }
            }
        }

        private void BtnLeft_Click(object sender, RoutedEventArgs e)
        {
            if (_gameStart && !_gameOver)
            {
                var m = emptyImage.Item1;
                var n = emptyImage.Item2;

                var idx_i = 0 + m;
                var idx_j = 1 + n;

                if (n != gameCols - 1)
                {
                    Canvas.SetLeft(_images[idx_i, idx_j], StartX + n * (Width_Shape + 2));
                    Canvas.SetTop(_images[idx_i, idx_j], StartY + m * (Height_Shape + 2));

                    _images[m, n] = _images[idx_i, idx_j];

                    _images[idx_i, idx_j].Tag = new Tuple<int, int>(m, n);
                    emptyImage = new Tuple<int, int>(idx_i, idx_j);// cap nhat emptyImage

                    _gameOver = CheckWin() == true ? true : false;

                    if (_gameOver && iTime > 0)
                    {
                        MessageBox.Show("YouWin");
                    }

                }
            }
        }

        private void BtnRight_Click(object sender, RoutedEventArgs e)
        {
            if (_gameStart && !_gameOver)
            {
                var m = emptyImage.Item1;
                var n = emptyImage.Item2;

                var idx_i = 0 + m;
                var idx_j = -1 + n;

                if (n != 0)
                {
                    Canvas.SetLeft(_images[idx_i, idx_j], StartX + n * (Width_Shape + 2));
                    Canvas.SetTop(_images[idx_i, idx_j], StartY + m * (Height_Shape + 2));

                    _images[m, n] = _images[idx_i, idx_j];

                    _images[idx_i, idx_j].Tag = new Tuple<int, int>(m, n);
                    emptyImage = new Tuple<int, int>(idx_i, idx_j);// cap nhat emptyImage

                    _gameOver = CheckWin() == true ? true : false;

                    if (_gameOver && iTime > 0)
                    {
                        MessageBox.Show("YouWin");
                    }

                }
            }

        }


        private void Button_SaveGame_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Tinh nang dang phat trien");
        }
    }

   
}

