using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Valentines2015
{
    class MusicBox
    {
        public List<LyricString> Lyrics { get; set; }
        public string MusicFile { get; set; }

        /// <summary>
        /// Create a new MusicBox from a collection of LyricStrings, and a file pointing to a playable music file.
        /// </summary>
        /// <param name="lyricsList"></param>
        /// <param name="musicPath"></param>
        public MusicBox(IEnumerable<LyricString> lyricsList, string musicPath)
        {
            Lyrics = lyricsList.ToList<LyricString>();
            MusicFile = Path.GetFullPath(musicPath);
        }

        /// <summary>
        /// Creates a new MusicBox from an external text file.
        /// </summary>
        /// <param name="externallyDefinedBox">Path to external file.</param>
        public MusicBox(string externallyDefinedBox)
        {
            using (StreamReader sr = new StreamReader(externallyDefinedBox))
            {
                ParseMusicScript(sr);
            }
        }

        /// <summary>
        /// Creates a new MusicBox from an external text file included with the project as a Resource.
        /// </summary>
        /// <param name="uri">The URI of the resource text file.</param>
        public MusicBox(Uri uri)
        {
            using (StreamReader sr = new StreamReader(Application.GetResourceStream(uri).Stream))
            {
                ParseMusicScript(sr);
            }
        }

        private void ParseMusicScript(StreamReader sr)
        {            
            Lyrics = new List<LyricString>(); //Init, or clear previous.

            int lineCount = 0;
            while (!sr.EndOfStream)
            {                
                string currentLine = sr.ReadLine();
                if (currentLine == null)
                {
                    return;
                }
                if (currentLine[0] == '#')
                {
                    continue;
                }

                if (lineCount == 0)
                {
                    MusicFile = Path.GetFullPath(currentLine);
                    if (!File.Exists(MusicFile))
                    {
                        throw new FileNotFoundException($"File {MusicFile} not found.", MusicFile);
                    }
                    lineCount++;
                    continue;
                }                

                //Splitting on pipes ( | ), because commas are valid in song lyrics. Pipes usually aren't.
                string[] line = currentLine.Split('|');
                string lyric = line[0].Replace("\\n", "\n");
                int writeTimeInMillis = Convert.ToInt32(line[1]);
                int waitTimeInMillis = line.Length > 2 ? Convert.ToInt32(line[2]) : 0; //The waitTime is an optional value. If there's nothing there, set it to zero.
                LyricString lyr = new LyricString(lyric, writeTimeInMillis, waitTimeInMillis);
                Lyrics.Add(lyr);
            }

        }
    }
}
