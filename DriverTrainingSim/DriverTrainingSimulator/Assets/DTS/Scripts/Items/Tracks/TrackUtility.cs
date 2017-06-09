using UnityEngine;

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml.Linq;
using System.Runtime.Serialization.Formatters.Binary;

namespace DTS.Items.Tracks
{
    public enum trackType { Generic, Autocross, Skidpad, Endurance }
    public enum ConversionType { CircumferenceRatio }

    /// <summary>
    /// Track object for use within unity, not serializable
    /// </summary>
    public class GameTrack : Track
    {
        /// <summary>
        /// Mesh generated from track data
        /// </summary>
        public Mesh TrackMesh;

        /// <summary>
        /// The game object that has the properties of this track
        /// </summary>
        private GameObject trackObject;

        /// <summary>
        /// Returns the game object for this track
        /// </summary>
        /// <returns></returns>
        public GameObject TrackObject
        {
            get
            {
                // TODO make game object from the mesh and return it
                return TrackObject;
            }
        }

        /// <summary>
        /// Creates a track from a .trkpt
        /// </summary>
        /// <param name="ind"></param>
        public GameTrack(int ind) : base(ind)
        {
            // TODO mesh generation
        }

        /// <summary>
        /// Creates a new game track from a .gpx file
        /// </summary>
        public GameTrack(string path) : base(path)
        {
            // TODO mesh generation
        }

    }

    /// <summary>
    /// Base Serializable track object
    /// </summary>
    [Serializable]
    public class Track
    {
        #region Static 

        #region Private Fields

        /// <summary>
        /// Indicates if an instance of this class has been created
        /// </summary>
        private static bool InstanceExists = false;

        /// <summary>
        /// Path to track saves folder
        /// </summary>
        private static string SavesPath = Application.dataPath + "\\Assets\\DTS\\Save Data\\Tracks";
        
        /// <summary>
        /// Manifest
        /// </summary>
        private static string ManifestPath = SavesPath + "\\Tracks.mfst";

        /// <summary>
        /// All saved tracks
        /// </summary>
        private static List<Track> savedTracks;

        #endregion

        #region Public Fields



        #endregion

        #region Public Methods

        /// <summary>
        /// Loads a new track
        /// </summary>
        /// <returns></returns>
        public static Track TrackFromGPX(string path)
        {
            Track t = new Track();
            AddPointsFromGPX(path, t);
            return t;
        }

        /// <summary>
        /// Saves a track object to a .track file
        /// </summary>
        public static void Save(Track t)
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(SavesPath + "\\" + t.name + ".track", FileMode.OpenOrCreate);
            bf.Serialize(file, t);
        }

        #endregion

        #region Private Methods
        
        /// <summary>
        /// Adds new trackpoints to a provided track object
        /// </summary>
        /// <param name="path"></param>
        /// <param name="t"></param>
        private static void AddPointsFromGPX(string path, Track t)
        {
            XDocument doc = XDocument.Load(path);
            var elements = doc.Elements("trkpt");
            foreach (XElement e in elements)
            {
                try
                {
                    float lat = Single.Parse(e.Attribute("lat").Value);
                    float lon = Single.Parse(e.Attribute("lon").Value);
                    float ele = Single.Parse(e.Element("ele").Value);
                    float heading = Single.Parse(e.Element("heading").Value);

                    TrackPoint p = new TrackPoint(lat, lon, ele, heading);
                    t.Add(p);

                }
                catch
                {
                    Debug.Log("Corrupted .gpx data point");
                }
            }
        }

        /// <summary>
        /// Initialises static parameters
        /// </summary>
        private static void IntitializeStatic()
        {
            if (!InstanceExists)
            {
                // read from the manifest file if it exists
                string[] trackNames;
                
                if (File.Exists(ManifestPath))
                {
                    trackNames = File.ReadAllLines(ManifestPath);
                }
            }
        }

        #endregion

        #endregion

        #region Instance

        #region Public Fields

        /// <summary>
        /// the given name of this track
        /// </summary>
        public string name;

        /// <summary>
        /// The type of track
        /// </summary>
        public trackType TrackType;

        #endregion

        #region Properties

        private TrackPoint firstPoint;
        public TrackPoint FirstPoint
        {
            get
            {
                if (firstPoint == null)
                {
                    firstPoint = TrackPoints.Peek();
                }
                return firstPoint;
            }
            private set { firstPoint = value; }
        }

        #endregion

        #region Private Fields

        private Queue<TrackPoint> TrackPoints = new Queue<TrackPoint>();

        #endregion

        #region Constructors

        /// <summary>
        /// Creates a new track
        /// </summary>
        public Track()
        {
            IntitializeStatic();
        }

