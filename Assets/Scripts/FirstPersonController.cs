using UnityEngine;
using UnityEngine.InputSystem; 

public class FirstPersonController : MonoBehaviour
{
    Rigidbody rigidBody;
    //Input Actions
    #region
    //private PlayerInput playerInput; //Why use this?  I think it's better to call actions directly from the action map.
    private PlayerControls playerControls; 
    private InputAction movementAction;
    private InputAction jumpAction;
    private InputAction attackAction;
    private InputAction reloadAction;
    private InputAction holsterAction;
    #endregion
    //Movement
    //NOTE -- Access is public for testing, but should be private when I land on values I like.  
    #region
    public float moveSpeed = 5f; 
    public float sprintSpeed = 10f;
    public float gravity = 30f;
    public float jumpForce = 5f; 
    #endregion

    private bool holstered = true; //Is this ever going to be checked?  If not then move it to the Holster() method.  

    private void Awake()
    {
        rigidBody = GetComponent<Rigidbody>();
        //playerInput = GetComponent<PlayerInput>(); //Again, why use this? 

        playerControls = new PlayerControls();
        playerControls.Player.Enable(); //We may want to pick the player's starting action map dynamically.  What if they load a game on a horse?  
                                        //Also being able to disable the player's inputs seems like it'd be a good idea.  

        movementAction = playerControls.Player.Movement;
        jumpAction = playerControls.Player.Jump;
        attackAction = playerControls.Player.Attack;
        reloadAction = playerControls.Player.Reload;
        holsterAction = playerControls.Player.Holster;
    }
    
    private void OnEnable()
    {
        jumpAction.performed += Jump; 
        attackAction.performed += Attack; 
        reloadAction.performed += Reload; 
        holsterAction.performed += Holster; 

    }
    private void OnDisable()
    {
        jumpAction.performed -= Jump;
        attackAction.performed -= Attack;
        reloadAction.performed -= Reload;
        holsterAction.performed -= Holster;
    }
    private void FixedUpdate()
    {
        Movement();
    }
    //CONTROLS
    public void Movement()
    {
        Vector2 inputVector = movementAction.ReadValue<Vector2>(); 

        rigidBody.AddForce(new Vector3(inputVector.x, 0, inputVector.y) * moveSpeed, ForceMode.Force);
    }
    public void Jump(InputAction.CallbackContext context)
    {
        //Probably want to read the input action value eventually so the player can control their jump height.  

        rigidBody.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);

        print("Jump!");
    }
    public void Attack(InputAction.CallbackContext context) //This is what's passed in by all the input actions.  
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

/*
 * It makes sense to me to make the controls Vector2 since we're not really controling the character in a three dimensional way. 
 * The character is basically a highly mobile tank.  
 * WASD/Arrows move the player left, right, forward, and back.  
 * Mouse look will determine their orientation.  Obviously the player can't just look up and hit W to move into the sky.  
 * Jump is the only Z axis movement.  
 

* //This is what a guard statement basically looks like.  Learn to use it more.  
* //if (!trueToPass)
* //    return; 

 * Sources:
 * - Using the Generate C# Class Option -- https://www.youtube.com/watch?v=a2vLaKGCYsA 
 * - How to use the new Input System -- https://www.youtube.com/watch?v=m5WsmlEOFiA TAKE NOTES
 * - There is a section going over how to handle pressing and holding -- https://docs.unity3d.com/Packages/com.unity.inputsystem@1.0/manual/index.html 
 * - Review the generated code later.  
 * - Search this -- https://www.youtube.com/results?search_query=press+and+hold+unity+input+system 
 * - Reviews holding a button down -- https://youtu.be/Yjee_e4fICc?t=813 

 * Notes: 
 * - Value = If the player has multiple controls working at the same time then the input system will select a main one.(?)
 *   - You'll usually use value since the player will normally only be using one controller at a time.  
 *   - Maybe this would falter in a split screen multiplayer situation.  
 * - Button = Very similar to Value, except called less often -- https://docs.unity3d.com/Packages/com.unity.inputsystem@1.0/manual/Actions.html#action-types
 * - Pass Through = If the player has multiple controls working at the same time the input system will simply receive any inputs.(?)
 * - Control Type assigns the data type (Vector2, Quaternion, "Eyes"(?), more... I don't really get all the options).  
 * - Callbacks -- https://docs.unity3d.com/Packages/com.unity.inputsystem@1.0/manual/Interactions.html#default-interaction 
 * - Vector2 Composite -- A group of different bindings (in this case a Vector2 - good for WASD movement).  
 * - Interactions has the option for Holding.
 *   - Test on jumping.  
 * - A modifier is like a combo.  If I understand correctly you use this with floats.  
 * - The input system lets you organize control schemes.  So if you are making a game for mobile and PC you can make a Mobile control scheme and Desktop control scheme.
 * - When generating code the namespace defaults to the Global Namespace.  
 * - Processors let you do things like Clamp the values.  This is the only example I understand off the bat after looking at it.  
 * - A button is a float.  It starts at 0 and goes to 1.  I guess this is to measure partial presses.  
 * - She said this initial method (the one I'm using; the one that WORKS) is not recommended by Unity.  But the second method did not work for me at all.
 *   - Why is this method not recommended?  
 *   - How do I get her method working?  
 *   - Which one is actually better? 
 *     - Truthfully the answer is the one that works first and foremost.  
 *     
 * - Generating the script means the generated script cannot easily be changed.  This makes rebinding keys difficult (but not impossible).
 *   - Rebinding for who?  The player or the dev?  
 *   - The player?  How does the player rebind controls?  
 *     - Unity has a component called Player Input.  
 *   - Put your PlayerInputs there.  
 *   - Select the default map.  
 *   - UI Input Module and Camera are really used for local multiplayer.  This works with the Player Input Manager component.  
 *   - What does "Behavior" mean?  
 *   - Under Behavior you can select Invoke Unity Events.  
 *     - She does not recommend doing this so okay.  I guess it's because it doesn't differentiate between different contexts?  
 * - Naming Convetions: 
 *   - Player Controls input action asset -- Goes on Player Input component.
 *   - Player Input component -- Goes on player character.
 *   - CharacterController script -- Goes on player character.
 *     - Access the Player Input component from here.  
 *     - Call the actions you want via their names (Strings) in the input action asset.  
 * - Read, understand -- https://stackoverflow.com/questions/155609/whats-the-difference-between-a-method-and-a-function 
 */
