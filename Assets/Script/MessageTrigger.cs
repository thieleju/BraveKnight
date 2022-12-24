using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Gravitons.UI.Modal;

public class MessageTrigger : MonoBehaviour
{

  public string title;
  public string body;

  bool triggered = false;

  void OnTriggerEnter2D(Collider2D other)
  {
    if (other.CompareTag("Player") && !triggered)
    {
      // show message 
      GameObject ui_manager = GameObject.Find("Demo");
      ui_manager.GetComponent<DemoManager>().ShowInfo(title, body);

      triggered = true;
    }
  }
}
