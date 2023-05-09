using FMODUnity;
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
    [SerializeField]
    string nextBiome;

    private FMOD.Studio.EventInstance musicInstance;
    public EventReference music;
    // Start is called before the first frame update
    void Start()
    {
        musicInstance = RuntimeManager.CreateInstance(music);
        musicInstance.start();
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
            Color h = im.GetPixel((int)(hit.textureCoord.x * im.width), (int)(hit.textureCoord.y * im.height));

            actBiome = CheckColor(h).name;
            musicInstance.setParameterByNameWithLabel("ActBiome", actBiome);
        }

        //Raycast que rodean para detectar biomas cercanos
        int rot = 360 / 12;
        Vector3 dir = new Vector3(1, 0, 0) * radiusForBlend;
        List<Tuple<BiomeDetecValue, Vector3>> biomesArround = new List<Tuple<BiomeDetecValue, Vector3>>();
        for (int i = 0; i < 12; i++)
        {
            dir = (Quaternion.AngleAxis(rot, Vector3.up) * dir);
            if (Physics.Raycast(transform.position + new Vector3(0, 1, 0) + dir, transform.TransformDirection(Vector3.down), out hit, Mathf.Infinity, layerMask))
            {
                Debug.DrawRay(transform.position + new Vector3(0, 1, 0) + dir, transform.TransformDirection(Vector3.down) * hit.distance, Color.red);
                Color h = im.GetPixel((int)(hit.textureCoord.x * im.width), (int)(hit.textureCoord.y * im.height));

                BiomeDetecValue b = CheckColor(h);
                if (b.name != actBiome)
                    biomesArround.Add(new Tuple<BiomeDetecValue, Vector3>(b, dir + transform.position + new Vector3(0, 1, 0)));
            }
        }

        //Calculo de las distancias al bioma cercano
        if (biomesArround.Count > 0)
        {
            float[] dists = GetDists(biomesArround);
            int menor = 0;
            for (int i = 1; i < dists.Length; i++)
            {
                if (dists[menor] > dists[i])
                    menor = i;
            }

            nextBiome = biomesArround[menor].Item1.name;
        }
        else
            nextBiome = "";
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

    float[] GetDists(List<Tuple<BiomeDetecValue, Vector3>> bA)
    {
        float[] dists = new float[bA.Count];

        Vector3 posP = transform.position + new Vector3(0, 1, 0);

        for (int i = 0; i < bA.Count; i++)
        {
            Vector3 pM = (bA[i].Item2 + posP) / 2;
            Vector3 p1 = posP;
            Vector3 p2 = bA[i].Item2;
            for (int j = 0; j < 10; j++)
            {
                int layerMask = 1 << 3;
                RaycastHit hit;
                if (Physics.Raycast(pM, transform.TransformDirection(Vector3.down), out hit, Mathf.Infinity, layerMask))
                {
                    Color h = im.GetPixel((int)(hit.textureCoord.x * im.width), (int)(hit.textureCoord.y * im.height));
                    BiomeDetecValue pmC = CheckColor(h);
                    if (pmC.name != bA[i].Item1.name)
                    {
                        p1 = pM;
                    }
                    else
                    {
                        p2 = pM;
                    }
                    pM = (p1 + p2) / 2.0f;
                }
            }
            Debug.DrawRay(pM, transform.TransformDirection(Vector3.down) * 10, Color.black);
            dists[i] = (transform.position + new Vector3(0, 1, 0) - pM).magnitude;
        }

        return dists;
    }
}
