using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class BiomeDetecValue
{
    public string name;
    public Color colorValue;
}

public class BiomeDetector : MonoBehaviour
{
    [SerializeField]
    Texture2D im;

    [SerializeField]
    BiomeDetecValue[] biomes;

    [SerializeField]
    string actBiome;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        // Bit shift the index of the layer (8) to get a bit mask
        int layerMask = 1 << 3;

        RaycastHit hit;
        // Does the ray intersect any objects excluding the player layer
        if (Physics.Raycast(transform.position + new Vector3(0, 1, 0), transform.TransformDirection(Vector3.down), out hit, Mathf.Infinity, layerMask))
        {
            Debug.DrawRay(transform.position + new Vector3(0, 1, 0), transform.TransformDirection(Vector3.down) * hit.distance, Color.green);
            Color h = im.GetPixel((int)(hit.textureCoord.x*im.width), (int)(hit.textureCoord.y*im.height));
            
            for(int i=0; i<biomes.Length; i++)
            {
                if (((int)(h.r * 1000) == (int)(biomes[i].colorValue.r * 1000)) && ((int)(h.b * 1000) == (int)(biomes[i].colorValue.b * 1000)) && ((int)(h.g * 1000) == (int)(biomes[i].colorValue.g * 1000)))
                {
                    actBiome = biomes[i].name;
                    Debug.Log(actBiome);
                }
            }
        }
    }
}
