using UnityEngine;
using System.Collections;
using System.Text;
using System.IO;
using System;

public class TrackGenerator : MonoBehaviour {

    public GameObject GeneratedTrack;

    private string Filename = "";
    private string RootDir = @"C:\Users\luke_\Desktop\FSAE Apps\DriverTrainingSim\DriverTrainingSimulator\Assets\Resources\";
    private float trackwidth = 2.5f;

    private GameObject[] TrackNodes;
    
    private void OnGUI()
    {
        Filename = GUI.TextArea(new Rect(10, 10, 100, 20), Filename);
        GUI.Label(new Rect(110, 10, 100, 20), "File");

        trackwidth = float.Parse(GUI.TextArea(new Rect(10, 30, 100, 20), trackwidth.ToString("0.00")));
        GUI.Label(new Rect(110, 30, 100, 20), "Track Width");

        if (GUI.Button(new Rect(10, 50, 100, 20), "Load"))
        {
            ReadFile(Filename);
        }
    }
    
    /// <summary>
    /// Read coordinates from a specified text-based file
    /// </summary>
    /// <param name="fileName">Path of the file in the resources folder to read</param>
    /// <returns>Success</returns>
    private bool ReadFile(string fileName)
    {
        // Handle any problems that might arise when reading the text
        try
        {
            string line;

            StreamReader theReader = new StreamReader(RootDir+fileName, Encoding.Default);

            using (theReader)
            {
                // While there's lines left in the text file, do this:
                do
                {
                    line = theReader.ReadLine();

                    if (line != null)
                    {
                        string[] coordinates = line.Split(';');
                        if (coordinates.Length > 0)
                        {
                            LayTrack(coordinates);
                        }
                        else
                        {
                            Debug.Log("No Entries! :/");
                        }
                            
                    }
                }
                while (line != null);

                theReader.Close();
                return true;
            }
        }
        // If anything broke in the try block, we throw an exception with information
        // on what didn't work
        catch (Exception e)
        {
            Debug.Log(e.Message);
            return false;
        }
    }

    /// <summary>
    /// Lays a track from a specified array of coordinates for the central path through that track
    /// </summary>
    /// <param name="coordinates"> Array of coordinates to use</param>
    private void LayTrack(string[] coordinates)
    {
        // initialise storage array
        TrackNodes = new GameObject[coordinates.Length];

        // clear the previous track
        for(int i = 0; i < GeneratedTrack.transform.childCount; i++)
        {
            Destroy(GeneratedTrack.transform.GetChild(0).gameObject);
        }

        // add objects
        for (int i = 0; i < coordinates.Length; i++)
        {
            // place the plane
            string[] node = coordinates[i].Split(',');
            TrackNodes[i] = GameObject.CreatePrimitive(PrimitiveType.Quad);
            TrackNodes[i].transform.parent = GeneratedTrack.transform;
            TrackNodes[i].name = "Node";
            TrackNodes[i].transform.position = new Vector3(float.Parse(node[0]), 0, float.Parse(node[1]));
            
           
            // if is the first, 
            if (i == 0)
            {
                TrackNodes[i].transform.localScale = new Vector3(0.1f * trackwidth, 1f, 0.1f);
                continue;
            }

            // orient away from previous           
            TrackNodes[i].transform.rotation = Quaternion.LookRotation(TrackNodes[i].transform.position - TrackNodes[i-1].transform.position);

            // if not last, slerp previous node towards current node
            if (i != coordinates.GetLength(0) - 1)
            {
                TrackNodes[i-1].transform.rotation = Quaternion.Slerp(TrackNodes[i].transform.rotation, TrackNodes[i-1].transform.rotation, 0.5f);
            }
        }

        // configure quads
        foreach(GameObject o in TrackNodes)
        {
            Mesh m = new Mesh();

        }
    }
}
