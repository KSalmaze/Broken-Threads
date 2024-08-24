using System;
using UnityEngine;

namespace DefaultNamespace
{
    [System.Serializable]
    public class TransformData
    {
        public float[] position;
        
        public TransformData(Transform transform)
        {
            position = new float[]
            {
                transform.position.x,
                transform.position.y,
                transform.position.z
            };
        }
    }
}