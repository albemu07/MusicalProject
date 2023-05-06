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
    float radiusForBlend;

    [SerializeField]
    string actBiome;
    string nextBiome;
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

            actBiome = CheckColor(h).name;
        }

        int rot = 360 / 12;
        Vector3 dir = new Vector3(1, 0, 0) * radiusForBlend;
        List<BiomeDetecValue> biomesArround = new List<BiomeDetecValue>();
        for (int i = 0; i < 12; i++)
        {
            dir = (Quaternion.AngleAxis(rot, Vector3.up) * dir);
            Debug.Log(dir);
            if (Physics.Raycast(transform.position + new Vector3(0, 1, 0) + dir, transform.TransformDirection(Vector3.down), out hit, Mathf.Infinity, layerMask))
            {
                Debug.DrawRay(transform.position + new Vector3(0, 1, 0) + dir, transform.TransformDirection(Vector3.down) * hit.distance, Color.red);
                Color h = im.GetPixel((int)(hit.textureCoord.x * im.width), (int)(hit.textureCoord.y * im.height));

                BiomeDetecValue b = CheckColor(h);
                if (b.name != actBiome)
                    biomesArround.Add(b);
            }
        }
    }

    BiomeDetecValue CheckColor(Color h)
    {
        BiomeDetecValue biome = null;
        for (int i = 0; i < biomes.Length; i++)
        {
            int r1 = (int)(h.r * 100);
            int r2 = (int)(biomes[i].colorValue.r * 100);
            int g1 = (int)(h.g * 100);
            int g2 = (int)(biomes[i].colorValue.g * 100);
            int b1 = (int)(h.b * 100);
            int b2 = (int)(biomes[i].colorValue.b * 100);
            if ((r1 == r2) && (g1 == g2) && (b1 == b2))
                biome = biomes[i];
        }

        return biome;
    }
}
