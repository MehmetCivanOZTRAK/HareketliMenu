using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuController : MonoBehaviour
{
    [Header("Ana Menü")]
    [SerializeField] private GameObject mainMenu;

    [Header("Panel dışında gizlenecek sahne objeleri")]
    [SerializeField] private GameObject sceneMenuRoot;

    [Header("Paneller")]
    [SerializeField] private GameObject settingsPanel;
    [SerializeField] private GameObject quitPanel;
      
    [Header("Quit Warning")]  
    [SerializeField] private QuitWarning quitWarning;
    
    private void Start()
    {
        mainMenu.SetActive(true);
        sceneMenuRoot.SetActive(true);
        settingsPanel.SetActive(false);
        quitPanel.SetActive(false);
    }

    public void GameStart()
    {
        SceneManager.LoadScene("GameScene");
    }

    public void OpenSettings()
    {
        mainMenu.SetActive(false);
        sceneMenuRoot.SetActive(false);
        settingsPanel.SetActive(true);
        quitPanel.SetActive(false);
    }

    public void CloseSettings()
    {
        settingsPanel.SetActive(false);
        mainMenu.SetActive(true);
        sceneMenuRoot.SetActive(true);
    }

    public void OpenQuit()
    {
        mainMenu.SetActive(false);
        sceneMenuRoot.SetActive(false);
        quitPanel.SetActive(true);
        settingsPanel.SetActive(false);
    }

    public void CloseQuit()
    {
        quitPanel.SetActive(false);
        mainMenu.SetActive(true);
        sceneMenuRoot.SetActive(true);
        
        if(quitWarning!=null) quitWarning.HideQuitWarning();
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}