using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Game : MonoBehaviour
{
    public Material material;

    public GameObject objPrefab;

    public List<Objeto> listaObjetos = new List<Objeto>();

    public List<Material> materiais = new List<Material>();

    public List<Transform> spawnPoints = new List<Transform>();

    // Start is called before the first frame update
    void Start()
    {
        foreach (GameObject spawn in GameObject.FindGameObjectsWithTag("SpawnPoint"))
        {
            spawnPoints.Add(spawn.transform);
        }

        for (int i = 0; i < 6; i++)
        {
            int pos = Random.Range(0, spawnPoints.Count);

            if (pos >= spawnPoints.Count || pos < 0)
                break;

            GameObject go = Instantiate(objPrefab, spawnPoints[pos].position, Quaternion.identity);

            listaObjetos.Add(go.GetComponent<Objeto>());

            spawnPoints.RemoveAt(pos);
        }
    }
    // Update is called once per frame
    void Update()
    {

    }
}