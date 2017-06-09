using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;

namespace DTS.Controllers
{
    /// <summary>
    /// Controller for the main menu
    /// </summary>
    public class MainMenuController : MenuController
    {
        [SerializeField]
        private float OrbitSpeed = 5f;

        protected override void OnStart()
        {
            CameraController.main.Orbit(new Vector3(50, 10, 0), Vector3.zero, OrbitSpeed);
        }

        protected override void OnActivateUI()
        {
            CameraController.main.Orbit(new Vector3(50, 10, 0), Vector3.zero, OrbitSpeed);
        }

    }
}
