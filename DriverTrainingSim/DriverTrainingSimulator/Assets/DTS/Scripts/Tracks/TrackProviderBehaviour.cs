using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using NUnit.Framework;
using System;
using System.Runtime.InteropServices;

namespace DTS.Tracks
{
    /// <summary>
    /// Monobehaviour interface to the track provider
    /// </summary>
    public class TrackProviderBehaviour : MonoBehaviour, ITrackFactory, IFileInterface {

        public TrackProvider provider;

        // Use this for initialization
        void Start () {
            provider.SetTrackFactory(this);
	    }
	
	    // Update is called once per frame
	    void Update () {
	
	    }

        #region IFileInterface Implementation

        public List<TrackPoint> Read()
        {
            return provider.Read();
        }

        public void Write(List<TrackPoint> points)
        {
            throw new NotImplementedException();
        }

        #endregion

    }

    /// <summary>
    /// 
    /// </summary>
    public interface ITrackFactory
    {

    }

    /// <summary>
    /// 
    /// </summary>
    public interface IFileInterface
    {
        List<TrackPoint> Read();
        void Write(List<TrackPoint> points);
    }

    /// <summary>
    /// Holds all results of track generation and all 
    /// </summary>
    [System.Serializable]
    public class TrackProvider
    {
        #region Humble Object Pattern

        private ITrackFactory trackFactory;
        private IFileInterface fileInterface;

        /// <summary>
        /// Sets the ITrackFactory interface for this class
        /// </summary>
        /// <param name="TrackFactory"></param>
        public void SetTrackFactory(ITrackFactory TrackFactory)
        {
            trackFactory = TrackFactory;
        }

        /// <summary>
        /// Sets the ITrackFactory interface for this class
        /// </summary>
        /// <param name="TrackFactory"></param>
        public void SetFileInterface(IFileInterface FileInterface)
        {
            fileInterface = FileInterface;
        }

        #endregion

        /// <summary>
        /// A dictionary of all available track gameobjects, referenced by their names
        /// </summary>
        public Dictionary<string, TrackBehaviour> Tracks = new Dictionary<string, TrackBehaviour>();

        internal List<TrackPoint> Read()
        {
            throw new NotImplementedException();
        }

        internal List<TrackPoint> Write()
        {
            throw new NotImplementedException();
        }

    }

    /// <summary>
    /// 
    /// </summary>
    public class FileInterface
    {
        public void ReadGpxFile()
        {
            XmlTextReader xtr = new XmlTextReader("..\\..\\..\\..\\testCases.xml");
            List<TrackPoint> trackPoints = new List<TrackPoint>();

            xtr.Read(); // advance to <gpx> tag

            // advance through catching each trackpoint
            while (!xtr.EOF) //load loop
            {
                if(xtr.Name == "trkpt")
                {
                    // read the gps coordinates
                    TrackPoint trkpt = new TrackPoint(float.Parse(xtr.GetAttribute("lat")), float.Parse(xtr.GetAttribute("lon")));

                    // advance to heading
                    do
                    {
                        xtr.Read();
                    }
                    while (!(xtr.Name == "heading"));

                    // read the heading
                    trkpt.heading = float.Parse(xtr.ReadElementString("heading"));

                }

                xtr.Read(); // advance
            } 

        }
    }

    public class TrackPoint
    {
        public TrackPoint(float Lat, float Lon)
        {
            lat = Lat;
            lon = Lon;
        }

        public float lat;
        public float lon;
        public float heading;
    }
    
}
