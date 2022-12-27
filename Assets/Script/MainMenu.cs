using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{

  public void PlayGame()
  {
    PlaySoundClick();
    SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
  }

  public void QuitGame()
  {
    PlaySoundClick();
    Application.Quit();
  }

  public void PlaySoundHover()
  {
    // play sound
    FindObjectOfType<AudioManager>().Play("MenuHover");
  }

  public void PlaySoundClick()
  {
    // play sound
    FindObjectOfType<AudioManager>().Play("MenuClick");
  }
}
