using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIMain : MonoBehaviour
{
    public GameObject settingsScreen;
    public GameObject creditsScreen;
    public GameObject mainScreen;

    public AudioSource ses;
    public AudioClip sesclip;

    public void StartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void Settings()
    {
        mainScreen.SetActive(false);
        settingsScreen.SetActive(true);
    }

    public void set2Menu()
    {
        mainScreen.SetActive(true);
        settingsScreen.SetActive(false);
    }

    public void Credits()
    {
        mainScreen.SetActive(false);
        creditsScreen.SetActive(true);
    }

    public void cre2Menu()
    {
        mainScreen.SetActive(true);
        creditsScreen.SetActive(false);
    }

    public void Exit()
    {
        Application.Quit();
    }

    private void Awake()
    {
        foreach (var item in Resources.FindObjectsOfTypeAll<Button>())
        {
            item.onClick.AddListener(() => buttonAudio());
        }
    }

    public void buttonAudio()
    {
        ses.PlayOneShot(sesclip, 1);
    }
}
