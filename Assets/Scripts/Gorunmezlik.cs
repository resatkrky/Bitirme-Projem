using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gorunmezlik : MonoBehaviour
{
    public Material[] materials;
    int Materialindex , audioIndex; //dizi için index

    public float CloakTime = 3f; //karakteri görünmez olmas süresi
    float cloakCoolDown; //bekleme süresi
    public bool cloakEngeged; //bool döndürerk görünmez olup olmadığını kontrol
    WaitForSeconds cloakTime = new WaitForSeconds(4f);
    AudioSource audioSource;
    public AudioClip[] audioClips;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        cloakCoolDown = Time.deltaTime;

        if (Input.GetButtonDown("Cloak")) //e tuşuna basınca karakter görünmez olacak
        {
            if (!cloakEngeged) //!clockEngeged karakter görünmnez değilse
            {
                if (CloakTime > cloakCoolDown) //karakterin enerjisi dolduysa
                {
                    StartCoroutine(ClockCo()); //görünmez ol
                }
            } 
           
        }

        if (Input.GetButtonDown("Fire1") && cloakEngeged)
        {
            if (audioSource != null)
            {
                audioSource.PlayOneShot(audioClips[GetNextAudioClip()]);
            }

            StopAllCoroutines(); //ateş edildiğinde görünür olmak için Coroutine durmak zorunda
            GetComponent<Renderer>().material = materials[GetNextMaterial()]; //karakter ateş ettiğinde görünür olacak
            cloakEngeged = false;
            cloakCoolDown = 0; //tekrar görünmez olmak için 0'a eşitlenmezse sürekli yukarı çıkar ve karakter görünmez olmaz
        }
    }

    int GetNextMaterial()
    {
        Materialindex++;

        if(Materialindex == materials.Length) ///toplam materyal saysına eşitlenirse index sıfırlanır
        {
            Materialindex = 0;
        }

        return Materialindex;
    }

    int GetNextAudioClip()
    {
        audioIndex++;

        if (audioIndex == audioClips.Length) ///toplam materyal saysına eşitlenirse index sıfırlanır
        {
            audioIndex = 0;
        }

        return audioIndex;
    }

    IEnumerator ClockCo()
    {

        if(audioSource != null)
        {
            audioSource.PlayOneShot(audioClips[GetNextAudioClip()]);
        }

        cloakEngeged = true;

        GetComponent<Renderer>().material = materials[GetNextMaterial()]; //material'ı aldım

        yield return cloakTime; //ilk iki satırı koştuktan sonra bekleme süresi 2 saniye bundan sonra diğer olay

        if (audioSource != null)
        {
            audioSource.PlayOneShot(audioClips[GetNextAudioClip()]);
        }

        cloakEngeged = false;

        GetComponent<Renderer>().material = materials[GetNextMaterial()]; //material'imi al bir sonraki material ile değişir

        cloakCoolDown = 0;
    }
}
