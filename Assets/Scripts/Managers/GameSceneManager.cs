using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameSceneManager : MonoBehaviourSingleton<GameSceneManager>
{
    [SerializeField]
    public string MainMenuScenePath;

    [SerializeField]
    public string LevelScenePath;

    public delegate void SimpleSceneLoadDelegate();

    // Useful Events to Listen to When Scene Loading Begins and Ends.
    // Can be used to show a loading screen overlay for example
    public event SimpleSceneLoadDelegate OnLoadingSceneBegin;
    public event SimpleSceneLoadDelegate OnLoadingSceneComplete;

    public bool bIsLoadingScene { get; private set; } = false;

    public string CurrentLevel { get; private set; } = "none";

    private void Start()
    {
        GoToMainMenu();
    }


    public void GoToMainMenu()
    {
        //Check if we should unload the current scene.
        //I'm far from a fan of using strings this much but it's an easy starting point
        if (CurrentLevel != "none" && CurrentLevel != MainMenuScenePath)
        {
            SceneManager.UnloadScene(CurrentLevel);
        }

        CurrentLevel = MainMenuScenePath;
        bIsLoadingScene = true;

        LoadNewLevelAsync(CurrentLevel);
    }

    //Dummy function for loading the first level. This needs to be expanded for better level handling
    //Levels can also be referenced by "Build Order" which is the oder they are set in the project options.
    public void GoToLevel() 
    {
        if(CurrentLevel != "None" && CurrentLevel != LevelScenePath) 
        {
            SceneManager.UnloadScene(CurrentLevel);
        }

        CurrentLevel = LevelScenePath;
        bIsLoadingScene = true;

        LoadNewLevelAsync(CurrentLevel);
    }

    void LoadNewLevelAsync(string LevelPath) 
    {
        //Notify our listeners that we are loading a new scene. It is now valid to get current level to check which scene is loading.
        OnLoadingSceneBegin?.Invoke();

        /* Async Scene Loading with Additive mode. This will load a scene on top of our current scene, meaning it will not destoy the gameobjects that are in our "Main Scene" Where the GameSceneManager lives
         * Subscribe to complete event on the async action to notify us when the loading is complete. 
         * This is hardly useful for a project of this size. Our levels will load in fractions of a second but useful for larger level setups
         */
        AsyncOperation sceneloading = SceneManager.LoadSceneAsync(LevelPath, LoadSceneMode.Additive);
        sceneloading.completed += OnSceneLoadCompleted;
    }


    void OnSceneLoadCompleted(AsyncOperation opertation) 
    {
        bIsLoadingScene = false;
        OnLoadingSceneComplete?.Invoke();
    }

}
