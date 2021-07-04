﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public GameObject pauseMenu;
    public GameObject settingsScreen;
    public Gun gun;
    public bool isPaused = false;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            isPaused = !isPaused; //ilk tıklayınca true ikinci tıklama false yapsın yani tam tersi
        }

        if (isPaused)
        {
            pauseMenu.SetActive(true);
            Time.timeScale = 0; //durunca karakter hareket edemez
            Cursor.visible = true;
            gun.enabled = false; //Pause menüsünde Gun scripti kapalı
        }
        else
        {
            pauseMenu.SetActive(false);
            Time.timeScale = 1;
            Cursor.visible = false;
            gun.enabled = true;
        }
    }

    public void Resume()
    {
        isPaused = false;
        
    }

    public void Restart()
    {
        SceneManager.LoadScene("Level 1");
    }

    public void Settings()
    {
        pauseMenu.SetActive(false);
        settingsScreen.SetActive(true);
    }

    public void settings2Pause()
    {
        pauseMenu.SetActive(true);
        settingsScreen.SetActive(false);
    }

    public void Exit()
    {
        Application.Quit();
    }

}
