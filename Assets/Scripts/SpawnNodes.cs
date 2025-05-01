using System.Buffers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ONLY USED FOR SETTING UP THE GAME BOARD- NOT NEEDED ANYMORE (but didn't want to delete)
public class SpawnNodes : MonoBehaviour
{
    int numToSpawn = 28;
    public float spawnOffset = 0.3f;
    public float currentSpawnOffset;

    // Start is called before the first frame update
    void Start()
    {
            gameObject.name = "Node";
            return;

    }

    // Update is called once per frame
    void Update()
    {

    }
}
