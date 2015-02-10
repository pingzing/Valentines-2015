using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Valentines2014
{
    class LyricString
    {
        public string Lyric { get; set; }
        public int CurrentLetter { get; set; }

        private int millisecondsToWrite = 0;
        public int MillisecondsToWrite { get { return millisecondsToWrite; } set { millisecondsToWrite = value; } }

        private int millisecondsToWait = 0;
        public int MillisecondsToWait { get { return millisecondsToWait; } set { millisecondsToWait = value; } }    

        public LyricString(string newLyric)
        {
            Lyric = newLyric;
            millisecondsToWrite = 0;
            millisecondsToWait = 0;
        }

        public LyricString(string newLyric, int newMsToWrite)
        {
            Lyric = newLyric;
            millisecondsToWrite = newMsToWrite;
            millisecondsToWait = 0;
        }

        public LyricString(string newLyric, int newMsToWrite, int newMsToWait)
        {
            Lyric = newLyric;
            millisecondsToWrite = newMsToWrite;
            millisecondsToWait = newMsToWait;
        }
    }
}
