using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Media;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Threading;
using Valentines2014;
using Valentines2015.MVVM;

namespace Valentines2015
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : BindableWindowBase
    {
        private readonly Queue<LyricString> _lyricsQueue = new Queue<LyricString>();
        private readonly MediaPlayer _mp3Player = new MediaPlayer();
        private SoundPlayer _wavPlayer = new SoundPlayer();
        

        public MainWindow()
        {
            InitializeComponent();                       
        }        
        private async Task PlayValentine(string scriptPath)
        {
            MusicBox box = new MusicBox(scriptPath);
            MainTextBlock.Text = "";
            if(box.TextColor.HasValue)
            {
                MainTextBlock.Foreground = new SolidColorBrush(box.TextColor.Value);
            }
            else
            {
                MainTextBlock.Foreground = new SolidColorBrush(Colors.Red);
            }
            foreach (LyricString lyr in box.Lyrics)
            {
                _lyricsQueue.Enqueue(lyr);
            }
            if (!String.IsNullOrEmpty(box.MusicFile))
            {
                if(!File.Exists(box.MusicFile))
                {
                    throw new FileNotFoundException("Sound file not found in PlayValentine.", box.MusicFile);
                }
                if (Path.GetExtension(box.MusicFile) == ".wav")
                {
                    _wavPlayer = new SoundPlayer(box.MusicFile);
                    _wavPlayer.Play();
                }
                else if(Path.GetExtension(box.MusicFile) == ".mp3")
                {                    
                    _mp3Player.Stop();
                    _mp3Player.Open(new Uri(box.MusicFile));
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
                    worker.RunWorkerAsync(_mp3Player);
                }
                else
                {
                    throw new ArgumentException("Extension is not .mp3 or .wav.", box.MusicFile);                   
                }
            }
            await PrintLinesRecursive(null);
        }        
        
        private async Task PrintLinesRecursive(LyricString previousLyric)
        {            
            //Wait out the previous Lyric's WaitTime.            
            if (previousLyric != null)
            {
                await Task.Delay(previousLyric.MillisecondsToWait);
            }

            LyricString lyr = _lyricsQueue.Dequeue();
            TimeSpan pauseTimeInMillis = new TimeSpan(0, 0, 0, 0, lyr.MillisecondsToWrite / lyr.Lyric.Length);

            DispatcherTimer dTimer = new DispatcherTimer
            {
                Interval = pauseTimeInMillis
            };

            //Each tick prints a single character. If we're at the end of the line, 
            //it stops the current timer-loop and makes a new method call.
            dTimer.Tick += (ob, ev) =>
                {
                    MainTextBlock.Text += lyr.Lyric[lyr.CurrentLetterIndex];
                    if (lyr.CurrentLetterIndex == lyr.Lyric.Length - 1)
                    {
                        dTimer.Stop();
                        if (_lyricsQueue.Count > 0)
                        {
                            PrintLinesRecursive(lyr).AndIgnore(); 
                        }                        
                    }
                    else
                    {
                        lyr.CurrentLetterIndex++;
                    }
                };
            dTimer.Start();
        }
              

        private async void OpenMenuItem_Click(object sender, RoutedEventArgs e)
        {
            var open = new OpenWindow();

            if (open.ShowDialog() == true)
            {
                try
                {
                    await PlayValentine(open.ReturnSelection.Path);
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
