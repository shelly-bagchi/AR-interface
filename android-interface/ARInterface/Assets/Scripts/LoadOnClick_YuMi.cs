using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadOnClick_YuMi : MonoBehaviour
{
    private Button thisButton;

    //public GameObject loadingImage;

    void Start()
    {
        thisButton = this.GetComponent<Button>();
        thisButton.onClick.AddListener(LoadScene);
    }

    private void LoadScene()
    {
        //loadingImage.SetActive(true);
        //SceneManager.LoadSceneAsync("Scenes/ARInterface_YuMi", LoadSceneMode.Single);
        SceneManager.LoadSceneAsync(3, LoadSceneMode.Single);
    }
}