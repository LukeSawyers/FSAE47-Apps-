using UnityEngine;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System;

namespace DTS.Legacy
{

    public class GPXFileParser
    {
        List<Vector3> ReadPoints(string fileDir)
        {
            List<string> lines = new List<string>();
            List<Vector3> points = new List<Vector3>();

            // Read the file out into lines
            StreamReader theReader = new StreamReader(fileDir, Encoding.Default);
            using (theReader)
            {
                // While there's lines left in the text file, do this:
                string line = "";
                do
                {
                    line = theReader.ReadLine();
                    lines.Add(line);
                }
                while (line != null);

                theReader.Close();
            }

            bool first = true;
            Vector3 initialOffset = new Vector3();

            // break up the string
            foreach (string line in lines)
            {
                if (line.Contains("trkpt"))
                {
                    // get the latitude, longditue and elevation values for this entry
                    int latStart = line.IndexOf("lat=") + 5;
                    int latLen = line.IndexOf(" lon=") - 2 - latStart;
                    string lat_s = line.Substring(latStart, latLen);

                    int lonStart = line.IndexOf("lon=") + 5;
                    int lonLen = line.IndexOf("><ele>") - 2;
                    string lon_s = line.Substring(lonStart, lonLen);

                    int eleStart = line.IndexOf("<ele>") + 5;
                    int eleLen = line.IndexOf("</ele>") - 1;
                    string ele_s = line.Substring(eleStart, eleLen);

                    float lat;
                    float lon;
                    float ele;

                    try
                    {
                        lat = float.Parse(lat_s);
                        lon = float.Parse(lon_s);
                        ele = float.Parse(ele_s);
                    }
                    catch { continue; }

                    Vector3 point = new Vector3();

                    float cosLat = Mathf.Cos(lat * Mathf.PI / 180.0f);
                    float sinLat = Mathf.Sin(lat * Mathf.PI / 180.0f);
                    float cosLon = Mathf.Cos(lon * Mathf.PI / 180.0f);
                    float sinLon = Mathf.Sin(lon * Mathf.PI / 180.0f);

                    float rad = 6378137.0f;
                    float f = 1.0f / 298.257224f;
                    float C = 1.0f / Mathf.Sqrt(cosLat * cosLat + (1 - f) * (1 - f) * sinLat * sinLat);
                    float S = (1.0f - f) * (1.0f - f) * C;
                    float h = 0.0f;

                    point.x = (rad * C + h) * cosLat * cosLon;
                    point.y = (rad * C + h) * cosLat * sinLon;
                    point.z = (rad * S + h) * sinLat + ele;

                    if (first)
                    {
                        initialOffset = point;
                    }

                    points.Add(point - initialOffset);
                }
            }
            return points;
        }
    }
}
