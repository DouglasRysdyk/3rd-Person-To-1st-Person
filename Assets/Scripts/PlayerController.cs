using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    //Declarations
    #region "Misc."
    public CharacterController characterController;
    public LayerMask groundMask; 
    #endregion
    #region "GameObjects"
    public GameObject thirdPersonCamera; 
    public GameObject firstPersonCamera;
    #endregion
    #region "Transforms"
    public Transform playerBody;
    public Transform followTarget; 
    public Transform thirdPersonCameraTransform; 
    public Transform firstPersonCameraTransform; 
    public Transform groundCheck;
    #endregion
    #region "Input Actions"
    private PlayerControls playerControls;
    private InputAction movementAction;
    private InputAction attackAction;
    private InputAction jumpAction;
    private InputAction reloadAction;
    private InputAction holsterAction;
    private InputAction mouseAction;
    private InputAction sprintAction;
    private InputAction rightMouseAction;
    #endregion
    #region "Character Control Variables"
    //NOTE -- Some access is public for testing, but should be private when I land on values I like.  
    private Vector3 velocity;
    private float xRotation = 0f;
    public float moveSpeed = 6f;
    public float sprintMultiplier = 3f;
    public float gravity = -9.81f;
    public float jumpForce = 5f;
    public float mouseSensitivity = 100f;
    public float groundDistance = 0.4f;
    public float turnSmoothTime = 0.1f;
    private bool isGrounded;
    private bool holstered = true; 
    private bool aimDownSights = false; 
    #endregion
    //SET UP 
    private void Awake()
    {
        characterController = GetComponent<CharacterController>();

        //Reference the player controls and enable the Player action map.  
        playerControls = new PlayerControls();
        //Enabled the controls here initially but I may want to try changing the action map later based on whether or not the player is aiming down sights.  
        //We may want to pick the player's starting action map dynamically.  What if they load a game on a horse?  
        //Also being able to disable the player's inputs seems like it'd be a good idea.  
        playerControls.Player.Enable(); 

        movementAction = playerControls.Player.Movement;
        attackAction = playerControls.Player.Attack;
        jumpAction = playerControls.Player.Jump;
        reloadAction = playerControls.Player.Reload;
        holsterAction = playerControls.Player.Holster;
        mouseAction = playerControls.Player.Look;
        sprintAction = playerControls.Player.Sprint;
        rightMouseAction = playerControls.Player.Aim; 

        Cursor.lockState = CursorLockMode.Locked;
    }
    private void OnEnable()
    {
        //When a button is pressed the action is performed.  
        attackAction.performed += Attack;
        jumpAction.performed += Jump;
        reloadAction.performed += Reload;
        holsterAction.performed += Holster;
        sprintAction.started += StartSprint; sprintAction.canceled += CancelSprint;
        rightMouseAction.started += StartAimDownSights; rightMouseAction.canceled += CancelAimDownSights;
    }
    private void OnDisable()
    {
        //Performed actions need to be disabled to prevent memory leaks.
        attackAction.performed -= Attack;
        jumpAction.performed -= Jump;
        reloadAction.performed -= Reload;
        holsterAction.performed -= Holster;
        sprintAction.started -= StartSprint; sprintAction.canceled -= CancelSprint;
        rightMouseAction.started -= StartAimDownSights; rightMouseAction.canceled -= CancelAimDownSights;
    }
    void Update()
    {
        //Set the default camera to the Third Person mode with the Third Person controls.
        if (!aimDownSights)
        {
            thirdPersonCamera.SetActive(true);
            firstPersonCamera.SetActive(false);
            MouseLook();
            Movement();
        }
        //When aiming down sights, turn off the Third Person mode, and turn on First Person mode with the associated controls. 
        else if (aimDownSights)
        {
            //May want to enable aim down sights action map later, but at this time it's not even designed (02-14-2022). 
            thirdPersonCamera.SetActive(false);
            firstPersonCamera.SetActive(true);
            AimMouseLook();
        }
        //Maybe unnecessary.  
        else
        {
            return; 
        }
    }
    //CONTROLS
    public void Movement()
    {
        float horizontal = movementAction.ReadValue<Vector2>().x;
        float vertical = movementAction.ReadValue<Vector2>().y;

        //Get and store the player's movement inputs.  
        Vector3 direction = new Vector3(horizontal, 0, vertical).normalized;
        //Get the player's movement inputs and set the TPS camera's rotation (via the followTarget) to follow the player.  
        direction = direction.x * followTarget.right.normalized + direction.z * followTarget.forward.normalized;

        //If the player is inputting anything.  
        if (direction.magnitude >= 0.1f)
        {
            //float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + thirdPersonCameraTransform.eulerAngles.y;
            //float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);

            //Turn the player's body to follow the followTarget.  
            transform.rotation = Quaternion.Euler(0, followTarget.transform.rotation.eulerAngles.y, 0);
            //Set the camera directly behind the player.  
            followTarget.transform.localEulerAngles = new Vector3(0, 0, 0);
            //followTarget.transform.localEulerAngles = new Vector3(thirdPersonCameraTransform.eulerAngles.x, 0, 0);
            //playerBody.transform.localEulerAngles = new Vector3(0, angle, 0);
            //Turn the playerBody with the camera's rotation.  
            playerBody.transform.localRotation = Quaternion.Euler(thirdPersonCameraTransform.rotation.x, thirdPersonCameraTransform.rotation.y, 0f);

            characterController.Move(direction.normalized * moveSpeed * Time.deltaTime);
        }

        Gravity();
    }
    public void MouseLook()
    {
        float mouseX = mouseAction.ReadValue<Vector2>().x * mouseSensitivity * Time.deltaTime;
        float mouseY = mouseAction.ReadValue<Vector2>().y * mouseSensitivity * Time.deltaTime;

        //Transform the followTarget's rotation.  
        followTarget.transform.rotation *= Quaternion.AngleAxis(mouseX, Vector3.up);
        followTarget.transform.rotation *= Quaternion.AngleAxis(mouseY, Vector3.right);

        //Get the camera's followTarget rotation angles to clamp that rotation later.  
        var camAngles = followTarget.transform.localEulerAngles;
        camAngles.z = 0;

        //Separate the followTarget's X specifically in order to clamp it.  
        var camAngle = followTarget.transform.localEulerAngles.x;

        //Clamp the Up/Down rotation
        if (camAngle > 180 && camAngle < 340)
        {
            camAngles.x = 340;
        }
        else if (camAngle < 180 && camAngle > 40)
        {
            camAngles.x = 40;
        }

        //Set the TPS camera's rotation.  
        followTarget.transform.localEulerAngles = camAngles;

        //In order to keep the player's orientation, set the FPS camera's rotation to be the same as the followTarget's.  
        //The followTarget basically just holds the TPS camera's rotation.  
        firstPersonCameraTransform.transform.localEulerAngles = camAngles; 
        //firstPersonCameraTransform.transform.rotation = Quaternion.Euler(camAngles.x, camAngles.y, 0f); 
    }
    private void AimMouseLook()
    {
        float mouseX = mouseAction.ReadValue<Vector2>().x * mouseSensitivity * Time.deltaTime;
        float mouseY = mouseAction.ReadValue<Vector2>().y * mouseSensitivity * Time.deltaTime;

        //Control vertical aim.  
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        //Need to optimize.  
        //Rotate the FPS camera vertically.   
        firstPersonCameraTransform.transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        //Rotate the FPS camera horizontally.  
        firstPersonCameraTransform.Rotate(Vector3.up * mouseX);
        //Rotate the character body horizontally to match it with the camera.  
        ////This feels dumb.  It'd be smarter to match its orientation to whatever camera is active I think. 
        playerBody.Rotate(Vector3.up * mouseX);  
    }
    public void Attack(InputAction.CallbackContext context)
    {
        if (aimDownSights)
            print("Attack!");
    }
    public void Jump(InputAction.CallbackContext context)
    {
        //This isn't really working.
        if (isGrounded == true)
            velocity.y = Mathf.Sqrt(jumpForce * -2f * gravity);
        //characterController.Move(Vector3.up);

        //Probably want to read the input action value eventually so the player can control their jump height.  
    }
    public void Reload(InputAction.CallbackContext context)
    {
        print("Reloading...");
    }
    public void Holster(InputAction.CallbackContext context)
    {
        //Try condesning to a ternary (?:) operator? 
        if (holstered == true)
        {
            print("Draw the gun");
            holstered = false;
        }
        else
        {
            print("Holster the gun");
            holstered = true;
        }
    } 
    public void StartSprint(InputAction.CallbackContext context)
    {
        //Increase the player's speed. 
        moveSpeed *= sprintMultiplier;
    }
    public void CancelSprint(InputAction.CallbackContext context)
    {
        //Reset the character's movement speed.  
        moveSpeed /= sprintMultiplier;
    }
    public void StartAimDownSights(InputAction.CallbackContext context)
    {
        aimDownSights = true;
        //playerBody.transform.localRotation = Quaternion.Euler(followTarget.rotation.x, followTarget.rotation.y, 0f);
        playerBody.transform.localEulerAngles = firstPersonCameraTransform.localEulerAngles;
        //playerBody.transform.rotation = Quaternion.Euler(0f, playerBody.transform.localEulerAngles.y, 0f);
        //IDEA -- playerControls.Player.Disable(); Can enable the aim down sights action map here.  At the moment it's not even designed yet though (02-14-2022).

        //IDEA -- If I need different button functionality then I can switch the input action map.
        //Would need to switch the action map back in CancelAimDownSights. 
        //TODO
        //Get the FollowTarget or the ThirdPersonCameraTransform's rotation.  
        //Set the FPS camera to that rotation.  
        //Set the playerBody to that rotation.  
    }
    public void CancelAimDownSights(InputAction.CallbackContext context)
    {
        //IDEA -- playerControls.Player.Enable();
        aimDownSights = false;
        //TODO Probably.  
        //Get the FPS camera's or the FollowTarget's rotation.  
        //Set the ThirdPersonCamera to that rotation.  
    }
    //OPERATIONS
    private void Gravity()
    {
        //Create an object (basically a raycast) that returns true if the playerBody touches the ground. 
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        //-2 because it forces the palyer on to the ground a bit better than 0.  
        //Prevent velcoity from increasing too much.  
        if (isGrounded && velocity.y < 0)
            velocity.y = -2f; 

        //Apply gravity constantly independent of the framerate.  Twice, because the that's how the equation for gravity works.  
        velocity.y += gravity * Time.deltaTime;
        characterController.Move(velocity * Time.deltaTime);
    }
}
//OLD CODE
/*
{
        #region "Movement OLD CODE"
        //OLD CODE
        
        float horizontal = movementAction.ReadValue<Vector2>().x;
        float vertical = movementAction.ReadValue<Vector2>().y;
        Vector3 direction = new Vector3(horizontal, 0, vertical).normalized;

        if (direction.magnitude >= 0.1f)
        {
            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + thirdPersonCamera.eulerAngles.y;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);
            transform.rotation = Quaternion.Euler(0f, angle, 0f);

            Vector3 moveDirection = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
            characterController.Move(moveDirection.normalized * moveSpeed * Time.deltaTime);
        }

        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        if (isGrounded && velocity.y < 0)
            velocity.y = -2f; //Forces the player on to the gfround better 

        float moveX = movementAction.ReadValue<Vector2>().x;
        float moveZ = movementAction.ReadValue<Vector2>().y;

        velocity.y += gravity * Time.deltaTime;
        characterController.Move(velocity * Time.deltaTime);
        
        //Unity's Code 
        
        nextRotation = Quaternion.Lerp(followTransform.transform.rotation, nextRotation, Time.deltaTime * rotationLerp);

        if (_move.x == 0 && _move.y == 0) 
        {   
            nextPosition = transform.position;

            if (aimValue == 1)
            {
                //Set the player rotation based on the look transform
                transform.rotation = Quaternion.Euler(0, followTransform.transform.rotation.eulerAngles.y, 0);
                //reset the y rotation of the look transform
                followTransform.transform.localEulerAngles = new Vector3(angles.x, 0, 0);
            }

            return; 
        }
        float moveSpeed = speed / 100f;
        Vector3 position = (transform.forward * _move.y * moveSpeed) + (transform.right * _move.x * moveSpeed);
        nextPosition = transform.position + position;        
        

        //Set the player rotation based on the look transform
        transform.rotation = Quaternion.Euler(0, followTransform.transform.rotation.eulerAngles.y, 0);
        //reset the y rotation of the look transform
        followTransform.transform.localEulerAngles = new Vector3(angles.x, 0, 0);
        #endregion

        #region "MouseLook OLD CODE"
        //OLD CODE
        
        float mouseX = mouseAction.ReadValue<Vector2>().x * mouseSensitivity * Time.deltaTime;
        float mouseY = mouseAction.ReadValue<Vector2>().y * mouseSensitivity * Time.deltaTime;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        followTarget.transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        playerBody.Rotate(Vector3.up * mouseX);
        #endregion

        #region Player Based Rotation
        //Move the player based on the X input on the controller
        //transform.rotation *= Quaternion.AngleAxis(_look.x * rotationPower, Vector3.up);
        #endregion

        #region Follow Transform Rotation
        //Rotate the Follow Target transform based on the input
        followTransform.transform.rotation *= Quaternion.AngleAxis(_look.x * rotationPower, Vector3.up);
        #endregion

        #region Vertical Rotation
        followTransform.transform.rotation *= Quaternion.AngleAxis(_look.y * rotationPower, Vector3.right);

        var angles = followTransform.transform.localEulerAngles;
        angles.z = 0;

        var angle = followTransform.transform.localEulerAngles.x;

        //Clamp the Up/Down rotation
        if (angle > 180 && angle < 340)
        {
            angles.x = 340;
        }
        else if(angle < 180 && angle > 40)
        {
            angles.x = 40;
        }


        followTransform.transform.localEulerAngles = angles;
        #endregion

        
        nextRotation = Quaternion.Lerp(followTransform.transform.rotation, nextRotation, Time.deltaTime * rotationLerp);

        if (_move.x == 0 && _move.y == 0) 
        {   
            nextPosition = transform.position;

            if (aimValue == 1)
            {
                //Set the player rotation based on the look transform
                transform.rotation = Quaternion.Euler(0, followTransform.transform.rotation.eulerAngles.y, 0);
                //reset the y rotation of the look transform
                followTransform.transform.localEulerAngles = new Vector3(angles.x, 0, 0);
            }

            return; 
        }
        float moveSpeed = speed / 100f;
        Vector3 position = (transform.forward * _move.y * moveSpeed) + (transform.right * _move.x * moveSpeed);
        nextPosition = transform.position + position;        
        

        //Set the player rotation based on the look transform
        transform.rotation = Quaternion.Euler(0, followTransform.transform.rotation.eulerAngles.y, 0);
        //reset the y rotation of the look transform
        followTransform.transform.localEulerAngles = new Vector3(angles.x, 0, 0);
}
*/
/*
using UnityEngine;
using UnityEngine.InputSystem;

public class FirstPersonController : MonoBehaviour
{
    CharacterController characterController;
    public LayerMask groundMask;

    public Transform playerBody;
    public Transform playerCamera;
    public Transform groundCheck;
    
    #region "Input Actions"
    //private PlayerInput playerInput; //Why use this?  I think it's better to call actions directly from the action map.
    private PlayerControls playerControls; 
    //private InputAction movementAction;
    private InputAction jumpAction;
    private InputAction attackAction;
    private InputAction reloadAction;
    private InputAction holsterAction;
    private InputAction mouseAction;
    #endregion
    #region "Character Control Variables"
    //NOTE -- Some access is public for testing, but should be private when I land on values I like.  
    private Vector3 velocity;
    private float xRotation = 0f;
    //public float moveSpeed = 5f; 
    //public float sprintSpeed = 10f;
    //public float gravity = -9.81f; 
    //public float jumpForce = 5f;
    public float mouseSensitivity = 100f;
    //public float groundDistance = 0.4f;
    //private bool isGrounded;
    private bool holstered = true; //Is this ever going to be checked?  If not then move it to the Holster() method.
    #endregion  

    //SET UP
    private void Awake()
    {
        //playerInput = GetComponent<PlayerInput>(); //Again, why use this? 

        characterController = GetComponent<CharacterController>();

        playerControls = new PlayerControls();
        playerControls.Player.Enable(); //We may want to pick the player's starting action map dynamically.  What if they load a game on a horse?  
                                        //Also being able to disable the player's inputs seems like it'd be a good idea.  

        //movementAction = playerControls.Player.Movement;
        jumpAction = playerControls.Player.Jump;
        attackAction = playerControls.Player.Attack;
        reloadAction = playerControls.Player.Reload;
        holsterAction = playerControls.Player.Holster;
        mouseAction = playerControls.Player.Look;

        Cursor.lockState = CursorLockMode.Locked; 
    } 
    private void OnEnable()
    {
        //jumpAction.performed += Jump; 
        attackAction.performed += Attack; 
        reloadAction.performed += Reload; 
        holsterAction.performed += Holster; 
    }
    private void OnDisable()
    {
        //jumpAction.performed -= Jump;
        attackAction.performed -= Attack;
        reloadAction.performed -= Reload;
        holsterAction.performed -= Holster;
    }
    private void Update()
    {
        MouseLook();
        //Movement();
    }
    //CONTROLS
    public void MouseLook()
    {
        float mouseX = mouseAction.ReadValue<Vector2>().x * mouseSensitivity * Time.deltaTime;
        float mouseY = mouseAction.ReadValue<Vector2>().y * mouseSensitivity * Time.deltaTime;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);
 
        playerCamera.transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        playerBody.Rotate(Vector3.up * mouseX);
    }
    /*
    public void Movement()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        if (isGrounded && velocity.y < 0 )
            velocity.y = -2f; //Forces the player on to the gfround better 

        float moveX = movementAction.ReadValue<Vector2>().x; 
        float moveZ = movementAction.ReadValue<Vector2>().y;

        Vector3 direction = transform.right * moveX + transform.forward * moveZ;
        characterController.Move(direction * moveSpeed * Time.deltaTime);

        velocity.y -= gravity * Time.deltaTime;
        characterController.Move(velocity * Time.deltaTime);
    }
    */
