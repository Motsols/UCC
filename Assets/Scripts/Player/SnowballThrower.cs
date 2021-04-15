using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnowballThrower : MonoBehaviour
{
#region EDITOR EXPOSED FIELDS
    //Prefab used as our Snowball
    [SerializeField]
    SnowBall SnowballPrefab;

    //GameObject transform component to use as the spawning point for our snowballs
    [SerializeField]
    GameObject SnowballAnchor;

    //The pitch (up down) we launch our snowball at.
    //float If 0 we launch it straight ahead, 1 straight up, -1 straight down.
    [SerializeField, Range(-1f, 1f)]
    float LaunchPitch = .2f;

    //Time it takes to create a snowball
    [SerializeField]
    float SnowballCreationTime = 1.5f;

    //When snowball is thrown we get the number of seconds the fire button has been held down as an input
    //To Translate this into a usable force we multiply it by this number. Exposed for balancing
    [SerializeField]
    float SnowballForceMultiplier = 100f;

    //An upper ceiling for SnowballForceMultiplier so it can't get completely out of hand
    [SerializeField]
    float MaxSnowballForceMultiplier = 10000f;



    #endregion

    #region EVENTS

    /** Due to Unitys structure where we use a myriad of dirrent components which often needs to communicate
     * using an observer pattern is a great way to avoid having to keep reference to other GameObjects and Components.
     */

    //A simple delegate definition for notifiers
    public delegate void SimpleSnowballDelegate();

    //A delegate definition for snowball launches
    public delegate void SnowballThrowDelegate(SnowballThrower thrower, SnowBall snowball, Vector3 startPosition, Vector3 launchVector);

    

    //Event notifying when SnowballThrower starts creating a snowball
    public event SimpleSnowballDelegate OnStartSqueezingSnowball;

    //Event notifying when SnowballThrower has completed/failed creating a snowball. check bHasSnowball for sucess/fail
    public event SimpleSnowballDelegate OnEndSqueezingSnowball;

    //Static event notifying us when ANY snowball is thrown. This is useful for i.e a centrailized event system that creates particle systems on snowball launches
    public static event SnowballThrowDelegate OnAnySnowballThrown;

    /** Centralized systems for effects, sounds etc is a good idea since we can do object pooling and reuse GameObjects for such things. Again this is a bit over the top for
     *  a tiny little project like this but a nice design choice which keeps things clean.
     */

#endregion


    //Launch Vector for our snowball, relative to our character rotation
    Vector3 LaunchVector;

    //Our Current Snowball
    public SnowBall CurrentSnowBall { get; private set; }

    //Bool Property for Checking if we are Currently Holding a Snowball
    //We could do a null check on CurrentSnowball but Unity unfortunately has a null check override which makes null checks rather slow
    //So this is strictly not necessary but good to keep in mind... but not really for small projects with limited overhead like this one
    public bool bHasSnowball { get; private set; }

    //Bool Property for Checking if our snowball is ready to be thrown
    //We have implemented a Coroutine that creates the snowball over time since we want to simulate the "krama en snöboll" sequence
    public bool bSnowballIsReady { get; private set; }

    //Bool Property for Checking if we are currently creating a snowball
    public bool bIsCreatingSnowball
    {
        get { return bHasSnowball && !bSnowballIsReady; }
    }

    //Bool Field for checking that this component has been successfully initialized
    bool Initialized = true;

    private void Awake()
    {
        //Calculate our launch vector, this will always be relative to the attachment rotation and just lauch the snowball forward and slightly up/down
        LaunchVector = new Vector3(0f, LaunchPitch, 0f);
        bHasSnowball = false;
        bSnowballIsReady = false;


        if(SnowballPrefab == null) 
        {
            Initialized = false;
        }
    }

    //Create a new snowball, will fail if we already have a snowball.
    //Attaches the snowball to a GameObject transform so the snowball will be attached until we throw it
    public bool CreateSnowball() 
    {
        if(SnowballAnchor == null) 
        {
            Debug.LogError("No Snowball Anchor has been set for the snowball thrower");
            return false;
        }

        if (!Initialized) 
        {
            Debug.LogError("Unable to create snowball. Snowball prefab not set");
            return false;
        }


        //Don't create a snowball if we already have one
        if (bHasSnowball) 
        {
            Debug.Log("Snowball Already Exists");
            return false;
        }

        CurrentSnowBall = Instantiate(SnowballPrefab) as SnowBall;
        if (!CurrentSnowBall.InitSnowball(this)) 
        {
            Debug.LogError("Failed to Create Snowball");
            Destroy(CurrentSnowBall);
        }
        CurrentSnowBall.transform.SetParent(SnowballAnchor.transform);
        CurrentSnowBall.transform.localPosition = Vector3.zero;
        CurrentSnowBall.transform.localRotation = Quaternion.identity;

        bHasSnowball = true;
        bSnowballIsReady = false;

        //Couroutines: See below
        StartCoroutine("CreateSnowballCoroutine");
        //StopCoroutine to cancel a running coroutine. Only works with coroutines that have been started with a name string unfortunately

        return true;
    }

    //Interuption function when creating snowball. Not Implemented
    void InteruptCreateSnowball() 
    {
        if(!bHasSnowball || !bIsCreatingSnowball) 
        {
            return;
        }

        StopCoroutine("CreateSnowballCoroutine");

        Destroy(CurrentSnowBall);
        bHasSnowball = false;
        bSnowballIsReady = false;
    }

    //Avoiding using Update is something to strive for. MonoBehaviours can use coroutines to carry out functionality over time
    //Here we create a snowball over the SnowballCreationTime (This coroutine already exists and is called WaitForSeconds which is a non blocking wait coroutine)
    //But this is an example of how a coroutine is implemented. The coroutine should return "yeild return null" while not completed
    public IEnumerator CreateSnowballCoroutine() 
    {
        Debug.Log("Waiting for Snowball Packing");
        float waitedTime = 0f;
        while(waitedTime < SnowballCreationTime) 
        {
           
            waitedTime += Time.deltaTime;

            //TODO: Grow the snowball here, increasing it's size over time until it's finished
            yield return null;
        }

        Debug.Log("Snowball Ready");
        bSnowballIsReady = true;
    }

    //Throw our Current Snowball. Will Return False if we do not have a Current Snowball
    public bool ThrowSnowball(Vector3 direction,float inputHeldForSeconds) 
    {
        if (!bHasSnowball || !Initialized) 
        {
            return false;
        }

        CurrentSnowBall.gameObject.transform.SetParent(null);

        bHasSnowball = false;
        bSnowballIsReady = false;

        //Calculate the force to apply to the snowball based on force multiplier and second the fire button has been held down
        float force = inputHeldForSeconds * Mathf.Min(SnowballForceMultiplier, MaxSnowballForceMultiplier);
        Vector3 launchDirection = (direction + LaunchVector).normalized;

        //Launch the callback and supply a callback function for hits
        CurrentSnowBall.Launch(direction, force,OnSnowballHit);

        //Raise snowball thrown event
        OnAnySnowballThrown?.Invoke(this, CurrentSnowBall, transform.position, launchDirection);
        CurrentSnowBall = null;

        return true;
    }


    //Callback function for snowball hits
    void OnSnowballHit(SnowBall snowball,Collision collision,bool bPlayerHit) 
    {
        //TODO: We can do point counts etc from here.
    }

}
