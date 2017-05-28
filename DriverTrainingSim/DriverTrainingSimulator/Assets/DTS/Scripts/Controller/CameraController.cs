using UnityEngine;
using System.Collections;

namespace DTS.Controllers
{
    /// <summary>
    /// Manages the movement of the camera within the game
    /// </summary>
    public class CameraController : MonoBehaviour
    {

        public static CameraController main;

        #region Fields

        // General
        public float LerpStrength = 3f;

        // Orbiting
        private Vector3 orbitCentre = new Vector3(0, 0, 0);
        private Transform orbitPoint;

        
        private float OrbitSpeed = 5f;

        // Following
        private Transform followTransform;

        // Modes
        public enum CameraMode { Free, Orbit, Fix, Lerp };
        private CameraMode cameraMode = CameraMode.Orbit;

        #endregion

        #region Mono

        private void Awake()
        {
            GameObject go = new GameObject();
            orbitPoint = go.transform;
            if (main == null)
            {
                main = this;
            }
        }

        // Update is called once per frame
        void Update()
        {
            switch (cameraMode)
            {
                case CameraMode.Orbit:
                    OrbitPoint();
                    break;
                case CameraMode.Fix:
                    FollowPoint(followTransform, false);
                    break;
                case CameraMode.Lerp:
                    FollowPoint(followTransform, true);
                    break;
            }
        }

        #endregion

        #region Private Methods
        private void OrbitPoint()
        {
            // move the follow point
            orbitPoint.RotateAround(orbitCentre, Vector3.up, OrbitSpeed * Time.deltaTime);
            orbitPoint.LookAt(orbitCentre);

            // lerp the camera to the point
            FollowPoint(orbitPoint, true);
        }

        private void FollowPoint(Transform followTransform, bool lerp)
        {
            if (followTransform != null)
            {
                if (lerp)
                {
                    transform.position = Vector3.Lerp(transform.position, followTransform.position, LerpStrength * Time.deltaTime);
                    transform.rotation = Quaternion.Lerp(transform.rotation, followTransform.rotation, LerpStrength * Time.deltaTime);
                }
                else
                {
                    transform.position = followTransform.position;
                    transform.rotation = followTransform.rotation;
                }
            }
        }
        #endregion

        #region Public Accessors

        public void Orbit(Vector3 _orbitPoint, Vector3 _orbitCentre, float _OrbitSpeed)
        {
            cameraMode = CameraMode.Orbit;
            orbitPoint.position = _orbitPoint;
            orbitCentre = _orbitCentre;
            OrbitSpeed = _OrbitSpeed;
        }


        #endregion
    }
}
