using UnityEngine;

public class AutoPlayerBrain : MonoBehaviour
{

    [SerializeField] public float gravityModifier = 1f;
    [SerializeField] public float forwardSpeedModifier = 1f;
    [SerializeField] public float sideSpeedModifier = 1f;
    [SerializeField] public float jumpHeightModifier = 1f;

    public AutoPlayerInput CalculateInput(AutoPlayer player)
    {
        // TODO: raycast ahead & make decisions about when to move left/right or jump
        return default;
    }
    
}
