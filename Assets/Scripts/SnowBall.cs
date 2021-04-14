using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SphereCollider))]
[RequireComponent(typeof(Rigidbody))]
public class SnowBall : MonoBehaviour
{
    //The character that instantiated this snowball
    SnowballThrower OwningThrower;

    //Collider for this Snowball. Handles our collision detection
    SphereCollider SnowballCollider;

    //Rigidbody for this Snowball. Handles our physics simulation
    Rigidbody SnowballRigidBody;

    //Delegate declaration for our callback type 
    public delegate void SnowballHitCallback(SnowBall snowball,Collision collision,bool bPlayerHit);

    public delegate void AnySnowballHitDelegate(SnowballThrower thrower, SnowBall snowBall, Collision collision, bool bPlayerHit);

    //Static event notifying us when ANY snowball hits. This is useful for i.e a centrailized event system that creates particle systems on snowball launches
    public static event AnySnowballHitDelegate OnAnySnowballHit;

    //Event raised when the snowball hits something
    event SnowballHitCallback OnSnowballHit;

    void Awake()
    {
        SnowballCollider = GetComponent<SphereCollider>();
        if(SnowballCollider == null) 
        {
            Debug.LogError("Failed to Find Sphere Collider on Snowball");
        }

        //We disable the collider by default, only activating it once the snowball has been launched
        SnowballCollider.enabled = false;

        SnowballRigidBody = GetComponent<Rigidbody>();
        SnowballRigidBody.isKinematic = true;
        if(SnowballRigidBody == null) 
        {
            Debug.LogError("Failed to find Rigid Body Component on Snowball");
        }

    }

    //Register a Callback for Snowball Hits
    void AddSnowballHitListener(SnowballHitCallback callback) 
    {
        OnSnowballHit += callback;
    }

    //Unregister a Callback for Snowball Hits
    void RemoveSnowballHitListener(SnowballHitCallback callback) 
    {
        OnSnowballHit -= callback;
    }

    //The Snowball Collision Component will Notify us of Collisions using the Unity Message System
    void OnCollisionEnter(Collision collision)
    {
        bool bPlayerHit = false;

        //We check if they GameObject we hit has a PlayerController. If it does it's a player
        if (collision.gameObject.GetComponent<PlayerController>() != null) 
        {
            Debug.Log("Snowball: " + this + " Hit Player: " + collision.gameObject);
            bPlayerHit = true;
        }
        else 
        {
            Debug.Log("Snowball hit: " + collision.gameObject);
        }


        //Invoke events for any listeners we have, both for static any snowball hits (effects etc) and specific snowball hits for points counts etc.
        OnAnySnowballHit?.Invoke(OwningThrower, this, collision, bPlayerHit);
        OnSnowballHit?.Invoke(this, collision, bPlayerHit);

        //TODO: Destroy Snowball
    }

    public void Launch(Vector3 direction,float force,SnowballHitCallback callback) 
    {
        //Make our snowball kinematic so it will actually fly away and behave correctly
        SnowballRigidBody.isKinematic = false;

        //Add an impulse force to the center of the snowball to launch it
        SnowballRigidBody.AddForce(direction * force, ForceMode.Impulse);

        //Enable our collider to detect collisions
        SnowballCollider.enabled = true;
    }

    public bool InitSnowball(SnowballThrower owningThrower) 
    {
        //We only allow Player Controllers to 
        if(owningThrower != null) 
        {
            OwningThrower = owningThrower;
            //Find the Collider on the same GameObject as the Thrower Component, and Check that it's not Null just to be Safe.
            Collider playerCollider = OwningThrower.GetComponent<Collider>();
            if (playerCollider == null)
            {
                Debug.LogError("Failed to find Collider on owning Player Controller GameObject");
                return false;
            }

            //Ignore collision between the snowball and it's owning player. We don't want to hit ourselves
            Physics.IgnoreCollision(SnowballCollider, playerCollider);

            //Initialization Completed
            return true;
        }

        //Fail out if Player Controller is null
        return false;
    }


}
