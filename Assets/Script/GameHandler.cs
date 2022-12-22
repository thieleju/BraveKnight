using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class GameHandler : MonoBehaviour
{

  private KeyCode[] keyCodes = {
         KeyCode.Alpha1,
         KeyCode.Alpha2,
         KeyCode.Alpha3,
         KeyCode.Alpha4,
         KeyCode.Alpha5,
         KeyCode.Alpha6,
         KeyCode.Alpha7,
         KeyCode.Alpha8,
         KeyCode.Alpha9,
     };

  void Start()
  {
    // Load first room
    LoadRoom(0);
  }

  // Load room on key press
  void Update()
  {
    for (int i = 0; i < keyCodes.Length; i++)
      if (Input.GetKeyDown(keyCodes[i]))
        LoadRoom(i);
  }

  public void LoadRoom(int roomNumber)
  {
    GameObject hero = GameObject.Find("HeroKnight");
    GameObject current_room = GameObject.Find("Room " + roomNumber);

    if (current_room == null) return;
    Debug.Log("Loading room " + roomNumber);

    // move hero to spawn point
    GameObject spawn_point = current_room.transform.Find("Spawnpoint").gameObject;
    hero.transform.position = spawn_point.transform.position;

    // Find mobs in Mobs folder 
    GameObject mobs = current_room.transform.Find("Mobs").gameObject;
    if (mobs != null) return;

    // Enable all mobs
    GameObject[] enemies = mobs.GetComponentsInChildren<GameObject>();
    Debug.Log("Found " + enemies.Length + " enemies");
    foreach (GameObject enemy in enemies)
    {
      enemy.GetComponent<Mob>().enabled = true;
    }
  }

}
