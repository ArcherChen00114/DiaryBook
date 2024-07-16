using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : Singleton<UIManager>
{
    [SerializeField]
    GameObject PausePanel;
    [SerializeField]
    GameObject DisableTouchPad;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void TogglePausePanel() {
        if (PausePanel.gameObject.activeSelf)
        {
            PausePanel.gameObject.SetActive(false); 
            EnablePad();
        }
        else
        {
            PausePanel.gameObject.SetActive(true);
            DisablePad();
        }
    }

    public void ExitGame()
    {
        Debug.Log("Exit");
        Application.Quit();
    }
    public void DisablePad() {
        DisableTouchPad.SetActive(false);
    }
    public void EnablePad()
    {
        DisableTouchPad.SetActive(true);
    }
}
