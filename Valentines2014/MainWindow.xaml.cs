using System;
using System.Collections.Generic;
using System.ComponentModel;
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
            
            if (this.PrintReady == null)
            {
                this.PrintReady += MainWindow_PrintReady;
            }
        }

        MediaPlayer mp3Player = new MediaPlayer();
        private void PlayValentine(string scriptPath)
        {
            MusicBox box = new MusicBox(scriptPath);
            FillQueue(box);
            if (!String.IsNullOrEmpty(box.MusicFile))
            {
                if(!File.Exists(box.MusicFile))
                {
                    throw new FileNotFoundException("Sound file not found in PlayValentine.", box.MusicFile);
                }
                if (Path.GetExtension(box.MusicFile) == ".wav")
                {
                    SoundPlayer player = new SoundPlayer(box.MusicFile);
                    player.Play();
                }
                else if(Path.GetExtension(box.MusicFile) == ".mp3")
                {                    
                    mp3Player.Stop();
                    mp3Player.Open(new Uri(box.MusicFile));
                    BackgroundWorker worker = new BackgroundWorker();   
                    //On a Background worker so it doesn't stop playing when the DispatcherTimer in PrintReady updates.
                    //Then, invoked through the Dispatcher so it has access to the global MediaPlayer.
                    //MediaPlayer is global so we can only have one MP3 playing at a time!
                    worker.DoWork += (s, e) =>
                    {
                        this.Dispatcher.Invoke(() =>
                        {
                            MediaPlayer threadLocalPlayer = e.Argument as MediaPlayer;
                            threadLocalPlayer.Play();
                        });                        
                    };
                    worker.RunWorkerAsync(mp3Player);
                }
                else
                {
                    throw new ArgumentException("Extension is not .mp3 or .wav.", box.MusicFile);                   
                }
            }
            MainWindow_PrintReady(null, null);
        }

        private void FillQueue(MusicBox box)
        {
            foreach (LyricString lyr in box.Lyrics)
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
            var open = new OpenWindow();

            if (open.ShowDialog() == true)
            {
                try
                {
                    PlayValentine(open.ReturnSelection.Path);
                }         
                catch(FileNotFoundException ex)
                {
                    MessageBox.Show("Could not find file at " + ex.FileName + ".", "Error - File not found", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                catch(ArgumentException ex)
                {
                    MessageBox.Show(ex.ParamName + " is not an MP3 or a WAV.", "Error - Invalid file", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
    }
}
