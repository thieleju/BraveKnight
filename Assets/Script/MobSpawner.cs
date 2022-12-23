using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MobSpawner : MonoBehaviour
{
  public GameObject[] spawns;

  public GameObject skeleton_prefab;
  public GameObject goblin_prefab;

  public void SpawnMobs()
  {
    foreach (GameObject spawn in spawns)
    {
      // spawn mobs based on the name of the spawn point
      switch (spawn.name)
      {
        case "Skeleton":
          Instantiate(skeleton_prefab, spawn.transform.position, Quaternion.identity);
          Debug.Log("Spawned skeleton at " + spawn.transform.position);
          break;
        case "Goblin":
          Instantiate(goblin_prefab, spawn.transform.position, Quaternion.identity);
          Debug.Log("Spawned goblin at " + spawn.transform.position);
          break;
        default:
          Debug.Log("Unknown spawn point: " + spawn.name);
          break;
      }
    }
  }
}
