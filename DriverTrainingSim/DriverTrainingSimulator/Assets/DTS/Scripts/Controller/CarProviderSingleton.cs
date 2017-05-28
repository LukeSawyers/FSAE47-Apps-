using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Serialization;
using System.Collections;

namespace DTS.Controllers
{
    /// <summary>
    /// Provides all the vehicle prefabs and manages the showroom interface
    /// </summary>

    public class CarProviderSingleton : MonoBehaviour
    {

        public static CarProviderSingleton Instance = null;

        #region Fields

        [SerializeField]
        private Car[] Cars;

        [SerializeField]
        [Range(1, 30)]
        public float CarOrbitSpeed = 5f;

        [Header("Buttons")]
        [SerializeField]
        private Button NextCar;

        [SerializeField]
        private Button PreviousCar;

        [SerializeField]
        private Button SelectCar;

        private Vector3[] OrbitStartPoints;
        private GameObject[] ShowroomCarRefs;
        private Car Selected;
        private int index = 0;
        private int length = 0;

        #endregion

        #region Monobehaviours
        // Use this for initialization
        void Awake()
        {
            if (Instance == null)
                Instance = this;

            else if (Instance != this)
                Destroy(gameObject);
        }

        void Start()
        {
            // Set up the cars
            length = Cars.Length;
            OrbitStartPoints = new Vector3[length];
            ShowroomCarRefs = new GameObject[length];

            // For all cars
            for (int i = 0; i < length; i++)
            {
                // Create Orbit Start Points
                OrbitStartPoints[i] = Cars[i].ShowCaseTransform.transform.position + new Vector3(3.5f, 1f, 0);

                // Instansiate into showroom
                ShowroomCarRefs[i] = Instantiate(Cars[i].StaticCar, Cars[i].ShowCaseTransform.transform.position, Cars[i].ShowCaseTransform.transform.rotation) as GameObject;
                ShowroomCarRefs[i].SetActive(false);

            }

            // Subscribe buttons
            NextCar.onClick.AddListener(NextCarClicked);
            PreviousCar.onClick.AddListener(PreviousCarClicked);
            SelectCar.onClick.AddListener(SelectCarClicked);
        }

        // Update is called once per frame
        void Update()
        {

        }

        #endregion

        #region Public Accessors

        /// <summary>
        /// Retrieves the points required for orbiting the current car
        /// </summary>
        /// <param name="OrbitStart"></param>
        /// <param name="OrbitCentre"></param>
        public void GetCameraPoints(out Vector3 OrbitStart, out Vector3 OrbitCentre)
        {

            // set the output
            OrbitStart = OrbitStartPoints[index];
            OrbitCentre = Cars[index].ShowCaseTransform.transform.position;
            ShowroomCarRefs[index].SetActive(true);
        }

        /// <summary>
        /// Changes what car is being shown in the showroom
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
        #endregion

        #region Event Handlers

        private void NextCarClicked()
        {
            ChangeShowRoomCar(1);
        }

        private void PreviousCarClicked()
        {
            ChangeShowRoomCar(-1);
        }

        private void SelectCarClicked()
        {
            Selected = Cars[index];
        }

        #endregion

        /// <summary>
        /// Class used to group car data together
        /// </summary>
        [System.Serializable]
        public class Car
        {
            public GameObject MovingCar;
            public GameObject StaticCar;
            public GameObject ShowCaseTransform;
        }
    }
}



