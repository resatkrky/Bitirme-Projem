using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class Takip : MonoBehaviour
{
    public NavMeshAgent navMeshAgent;//AI'ın bana hareket etmesi

    public Transform target; //benim karakterim

    public float distance;

    Animator zombieAnim;

    public float health ;

    private float iconHealth;

    public bool attack;

    public Image health_icon;

    public Gorunmezlik cloak;

    void Start()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();

        zombieAnim = GetComponent<Animator>();

        attack = false;

        iconHealth = health;

        health_icon.fillAmount = health/iconHealth;
    }

    void Update()
    {
        zombieAnim.SetFloat("speed", navMeshAgent.velocity.magnitude );

        zombieAnim.SetFloat("health", health);

        distance = Vector3.Distance(transform.position, target.position); //target hedef arası mesafe

        health_icon.fillAmount = health / iconHealth;

        if(health >= 1)
        {
            if (distance > 2f && distance < 25f)
            {
                navMeshAgent.enabled = true; //takip sistemi açık
                navMeshAgent.destination = target.position; //Zombie karakterimize kosacak
                attack = false;
                CancelInvoke("Give_Damage"); //fonk çalışmasın

                if (cloak.cloakEngeged == true) //karakter uzakta görünmez olduğunda zombie göremiyor
                {
                    navMeshAgent.enabled = false;
                    health = 100;
                }
                
            }
            else if (distance <= 2f)
            {
                attack = true;
                zombieAnim.SetBool("attack", attack);

                if (attack == true)
                {
                    //Give_Damage();
                    InvokeRepeating("Give_Damage", 0.0f, 2.0f); //fonk 2 sn arayla çalışsın
                    attack = true;

                    if (cloak.cloakEngeged == true) //karakter yakında görünmez olduğunda zombie görüyor
                    {
                      //  cloak.cloakEngeged = false;
                        navMeshAgent.enabled = true;
                    }
                }
            }
            else
            {
                attack = false;
                navMeshAgent.enabled = false; //karakteri takip etme kapalı
            }
           
            //if (cloak.cloakEngeged == true) //karakter görünmez olduğunda zombie göremiyor
            //{
            //    navMeshAgent.enabled = false;
            //}

        }
        else 
        {
            health = 0;
            navMeshAgent.enabled = false; //AI'ın canı yoksa takip etmeyi bırakacak
        }
            
    }

    void Give_Damage()
    {
        GameObject.FindWithTag("Player").GetComponent<Gamer>().reduceHealth(); //Gamer scriptindeki fonku al
    }
}
