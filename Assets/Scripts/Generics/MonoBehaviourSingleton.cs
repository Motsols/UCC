using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//Generic MonoBehaviour singleton pattern using CRTP for use with MonoBehaviour Managers i.e SceneManger
public class MonoBehaviourSingleton<T>:MonoBehaviour where T:MonoBehaviourSingleton<T>
{
    public static T Instance { get; protected set; }
   

    void Awake() 
    {
        //Ensure that we only ever have one instance
        if(Instance != null && Instance != this) 
        {
            Destroy(this);
            throw new System.Exception("An Instance of Singleton Already Exists");
        }
        else 
        {
            Instance = (T)this;
        }
    }
}
