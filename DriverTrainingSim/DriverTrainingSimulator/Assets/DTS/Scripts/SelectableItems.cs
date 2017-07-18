using UnityEngine;
using System.Collections;
using System;

namespace DTS.Items
{
    /// <summary>
    /// Interface for prefabs that can be selected by the user in a menu
    /// </summary>
    public interface IOrbitable
    {
        /// <summary>
        /// Accessor for the orbit speed of the selectable object
        /// </summary>
        float OrbitSpeed { get; }

        /// <summary>
        /// Accessor to return the center of the desired orbit
        /// </summary>
        Vector3 OrbitCentre { get; }

        /// <summary>
        /// Accessor to return the start of the desired orbit
        /// </summary>
        Vector3 OrbitStart { get; }

        /// <summary>
        /// Returns the object to be shown
        /// </summary>
        GameObject OrbitingObject { get; }

    }

    /// <summary>
    /// Class used to group car data together
    /// </summary>
    [Serializable]
    public class SelectableCar : IOrbitable
    {
        public GameObject MovingCar;
        public GameObject StaticCar;
        public GameObject ShowCaseTransform;

        public float orbitSpeed = 3f;
        public Vector3 orbitStart = new Vector3(3, 2, 0);

        #region IOrbitable

        public float OrbitSpeed
        {
            get { return orbitSpeed; }

        }

        public Vector3 OrbitCentre
        {
            get { return StaticCar.transform.position; }
        }

        public Vector3 OrbitStart
        {
            get { return OrbitStart; }

        }

        public GameObject OrbitingObject { get { return StaticCar; } }

        #endregion
    }
}
