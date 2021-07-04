using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Gamer : MonoBehaviour
{
    public string nextScene,normalScene;
    float currentHealth = 100f; //karakterimin canı

    public TMPro.TextMeshProUGUI health_text;
    public TMPro.TextMeshProUGUI game_result_text;
    public TMPro.TextMeshProUGUI time_text;
    public TMPro.TextMeshProUGUI ammo_text;

    public Image target_icon;
    public Takip zombie;
    public Gun gun;
    public CharacterController characterController;

    public bool isAlive;
    public float reducingHealth = 0.1f;

    public GameObject[] Weapons;

    private void Start()
    {
        game_result_text.enabled = false;

        isAlive = true;
    }

    public void reduceHealth()
    {
        currentHealth -= reducingHealth;

        int labelHealth = (int) currentHealth;

        health_text.text = "%" + labelHealth.ToString();

        if(currentHealth <= 0) //karakterimin canı 0'dan küçük olursa
        {
            StartCoroutine(RestartTimeHold());
        }
        else
        {
            if (zombie.health == 0)
             {
                 StartCoroutine(NextSceneTimeHold());
             }
        }
    }

    IEnumerator NextSceneTimeHold()
    {
        zombie.health = 0;
        isAlive = true;
        game_result_text.enabled = true;
        health_text.enabled = false;
        time_text.enabled = false;
        game_result_text.text = "YOU KILLED THE BOSS \n NEXT LEVEL IS LOADING.";
        yield return new WaitForSeconds(2);
        game_result_text.text = "YOU KILLED THE BOSS \n NEXT LEVEL IS LOADING..";
        yield return new WaitForSeconds(2);
        game_result_text.text = "YOU KILLED THE BOSS \n NEXT LEVEL IS LOADING...";
        yield return new WaitForSeconds(2);
        game_result_text.text = "YOU KILLED THE BOSS \n NEXT LEVEL IS LOADING.";
        yield return new WaitForSeconds(2);
        game_result_text.text = "YOU KILLED THE BOSS \n NEXT LEVEL IS LOADING..";
        yield return new WaitForSeconds(2);
        game_result_text.text = "YOU KILLED THE BOSS \n NEXT LEVEL IS LOADING...";
        SceneManager.LoadScene(nextScene);
    }

    IEnumerator RestartTimeHold()
    {
        currentHealth = 0f;
        isAlive = false;
        health_text.text = "%" + currentHealth.ToString();
        game_result_text.enabled = true;
        time_text.enabled = false;
        ammo_text.enabled = false;
        target_icon.enabled = false;
        gun.enabled = false;
        characterController.enabled = false;
        game_result_text.text = "YOU ARE DEAD \n THE LEVEL IS RESTARTING";
        yield return new WaitForSeconds(5);
        SceneManager.LoadScene(normalScene);
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            foreach (GameObject obj in Weapons)
            {
                obj.SetActive(false);
            }
            Weapons[0].SetActive(true);
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            foreach (GameObject obj in Weapons)
            {
                obj.SetActive(false);
            }
            Weapons[1].SetActive(true);
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            foreach (GameObject obj in Weapons)
            {
                obj.SetActive(false);
            }
            Weapons[2].SetActive(true);
        }
    }
}
