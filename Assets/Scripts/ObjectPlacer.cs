using UnityEngine;
using System.IO;
using System.Collections;
public class ObjectPlacer : MonoBehaviour
{
    [System.Serializable]
    public class PrefabCodePair
    {
        public GameObject prefab;
        public string code;
    }

    [SerializeField]
    private Scroller scroller;
    [SerializeField]
    private GameObject loading;
    [SerializeField]
    private GameObject finisher;

    public PrefabCodePair[] prefabCodePairs;
    public string filePath;
    public AudioClip[] songs;
    public Transform parentObject;

    void Start()
    {
        StartCoroutine(PlaceObjectsCoroutine());
        loading.SetActive(true);
    }

    private IEnumerator PlaceObjectsCoroutine()
    {
        AudioSource mapa = GameObject.Find("Map").GetComponent<AudioSource>();
        mapa.clip = songs[SongSelect.id];
        bool isBeatmapSection = false;
        float lastX= 0;
        filePath = "Assets/Beatmaps/map.txt";

        string[] lines = File.ReadAllLines(filePath);

        for (int i = 0; i < lines.Length; i++)
        {
            string line = lines[i];

            if (line.Trim() == "[Beatmap]")
            {
                isBeatmapSection = true;
                continue;
            }

            if (isBeatmapSection)
            {
                string[] parts = line.Split(';');

                if (parts.Length == 3)
                {
                    string code = parts[0];

                    if (float.TryParse(parts[1].Trim(), out float x) && float.TryParse(parts[2].Trim(), out float y))
                    {
                        foreach (var pair in prefabCodePairs)
                        {
                            if (pair.code == code)
                            {
                                if (i == lines.Length - 1)
                                {
                                    GenerateLastKey(x / 100, y, pair.prefab);
                                    lastX = x / 100;
                                }
                                else
                                {
                                    GenerateKey(x / 100, y, pair.prefab);
                                }
                            }
                        }
                    }
                    else
                    {
                        Debug.LogWarning("Failed to parse coordinates: " + parts[1].Trim() + ", " + parts[2].Trim());
                    }
                }

                if (parts.Length == 4)
                {
                    string code = parts[0];

                    if (float.TryParse(parts[1], out float x1) && float.TryParse(parts[2], out float x2) && float.TryParse(parts[3], out float y))
                    {
                        foreach (var pair in prefabCodePairs)
                        {
                            if (pair.code == code)
                            {
                                if (i == lines.Length - 1)
                                {
                                    GenerateLastSlider(x1 / 100, x2 / 100, y, pair.prefab);
                                    lastX = x2 / 100;
                                }
                                else
                                {
                                    GenerateSlider(x1 / 100, x2 / 100, y, pair.prefab);
                                }
                            }
                        }
                    }
                    else
                    {
                        Debug.LogWarning("Failed to parse coordinates: " + parts[1].Trim() + ", " + parts[2].Trim() + ", " + parts[3].Trim());
                    }
                }
            }

            yield return null;
        }
        mapa.Play();

        loading.SetActive(false);
        scroller.startMap(lastX);
    }

    private void GenerateLastKey(float x, float y, GameObject prefab)
    {
        Vector3 position = new(x, y, 0);
        GameObject newObject = Instantiate(prefab, position, Quaternion.identity);
        newObject.transform.SetParent(parentObject);

        Vector3 position2 = new(x+3, y, 0);
        GameObject newObject2 = Instantiate(finisher, position2, Quaternion.identity);
        newObject2.transform.SetParent(parentObject);
    }

    private void GenerateLastSlider(float x1, float x2, float y, GameObject prefab)
    {
        float diff = x2 - x1;
        float x = diff / 2;
        x += x1;
        Vector3 position = new(x, y, 0);
        GameObject newObject = Instantiate(prefab, position, Quaternion.identity);
        newObject.transform.localScale = new Vector3(diff, newObject.transform.localScale.y, newObject.transform.localScale.z);
        newObject.transform.SetParent(parentObject);

        Vector3 position2 = new(x2 + 3, y, 0);
        GameObject newObject2 = Instantiate(finisher, position2, Quaternion.identity);
        newObject2.transform.SetParent(parentObject);
    }
    private void GenerateKey(float x, float y, GameObject prefab)
    {
        if (prefab != null)
        {
            Vector3 position = new(x, y, 0);
            GameObject newObject = Instantiate(prefab, position, Quaternion.identity);
            newObject.transform.SetParent(parentObject);
        }
    }

    private void GenerateSlider(float x1, float x2, float y, GameObject prefab)
    {
        if (prefab != null)
        {
            float diff = x2 - x1;
            float x = diff / 2;
            x += x1;
            Vector3 position = new(x, y, 0);
            GameObject newObject = Instantiate(prefab, position, Quaternion.identity);
            newObject.transform.localScale = new Vector3(diff, newObject.transform.localScale.y, newObject.transform.localScale.z);
            newObject.transform.SetParent(parentObject);
        }
    }
}

