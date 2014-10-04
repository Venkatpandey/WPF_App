using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.IO; // stream und Speicherung ... File
using System.Windows.Threading;
using System;
using System.Globalization;
using Microsoft.Win32;  // für FileDialog


public partial class Window1 : Window
{
    bool vidStopped = false;
        
    public Window1()
    {
        InitializeComponent();
    }

    void timer_Tick(object sender, System.EventArgs e)
    {
        //pB_progress.Maximum = video.NaturalDuration.TimeSpan.TotalSeconds;
        pB_progress.Value = video.Position.TotalSeconds;
    }

    private void b_Click(object sender, RoutedEventArgs e)
    {
        Close(); 
    }

    private void progressBar1_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
    {
        video.Volume = volSlider.Value;
    }

    private void slider1_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
    {
        video.Volume = volSlider.Value;
    }

    private void b_farbe_Click(object sender, RoutedEventArgs e)
    {
        Button btn = (Button) e.OriginalSource;
        if (btn != null)
            f_myWindow.Background = btn.Background;
    }

    private void f_myWindow_Loaded(object sender, RoutedEventArgs e)
    {
        video.Play();
        DispatcherTimer timer = new DispatcherTimer(); // benötigt Threading
        timer.Interval = new TimeSpan(0, 0, 1);
        timer.Tick += new EventHandler(timer_Tick); 
        timer.Start();
    }

    private void b_abfoto_Click(object sender, RoutedEventArgs e)
    {        
        RenderTargetBitmap bmp = 
            new RenderTargetBitmap((int)video.ActualWidth, (int)video.ActualHeight, 96d, 96d, PixelFormats.Pbgra32);       
        bmp.Render(video);
        PngBitmapEncoder png = new PngBitmapEncoder();

        png.Frames.Add(BitmapFrame.Create(bmp));
        Clipboard.SetImage(bmp);

        using (Stream stm = File.Create("test.png"))
        {
            png.Save(stm);
        }
    }

    private void b_clip_Click(object sender, RoutedEventArgs e)
    {
        EllipseGeometry gm;
        string clpArt = video.Tag.ToString();
        switch (clpArt) {
            case "1":
                gm = new EllipseGeometry(new Point(200, 180), 180, 180);
                video.Clip = gm;
                video.Tag = "2";
                break;
            case "2":
                Typeface typeFace = new Typeface(new FontFamily("Broadway"), 
                    FontStyles.Normal, FontWeights.ExtraBold, FontStretches.Condensed);
                FormattedText formattedText = new FormattedText("TEXT", CultureInfo.InvariantCulture, 
                    FlowDirection.LeftToRight, typeFace, 150, Brushes.Black);
                video.Clip = formattedText.BuildGeometry(new Point(0,20));
                video.Tag = "0";
                break;
            default:
                video.Clip = null;
                video.Tag = "1";
                break;
        }         
    }

    private void b_dreh_MouseUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
    {
        double winkel = (double)video.RenderTransform.GetValue(RotateTransform.AngleProperty);
        if (e.ChangedButton == System.Windows.Input.MouseButton.Left)
            winkel += 5;
        else
            winkel -= 5;
        RotateTransform rt = new RotateTransform(winkel, 200, 180);
        video.RenderTransform = rt;
    }

    private void video_mouseUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
    {
        if (e.ClickCount == 2)
            MessageBox.Show("bb");
        else
        {
            if (video.SpeedRatio == 1)
                video.SpeedRatio = 0;
            else
                video.SpeedRatio = 1;
        }
    }

    private void video_MouseWheel(object sender, System.Windows.Input.MouseWheelEventArgs e)
    {
        double op = video.Opacity + e.Delta/1000.0;
        if (op<0) op=0.0;
        if (op > 1) op = 1.0;
        video.SetValue(MediaElement.OpacityProperty, op);
    }

    private void pB_progress_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
    {
        MessageBox.Show(pB_progress.Value.ToString());
    }

    private void b_stop_Click(object sender, RoutedEventArgs e)
    {
        video.LoadedBehavior = MediaState.Manual;
        if (vidStopped)
            video.Play();
        else
            video.Stop();
        vidStopped = !vidStopped;

    }

    private void b_open_Click(object sender, RoutedEventArgs e)
    {
        OpenFileDialog dat = new OpenFileDialog();
        dat.Filter = "Videodateien|*.wmv;*.avi;*.mpg";
        if (dat.ShowDialog() == true)
        {
            video.Source = new Uri(dat.FileName);
            video.SpeedRatio = 1;
        }
    }

}