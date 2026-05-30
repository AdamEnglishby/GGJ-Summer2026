using UnityEngine;

public class AutoPlayerBrain : MonoBehaviour
{
    
    [SerializeField] private AutoPlayerBrainConfiguration sensorConfig = new();

    [SerializeField] public float gravityModifier = 1f;
    [SerializeField] public float forwardSpeedModifier = 1f;
    [SerializeField] public float sideSpeedModifier = 1f;
    [SerializeField] public float jumpHeightModifier = 1f;

    public AutoPlayerInput CalculateInput(AutoPlayer player)
    {
        var input = new AutoPlayerInput();
        var lookAhead = player.Config.forwardSpeed * forwardSpeedModifier * sensorConfig.lookAheadTime;
        var currentLane = GetLaneIndex(player.transform.position.x);
        var targetLane = ChooseTargetLane(player, currentLane, lookAhead);

        input.Move = CalculateLateralInput(player.transform.position.x, targetLane);

        if (player.IsGrounded && ShouldJump(player, targetLane, lookAhead)) input.Jump = true;

        return input;
    }

    public float GetJumpVelocity(AutoPlayerConfiguration config) => Mathf.Sqrt(2f * config.gravityStrength * gravityModifier * config.baseJumpHeight) * jumpHeightModifier;

    private int GetLaneIndex(float x)
    {
        var halfLanes = (sensorConfig.laneCount - 1) / 2f;
        return Mathf.Clamp(Mathf.RoundToInt(x / sensorConfig.laneSpacing), (int) -halfLanes, (int) halfLanes);
    }

    private float GetLaneWorldX(int laneIndex) => laneIndex * sensorConfig.laneSpacing;

    private Vector3 GetRayOrigin(AutoPlayer player, int lane, float probeHeight)
    {
        var feetY = player.transform.position.y + player.FeetOffsetY + sensorConfig.rayOriginYOffset;
        return new Vector3(GetLaneWorldX(lane), feetY + probeHeight, player.transform.position.z);
    }

    private int ChooseTargetLane(AutoPlayer player, int currentLane, float lookAhead)
    {
        var halfLanes = (sensorConfig.laneCount - 1) / 2f;
        var bestLane = currentLane;
        var bestScore = float.MinValue;

        for (var lane = -(int)halfLanes; lane <= (int)halfLanes; lane++)
        {
            var score = ScoreLane(player, lane, lookAhead);
            score -= Mathf.Abs(lane - currentLane) * 0.25f;

            if (score <= bestScore)
                continue;

            bestScore = score;
            bestLane = lane;
        }

        return bestLane;
    }

    private float ScoreLane(AutoPlayer player, int lane, float lookAhead)
    {
        var origin = GetRayOrigin(player, lane, 0f);

        if (IsLaneBlocked(origin, lookAhead, sensorConfig.bodyHeight)) return -100f;
        if (IsLaneBlocked(origin, lookAhead, sensorConfig.lowObstacleHeight)) return CanClearLowObstacle(player, lane, lookAhead) ? 10f : -50f;

        return 100f - Mathf.Abs(GetLaneWorldX(lane) - player.transform.position.x);
    }

    private bool IsLaneBlocked(Vector3 feetOrigin, float distance, float probeHeight)
    {
        var origin = feetOrigin;
        origin.y += probeHeight;
        return Physics.Raycast(origin, Vector3.forward, distance, sensorConfig.obstacleLayers, QueryTriggerInteraction.Ignore);
    }

    private float CalculateLateralInput(float currentX, int targetLane)
    {
        var deltaX = GetLaneWorldX(targetLane) - currentX;
        return Mathf.Abs(deltaX) <= sensorConfig.laneAlignThreshold ? 0f : Mathf.Clamp(deltaX / sensorConfig.laneSpacing, -1f, 1f);
    }

    private bool ShouldJump(AutoPlayer player, int lane, float lookAhead)
    {
        var origin = GetRayOrigin(player, lane, 0f);

        if (!IsLaneBlocked(origin, lookAhead, sensorConfig.lowObstacleHeight)) return false;
        if (IsLaneBlocked(origin, lookAhead, sensorConfig.bodyHeight)) return false;

        var distance = GetObstacleDistance(origin, lookAhead, sensorConfig.lowObstacleHeight);
        return distance > 0f && CanClearAtDistance(player, distance);
    }

    private bool CanClearLowObstacle(AutoPlayer player, int lane, float lookAhead)
    {
        var origin = GetRayOrigin(player, lane, 0f);
        var distance = GetObstacleDistance(origin, lookAhead, sensorConfig.lowObstacleHeight);
        return distance > 0f && CanClearAtDistance(player, distance);
    }

    private float GetObstacleDistance(Vector3 feetOrigin, float maxDistance, float probeHeight)
    {
        var origin = feetOrigin;
        origin.y += probeHeight;

        var raycast = Physics.Raycast(
            origin, 
            Vector3.forward, 
            out var hit, 
            maxDistance, 
            sensorConfig.obstacleLayers,
            QueryTriggerInteraction.Ignore
        );
        return raycast ? hit.distance : -1f;
    }

    private bool CanClearAtDistance(AutoPlayer player, float obstacleDistance)
    {
        var jumpVelocity = GetJumpVelocity(player.Config);
        var gravity = player.Config.gravityStrength * gravityModifier;
        if (gravity <= 0f || jumpVelocity <= 0f) return false;

        var timeToApex = jumpVelocity / gravity;
        var maxJumpHeight = jumpVelocity * jumpVelocity / (2f * gravity);
        var airTime = timeToApex * 2f;
        var forwardSpeed = player.Config.forwardSpeed * forwardSpeedModifier;
        var jumpTravel = forwardSpeed * airTime;
        var jumpLeadDistance = forwardSpeed * timeToApex * sensorConfig.jumpLeadTimeMultiplier;

        if (maxJumpHeight < sensorConfig.lowObstacleHeight) return false;
        if (obstacleDistance > jumpTravel) return false;

        return obstacleDistance <= jumpLeadDistance;
    }
    
}
