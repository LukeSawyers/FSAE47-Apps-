using UnityEngine;
using System.Collections.Generic;

namespace DTS.Tracks
{
    /// <summary>
    /// Represents a track GameObject
    /// </summary>
    public class TrackBehaviour : MonoBehaviour, ITrack
    {
        public Track track;
    }

    public interface ITrack
    {

    }

    /// <summary>
    /// 
    /// </summary>
    [System.Serializable]
    public class Track
    {
        private ITrack track;

        public void SetTrack(ITrack track)
        {
            this.track = track;
        }
    }
}