using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Gravitons.UI.Modal;

//when something get into the alta, make the runes glow
public class PropsAltar : MonoBehaviour
{
  public List<SpriteRenderer> runes;
  public float lerpSpeed;
  public bool isEnd;

  private Color curColor;
  private Color targetColor;

  private void OnTriggerEnter2D(Collider2D other)
  {
    targetColor = new Color(1, 1, 1, 1);

    if (!isEnd) return;


    // wait 2 seconds
    StartCoroutine(waiter());
  }

  IEnumerator waiter()
  {
    // play sound
    FindObjectOfType<AudioManager>().Play("Success");

    yield return new WaitForSeconds(1);

    // show message 
    GameObject ui_manager = GameObject.Find("Demo");
    ui_manager.GetComponent<DemoManager>().ShowInfo("Congratulations!", "You slayed the King of Despair! You win!");

    // load first room
    // move to spawn point
    GameObject game_handler = GameObject.Find("Main Camera");
    game_handler.GetComponent<GameHandler>().LoadRoom(0);

    // reset player health
    GameObject player = GameObject.FindWithTag("Player");
    player.GetComponent<HeroKnight>().HealHealth(100);
  }

  private void OnTriggerExit2D(Collider2D other)
  {
    targetColor = new Color(1, 1, 1, 0);
  }

  private void Update()
  {
    curColor = Color.Lerp(curColor, targetColor, lerpSpeed * Time.deltaTime);

    foreach (var r in runes)
    {
      r.color = curColor;
    }
  }
}

