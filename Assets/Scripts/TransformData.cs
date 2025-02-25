using UnityEngine;

[System.Serializable]
public class TransformData
{
    public float[] position;
    public float[] rotation;
    
    public TransformData(Transform transform)
    {
        position = new []
        {
            transform.position.x,
            transform.position.y,
            transform.position.z
        };

        rotation = new[]
        {
            transform.rotation.x,
            transform.rotation.y,
            transform.rotation.z,
            transform.rotation.w
        };
    }
}