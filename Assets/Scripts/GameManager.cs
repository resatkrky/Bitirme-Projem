using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public string nextScene , normalScene;

    public TMPro.TextMeshProUGUI time_txt;

    public TMPro.TextMeshProUGUI game_result_text;

    public TMPro.TextMeshProUGUI ammo_text;

    public TMPro.TextMeshProUGUI health_text;

    public Image target_icon;

    public int time = 300;

    public Takip zombie;

    public GameObject[] zombies;

    public Gun gun;

    public CharacterController characterController;

    public bool isAlive;

    //GameObject[] bornPoint;

    void Start()
    {
        game_result_text.enabled =false;

        //bornPoint = GameObject.FindGameObjectsWithTag("dogma_noktasi");

        InvokeRepeating("reduceTime", 0.0f, 1.0f);

        // InvokeRepeating("produceZombie", 0.0f, 5.0f);

        foreach (GameObject zombie in zombies) //oyunun başında zombieleri sahneye ekler
        {
            zombie.SetActive(true);
            
        }
        isAlive = true;
    }
    
    public void reduceTime()
    {
        time--;
        time_txt.text = time.ToString();

        if(time >= 1)
        {
                if (zombie.health == 0)
                {
                    StartCoroutine(NextSceneTimeHold());
                }
        }
        else
        {
            StartCoroutine(RestartTimeHold());
        }
        
    }
    IEnumerator NextSceneTimeHold()
    {
        game_result_text.enabled = true;
        health_text.enabled = false;
        time_txt.enabled = false;
        isAlive = true;
        game_result_text.text = "YOU KILLED THE BOSS \n NEXT LEVEL IS LOADING.";
        yield return new WaitForSeconds(3);
        SceneManager.LoadScene(nextScene);
    }

    IEnumerator RestartTimeHold()
    {
        time = 0;
        isAlive = false;
        time_txt.text = time.ToString();
        game_result_text.enabled = true;
        time_txt.enabled = false;
        ammo_text.enabled = false;
        target_icon.enabled = false;
        gun.enabled = false;
        characterController.enabled = false;
        game_result_text.text = "YOU ARE DEAD \n THE LEVEL IS RESTARTING.";
        yield return new WaitForSeconds(3);
        SceneManager.LoadScene(normalScene);
        
    }

    //void produceZombie()
    //{
    //    int ran = Random.Range(0, bornPoint.Length);

    //    GameObject reZombie = Instantiate(zombie, bornPoint[ran].transform.position, Quaternion.identity);

    //    reZombie.GetComponent<Takip>().health = 100f;
    //}
}
