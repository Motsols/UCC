using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class MainMenuController : MonoBehaviour
{

    public void OnFire(InputAction.CallbackContext context) 
    {
        //check if button is pressed and we are not already loading a level
        //Due to to the singleton structure of GameSceneManager it's easy to read and call methods use by getting the instance from anywhere
        if (context.ReadValueAsButton() && !GameSceneManager.Instance.bIsLoadingScene) 
        {
            GameSceneManager.Instance.GoToLevel();
        }
    }


}