/*
public void Jump(InputAction.CallbackContext context)
{
    if (isGrounded == true)
        velocity.y = Mathf.Sqrt(jumpForce * 2f * gravity);

    //Probably want to read the input action value eventually so the player can control their jump height.  
}

public void Attack(InputAction.CallbackContext context)
{
    if (holstered == true)
    {
        print("Gun is holstered.");
    }
    else
    {
        print("Attack!");
    }
}
public void Reload(InputAction.CallbackContext context)
{
    print("Reloading...");
}
public void Holster(InputAction.CallbackContext context)
{
    //Try condesning to a ternary (?:) operator? 
    if (holstered == true)
    {
        print("Draw the gun");
        holstered = false;
    }
    else
    {
        print("Holster the gun");
        holstered = true;
    }
}
}
*/
#region "Original code -- AimMouseLook"
/*
xRotation -= mouseY;
xRotation = Mathf.Clamp(xRotation, -90f, 90f);

firstPersonCameraTransform.transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
playerBody.Rotate(Vector3.up * mouseX);

//Fix ideas: 
//firstPersonCameraTransform.transform.localRotation *= Quaternion.AngleAxis(mouseX, Vector3.up);
//firstPersonCameraTransform.transform.localRotation *= Quaternion.AngleAxis(mouseY, Vector3.right);
//playerBody.Rotate(Vector3.up * mouseX);
//playerBody.Rotate(Vector3.right * mouseY);
*/
#endregion


