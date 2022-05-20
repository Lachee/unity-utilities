using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Lachee.Utilities
{
    /// <summary>
    /// Spins an object at the given speed
    /// </summary>
    public class Spin : MonoBehaviour
    {
        public Vector3 speed = Vector3.one;
        private void Update()
        {
            transform.Rotate(speed * Time.deltaTime);
        }

    }

}