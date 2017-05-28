using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

namespace DTS.Controllers
{

    
    /// <summary>
    /// Governs program flow and manages the main menu
    /// </summary>
    public class GameConrollerSingleton : MonoBehaviour
    {

        public static GameConrollerSingleton Instance = null;

        #region Fields
        [Header("Universal Navigaion Buttons")]
        [SerializeField]
        private Button[] BackButtons;
        [SerializeField]
        private Button[] AboutButtons;

        [Header("UI Objects")]
        [SerializeField]
        private GameObject MainMenuUI;
        [SerializeField]
        private GameObject SelectCarUI;
        [SerializeField]
        private GameObject SelectEventUI;
        [SerializeField]
        private GameObject SelectTrackUI;
        [SerializeField]
        private GameObject TracksMenuUI;

        [Header("Main Menu Buttons")]
        [SerializeField]
        private Button JustRaceButton;
        [SerializeField]
        private Button TimedEventButton;
        [SerializeField]
        private Button ScenariosButton;
        [SerializeField]
        private Button TracksButton;

        [SerializeField]
        [Range(1, 30)]
        private float MainMenuCameraOrbitSpeed = 0.01f;

        /*
        [Header("Select Event")]

        public Button EventNext;
        public Button EventPrevious;
        public Button EventSelect;

        [Header("Select Track")]

        public Button TrackNext;
        public Button TrackPrevious;
        public Button TrackTracks;
        public Button TrackSelect;

        [Header("Tracks Menu")]

        public Button ImportTrack;
        public Button NextTrack;

        [Header("About Menu")]
        public GameObject AboutUI;*/

        #endregion

        #region Navigation 
        private enum ProgramState { Null, MainMenu, SelectCar, SelectEvent, SelectTrack, TracksMenu, AboutScreen };
        private static class NavigationHistory
        {

            private static Stack<ProgramState> stack = new Stack<ProgramState>();
            public delegate void NavStateChange(ProgramState newState, ProgramState oldState);
            public static event NavStateChange OnNavStateChanged;

            public static void Push(ProgramState newState)
            {
                ProgramState oldState = ProgramState.Null;
                try
                {
                    oldState = stack.Peek();
                }
                catch { }
                OnNavStateChanged(newState, oldState);
                stack.Push(newState);
            }

            public static ProgramState Peek()
            {
                ProgramState peeked = ProgramState.Null;
                try
                {
                    peeked = stack.Peek();
                }
                catch { }
                return peeked;
            }

            public static ProgramState Pop()
            {
                ProgramState popped = stack.Pop();
                OnNavStateChanged(stack.Peek(), popped);
                return popped;
            }

        }
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

        private void Start()
        {

            // push initial navigation state
            NavigationHistory.OnNavStateChanged += NavigationHistoryChanged;
            NavigationHistory.Push(ProgramState.MainMenu);

            // subscribe to button events
            foreach (Button b in BackButtons)
            {
                b.onClick.AddListener(BackClicked);
            }
            foreach (Button b in AboutButtons)
            {
                b.onClick.AddListener(AboutClicked);
            }

            // main menu buttons
            JustRaceButton.onClick.AddListener(JustRaceClicked);
            TimedEventButton.onClick.AddListener(TimedEventClicked);
            ScenariosButton.onClick.AddListener(ScenariosClicked);
            TracksButton.onClick.AddListener(TracksClicked);

        }

        // Update is called once per frame
        private void Update()
        {

            switch (NavigationHistory.Peek())
            {
                case ProgramState.MainMenu:

                    break;
                case ProgramState.SelectCar:

                    break;
                case ProgramState.SelectEvent:

                    break;
                case ProgramState.SelectTrack:

                    break;
                case ProgramState.TracksMenu:

                    break;
            }
        }

        #endregion

        private void NavigationHistoryChanged(ProgramState newState, ProgramState oldState)
        {
            if (oldState == ProgramState.Null)
            {
                goto next;
            }

            #region Old State
            switch (oldState)
            {
                case ProgramState.MainMenu:
                    MainMenuUI.SetActive(false);

                    break;
                case ProgramState.SelectCar:
                    SelectCarUI.SetActive(false);
                    break;
                case ProgramState.SelectEvent:

                    break;
                case ProgramState.SelectTrack:

                    break;
                case ProgramState.TracksMenu:

                    break;
            }
            #endregion

            next:
            if (newState == ProgramState.Null)
            {
                return;
            }

            #region New State
            switch (newState)
            {
                case ProgramState.MainMenu:
                    MainMenuUI.SetActive(true);
                    CameraController.main.Orbit(new Vector3(50, 10, 0), new Vector3(0, 0, 0), MainMenuCameraOrbitSpeed);
                    break;
                case ProgramState.SelectCar:
                    SelectCarUI.SetActive(true);
                    CarProviderSingleton.Instance.StartOrbiting();
                    
                    break;
                case ProgramState.SelectEvent:

                    break;
                case ProgramState.SelectTrack:

                    break;
                case ProgramState.TracksMenu:

                    break;

            }
            #endregion
        }

        #region Event Handlers

        private void JustRaceClicked()
        {
            NavigationHistory.Push(ProgramState.SelectCar);
        }

        private void BackClicked()
        {
            NavigationHistory.Pop();
        }

        private void AboutClicked()
        {
            NavigationHistory.Push(ProgramState.AboutScreen);
        }

        private void TimedEventClicked()
        {
            NavigationHistory.Push(ProgramState.SelectEvent);
        }

        private void ScenariosClicked()
        {
            //TODO
        }

        private void TracksClicked()
        {
            NavigationHistory.Push(ProgramState.TracksMenu);
        }



        private void SelectCarClicked()
        {

        }

        #endregion

    }
}
