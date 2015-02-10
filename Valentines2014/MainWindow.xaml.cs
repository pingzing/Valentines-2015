using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Media;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Threading;

namespace Valentines2014
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Queue<LyricString> LyricsQueue = new Queue<LyricString>();

        public MainWindow()
        {
            InitializeComponent();
            this.PrintReady += MainWindow_PrintReady;
            PlayValentine();
        }

        private void PlayValentine()
        {                    
            //Solution-relative file.
            MusicBox eightMelodiesBox = new MusicBox(new Uri("Valentines2014;component/8_melodies_musicbox.txt", UriKind.Relative));
            FillQueue(eightMelodiesBox);
            if (!String.IsNullOrEmpty(eightMelodiesBox.MusicFile))
            {
                SoundPlayer player = new SoundPlayer(eightMelodiesBox.MusicFile);
                player.Play();
            }            
            MainWindow_PrintReady(null, null);
        }

        private void FillQueue(MusicBox box)
        {
            foreach(LyricString lyr in box.Lyrics)
            {
                LyricsQueue.Enqueue(lyr);
            }
        }

        //This may not even need to be an event handler.
        async void MainWindow_PrintReady(object sender, EventArgs e)
        {
            //Wait out the previous Lyric's WaitTime.
            LyricString oldLyr = sender as LyricString;
            if (oldLyr != null)
            {
                await Task.Delay(oldLyr.MillisecondsToWait);
            }

            LyricString lyr = LyricsQueue.Dequeue();
            TimeSpan pauseTimeInMillis = new TimeSpan(0, 0, 0, 0, lyr.MillisecondsToWrite / lyr.Lyric.Length);

            DispatcherTimer dTimer = new DispatcherTimer();
            dTimer.Interval = pauseTimeInMillis;

            //Each tick prints a single character & moves to next line if available
            dTimer.Tick += (ob, ev) =>
                {
                    MainTextBlock.Text += lyr.Lyric[lyr.CurrentLetter];
                    if (lyr.CurrentLetter == lyr.Lyric.Length - 1)
                    {
                        dTimer.Stop();
                        if (LyricsQueue.Count > 0)
                        {
                            MainWindow_PrintReady(lyr, null);
                        }
                    }
                    else
                    {
                        lyr.CurrentLetter++;
                    }
                };
            dTimer.Start();
        }

        private void OnPrintReady(object sender, EventArgs e)
        {
            EventHandler handler = PrintReady;
            if (handler != null)
            {
                handler(sender, e);
            }
        }

        public event EventHandler PrintReady;

        private void OpenMenuItem_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
