using UnityEngine;
using UnityEngine.InputSystem;
using FishNet.Connection;
using FishNet.Object;

public class FirstPersonController : NetworkBehaviour
{
    [Header("Base setup")]
    public float walkingSpeed = 7.5f;
    public float runningSpeed = 11.5f;
    public float acceleration = 10.0f;
    public float jumpSpeed = 8.0f;
    public float gravity = 20.0f;
    public float lookSpeed = 2.0f;
    public float lookXLimit = 70.0f;
 
    CharacterController characterController;
    Vector3 moveDirection = Vector3.zero;
    float rotationX = 0;
 
    [HideInInspector]
    public bool canMove = true;
 
    [SerializeField]
    private float cameraYOffset = 0.4f;
    private Camera playerCamera;

    // Audio
    public AudioClip LandingAudioClip;
    public AudioClip[] FootstepAudioClips;
    [Range(0, 1)] public float FootstepAudioVolume = 0.5f;

    // Animator
    private Animator _animator;

    private float _animationBlend = 0f;

    // Animation IDs
    private int _animIDSpeed;
    private int _animIDGrounded;
    private int _animIDJump;
    private int _animIDFreeFall;
    private int _animIDMotionSpeed;

    // Pause
    private PauseController pauseController;
    
    // Methods
    public override void OnStartClient()
    {
        base.OnStartClient();
        if (base.IsOwner)
        {

            playerCamera = Camera.main;
            playerCamera.transform.position = new Vector3(transform.position.x, transform.position.y + cameraYOffset, transform.position.z);
            playerCamera.transform.SetParent(transform);

            pauseController = playerCamera.GetComponent<PauseController>();

            foreach (Transform child in transform)
            {
                // Change the skinned mesh renderer to shadows only 
                if (child.GetComponent<SkinnedMeshRenderer>() != null)
                {
                    child.GetComponent<SkinnedMeshRenderer>().shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.ShadowsOnly;
                }
            }
        }
        else
        {
            gameObject.GetComponent<FirstPersonController>().enabled = false;
        }
    }
 
    void Start()
    {
        characterController = GetComponent<CharacterController>();
 
        // Animator
        AssignAnimationIDs();

        if (_animator == null) {
            _animator = GetComponent<Animator>();
        }
        _animator.fireEvents = false;

        // Lock cursor
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
 
    bool isWalking = false;
    void Update()
    {
        bool isPaused = pauseController != null ? pauseController.isPaused : false;

        // Press Left Shift to run
        isWalking = Input.GetKey(KeyCode.LeftShift);
 
        // We are grounded, so recalculate move direction based on axis
        Vector3 forward = transform.TransformDirection(Vector3.forward);
        Vector3 right = transform.TransformDirection(Vector3.right);
 
        float curSpeedX = canMove ? (isWalking ? walkingSpeed : runningSpeed) * Input.GetAxis("Vertical") : 0;
        float curSpeedY = canMove ? (isWalking ? walkingSpeed : runningSpeed) * Input.GetAxis("Horizontal") : 0;
        float movementDirectionY = moveDirection.y;
        float speed = 0.0f;
        if (curSpeedX != 0) {
            speed = Mathf.Lerp(curSpeedX, isWalking ? walkingSpeed : runningSpeed, Time.deltaTime * acceleration);
            speed = Mathf.Round(speed * 1000f) / 1000f;
        }
        moveDirection = (forward * speed) + (right * curSpeedY);
 
        if (Input.GetButton("Jump") && canMove && characterController.isGrounded && !isPaused)
        {
            moveDirection.y = jumpSpeed;
        }
        else
        {
            moveDirection.y = movementDirectionY;
        }
 
        if (!characterController.isGrounded)
        {
            moveDirection.y -= gravity * Time.deltaTime;
        }
 
        // Move the controller
        characterController.Move(moveDirection * Time.deltaTime);
 
        // Player and Camera rotation
        if (canMove && playerCamera != null && !isPaused)
        {
            rotationX += -Input.GetAxis("Mouse Y") * lookSpeed;
            rotationX = Mathf.Clamp(rotationX, -lookXLimit, lookXLimit);
            playerCamera.transform.localRotation = Quaternion.Euler(rotationX, 0, 0);
            transform.rotation *= Quaternion.Euler(0, Input.GetAxis("Mouse X") * lookSpeed, 0);
        }

        // On free fall
        if (!characterController.isGrounded) {
            if (characterController.velocity.y < 0) {
                _animator.SetBool(_animIDFreeFall, true);
                _animator.SetBool(_animIDGrounded, false);
                _animator.SetBool(_animIDJump, false);
            } else {
                _animator.SetBool(_animIDFreeFall, false);
                _animator.SetBool(_animIDGrounded, false);
                _animator.SetBool(_animIDJump, true);
            }
        }
        // On ground
        else {
            _animator.SetBool(_animIDGrounded, true);
            _animator.SetBool(_animIDJump, false);
            _animator.SetBool(_animIDFreeFall, false);

            // On move
            if (curSpeedX != 0 || curSpeedY != 0) {
                _animationBlend = Mathf.Lerp(_animationBlend, (isWalking ? walkingSpeed : runningSpeed), Time.deltaTime * acceleration);
                if (_animationBlend < 0.01f) _animationBlend = 0f;

                _animator.SetFloat(_animIDSpeed, _animationBlend);
                _animator.SetFloat(_animIDMotionSpeed, 1.0f);
            } else {
                _animationBlend = Mathf.Lerp(_animationBlend, 0f, Time.deltaTime * acceleration);
                if (_animationBlend < 0.01f) _animationBlend = 0f;

                _animator.SetFloat(_animIDSpeed, _animationBlend);
                _animator.SetFloat(_animIDMotionSpeed, 0.0f);
            }
        }
        // On jump
        // if (Input.GetButton("Jump") && canMove && characterController.isGrounded) {
        //     _animator.SetBool(_animIDJump, true);
        // }
    }

    private void OnFootstep(AnimationEvent animationEvent)
    {
        Debug.Log("OnFootstep");
        if (isWalking) return;

        if (animationEvent.animatorClipInfo.weight > 0.5f)
        {
            if (FootstepAudioClips.Length > 0)
            {
                var index = Random.Range(0, FootstepAudioClips.Length);
                AudioSource.PlayClipAtPoint(FootstepAudioClips[index], transform.TransformPoint(characterController.center), FootstepAudioVolume);
            }
        }
    }

    private void OnLand(AnimationEvent animationEvent)
    {
        Debug.Log("OnLand");
    }

    // Fancy pants animation stuff
    private void AssignAnimationIDs()
    {
        _animIDSpeed = Animator.StringToHash("Speed");
        _animIDGrounded = Animator.StringToHash("Grounded");
        _animIDJump = Animator.StringToHash("Jump");
        _animIDFreeFall = Animator.StringToHash("FreeFall");
        _animIDMotionSpeed = Animator.StringToHash("MotionSpeed");
    }
}