        /// <summary>
        /// Creates a new track and loads points from a GPX file
        /// </summary>
        /// <param name="path"></param>
        public Track(string path)
        {
            IntitializeStatic();
            AddPointsFromGPX(path, this);
        }

        
        /// <summary>
        /// Creates a new track from a .trkpt file
        /// </summary>
        /// <param name="ind"></param>
        public Track(int ind)
        {

        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Loads points into this track from a GPX file
        /// </summary>
        public void LoadPointsFromGPX(string path)
        {
            AddPointsFromGPX(path, this);
        }

        /// <summary>
        /// Adds a trackpoint to the queue
        /// </summary>
        /// <param name="newPoint"></param>
        public void Add(TrackPoint newPoint)
        {
            TrackPoints.Enqueue(newPoint);
        }

        /// <summary>
        /// Returns a copy of the trackpoints queue object
        /// </summary>
        /// <param name="newPoint"></param>
        /// <returns></returns>
        public Queue<TrackPoint> GetPoints(TrackPoint newPoint)
        {
            return new Queue<TrackPoint>(TrackPoints);
        }

        #endregion

        #region Private Methods

        

        #endregion

        #endregion

    }

    /// <summary>
    /// Represents a reported GPS point in a track
    /// </summary>
    [Serializable]
    public class TrackPoint
    {

        public float? lat;
        public float? lon;
        public float? ele;
        public float? heading;

        /// <summary>
        /// Creates a new trackpoint
        /// </summary>
        /// <param name="Lat"></param>
        /// <param name="Lon"></param>
        /// <param name="Ele"></param>
        public TrackPoint(float Lat, float Lon, float Ele, float Heading)
        {
            lat = Lat;
            lon = Lon;
            ele = Ele;
            heading = Heading;
            float X;
            float Y;
            float Z;
            GPSConverter.GPSToCartesian(Lat, Lon, Ele, out X, out Y, out Z, ConversionType.CircumferenceRatio);
            x = X;
            y = Y;
            z = Z;
        }

        /// <summary>
        /// Returns the Unity Vector3 represenation of this trackpoint
        /// </summary>
        /// <returns>Unity Vector of Point</returns>
        public Vector3 ToUnityVector3()
        {
            return new Vector3(x, y, z);
        }

        /// <summary>
        /// Returns a quaternion of the heading at this trackpoint
        /// </summary>
        /// <returns></returns>
        public Quaternion ToUnityQuaternion()
        {
            return Quaternion.Euler(0, (float)heading, 0);
        }

        private float? _x_original;
        private float? _x;

        /// <summary>
        /// The x value of this point, returns the original GPS point unless its been modified
        /// </summary>
        public float x
        {
            get
            {
                if (_x != null)
                    return (float)_x;
                else
                    return 0f;
            }

            // set the original the first time this is called, then only change the modified one
            set
            {
                if (_x_original == null)
                    _x_original = value;

                _x = value;
            }
        }

        private float? _y_original;
        private float? _y;

        /// <summary>
        /// The y value of this point, returns the original GPS point unless its been modified
        /// </summary>
        public float y
        {
            get
            {
                if (_y != null)
                    return (float)_y;
                else
                    return 0f;
            }

            // set the original the first time this is called, then only change the modified one
            set
            {
                if (_y_original == null)
                    _y_original = value;
                _y = value;

            }
        }

        private float? _z_original;
        private float? _z;

        /// <summary>
        /// The z value of this point, returns the original GPS point unless its been modified
        /// </summary>
        public float z
        {
            get
            {
                if (_z != null)
                    return (float)_z;
                else
                    return 0f;
            }

            // set the original the first time this is called, then only change the modified one
            set
            {
                if (_z_original == null)
                    _z_original = value;

                 _z = value;
            }
        }

    }

    /// <summary>
    /// Static utility class with GPS conversion functions
    /// </summary>
    public static class GPSConverter
    {
        private const float ECtoLLRatio = 111319.44444f;

        /// <summary>
        /// Converts lat long and elevation data to cartesian coordinates using a number of methods
        /// </summary>
        /// <param name="lat"></param>
        /// <param name="lon"></param>
        /// <param name="ele"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        /// <param name="t"></param>
        public static void GPSToCartesian(float lat, float lon, float ele, out float x, out float y, out float z, ConversionType t)
        {
            switch (t)
            {
                // Assumes flat earth level surface
                case ConversionType.CircumferenceRatio:
                    z = lat * ECtoLLRatio;
                    x = lon * ECtoLLRatio;
                    y = 0;
                    break;
                default:
                    x = 0;
                    y = 0;
                    z = 0;
                    break;
            }
        }
    }

}

