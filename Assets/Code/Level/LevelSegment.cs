using UnityEngine;

public class LevelSegment : MonoBehaviour
{
    
    [SerializeField] private float length = 20f;
    [SerializeField] private bool useTransformScale = true;

    private float Length => useTransformScale ? length * transform.localScale.z : length;

    public Vector3 NextSegmentPosition => transform.position + Vector3.forward * Length;
    
}
