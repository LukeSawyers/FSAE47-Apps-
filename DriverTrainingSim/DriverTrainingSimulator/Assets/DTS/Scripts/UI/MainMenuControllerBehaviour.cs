using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;

namespace DTS.UI
{
    /// <summary>
    /// Controller for the main menu
    /// </summary>
    public class MainMenuControllerBehaviour : MenuControllerBehaviour
    {
        [Header("Main Menu Controller")]
        [SerializeField]
        private float OrbitSpeed = 5f;

        [SerializeField]
        private Transform OrbitCentre;

        protected override void OnStart()
        {
            CameraControllerBehaviour.main.Orbit(CameraControllerBehaviour.main.transform.position, OrbitCentre.position, OrbitSpeed);
        }

        protected override void OnActivateUI()
        {
            CameraControllerBehaviour.main.Orbit(CameraControllerBehaviour.main.transform.position, OrbitCentre.position, OrbitSpeed);
            
        }

    }
}
