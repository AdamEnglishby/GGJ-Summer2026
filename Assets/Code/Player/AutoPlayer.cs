using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class AutoPlayer : MonoBehaviour
{
    
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
            _verticalVelocity -= config.gravityStrength * brain.gravityModifier * Time.deltaTime * 2;
        }

        if (_input.Jump && _controller.isGrounded)
        {
            _input.Jump = false;
            _verticalVelocity = brain.GetJumpVelocity(config);
        }
        
        var moveInput = new Vector3(
            _input.Move * config.sideSpeed * brain.sideSpeedModifier,
            _verticalVelocity,
            config.forwardSpeed * brain.forwardSpeedModifier);
        
        _controller.Move(moveInput * Time.deltaTime);
    }
    
}
