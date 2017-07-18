using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SunBehaviour : MonoBehaviour {

    public static SunBehaviour main;

    [Header("Sun Movement")]

    [SerializeField]
    private bool isMain = false;

    [SerializeField]
    private Transform Centre;

    public float secondsInDay = 10000;

    [SerializeField]
    [Range(-1f,1f)]
    private float XAxisAlignment = 0;

    [SerializeField]
    [Range(-1f, 1f)]
    private float YAxisAlignment = 0;

    [SerializeField]
    [Range(-1f, 1f)]
    private float ZAxisAlignment = 0;

    [Header("Sun Appearance")]
    [SerializeField]
    private Light Sun;

    [SerializeField]
    private Color Day;

    [SerializeField]
    private Color Evening;

    [SerializeField]
    private Color Night;

    [Header("Night Lights")]
    [SerializeField]
    private GameObject NightLights;

    [SerializeField]
    private float NightLightsDistFromCamera = 200;

    public bool cycle = true;

    private Vector3 axis;
    private float angleDelta;
    private float distance;
    private Color color;
    private Light[] NightLightsArray;
    private bool SunDown = false;

    private void Start()
    {
        if(isMain && main == null)
        {
            main = this;
        }

        SunDown = transform.position.y < 0;
        NightLightsArray = NightLights.GetComponentsInChildren<Light>();
    }

    // Update is called once per frame
    void Update () {

        if (cycle)
        {
            // calculate the rotation axis
            axis = new Vector3(XAxisAlignment, YAxisAlignment, ZAxisAlignment);
            axis = axis.normalized;
            
            // calculate the angle delta
            angleDelta = 180 / secondsInDay;
            distance = Mathf.Sqrt((transform.position - Centre.position).magnitude);

            // if we're above the horizon
            if(transform.position.y > 0)
            {
                SunDown = false;
                color = Color.Lerp(Evening, Day, Mathf.Sqrt(Mathf.Abs(transform.position.y)) / distance);
            }
            else
            {
                if (Mathf.Abs(transform.position.y) > 0.05 * (distance * distance))
                {
                    Sun.enabled = false;
                }
                else
                {
                    Sun.enabled = true;
                    SunDown = true;
                    color = Color.Lerp(Evening, Night, Mathf.Sqrt(Mathf.Abs(transform.position.y)) / distance);
                }
                
            }

            Sun.color = color;

            // rotate around the centre transform
            transform.RotateAround(Centre.position, axis, angleDelta*Time.deltaTime);

            StartCoroutine(LightsOn());

            transform.LookAt(Centre);

        }
	}

    /// <summary>
    /// Coroutine looks at each light on each frame, reduces overhead
    /// </summary>
    /// <returns></returns>
    private IEnumerator LightsOn()
    {

            foreach(Light l in NightLightsArray)
            {
                l.gameObject.SetActive(SunDown && (l.transform.position - Camera.main.transform.position).magnitude < NightLightsDistFromCamera);
                yield return null;
            }
    }

    public void SetToMidday()
    {
        transform.position = new Vector3(Centre.position.x, Mathf.Pow(distance, 2), Centre.position.z);
    }
}
