namespace Gravitons.UI.Modal
{
  using UnityEngine.UI;
  using UnityEngine;

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
      ModalManager.Show(title, body, new[] { new ModalButton() { Text = "OK" } });
    }

    /// <summary>
    /// Shows a modal with callback
    /// </summary>
    public void ShowModalCloseGame()
    {
      ModalManager.Show("Quit?", "Do you want to quit the game?",
          new[] { new ModalButton() { Text = "YES", Callback = QuitGame }, new ModalButton() { Text = "NO" } });
    }

    private void QuitGame()
    {
      Debug.Log("Quitting game");
      Application.Quit();
    }

    // private void OnDestroy()
    // {
    //   button.onClick.RemoveListener(ShowModalWithCallback);
    // }
  }
}