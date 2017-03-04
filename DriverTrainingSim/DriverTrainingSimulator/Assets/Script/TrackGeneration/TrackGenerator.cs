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

    private void LayTrack(string[] coordinates)
    {
        // create the track
        for(int i = 0; i < GeneratedTrack.transform.childCount; i++)
        {
            Destroy(GeneratedTrack.transform.GetChild(0).gameObject);
        }

        GameObject LastNode = new GameObject();

        for (int i = 0; i < coordinates.GetLength(0); i++)
        {
            // place the plane
            string[] node = coordinates[i].Split(',');
            GameObject Node = GameObject.CreatePrimitive(PrimitiveType.Plane);
            Node.transform.parent = GeneratedTrack.transform;
            Node.name = "Node";
            Node.transform.position = new Vector3(float.Parse(node[0]), 0, float.Parse(node[1])); ;
            Node.transform.localScale = new Vector3(0.1f *(Node.transform.position - LastNode.transform.position).magnitude, 1f, 0.1f * trackwidth);

            // if is the first, do nothing
            if (i == 0)
            {
                LastNode = Node;
                continue;
            }

            // orient away from previous           
            Node.transform.rotation = Quaternion.LookRotation(Node.transform.position - LastNode.transform.position);

            // if not last, slerp previous towards current
            if (i != coordinates.GetLength(0) - 1)
            {
                LastNode.transform.rotation = Quaternion.Slerp(Node.transform.rotation, LastNode.transform.rotation, 0.5f);
            }

            // store the last node
            LastNode = Node;
        }
    }
}
