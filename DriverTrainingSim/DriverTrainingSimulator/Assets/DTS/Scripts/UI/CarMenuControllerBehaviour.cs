using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DTS.UI
{
    public class CarMenuControllerBehaviour : MenuControllerBehaviour
    {
        [SerializeField]
        private float OrbitSpeed = 5f;

        [SerializeField]
        private Transform ViewPosition;

        protected override void OnStart()
        {
            CameraControllerBehaviour.main.transform.position = ViewPosition.position;
            CameraControllerBehaviour.main.transform.rotation = ViewPosition.rotation;
        }

        protected override void OnActivateUI()
        {
            CameraControllerBehaviour.main.transform.position = ViewPosition.position;
            CameraControllerBehaviour.main.transform.rotation = ViewPosition.rotation;
        }

        
    }
}

