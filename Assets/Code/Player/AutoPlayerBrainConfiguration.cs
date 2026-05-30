using System;
using UnityEngine;

[Serializable]
public class AutoPlayerBrainConfiguration
{
    
    public float laneSpacing = 2f;
    public int laneCount = 3;
    public float lookAheadTime = 2f;
    public float laneAlignThreshold = 0.15f;
    public LayerMask obstacleLayers;
    public float lowObstacleHeight = 0.75f;
    public float bodyHeight = 1f;
    public float rayOriginYOffset = 0.5f;
    public float jumpLeadTimeMultiplier = 0.85f;
    public float overheadObstacleHeight = 1.5f;
    public float slideClearanceHeight = 0.6f;
    public float slideLeadTimeMultiplier = 1f;
    
}
