using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class AutoPlayer : MonoBehaviour
{
    
    [SerializeField] private AutoPlayerConfiguration config;
    
    private CharacterController _controller;
    private AutoPlayerInput _input;
    private float _verticalVelocity;

    private void Awake()
    {
        Application.targetFrameRate = 60;
        _controller = GetComponent<CharacterController>();
    }

    private void Update()
    {
        if (_controller.isGrounded) _verticalVelocity = -2f;
        else _verticalVelocity -= config.gravityStrength;

        if (_input.Jump)
        {
            _input.Jump = false;
            _verticalVelocity = config.baseJumpHeight;
        }
        
        var moveInput = new Vector3(_input.Move, _verticalVelocity, config.forwardSpeed);
        
        _controller.Move(moveInput * Time.deltaTime);
    }
    
}
