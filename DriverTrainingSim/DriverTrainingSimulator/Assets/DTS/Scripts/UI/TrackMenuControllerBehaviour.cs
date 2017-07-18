using UnityEngine;
using System.Collections;

namespace DTS.UI
{
    public class TrackMenuControllerBehaviour : MenuControllerBehaviour
    {
        [Header("Track Menu Controller")]

        public float followSpeed = 5;

        [SerializeField]
        private Transform ViewPosition;

        protected override void OnActivateUI()
        {
            CameraControllerBehaviour.main.LerpTo(ViewPosition, followSpeed);
            SunBehaviour.main.SetToMidday();
            SunBehaviour.main.secondsInDay = 100000000000;

        }

    }
}
