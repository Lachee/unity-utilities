using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Lachee.Utilities
{

    public class Spin : MonoBehaviour
    {
        public Vector3 speed = Vector3.one;
        private void Update()
        {
            transform.Rotate(speed * Time.deltaTime);
        }

    }

}