using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityStandardAssets.CrossPlatformInput;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent (typeof(AudioSource))]

public class Gun : MonoBehaviour {
    public int gunDamage = 10; //Ateş edildiğinde candan azaltılacak
    public float fireRate = 0.25f; //Kaç saniyede bir ateş edilebilecek
    public float fireTime = 0.0f; //Ateş etme zamani
    public float weapoRange = 50f; //ne kadar uzaktaki düşmanı vurabilicez 50f 50 metre
    public float hitForce = 100f; //objeyi vurduğumuz yöne hareket ettiricez
    public float targetValue = 40f; //yakınlaşma miktarı nişan
    public float smoothTime; 
    public Transform firePoint; //ateş edeceğimiz yer
    public GameObject muzzleEffect ,bulletEjectionEffect; //merminin ateş etme efekti ve mermi kovanlarının çıkma efekti
    public GameObject bloodEffect, metalEffect ,impactEffect; //vurduktan sonra mermi cıkması 

    private Camera fpsCam; //fps kamerası
    private WaitForSeconds shotDuration = new WaitForSeconds(0.07f); //ateş etmek için 0.07 saniye beklenecek
    private AudioSource gunAudio; // silahsesi
    private float nextFire; //ne zaman bir sonraki atışı yapıcağım
    private Animator animator;
    public float headDamage1 = 100f, legDamage1 = 50f , ArmDamage1 = 30f , bodyDamage1=15f;
    public float headDamageBoss1 = 65f, legDamageBoss1 = 35f, ArmDamageBoss1 = 20f , bodyDamageBoss1 = 10f;
    public float headDamage2 = 40f, legDamage2 = 25f, ArmDamage2 = 20f , bodyDamage2 = 10f;
    public float headDamageBoss2 = 25f, legDamageBoss2 = 15f, ArmDamageBoss2 = 10f, bodyDamageBoss2 = 7f;
    public float headDamage3 = 20f, legDamage3 = 10f, ArmDamage3 = 15f , bodyDamage3 = 5f;
    public float headDamageBoss3 = 10f, legDamageBoss3 = 2f, ArmDamageBoss3 = 5f, bodyDamageBoss3 = 5f;
    public CharacterController character;

    public Image target_icon;

    public TMPro.TextMeshProUGUI ammo_text;
    public int ammoPer = 30, ammoTotal = 240; //kurşun değerleri


    //Nişan alma
    private IEnumerator coChangeFov; //Kameranın yakınlaşması
    private float startingFieldOfView; //Baştaki kamera bakış durumu
    
    void Start () {
        fpsCam = Camera.main; //fps kameramı ayarladım
        gunAudio = GetComponent<AudioSource>(); //sesi al
        animator = GetComponentInParent<Animator>();
        startingFieldOfView = fpsCam.fieldOfView; //nişan için başlangıç kamera bakış açısı
        showAmmo();

    }

    void Update () {

        if (Input.GetKeyDown(KeyCode.R)) //R'ye basınca mermi doldurma
        {
            if (ammoTotal > 0)
            {
                changeAmmo();
            }
        }

        if (Input.GetMouseButton(0)) //Mouse sol click basılırsa
        {
            if(Time.time >= fireTime) //zaman ateş zamanına büyük eşit olursa
            {
                if(ammoPer > 0) //şarjördeki mermi 0'dan büyük olursa
                {
                    Shooting(); //ateş et
                    
                    fireTime = Time.time + fireRate; //ateş zamanını artır
                }
                else
                {
                    stopShooting(); //kurşun yoksa ateş etme
                }
            }
        }
        //if (Input.GetMouseButtonUp(0))
        //{
        //    stopShooting();
        //}
        AimDown(); //Aim Alma

        animPlay();
    }

    void stopShooting() //ateş etmeyi durdurma
    {
        if(ammoPer == 0)
        {
            ammoPer = 0;
            CancelInvoke("Shooting");
        }
    }

    void showAmmo() //kurşun sayısını ekranda gösterme
    {
        ammo_text.text = ammoPer + "/" + ammoTotal;
    }

    void changeAmmo()
    {
        
        ammoPer = 30;
        ammoTotal -= 30;
        showAmmo();
        
    }

