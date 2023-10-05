using System.Collections;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

// Sets the script to be executed later than all default scripts
// This is helpful for UI, since other things may need to be initialized before setting the UI
[DefaultExecutionOrder(1000)]
public class MenuUIHandler : MonoBehaviour
{
    [SerializeField] TMP_InputField _textFieldUserName;
    private string _userName;

    public void StartGame()
    {
        if (_textFieldUserName.text.Length == 0)
        {
            Debug.Log("Please enter your name");
        }
        else
        {
            _userName = _textFieldUserName.text;
            MainManager.Instance.SetUserName(_userName);
            MainManager.Instance.WaitOneSec();
            SceneManager.LoadScene(1);

        }
    }



    public void Exit()
    {
#if UNITY_EDITOR
        EditorApplication.ExitPlaymode();
#else
        Application.Quit(); // original code to quit Unity player
#endif    
    }    
}
