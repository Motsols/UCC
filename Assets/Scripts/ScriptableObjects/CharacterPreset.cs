using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
class CharacterMeshSetup 
{
    [SerializeField]
    Mesh Mesh = null;

    [SerializeField]
    Vector3 Scale = new Vector3(1, 1, 1);

    [SerializeField]
    Vector3 PositionOffset = Vector3.zero;
} 


[CreateAssetMenu(fileName = "CharacterPreset", menuName = "ScriptableObjects/Character Preset", order = 1)]
public class CharacterPreset : ScriptableObject
{
    [Header("Character")]
    [SerializeField]
    string CharacterName = "The Dude";

    [Header("Movement")]
    [SerializeField]
    float MovementSpeed = 4f;

    [Header("Visuals")]
    [SerializeField]
    CharacterMeshSetup BodyMesh;
    
    [SerializeField]
    CharacterMeshSetup GlassesMesh;

    [SerializeField]
    CharacterMeshSetup HatMesh;

    [SerializeField]
    Transform SnowballAnchorPositionOffset;

    [Header("Snowball")]
    [SerializeField]
    GameObject DefaultSnowball;

    private void OnValidate()
    {
        if (DefaultSnowball.GetComponent<SnowBall>() == null)
        {
            DefaultSnowball = null;
        }
    }

}
