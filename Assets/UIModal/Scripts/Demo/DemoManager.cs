namespace Gravitons.UI.Modal
{
  using UnityEngine.UI;
  using UnityEngine;
  using UnityEngine.SceneManagement;

  /// <summary>
  /// Manages the UI in the demo scene
  /// </summary>
  public class DemoManager : MonoBehaviour
  {
    // public Button button;
    public Image image;

    // private void Start()
    // {
    //   button.onClick.AddListener(ShowModalWithCallback);
    // }

    /// <summary>
    /// Show a simple modal
    /// </summary>
    private void ShowModal()
    {
      ModalManager.Show("Modal Title", "Show your message here", new[] { new ModalButton() { Text = "OK" } });
    }

    public void ShowInfo(string title, string body)
    {
      // play sound
      FindObjectOfType<AudioManager>().Play("MenuHover");
      ModalManager.Show(title, body, new[] { new ModalButton() { Text = "OK" } });
    }

    /// <summary>
    /// Shows a modal with callback
    /// </summary>
    public void ShowModalCloseGame()
    {
      // play sound
      FindObjectOfType<AudioManager>().Play("MenuHover");
      ModalManager.Show("Quit?", "Do you want to go back to main menu?",
          new[] { new ModalButton() { Text = "YES", Callback = QuitGame }, new ModalButton() { Text = "NO" } });
    }

    private void QuitGame()
    {
      SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - 1);
    }

    // private void OnDestroy()
    // {
    //   button.onClick.RemoveListener(ShowModalWithCallback);
    // }
  }
}