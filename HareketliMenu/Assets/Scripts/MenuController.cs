using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuController : MonoBehaviour
{
   [Header("Paneller")]
   [SerializeField] private GameObject settingspanel;
   [SerializeField] private GameObject quitpanel;
   
   public void GameStart()
   {
      SceneManager.LoadScene("GameScene");
   }

   public void OpenSettings()
   {
      settingspanel.SetActive(true);
   }

   public void QuitSettings()
   {
      settingspanel.SetActive(false);
   }
   public void OpenQuit()
   {
      quitpanel.SetActive(true);
   }
}
