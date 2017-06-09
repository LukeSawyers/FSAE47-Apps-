using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Serialization;
using System.Collections;
using DTS.Controllers;

namespace DTS.Items
{
    /// <summary>
    /// Provides all the vehicle prefabs data to all other scripts in the application
    /// </summary>
    public class CarProviderSingleton : MonoBehaviour
    {

        /// <summary>
        /// Singleton instance
        /// </summary>
        public static CarProviderSingleton Instance = null;

        /// <summary>
        /// Cars to use
        /// </summary>
        [SerializeField]
        private IOrbitable[] Items;

        /// <summary>
        /// Orbit speed to use for the camera
        /// </summary>
        [SerializeField]
        [Range(1, 30)]
        public float CarOrbitSpeed = 5f;

        /// <summary>
        /// Orbit start points for each car
        /// </summary>
        private Vector3[] OrbitStartPoints;

        /// <summary>
        /// Transforms to use to orbit each car
        /// </summary>
        private GameObject[] ShowroomCarRefs;

        /// <summary>
        /// Returns the currently selected car
        /// </summary>
        private IOrbitable Selected
        {
            get { return Items[index]; }
        }

        /// <summary>
        /// Current index
        /// </summary>
        private int index = 0;

        /// <summary>
        /// Number of cars, max index
        /// </summary>
        private int length = 0;

        /// <summary>
        /// Enforce singleton pattern on awake
        /// </summary>
        void Awake()
        {
            if (Instance == null)
                Instance = this;

            else if (Instance != this)
                Destroy(this);
        }

        /// <summary>
        /// Make orbiting object calculations
        /// </summary>
        void Start()
        {
            // Set up the cars
            length = Items.Length;
            OrbitStartPoints = new Vector3[length];
            ShowroomCarRefs = new GameObject[length];

            // For all cars
            for (int i = 0; i < length; i++)
            {
                // Create Orbit Start Points
                OrbitStartPoints[i] = Items[i].OrbitCentre + new Vector3(3.5f, 1f, 0);

                // Instansiate into showroom
                ShowroomCarRefs[i] = Instantiate(Items[i].OrbitingObject, Items[i].OrbitingObject.transform.position, Items[i].OrbitingObject.transform.rotation) as GameObject;
                ShowroomCarRefs[i].SetActive(false);

            }
        }

        /// <summary>
        /// Retrieves the points required for orbiting the current car
        /// </summary>
        /// <param name="OrbitStart"></param>
        /// <param name="OrbitCentre"></param>
        public void GetCameraPoints(out Vector3 OrbitStart, out Vector3 OrbitCentre)
        {

            // set the output
            OrbitStart = OrbitStartPoints[index];
            OrbitCentre = Items[index].OrbitingObject.transform.position;
            ShowroomCarRefs[index].SetActive(true);
        }

        /// <summary>
        /// Changes what car is being shown
        /// </summary>
        /// <param name="ind"></param>
        /// <param name="OrbitStart"></param>
        /// <param name="OrbitCentre"></param>
        public void ChangeShowRoomCar(int ind, out Vector3 OrbitStart, out Vector3 OrbitCentre)
        {
            // set the static model
            ShowroomCarRefs[index].SetActive(false);

            if (index + ind < 0)
                index = length - 1;
            else if (index + ind == length)
                index = 0;
            else
                index += ind;

            GetCameraPoints(out OrbitStart, out OrbitCentre);
        }

        /// <summary>
        /// Changes the car being shown and moves the camera
        /// </summary>
        /// <param name="ind"></param>
        public void ChangeShowRoomCar(int ind)
        {
            // set the static model
            ShowroomCarRefs[index].SetActive(false);

            if (index + ind < 0)
                index = length - 1;
            else if (index + ind == length)
                index = 0;
            else
                index += ind;
            Vector3 OrbitStart;
            Vector3 OrbitPoint;
            GetCameraPoints(out OrbitStart, out OrbitPoint);
            CameraController.main.Orbit(OrbitStart, OrbitPoint, CarOrbitSpeed);
        }

        /// <summary>
        /// Start orbiting around the current car
        /// </summary>
        public void StartOrbiting()
        {
            Vector3 OrbitStart;
            Vector3 OrbitPoint;
            CarProviderSingleton.Instance.GetCameraPoints(out OrbitStart, out OrbitPoint);
            CameraController.main.Orbit(OrbitStart, OrbitPoint, CarOrbitSpeed);
        }

        
    }
}



