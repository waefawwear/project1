using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public GameObject monster;
    public int maxSpawn = 2;
    public float spawnRange = 5.0f;

    private float spawnCoolTime = 3.0f;

    [Header("Debug Info")]
    public bool showSpawnRange = true;

    private void Update()
    {
        spawnCoolTime -= Time.deltaTime;

        if (transform.childCount < maxSpawn)
        {
            if(spawnCoolTime < 0.0f)
            {
                GameObject obj = Instantiate(monster, this.transform);
                Vector2 randPos = Random.insideUnitCircle * spawnRange;
                obj.transform.localPosition = randPos;
                obj.transform.localRotation = Quaternion.identity;
                spawnCoolTime = 3.0f;
            }
        }
    }
}