    public void Shooting()
    {
        ammoPer--;
        showAmmo();
      

        Vector3 rayOrigin = fpsCam.ViewportToWorldPoint(new Vector3(0.5f,0.5f,0.5f));//ateşlemeyi tam orta noktadan yapabilmek için viewport kullanılır ve vector3 her parmetresine 0.5f verilir
        Debug.DrawLine(rayOrigin, fpsCam.transform.forward * weapoRange, Color.green); // ateş ettiğimde ateş ettiğim yeri vurmak için görünmez çizgi çıkarır
        //drawline(benim vurduğum yer, vuracağım şeyin yeri, görünmez yeşil çizgi)

        if (Input.GetButton("Fire1") && Time.time > nextFire) //Sol click fire1 ve oyunun zamanı bir sonraki atışın zamanından büyükse yani ateş edildiğinde
        {
            nextFire = Time.time + fireRate; //bir sonraki atış zamanını ayarladım 
            muzzleEffect.GetComponent<ParticleSystem>().Play(); //merminin ateş etme efekti
            bulletEjectionEffect.GetComponent<ParticleSystem>().Play(); //mermi kovanlarının çıkma efekti
            StartCoroutine(shootEffect());
            
            RaycastHit hit; //bir şeye vurdum mu bilgi almamı sağlayacak

            if(Physics.Raycast(rayOrigin , fpsCam.transform.forward * weapoRange, out hit)) // raycast(vurduğum yer , vuracağım nesnenin yeri , vurdum mu )  
            {
                Debug.Log(hit.collider.name);// console da vurduğum seyin adını gösterir

                metalEffect.transform.position = hit.point; //metaleffect vurduğumuz nesneyi vurduğumuz yerde olacak
                metalEffect.GetComponent<ParticleSystem>().Play(); //particle efektleri al play() ise oyun oynanırken
               // metalEffect.transform.parent = hit.collider.gameObject.transform; //mermi vurduğum nesnenin child'ı olur
                metalEffect.transform.rotation = Quaternion.LookRotation(hit.normal); //quaternion vektörlere benziyor ama farklı olarak x,y,z,w olan 4 boyutlu ve lookrotation kameraya bakmasını sağlar
                metalEffect.transform.localScale = new Vector3(1, 1, 1);

                if (hit.collider.gameObject.GetComponent<HealtManager>() != null) //Healtmanager objesi var mı diye kontrol etti
                {
                    hit.collider.gameObject.GetComponent<HealtManager>().currentHealth -= gunDamage; //varsa gunDamage kadar canı azalttı
                }


                if(hit.rigidbody != null) //vurduğum nesnenin rigidbody'si var ise
                {
                    hit.rigidbody.AddForce(-hit.normal * hitForce); //vurduğum yerin tam tersinde vurduğum kadar geri git yani vuruş hissi
                }

                Instantiate(impactEffect, hit.point, Quaternion.LookRotation(hit.normal)); //yere vurunca mermi izi

                //Level 1 Normal Canavar Can Sistemi
                if (hit.collider.tag == "hitHead")
                {
                    Instantiate(bloodEffect, hit.point, Quaternion.LookRotation(hit.normal));
                    hit.collider.gameObject.transform.root.GetComponent<Takip>().health -= headDamage1;
                }

                if (hit.collider.tag == "hitBody")
                {
                    Instantiate(bloodEffect, hit.point, Quaternion.LookRotation(hit.normal));
                    hit.collider.gameObject.transform.root.GetComponent<Takip>().health -= bodyDamage1;
                }

                if (hit.collider.tag == "hitLeg")
                {
                    Instantiate(bloodEffect, hit.point, Quaternion.LookRotation(hit.normal));
                    hit.collider.gameObject.transform.root.GetComponent<Takip>().health -= legDamage1;
                }

                if (hit.collider.tag == "hitArm")
                {
                    Instantiate(bloodEffect, hit.point, Quaternion.LookRotation(hit.normal));
                    hit.collider.gameObject.transform.root.GetComponent<Takip>().health -= ArmDamage1;
                }

                //Level 1 Boss Canavar Can Sistemi
                if (hit.collider.tag == "hitHeadBoss")
                {
                    Instantiate(bloodEffect, hit.point, Quaternion.LookRotation(hit.normal));
                    hit.collider.gameObject.transform.root.GetComponent<Takip>().health -= headDamageBoss1;
                }

                if (hit.collider.tag == "hitBodyBoss")
                {
                    Instantiate(bloodEffect, hit.point, Quaternion.LookRotation(hit.normal));
                    hit.collider.gameObject.transform.root.GetComponent<Takip>().health -= bodyDamageBoss1;
                }

                if (hit.collider.tag == "hitLegBoss")
                {
                    Instantiate(bloodEffect, hit.point, Quaternion.LookRotation(hit.normal));
                    hit.collider.gameObject.transform.root.GetComponent<Takip>().health -= legDamageBoss1;
                }

                if (hit.collider.tag == "hitArmBoss")
                {
                    Instantiate(bloodEffect, hit.point, Quaternion.LookRotation(hit.normal));
                    hit.collider.gameObject.transform.root.GetComponent<Takip>().health -= ArmDamageBoss1;
                }

                // Level 2 Normal Canavar Can Sistemi 
                if (hit.collider.tag == "hitHead2")
                {
                    Instantiate(bloodEffect, hit.point, Quaternion.LookRotation(hit.normal));
                    hit.collider.gameObject.transform.root.GetComponent<Takip>().health -= headDamage2;
                }

                if (hit.collider.tag == "hitBody2")
                {
                    Instantiate(bloodEffect, hit.point, Quaternion.LookRotation(hit.normal));
                    hit.collider.gameObject.transform.root.GetComponent<Takip>().health -= bodyDamage2;
                }

                if (hit.collider.tag == "hitLeg2")
                {
                    Instantiate(bloodEffect, hit.point, Quaternion.LookRotation(hit.normal));
                    hit.collider.gameObject.transform.root.GetComponent<Takip>().health -= legDamage2;
                }

                if (hit.collider.tag == "hitArm2")
                {
                    Instantiate(bloodEffect, hit.point, Quaternion.LookRotation(hit.normal));
                    hit.collider.gameObject.transform.root.GetComponent<Takip>().health -= ArmDamage2;
                }

                //Level 2 Boss Canavar Can Sistemi
                if (hit.collider.tag == "hitHeadBoss2")
                {
                    Instantiate(bloodEffect, hit.point, Quaternion.LookRotation(hit.normal));
                    hit.collider.gameObject.transform.root.GetComponent<Takip>().health -= headDamageBoss2;
                }

                if (hit.collider.tag == "hitBodyBoss2")
                {
                    Instantiate(bloodEffect, hit.point, Quaternion.LookRotation(hit.normal));
                    hit.collider.gameObject.transform.root.GetComponent<Takip>().health -= bodyDamageBoss2;
                }

                if (hit.collider.tag == "hitLegBoss2")
                {
                    Instantiate(bloodEffect, hit.point, Quaternion.LookRotation(hit.normal));
                    hit.collider.gameObject.transform.root.GetComponent<Takip>().health -= legDamageBoss2;
                }

                if (hit.collider.tag == "hitArmBoss2")
                {
                    Instantiate(bloodEffect, hit.point, Quaternion.LookRotation(hit.normal));
                    hit.collider.gameObject.transform.root.GetComponent<Takip>().health -= ArmDamageBoss2;
                }

                // Level 3 Normal Canavar Can Sistemi 
                if (hit.collider.tag == "hitHead3")
                {
                    Instantiate(bloodEffect, hit.point, Quaternion.LookRotation(hit.normal));
                    hit.collider.gameObject.transform.root.GetComponent<Takip>().health -= headDamage3;
                }

                if (hit.collider.tag == "hitBody3")
                {
                    Instantiate(bloodEffect, hit.point, Quaternion.LookRotation(hit.normal));
                    hit.collider.gameObject.transform.root.GetComponent<Takip>().health -= bodyDamage3;
                }

                if (hit.collider.tag == "hitLeg3")
                {
                    Instantiate(bloodEffect, hit.point, Quaternion.LookRotation(hit.normal));
                    hit.collider.gameObject.transform.root.GetComponent<Takip>().health -= legDamage3;
                }

                if (hit.collider.tag == "hitArm3")
                {
                    Instantiate(bloodEffect, hit.point, Quaternion.LookRotation(hit.normal));
                    hit.collider.gameObject.transform.root.GetComponent<Takip>().health -= ArmDamage3;
                }

                //Level 3 Boss Canavar Can Sistemi
                if (hit.collider.tag == "hitHeadBoss3")
                {
                    Instantiate(bloodEffect, hit.point, Quaternion.LookRotation(hit.normal));
                    hit.collider.gameObject.transform.root.GetComponent<Takip>().health -= headDamageBoss3;
                }

                if (hit.collider.tag == "hitBodyBoss3")
                {
                    Instantiate(bloodEffect, hit.point, Quaternion.LookRotation(hit.normal));
                    hit.collider.gameObject.transform.root.GetComponent<Takip>().health -= bodyDamageBoss3;
                }

                if (hit.collider.tag == "hitLegBoss3")
                {
                    Instantiate(bloodEffect, hit.point, Quaternion.LookRotation(hit.normal));
                    hit.collider.gameObject.transform.root.GetComponent<Takip>().health -= legDamageBoss3;
                }

                if (hit.collider.tag == "hitArmBoss3")
                {
                    Instantiate(bloodEffect, hit.point, Quaternion.LookRotation(hit.normal));
                    hit.collider.gameObject.transform.root.GetComponent<Takip>().health -= ArmDamageBoss3;
                }

            }
        }
    }
    
