using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.InputSystem;

public class Coin : MonoBehaviour
{
    
    [SerializeField] private Transform coinMesh;
    [SerializeField] private LayerMask collectorLayer;
    [SerializeField] private Camera cam;
    [SerializeField] private float collectionAnimationDuration = 0.2f;
    [SerializeField] private ParticleSystem collectParticles;

    private bool _collected;
    
    private void Awake()
    {
        if (cam == null) cam = Camera.main;
    }

    private void Update()
    {
        coinMesh.Rotate(coinMesh.up, Time.deltaTime * 100f);

        if(_collected) return;
        
        var mousePressed = Mouse.current?.leftButton.wasPressedThisFrame;
        var touchscreenPressed = Touchscreen.current?.primaryTouch.press.wasPressedThisFrame;

        var position = default(Vector2);
        var pressed = false;
        
        if (mousePressed.HasValue && mousePressed.Value)
        {
            position = Mouse.current.position.ReadValue();
            pressed = true;
        }

        if (touchscreenPressed.HasValue &&  touchscreenPressed.Value)
        {
            position = Touchscreen.current.primaryTouch.position.ReadValue();
            pressed = true;
        }

        if (!pressed) return;
        var ray = cam.ScreenPointToRay(position);
        if (Physics.Raycast(ray, out var hit) && hit.transform == transform)
        {
            _ = Collect();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(_collected) return;
        if ((collectorLayer & (1 << other.gameObject.layer)) != 0)
        {
            _ = Collect();
        }
    }

    private async Task Collect()
    {
        GameStateManager.CoinCount++;
        
        _collected = true;
        collectParticles.Play();
        
        var initialScale = transform.localScale;
        var targetScale = Vector3.zero;
        var timer = 0f;

        while (timer < collectionAnimationDuration)
        {
            transform.localScale = Vector3.Lerp(initialScale, targetScale, timer / collectionAnimationDuration);
            timer += Time.deltaTime;
            await Task.Yield();
        }

        await Awaitable.WaitForSecondsAsync(2f);
        transform.localScale = targetScale;
        Destroy(gameObject);
    }
    
}
