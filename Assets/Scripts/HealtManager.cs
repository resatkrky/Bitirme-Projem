using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealtManager : MonoBehaviour
{
    public int currentHealth;
    public GameObject fructuredObject;

    void Update()
    {
        if(currentHealth <= 0)
        {
            currentHealth = 0; //hicbir zaman 0'ın altına düşmeyecek
            if (GetComponent<MeshRenderer>() != null) //nesne görünüyorsa
            {
                GetComponent<MeshRenderer>().enabled = false; //görünürlük kapalı 
                fructuredObject.SetActive(true); //parçalanan obje parçalansın
            }

            //else
            //{
            //    gameObject.SetActive(false); //nesne kapalı
            //}
        }
    }

    
}
