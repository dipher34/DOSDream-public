using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PerlinNoiseBuilder : MonoBehaviour
{
    [Header("Seed Settings")]
    public int width = 50, depth = 50; 
    public float scale = 10f, heightMultiplier = 10f, buildDelay = 0.005f;
    public GameObject itemPrefab;
    public Gradient colorGradient;

    private List<GameObject> currentItems = new();
    private float seedX, seedZ;

    void Start()
    {
        RandomizeSeed();
        StartCoroutine(GenerateNoise());
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.G))
        {
            StopAllCoroutines();
            ClearItems();
            RandomizeSeed();
            StartCoroutine(GenerateNoise());
        }
    }

    void RandomizeSeed()
    {
        seedX = Random.Range(0f, 1000f);
        seedZ = Random.Range(0f, 1000f);
    }

    IEnumerator GenerateNoise()
    {
        for (int x = 0; x < width; x++)
        {
            for (int z = 0; z < depth; z++)
            {
                float p = Mathf.PerlinNoise((x + seedX) / scale, (z + seedZ) / scale);
                int height = Mathf.RoundToInt(p * heightMultiplier);
                Color c = colorGradient.Evaluate(p);
                // Color c = new Color(1f, 1f, 1f, 1f); // Optimization

                for (int y = 0; y < height; y++)
                {
                    GameObject item = Instantiate(itemPrefab, new Vector3(x, y, z), Quaternion.identity, transform);
                    currentItems.Add(item);
                    item.transform.localScale = Vector3.zero;
                    StartCoroutine(ScaleIn(item));

                    var renderer = item.GetComponent<Renderer>();
                    if (renderer)
                    {
                        renderer.material = new Material(renderer.material);
                        renderer.material.color = c;
                    }
                }

                yield return new WaitForSeconds(buildDelay);
            }
        }
        Debug.Log($"SeedX: {seedX}, SeedZ: {seedZ}");
    }

    IEnumerator ScaleIn(GameObject obj)
    {
        Vector3 targetScale = Vector3.one;
        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime * 4f;
            obj.transform.localScale = Vector3.Lerp(Vector3.zero, targetScale, t);
            yield return null;
        }
    }

    void ClearItems()
    {
        foreach (GameObject item in currentItems)
        {
            if (item != null)
                Destroy(item);
        }
        currentItems.Clear();
    }
}
