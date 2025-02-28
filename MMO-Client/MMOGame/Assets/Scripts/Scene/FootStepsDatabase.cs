using System.Collections.Generic;
using UnityEngine;


//Variables for footstep system list
[System.Serializable]
public class GroundLayer
{
    public string layerName;
    public Texture2D[] groundTextures;
    public AudioClip[] footstepSounds;
}

// 每个场景中有一个，用于存储所有的脚步声音
public class FootStepsDatabase : MonoBehaviour
{
    public static FootStepsDatabase singleton;

    [Tooltip("Add textures for this layer and add sounds to be played for this texture")]
    public List<GroundLayer> groundLayers = new List<GroundLayer>();

    private Terrain _terrain;
    private TerrainData _terrainData;
    private TerrainLayer[] _terrainLayers;

    private RaycastHit _groundHit;
    private Texture2D _currentTexture;


    private void Awake()
    {
        if (!singleton) singleton = this;
        else if (singleton != this) Destroy(gameObject);
    }
    void Start()
    {
        GetTerrainData();
    }
    private void GetTerrainData()
    {
        if (Terrain.activeTerrain)
        {
            _terrain = Terrain.activeTerrain;
            _terrainData = _terrain.terrainData;
            _terrainLayers = _terrain.terrainData.terrainLayers;
        }
    }

    public AudioClip GetFootstep(RaycastHit raycastHit)
    {
        _groundHit = raycastHit;
        if (_groundHit.collider.GetComponent<Terrain>())
        {
            _currentTexture = _terrainLayers[GetTerrainTexture(raycastHit.point)].diffuseTexture;
        }
        if (_groundHit.collider.GetComponent<Renderer>())
        {
            _currentTexture = GetRendererTexture();
        }


        AudioClip targetClip = null;
        for (int i = 0; i < groundLayers.Count; i++)
        {
            for (int k = 0; k < groundLayers[i].groundTextures.Length; k++)
            {
                if (_currentTexture == groundLayers[i].groundTextures[k])
                {
                    targetClip = RandomClip(groundLayers[i].footstepSounds);
                    break;
                }
            }
        }

        return targetClip;
    }
    private AudioClip RandomClip(AudioClip[] clips)
    {
        // Getting the footstep sounds based on surface index.
        int n = Random.Range(1, clips.Length);

        // Move picked sound to index 0 so it's not picked next time.
        AudioClip temp = clips[n];
        clips[n] = clips[0];
        clips[0] = temp;

        return temp;
    }

    //Returns the zero index of the prevailing texture based on the controller location on terrain
    private int GetTerrainTexture(Vector3 controllerPosition)
    {
        float[] array = GetTerrainTexturesArray(controllerPosition);
        float maxArray = 0;
        int maxArrayIndex = 0;

        for (int n = 0; n < array.Length; ++n)
        {

            if (array[n] > maxArray)
            {
                maxArrayIndex = n;
                maxArray = array[n];
            }
        }
        return maxArrayIndex;
    }

    //Return an array of textures depending on location of the controller on terrain
    private float[] GetTerrainTexturesArray(Vector3 controllerPosition)
    {
        _terrain = Terrain.activeTerrain;
        _terrainData = _terrain.terrainData;
        Vector3 terrainPosition = _terrain.transform.position;

        int positionX = (int)(((controllerPosition.x - terrainPosition.x) / _terrainData.size.x) * _terrainData.alphamapWidth);
        int positionZ = (int)(((controllerPosition.z - terrainPosition.z) / _terrainData.size.z) * _terrainData.alphamapHeight);

        float[,,] layerData = _terrainData.GetAlphamaps(positionX, positionZ, 1, 1);

        float[] texturesArray = new float[layerData.GetUpperBound(2) + 1];
        for (int n = 0; n < texturesArray.Length; ++n)
        {
            texturesArray[n] = layerData[0, 0, n];
        }
        return texturesArray;
    }

    //Returns the current main texture of renderer where the controller is located now
    private Texture2D GetRendererTexture()
    {
        Texture2D texture;
        texture = (Texture2D)_groundHit.collider.gameObject.GetComponent<Renderer>().material.mainTexture;
        return texture;
    }

}
