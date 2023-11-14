using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace WatchV2WPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public Point Centerwatch { get; set; }
        public double Radius { get; set; }
        public Line? HourLine { get; set; } = null;
        public Line? MinuteLine { get; set; } = null;
        public Line? SecoundLine { get; set; } = null;
        public DispatcherTimer WatchTimer { get; set; }
        public MainWindow()
        {
            InitializeComponent();
        }



        private void watchcan_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            Centerwatch = new Point(watchcan.ActualWidth / 2, watchcan.ActualHeight / 2);
            Radius = (watchcan.ActualWidth / 2) > (watchcan.ActualHeight /2) ? watchcan.ActualHeight /2 : watchcan.ActualWidth / 2;
            RedrawWatch();
        }

        private void RedrawWatch()
        {
            watchcan.Children.Clear();
            RedrawDesign();
            RedrawPointers();
        }

        private void RedrawPointers()
        {
            HourLine = new Line { Stroke = new SolidColorBrush(Colors.Blue), StrokeThickness = 3 };
            MinuteLine = new Line { Stroke = new SolidColorBrush(Colors.Green), StrokeThickness = 2 };
            SecoundLine = new Line { Stroke = new SolidColorBrush(Colors.Yellow), StrokeThickness = 1 };

            HourLine.X1 = MinuteLine.X1 = SecoundLine.X1 = Centerwatch.X;
            HourLine.X2 = MinuteLine.X2 = SecoundLine.X2 = Centerwatch.X;
            HourLine.Y1 = MinuteLine.Y1 = SecoundLine.Y1 = Centerwatch.Y;
            HourLine.Y2 = watchcan.ActualHeight / 2 - (Radius * 0.5);
            MinuteLine.Y2 = watchcan.ActualHeight / 2 - (Radius * 0.6);
            SecoundLine.Y2 = watchcan.ActualHeight / 2 - (Radius * 0.8);

            watchcan.Children.Add(HourLine);
            watchcan.Children.Add(MinuteLine);
            watchcan.Children.Add(SecoundLine);

            var time = DateTime.Now;
            int secondangle = time.Second * 6;
            int minuteangle = (time.Minute * 6) + (time.Second / 10);
            int hourangle = (time.Hour % 12 * 30) + (time.Minute / 2);

            SecoundLine.RenderTransform = new RotateTransform(secondangle, SecoundLine.X1, SecoundLine.Y1);
            MinuteLine.RenderTransform = new RotateTransform(minuteangle, MinuteLine.X1, MinuteLine.Y1);
            HourLine.RenderTransform = new RotateTransform(hourangle, HourLine.X1, HourLine.Y1);
        }

        private void RedrawDesign()
        {
            for (int angle = 0; angle <= 360; angle += 30)
            {
                Line scaleLine = new Line
                {
                    Stroke = new SolidColorBrush(Colors.White),
                    StrokeThickness = 1,
                    X1 = watchcan.ActualWidth /2 ,
                    Y1 = watchcan.ActualHeight /2 - (Radius * 0.8),
                    X2 = watchcan.ActualWidth /2 ,
                    Y2 = watchcan.ActualHeight /2 - (Radius * 0.85)
                };
                RotateTransform rotatescale = new RotateTransform(angle, Centerwatch.X,Centerwatch.Y);
                scaleLine.RenderTransform = rotatescale;
                watchcan.Children.Add(scaleLine);

                if(angle == 0)
                {
                    continue;
                }

                TextBlock tbNumbers = new TextBlock {Text = (angle/30).ToString(), TextAlignment = TextAlignment.Center, Foreground = new SolidColorBrush(Colors.White), Width = 30, Height = 20 };
                //Umgekehrte Reihenfolge !!!!!!!!!
                TransformGroup transformGroupTB = new TransformGroup();
                transformGroupTB.Children.Add(new TranslateTransform(-tbNumbers.Width / 2, -tbNumbers.Height / 2));
                transformGroupTB.Children.Add(new RotateTransform(90 - angle));
                transformGroupTB.Children.Add(new TranslateTransform(Radius * 0.9, 0));
                transformGroupTB.Children.Add(new RotateTransform(-90 + angle));
                transformGroupTB.Children.Add(new TranslateTransform(Centerwatch.X, Centerwatch.Y));
                tbNumbers.RenderTransform = transformGroupTB;
                watchcan.Children.Add(tbNumbers);
            }

            
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            WatchTimer = new DispatcherTimer { IsEnabled = true, Interval = new TimeSpan (0,0,0,0,1000) };
            WatchTimer.Tick += WatchTimer_Tick;
        }

        private void WatchTimer_Tick(object? sender, EventArgs e)
        {
            RedrawWatch();
        }
    }
}
