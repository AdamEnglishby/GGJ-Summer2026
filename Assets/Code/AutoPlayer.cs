using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class AutoPlayer : MonoBehaviour
{
    
    private CharacterController _controller;

    private void Awake()
    {
        _controller = GetComponent<CharacterController>();
    }

    private void Update()
    {
        // TODO: make the guy move
    }
    
}
