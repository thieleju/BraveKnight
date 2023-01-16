using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class GameHandler : MonoBehaviour
{

  public GameObject player_prefab;

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
    // Instantiate player
    Instantiate(player_prefab, new Vector2(0, 0), Quaternion.identity);

    // set camera target
    GameObject camera = GameObject.Find("Main Camera");
    Transform player = GameObject.FindWithTag("Player").transform;
    camera.GetComponent<CameraFollow>().SetTarget(player);

    // Load first room
    LoadRoom(0);
  }

  // Load room on key press
  void Update()
  {
    // check for escape key
    if (Input.GetKeyDown(KeyCode.Escape))
    {
      // show close game modal
      GameObject ui_manager = GameObject.Find("Modal");
      ui_manager.GetComponent<MessageManager>().ShowModalCloseGame();
    }

    for (int i = 0; i < keyCodes.Length; i++)
      if (Input.GetKeyDown(keyCodes[i]))
        LoadRoom(i);
  }

  public void LoadRoom(int roomNumber)
  {
    // find player by tag and room by name
    GameObject player = GameObject.FindWithTag("Player");
    GameObject current_room = GameObject.Find("Room " + roomNumber);

    if (current_room == null) return;

    // move player to spawn point
    GameObject spawn_point = current_room.transform.Find("Spawnpoint").gameObject;
    player.transform.position = spawn_point.transform.position;

    Debug.Log("Loaded room " + roomNumber);

    // reset all chests
    GameObject[] chests = GameObject.FindGameObjectsWithTag("Chest");
    foreach (GameObject chest in chests)
      chest.GetComponent<Chest>().ResetChest();

    // Use mobspawner to spawn mobs
    GameObject spawner = current_room.GetComponent<MobSpawner>().gameObject;
    if (spawner == null) return;
    spawner.GetComponent<MobSpawner>().SpawnMobs();
  }

}
