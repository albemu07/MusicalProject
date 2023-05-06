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
                int r1 = (int)(h.r * 100);
                int r2 = (int)(biomes[i].colorValue.r * 100);
                int g1 = (int)(h.g * 100);
                int g2 = (int)(biomes[i].colorValue.g * 100);
                int b1 = (int)(h.b * 100);
                int b2 = (int)(biomes[i].colorValue.b * 100);
                if ((r1 == r2 || r1 + 1 == r2 || r1-1 == r2) && (g1 == g2 || g1 + 1 == g2 || g1 - 1 == g2) && (b1 == b2 || b1 + 1 == b2 || b1 - 1 == b2))
                    actBiome = biomes[i].name;
                Debug.Log(actBiome);                   
            }
        }
    }
}
