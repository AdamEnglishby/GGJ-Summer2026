using System.Threading.Tasks;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class AutoPlayer : MonoBehaviour
{
    
    private static readonly int Sliding = Animator.StringToHash("Sliding");

    [SerializeField] private LevelSpawner levelSpawner;
    [SerializeField] private AutoPlayerConfiguration config;
    [SerializeField] private AutoPlayerBrain brain;
    [SerializeField] private Animator animator;
    
    private CharacterController _controller;
    private AutoPlayerInput _input;
    private float _verticalVelocity;
    private float _slideTimer;
    private float _defaultHeight;
    private Vector3 _defaultCenter;

    private bool _runStarted;

    public AutoPlayerConfiguration Config => config;
    public bool IsGrounded => _controller.isGrounded;
    public float FeetOffsetY => -_controller.height * 0.5f + _controller.center.y;

    private void Awake()
    {
        _controller = GetComponent<CharacterController>();
        _defaultHeight = _controller.height;
        _defaultCenter = _controller.center;
    }

    [ContextMenu("Start Run")]
    public void StartRun()
    {
        levelSpawner.ResetLevel();
        
        transform.position = Vector3.zero;
        transform.rotation = Quaternion.identity;

        _runStarted = true;
    }

    private void Update()
    {
        if(!_runStarted) return;
        
        _input = brain.CalculateInput(this);
        
        if (_input.Slide && _controller.isGrounded && _slideTimer <= 0f)
        {
            animator.SetBool(Sliding, true);
            _input.Slide = false;
            _slideTimer = config.slideDuration;
        }

        if (_slideTimer > 0f)
        {
            _slideTimer -= Time.deltaTime;
            _controller.height = _defaultHeight * 0.5f;
            _controller.center = new Vector3(_defaultCenter.x, _defaultCenter.y - _defaultHeight * 0.25f, _defaultCenter.z);
        }
        else
        {
            animator.SetBool(Sliding, false);
            _controller.height = _defaultHeight;
            _controller.center = _defaultCenter;
        }
        
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
        _runStarted = false;
    }
    
}
