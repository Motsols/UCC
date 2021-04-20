using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SnowballType", menuName = "ScriptableObjects/Snowball Type", order = 1)]
public class SnowballType : ScriptableObject
{
    [SerializeField,Tooltip("Force multiplier used with this type of snowball. Applied on top of component force multiplier")]
    float SnowballForceMultiplier = 1f;

    [SerializeField, Tooltip("Prefab to use when instantiating this snowball type. Should only effect visuals")]
    GameObject SnowballPrefab = null;


    //We only want to allow snowballs to be added as prefabs in our SnowballPrefab
    //So we check that the GameObject (Prefab) reference has the required Snowball Component
    //If it does not we clear the value
    private void OnValidate()
    {
        if(SnowballPrefab.GetComponent<SnowBall>() == null) 
        {
            SnowballPrefab = null;
        }
    }
}
