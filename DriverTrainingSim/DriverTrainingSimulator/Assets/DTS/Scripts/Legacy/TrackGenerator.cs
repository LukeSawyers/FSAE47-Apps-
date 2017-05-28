using UnityEngine;
using System.Collections;
using System.Text;
using System.IO;
using System;

namespace DTS.Legacy
{

    public class TrackGenerator : MonoBehaviour {

        // exposed game object
        public GameObject GeneratedTrack;

        public Material TrackMaterial;

        // file stuff
        private string Filename = "";
        private string RootDir = @"C:\Users\luke_\Desktop\FSAE Apps\DriverTrainingSim\DriverTrainingSimulator\Assets\Resources\";
        private float trackwidth = 2.5f;

        // mesh stuff
        private GameObject[] TrackNodes;
        private Vector3[] Verticies;
        private int[] Triangles;
        private Vector3[] Normals;

        private void OnGUI()
        {
            Filename = GUI.TextArea(new Rect(10, 10, 100, 20), Filename);
            GUI.Label(new Rect(110, 10, 100, 20), "File");

            trackwidth = float.Parse(GUI.TextArea(new Rect(10, 30, 100, 20), trackwidth.ToString("0.00")));
            GUI.Label(new Rect(110, 30, 100, 20), "Track Width");

            if (GUI.Button(new Rect(10, 50, 100, 20), "Load"))
            {
                string[] stringCoordinates = ReadFile(Filename);
                StringCoordinatesToGameObjects(stringCoordinates);
                CreateMesh();
            }
        }
    
        /// <summary>
        /// Read coordinates from a specified text-based file
        /// </summary>
        /// <param name="fileName">Path of the file in the resources folder to read</param>
        /// <returns>Success</returns>
        private string[] ReadFile(string fileName)
        {
            // define return variable
            string[] coordinates = new string[1] { "Error Getting Input" };

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
                            coordinates = line.Split(';');
                            if (coordinates.Length < 1)
                            {
                                Debug.Log("No Entries! :/");
                            }
                            
                        }
                    }
                    while (line != null);

                    theReader.Close();
                
                }
            }
            // If anything broke in the try block, we throw an exception with information
            // on what didn't work
            catch (Exception e)
            {
                Debug.Log(e.Message);
            }
            return coordinates;
        }

        /// <summary>
        /// Converts a string array of coordinates to empty game objects and instansiates them
        /// </summary>
        /// <param name="StringCoordinates"></param>
        private void StringCoordinatesToGameObjects(string[] StringCoordinates)
        {
            // initialise storage array
            TrackNodes = new GameObject[StringCoordinates.Length];

            // clear the previous track
            for (int i = 0; i < GeneratedTrack.transform.childCount; i++)
            {
                Destroy(GeneratedTrack.transform.GetChild(i).gameObject);
            }

            // add objects
            for (int i = 0; i < StringCoordinates.Length; i++)
            {
                // place the plane
                string[] node = StringCoordinates[i].Split(',');
                TrackNodes[i] = new GameObject();
                TrackNodes[i].transform.parent = GeneratedTrack.transform;
                TrackNodes[i].name = "Node";

                try { TrackNodes[i].transform.position = new Vector3(float.Parse(node[0]), 0, float.Parse(node[1])); }
                catch { }


                // if is the first, 
                if (i == 0)
                {
                    TrackNodes[i].transform.localScale = new Vector3(0.1f * trackwidth, 1f, 0.1f);
                    continue;
                }

                // orient away from previous           
                TrackNodes[i].transform.rotation = Quaternion.LookRotation(TrackNodes[i].transform.position - TrackNodes[i - 1].transform.position);

                // if not last, slerp previous node towards current node
                if (i != StringCoordinates.GetLength(0) - 1)
                {
                    TrackNodes[i - 1].transform.rotation = Quaternion.Slerp(TrackNodes[i].transform.rotation, TrackNodes[i - 1].transform.rotation, 0.5f);
                }
            }
        }

        /// <summary>
        /// Creates a mesh from the transforms of TrackNodes
        /// </summary>
        private void CreateMesh()
        {
            // Verticies
            // Verticies are half the track width either side of each game object
            Verticies = new Vector3[TrackNodes.Length * 2];
            for(int i = 0; i < TrackNodes.Length; i++)
            {
                // left vertex
                Verticies[i * 2] = TrackNodes[i].transform.position - TrackNodes[i].transform.right * trackwidth / 2f;

                // right vertex
                Verticies[(i * 2) + 1] = TrackNodes[i].transform.position + TrackNodes[i].transform.right * trackwidth / 2f;
            }

            // Triangles
            Triangles = new int[(TrackNodes.Length - 1) * 6];
            for (int i = 0; i < (TrackNodes.Length - 1); i++)
            {
                // bottom left triangle
                Triangles[(i * 6) + 0] = (i * 2) + 0;
                Triangles[(i * 6) + 1] = (i * 2) + 2;
                Triangles[(i * 6) + 2] = (i * 2) + 1;

                // top right triangle
                Triangles[(i * 6) + 3] = (i * 2) + 2;
                Triangles[(i * 6) + 4] = (i * 2) + 3;
                Triangles[(i * 6) + 5] = (i * 2) + 1;

            }

            // Normals
            Normals = new Vector3[Verticies.Length];
            for(int i = 0; i < Normals.Length; i++)
            {
                Normals[i] = -Vector3.up;
            }

            // UVs? Nah 

            // create mesh
            var mf =  GeneratedTrack.GetComponent<MeshFilter>();
            if(mf == null)
            {
                mf = GeneratedTrack.AddComponent<MeshFilter>();
            }
        
        
            Mesh m = new Mesh();
            m.vertices = Verticies;
            m.triangles = Triangles;
            m.normals = Normals;
            mf.mesh = m;
        

            var mr = GeneratedTrack.GetComponent<MeshRenderer>();
            if(mr == null)
            {
                mr = GeneratedTrack.AddComponent<MeshRenderer>();
            }
            mr.material = TrackMaterial;

            var mc = GeneratedTrack.GetComponent<MeshCollider>();
            if (mc == null)
            {
                mc = GeneratedTrack.AddComponent<MeshCollider>();
            }
            mc.sharedMesh = m;
        }

        /// <summary>
        /// Lays a track from a specified array of coordinates for the central path through that track using planes
        /// </summary>
        /// <param name="coordinates"> Array of coordinates to use</param>
        private void LayPlaneTrack(string[] coordinates)
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
                TrackNodes[i] = GameObject.CreatePrimitive(PrimitiveType.Plane);
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
        }
    }
}
