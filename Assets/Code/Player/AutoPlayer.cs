using System.Threading.Tasks;
using UnityEngine;

// TODO: rotate the character towards its movement direction
[RequireComponent(typeof(CharacterController))]
public class AutoPlayer : MonoBehaviour
{
    
    [SerializeField] private LevelSpawner levelSpawner;
    [SerializeField] private AutoPlayerConfiguration config;
    [SerializeField] private AutoPlayerBrain brain;
    
    private CharacterController _controller;
    private AutoPlayerInput _input;
    private float _verticalVelocity;

    public AutoPlayerConfiguration Config => config;
    public bool IsGrounded => _controller.isGrounded;
    public float FeetOffsetY => -_controller.height * 0.5f + _controller.center.y;

    private void Awake() => _controller = GetComponent<CharacterController>();

    private void Update()
    {
        _input = brain.CalculateInput(this);
        
        if (_controller.isGrounded) _verticalVelocity = -2f;
        else _verticalVelocity -= config.gravityStrength * brain.gravityModifier * Time.deltaTime;

        if (_verticalVelocity < 0)
        {
            _verticalVelocity -= config.gravityStrength * brain.gravityModifier * Time.deltaTime;
        }

        if (_input.Jump && _controller.isGrounded)
        {
            _input.Jump = false;
            _verticalVelocity = brain.GetJumpVelocity(config);
        }
        
        var moveInput = new Vector3(
            _input.Move * config.sideSpeed * brain.sideSpeedModifier,
            _verticalVelocity,
            config.forwardSpeed * brain.forwardSpeedModifier * levelSpawner.DifficultyModifier);
        
        _controller.Move(moveInput * Time.deltaTime);
        
        var horizontalMove = new Vector3(moveInput.x, 0f, moveInput.z);
        if (horizontalMove.sqrMagnitude < 0.001f) return;
        
        var targetRotation = Quaternion.LookRotation(horizontalMove);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 360 * Time.deltaTime);
    }
    
    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (hit.gameObject.layer == LayerMask.NameToLayer("Obstacle"))
        {
            _ = Die();
        }
    }

    private async Task Die()
    {
        GameStateManager.CurrentGameState = GameStateManager.GameState.Death;
    }
    
}
