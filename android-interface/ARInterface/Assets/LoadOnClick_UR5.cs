using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadOnClick_UR5 : MonoBehaviour
{
    private Button thisButton;

    public GameObject loadingImage;

    void Start()
    {
        thisButton = this.GetComponent<Button>();
        thisButton.onClick.AddListener(LoadScene);
    }

    private void LoadScene()
    {
        //loadingImage.SetActive(true);
        //SceneManager.LoadSceneAsync("Scenes/ARInterface_UR5", LoadSceneMode.Single);
        SceneManager.LoadSceneAsync(1, LoadSceneMode.Single);
    }
}