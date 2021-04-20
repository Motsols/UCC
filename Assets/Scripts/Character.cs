using System.Collections;
using System.Collections.Generic;
using UnityEngine;



/* The Character Component should handle all functionality for the character
   It is designed as a data driven class that takes a CharacterPreset ScriptableObject for initialization and functionality
 */
public class Character : MonoBehaviour
{
    //Preset used for this character
    [SerializeField]
    CharacterPreset characterPreset;

    public bool InitializeCharacter(CharacterPreset preset) 
    {
        characterPreset = preset;
        if(preset == null) 
        {
            Debug.LogError("Invalid Character Preset Supplied");
            return false;
        }

        //TODO: Initialization

        return true;
    }
}
