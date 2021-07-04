using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public enum AIState //AI'ın durumlarını numaralandr variable olarak kullan
{
    Guarding,
    Aert,
    Attacking
}

public class EnemyAI : MonoBehaviour
{
    public AIState enemyState;

    public float speed; //animasyon geçiş hızı 
    public bool  isDieing; //AI öldü mü ?
    public float currentHealth; //AI mevcut canı
    public float scanRange; //taranacak aralık
    public float turnSpeed; //AI Dönme hızı

    bool isHit, isCrouching; //AI vuruldu mu ve eğildi mi ?
    float maxHealth = 100; //AI max can

    NavMeshAgent navAgent; //AI kendine nesneleri gözönüne alarak  yol çizmesi
    Animator animator; //animasyonları ayarlamak
    Transform target; //AI nereye gidecek 
    Transform Player; //AI nerede takip etmek
    CapsuleCollider bodyCol; //AI collider vererek nesnelere çarpmasını ve vurulmasını sağlamak

    void Start()
    {
        navAgent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        target = null;
        Player = GameObject.FindGameObjectWithTag("Player").transform;
        currentHealth = 100f;
        bodyCol = GetComponent<CapsuleCollider>();
    }

    void Update()
    {
        if(currentHealth >= 1)
        {
            switch (enemyState) //AI durumunu seçecek ve fonk'a göre işlem yapacak
            {
                case AIState.Guarding:
                    Guarding();
                    break;
                case AIState.Aert:
                    Alert();
                    break;
                case AIState.Attacking:
                    Attacking();
                    break;
            }
            navAgent.speed = speed;

            UpdateAnimations();

            
        }
        else
        {
            currentHealth = 0f;
            isDieing = true;
            animator.SetBool("isDieing",isDieing);
        }

    }

    void Guarding() // durma yani bölgeyi koruma durumu 
    {
        speed = 0;//hareket veya anim geçişi yok
        target = null; //burada ateş edecek hedef yok
    }

    void Alert()
    {
        isCrouching = true;
        FindCover();
    }

    void Attacking()
    {
        target = Player;

        //animator.SetTrigger("isFiring");

        navAgent.SetDestination(Player.transform.position);
        navAgent.stoppingDistance = 5f; //cover yoksa durma ve ateş etme mesafesi
        speed = 5f;
        isCrouching = false;

        if (navAgent.pathPending) //AI kendine yol daha önce buldu şuan bulmasına gerek yoksa
        {
            if (navAgent.remainingDistance <= navAgent.stoppingDistance) //AI'ın önünde nesne var mı
            {
                if (!navAgent.hasPath || navAgent.velocity.sqrMagnitude == 0f && currentHealth > 0) //AI'ın canı varsa ve yola sahipse
                {
                    animator.SetTrigger("isFiring"); //AI ateş etsin
                }
            }
        }

        TurnPlayer();
    }

    void FindCover()
    {
        GameObject[] Covers = GameObject.FindGameObjectsWithTag("Cover"); //cover taglı objeleri bulur ve gameobject dizisine ekler
        float shortestDistance = Mathf.Infinity; //AI'a en kısa mesafe 
        GameObject nearstCover = null; //AI'a en yakın cover

        foreach (GameObject cover in Covers) 
        {
            float distanceToCover = Vector3.Distance(transform.position, cover.transform.position); //AI covera olan uzaklığı

            if(distanceToCover < shortestDistance) //AI covera uzaklığı en kısa mesafeden az mı
            {
                shortestDistance = distanceToCover; //yeni en yakın mesafe
                nearstCover = cover; //AI'a en yakın cover
                speed = 5; //AI hareket etmesi için hız artmalı
            }
        }

        if(nearstCover != null && shortestDistance <= scanRange) //nearstcover varsa ve arama aralığndaysa 
        {
            target = nearstCover.transform; //AI'ın gideceği yer yani cover
            navAgent.SetDestination(target.transform.position); //target setlendi

            //AI'ın kendine yol bulduktan oraya gidip ateş etmesi
            if (!navAgent.pathPending) //AI kendine yol daha önce buldu şuan bulmasına gerek yoksa
            {
                if(navAgent.remainingDistance <= navAgent.stoppingDistance) //AI'ın önünde nesne var mı
                {
                    if(!navAgent.hasPath || navAgent.velocity.sqrMagnitude == 0f && !isDieing) //AI'ın canı varsa ve yola sahipse
                    {
                        enemyState = AIState.Attacking; //AI saldırı durumuna geçsin
                    }
                }
            }
        }
        else //cover yoksa
        {
            enemyState = AIState.Attacking; //direk saldırı durumuna geçsin
        }
    }

    void TurnPlayer()
    {
        Vector3 dir = target.transform.position - transform.position;//target ile mesafe
        Quaternion lookRotation = Quaternion.LookRotation(dir); //Target yönüne dönmek
        Vector3 rotation = Quaternion.Lerp(transform.rotation, lookRotation, Time.deltaTime * turnSpeed).eulerAngles; //eulerAngles sayesinde rotation değeri yavaş döner
        transform.rotation = Quaternion.Euler(rotation.x, rotation.y, rotation.z);
        animator.SetFloat("isTurning", turnSpeed);
        turnSpeed = 5f;
    }

    void UpdateAnimations()
    {
        animator.SetFloat("Speed", speed);

        animator.SetBool("isCrouching" ,isCrouching);

        //if (isDieing)
        //{
        //    animator.SetTrigger("isDieing");
        //    animator.SetFloat("health", currentHealth);
        //}

        if (isHit)
        {
            animator.SetTrigger("isHit");
            isHit = false;
        }
    }

    void OnTriggerEnter(Collider other) //AI'ın bizim karakterimizi algılaması
    {
        if(other.tag == "Player")
        {
            enemyState = AIState.Aert;
        }
    }

    void OnTriggerExit(Collider other) 
    {
        if (other.tag == "Player")
        {
            enemyState = AIState.Guarding;
        }
    }

}
