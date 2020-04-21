using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SetDropdownFromButton : MonoBehaviour
{
    private Button thisButton;
    //private string cmd;

    public Dropdown cmdMenu;
    public Button comButton;

    void Start()
    {
        thisButton = this.GetComponent<Button>();
        thisButton.onClick.AddListener(SetCmd);

        //cmdMenu = cmdMenu.GetComponent<Dropdown>();
        //comButton = comButton.GetComponent<Button>();
    }

    private void SetCmd()
    {
        string cmd = thisButton.name;
        Debug.LogFormat("Button clicked: {0}", cmd);

        // Interpret button
        switch(cmd)
        {
            case "Cmd1": cmdMenu.value = 0; break; // Get
            case "Cmd2": cmdMenu.value = 1; break; // Set
            case "Cmd3": cmdMenu.value = 3; break; // 0g
            case "Cmd4": cmdMenu.value = 5; break; // Grip
            default:        break;
        }

        // Click the communicate button
        comButton.onClick.Invoke();
    }
}