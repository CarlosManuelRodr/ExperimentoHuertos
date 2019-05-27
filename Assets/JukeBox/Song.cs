using UnityEngine;

namespace CorruptedSmileStudio.JukeBox
{
    /// <summary>
    ///	A class that represents a song.
    ///	</summary>
    [System.Serializable]
    public class Song
    {
        /// <summary>
        ///	The audio clip to play.
        ///	</summary>
        public AudioClip clip;
        /// <summary>
        ///	The name of the artist
        ///	</summary>
        public string artist;
        /// <summary>
        ///	The title of the song.
        ///	</summary>		
        public string title;

        /// <summary>
        ///	Returns a nicely formated artist - title string.
        ///	</summary>
        /// <returns>A formated string of ARTIST - TITLE</returns>
        public override string ToString()
        {
            return string.Format("{0} - {1}", artist, title);
        }
    }
}