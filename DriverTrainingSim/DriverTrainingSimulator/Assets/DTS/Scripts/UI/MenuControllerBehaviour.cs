using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System;

namespace DTS.UI
{
    /// <summary>
    /// Abstract class providing the framework for a menu controller 
    /// </summary>
    public abstract class MenuControllerBehaviour : MonoBehaviour
    {
        [Header("Menu Controller")]
        /// <summary>
        /// Previous UI, initialised by that object
        /// </summary>
        private MenuControllerBehaviour PreviousMenu;

        /// <summary>
        /// UI Associated with this menu
        /// </summary>
        [SerializeField]
        private GameObject UI;

        /// <summary>
        /// Back button Associated with moving to the previous UI
        /// </summary>
        [SerializeField]
        private Button BackButton;

        /// <summary>
        /// Next UI elements, each initialised by this object
        /// </summary>
        [SerializeField]
        private List<MenuControllerUITriggerPair> NextMenus;

        /// <summary>
        /// Marker to indicate that this is root
        /// </summary>
        private bool IsRoot = false;

        // Use this for initialization
        void Awake()
        {

            // determine if is root
            if (PreviousMenu == null)
            {
                IsRoot = true;
                UI.SetActive(true);
            }
            else
            {
                UI.SetActive(false);
                BackButton.onClick.AddListener(this.DeactivateUI);
            }
                

            // intialise children
            foreach (MenuControllerUITriggerPair c in NextMenus)
            {
                // associate nextmenu button click evnets
                if(c.menuController != null && c.Button != null)
                {
                    c.Button.onClick.AddListener(c.menuController.ActivateUI);
                    c.Button.onClick.AddListener(this.DeactivateUI);
                    c.menuController.PreviousMenu = this;
                }
                
            }

            this.OnAwake();

        }

        void Start()
        {
            if (!IsRoot)
                BackButton.onClick.AddListener(PreviousMenu.ActivateUI);

            this.OnStart();
        }

        /// <summary>
        /// Allows child objects to define other behaviours for Awake
        /// </summary>
        protected virtual void OnAwake()
        {

        }

        /// <summary>
        /// Allows child objects to define other behaviours for Start
        /// </summary>
        protected virtual void OnStart()
        {

        }

        /// <summary>
        /// Handles a button click that results in this menu activating
        /// </summary>
        public void ActivateUI()
        {
            UI.SetActive(true);
            OnActivateUI();
        }

        /// <summary>
        /// Handles a button click that results in deactivating this menu
        /// </summary>
        public void DeactivateUI()
        {
            UI.SetActive(false);
            OnDeactivateUI();
        }

        /// <summary>
        /// Called when the UI is activated
        /// </summary>
        protected virtual void OnActivateUI()
        {

        }

        /// <summary>
        /// Called when the UI is deactivated
        /// </summary>
        protected virtual void OnDeactivateUI()
        {

        }

        [Serializable]
        private class MenuControllerUITriggerPair
        {
            public MenuControllerBehaviour menuController;
            public Button Button;
        }
    }
}
