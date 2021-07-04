using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FinalScene : MonoBehaviour
{
    void Start()
    {
        Cursor.visible = true; 
    }


    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Backspace))
        {
            final2Menu();

        }
    }

    public void final2Menu()
    {
        SceneManager.LoadScene("Main Menu");
    }
}