    void AimDown()
    {
        if (Input.GetButton("Fire2"))
        {
            ZoomIn();
            animator.SetBool("isAiming", true); //Nişan alma animasyonunu gerçekleştirmek için
            target_icon.enabled = false; //Sahne ortasındaki imleci kapat
            //transform.localPosition = Vector3.Lerp(transform.localPosition, new Vector3(-0, 356f, 0.046f, 0), Time.deltaTime * smoothTime);
        }
        else
        {
            animator.SetBool("isAiming", false); //Nişan alma animasyonunu bırak
            target_icon.enabled = true; //Sahne ortasındaki imleci aç
            ZoomOut();
        }
    }
    void ZoomIn()
    {
        ChangeFov(fpsCam.fieldOfView, targetValue); //yakınlaştır
    }

    void ZoomOut()
    {
        ChangeFov(fpsCam.fieldOfView, startingFieldOfView); //uzaklaştır
    }

    void ChangeFov (float duration ,float value) //alttaki coroutine'i çalştırmak için
    {
        
        if(coChangeFov != null)
        {
            StopCoroutine(coChangeFov);
        }
        coChangeFov = CoChangeFov(duration, value);
        StartCoroutine(coChangeFov);
    }

    private IEnumerator CoChangeFov(float duration,float value)
    {
        float t = 0f; //zaman
        float startFov = fpsCam.fieldOfView; //başlangıç bakış durumu

        while (t != duration) // t istenilen zamana eşit değilse 
        {
            t += Time.deltaTime * smoothTime; //t'ye yeni değer atadım 
            if (t >= duration) t = duration; //t'nin verilen zamandan büyük olmasını istemiyorum 
            {
                fpsCam.fieldOfView = Mathf.Lerp(startFov, value, t / duration); //nişan alırken bir anda yakınlaşmak yerine yakınlaştırmayı gösterir
            }
            yield return null; //null olarak geri döndürecek

        }
    }
    private IEnumerator shootEffect() //sesi oynatabilmek için courotine oluşturdum
    {
        gunAudio.Play();
        gunAudio.pitch = Random.Range(0.8f, 1.1f); //Random ses verebilmek için
        animator.Play("Firing", -1, 0f); //ateş etme animasyonu
        yield return shotDuration;
    }

    IEnumerator reloadAnim()//Mermi değiştirne animasyonu gerçekleştirmek için courotine
    {
        animator.SetBool("isReload", true);

        yield return new WaitForSeconds(0.3f);

        animator.SetBool("isReload", false);
    }

    void animPlay()
    {
        animator.SetFloat("speed", character.velocity.magnitude); //yürüme ve koşma animasyonlarını ayarlamak için hız

        if (Input.GetKeyDown(KeyCode.R)) //R'ye basınca mermi değiştir
        {
            StartCoroutine(reloadAnim());
        }
    }

}