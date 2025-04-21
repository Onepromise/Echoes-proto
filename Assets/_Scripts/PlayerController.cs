using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 5f;
    public float rotationSpeed = 10f;
    
    [Header("Animation")]
    public Animator animator;
    
    // Components
    private CharacterController characterController;
    private Camera mainCamera;
    
    // Movement variables
    private Vector3 moveDirection;
    private bool isMoving;
    
    void Start()
    {
        characterController = GetComponent<CharacterController>();
        mainCamera = Camera.main;
        
        if (animator == null)
            animator = GetComponentInChildren<Animator>();
    }
    
    void Update()
    {
        // Get input
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");
        
        // Calculate move direction relative to camera
        Vector3 forward = mainCamera.transform.forward;
        Vector3 right = mainCamera.transform.right;
        
        forward.y = 0;
        right.y = 0;
        forward.Normalize();
        right.Normalize();
        
        moveDirection = forward * vertical + right * horizontal;
        
        // Apply movement
        if (moveDirection.magnitude > 0.1f)
        {
            // Normalize movement when going diagonal
            moveDirection.Normalize();
            
            // Move character
            characterController.Move(moveDirection * moveSpeed * Time.deltaTime);
            
            // Rotate character towards movement direction
            Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
            
            isMoving = true;
        }
        else
        {
            isMoving = false;
        }
        
        // Update animation parameters
        if (animator != null)
        {
            animator.SetBool("IsMoving", isMoving);
        }
    }
}