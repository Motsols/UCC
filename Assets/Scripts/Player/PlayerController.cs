using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;



[RequireComponent(typeof(PlayerInput))]
public class PlayerController : MonoBehaviour
{

#region Editor Exposed Fields
/* Exposing fields to the components editor UI is as simple as marking it as declaring it public, or better yet [SerializeField] since it does not require the field to be public to other classes.
 * Most basic value types, references to MonoBehaviour (Components), GameObjects and Assets can be exposed to the editor. You can also expose custom types by writing custom code (see documentation)
*/


    //Movement Speed in Meters/s
    [SerializeField]
    float MovementSpeed = 4f;

    //Rotation Speed in Degrees/s
    [SerializeField]
    float RotationSpeed = 360f;

#endregion


    //Pending Input Vector to be handled next update
    Vector2 PendingInputVector;

    //Our previous input vector from last frame. Useful for checking things like if we started movement this frame
    Vector2 PreviousInputVector;

    //Flag for Aiming
    public bool bAiming { get; private set; }

    //Current Velocity for Our Player Character
    public Vector3 Velocity { get; private set; }

    private void Awake()
    {

    }


    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        /*  To find out where we are aiming we deproject the mouse position to a world position and perform a RayCast from the camera position towards that position
         *  We then compare our hit position to our player characters position and calculate a "look at" rotation from that.
         *  We could set the rotation directly, but to get smooth movement we instead interpolate our rotation towards the desired rotation
         */
        if (bAiming) 
        {

        }

        if (PendingInputVector.SqrMagnitude() > 0) 
        {
            if (!TryMove()) 
            {
                Debug.Log("Failed To Move");
            }
        }
    }

    public void OnMovementInput(InputAction.CallbackContext context) 
    {
        PendingInputVector = context.ReadValue<Vector2>();
        //Debug.Log("OnMovementUpdate::Pending Vector: " + PendingInputVector);
    }

    public void OnAimInput(InputAction.CallbackContext context) 
    {
        bAiming = context.ReadValueAsButton();
        Debug.Log("bAiming: " + bAiming);
    }

    bool TryMove() 
    {
        //Cache current position;
        Vector3 previousPosition = transform.position;

        //Calculate our Movement Direction From Our Rotation and the Pending Input Vector
        Vector3 movementVector = transform.rotation * new Vector3(PendingInputVector.x,0f,PendingInputVector.y);

        //Try to actually move this GameObject by updating transform.position
        transform.position += movementVector * MovementSpeed * Time.deltaTime;

        //Calculate Velocity from the delta between our previous position and new position. Divide by Time.deltaTime to get meters/s
        Velocity = (transform.position - previousPosition)/Time.deltaTime;

        if (Velocity.sqrMagnitude > 0f) 
        {
            return true;
        }
        else 
        {
            return false;
        }
    }

}
