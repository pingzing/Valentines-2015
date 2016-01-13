using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Valentines2015
{
    class LyricString
    {
        public string Lyric { get; set; }
        public int CurrentLetterIndex { get; set; }

        public int MillisecondsToWrite { get; set; } = 0;

        public int MillisecondsToWait { get; set; } = 0;

        public LyricString(string newLyric)
        {
            Lyric = newLyric;
            MillisecondsToWrite = 0;
            MillisecondsToWait = 0;
        }

        public LyricString(string newLyric, int newMsToWrite)
        {
            Lyric = newLyric;
            MillisecondsToWrite = newMsToWrite;
            MillisecondsToWait = 0;
        }

        public LyricString(string newLyric, int newMsToWrite, int newMsToWait)
        {
            Lyric = newLyric;
            MillisecondsToWrite = newMsToWrite;
            MillisecondsToWait = newMsToWait;
        }
    }
}
