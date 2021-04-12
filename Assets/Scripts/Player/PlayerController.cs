using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;



[RequireComponent(typeof(PlayerInput))]
public class PlayerController : MonoBehaviour
{
    /* Exposing fields to the components editor UI is as simple as marking it as declaring it public, or better yet [SerializeField] since it does not require the field to be public to other classes.
     * Most basic value types, references to MonoBehaviour (Components), GameObjects and Assets can be exposed to the editor. You can also expose custom types by writing custom code (see documentation)
    */
    #region Editor Exposed Fields



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


    //Current Mouse Position
    Vector2 MousePosition;

    //Our Character Desired Look At Rotation
    Quaternion DesiredRotation;

//Preprocessor Macro for Fields that are only every used in the editor
#if UNITY_EDITOR

    Vector3 LookAtLocation;

#endif


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
        //bAiming bool is set by "Aim" input, meaning we only rotate our character while we are holding the right mouse button
        if (bAiming) 
        {
            /*  To find out where we are aiming we create a plane and fire a ray from our camera towards it. This gives us an intersection point in world space.
             *  The intersection point is where we want our character to look. With a little math we can calculate a "look at" rotation from this, meaning the rotation our character should have to look at this point
             *  Raycasts are very handy for finding intersections in a 3D space. In this case we create our own intersecting "geometry", but we could use Physics.Raycast to check if our ray intersects with actual geometry in the level
             *  To filter these rays we can use a "layer mask" to decide which geometry we want to intersect with. Meaning we can filter out things and bobs that might be in the way to get the information we want.
             */

            //We default our hit location to 100m infront of our current position. Probably never needed but just to avoid getting weird rotations IF we for some reason don't get a hit
            Vector3 planeHitLocation = transform.position + (transform.forward * 100f);

            //We define a plane which will function as the collision plane for our raycast. The Plane is always level with the world coodinate system (vector3.up = {0,1,0}) and offset it to our current position
            //Since we are only currently moving in 2D we could just use a hardcoded 0, but this gives us the same functionality if we put in move vertical movement
            Plane projectionPlane = new Plane(Vector3.up,transform.position);

            //The ray describes the raycast orgin and direction
            Ray projectionRay = Camera.main.ScreenPointToRay(MousePosition);

            //Output distance for our raycast hit
            float distance = 0f;

            //We "shoot" a ray from our camera to intersect with our place. We get a success bool returned and also the distance from the camera where the ray intersects the plane
            if(projectionPlane.Raycast(projectionRay,out distance))
            {
                //Get the vector intersecting our ray at the raycast hit distance
                planeHitLocation = projectionRay.GetPoint(distance);
            }


#if UNITY_EDITOR

            LookAtLocation = planeHitLocation;

#endif
            Vector3 direction = (planeHitLocation - transform.position).normalized;
            DesiredRotation = Quaternion.LookRotation(direction);

            //rotate us over time according to speed until we are in the required rotation
            transform.rotation = Quaternion.Slerp(transform.rotation, DesiredRotation, Time.deltaTime * (Mathf.Deg2Rad*RotationSpeed));
        }

        if (PendingInputVector.SqrMagnitude() > 0) 
        {
            if (!TryMove()) 
            {
                Debug.Log("Failed To Move");
            }
        }
    }

#region INPUT HANDLERS

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

    public void OnMousePositionChange(InputAction.CallbackContext context)
    {
        MousePosition = context.ReadValue<Vector2>();
        Debug.Log("Mouse Position: " + MousePosition);
    }

#endregion

    //Try to move the game object (actually the transform component on the game object) using the Pending Movement Input (PendingInputVector)
    //Return false if we did not move
    bool TryMove() 
    {
        /* Time.deltaTime is the time, in seconds, that's passed since our last updated frame.
         * Since the number of frames rendered per seconds will vary depending on load we we need to use delta time for all time based movement
         * If we do not do this we will move faster when frame rate is high and slower when frame rate is low.
         * We are just caching it here for convience
         */
        float deltaTime = Time.deltaTime;

        //Cache current position;
        Vector3 previousPosition = transform.position;

        //Calculate our Movement Direction From Our Rotation and the Pending Input Vector
        //Vector3 movementVector = transform.rotation * new Vector3(PendingInputVector.x,0f,PendingInputVector.y);

        //Calculate our Movement Direction From Our Desired Rotation and Pending Input Vector
        Vector3 movementVector = DesiredRotation * new Vector3(PendingInputVector.x, 0f, PendingInputVector.y);

        //Try to actually move this GameObject by updating transform.position
        transform.position += movementVector * MovementSpeed * deltaTime;

        //Calculate Velocity from the delta between our previous position and new position. Divide by Time.deltaTime to get meters/s
        Velocity = (transform.position - previousPosition)/deltaTime;

        //If the square sum of our new velocity is > 0 we moved
        if (Velocity.sqrMagnitude > 0f) 
        {
            return true;
        }
        else 
        {
            return false;
        }
    }

    //Draw Gizmos is a handy way to visually debug data in the editor
    void OnDrawGizmosSelected()
    {
//OnDrawGizmos should only ever be called in the editor, but might as well be on the safe side to avoid any exceptions in a built game
#if UNITY_EDITOR
        // Draw a yellow sphere at the transform's position
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(LookAtLocation, .5f);

#endif
    }
}
