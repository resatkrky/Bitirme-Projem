using System;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;
using UnityStandardAssets.Utility;
using Random = UnityEngine.Random;

namespace UnityStandardAssets.Characters.FirstPerson
{
    public class FirstPersonController : MonoBehaviour
    {
        public bool isWalking; //karakter yürüyor mu
        public float walkSpeed; //yürüme hýzý
        public float runSpeed;  //koþma hýzý
        [Range(0f, 1f)] public float runStepLenghten; //bir adýmýn uzunluk deðeri
        public float jumpSpeed; //zýplama hýzý
        public float stickToGroundForce; //Yere direk düþmesini saðlayan deðer
        public float gravityMultiplier; //Yer çekimi çarpaný
        public MouseLook m_MouseLook; //mouse hareketi
        public bool useFovKick; //görüþ alaný vuruþunu kullanýyor mu
        public FOVKick m_FovKick = new FOVKick();
        public bool useHeadBob; //karakter hareket ederken etrafýna bakýyor mu 
        public CurveControlledBob m_HeadBob = new CurveControlledBob();
        public LerpControlledBob jumpBob = new LerpControlledBob();
        public float stepInterval; //adým aralýðý
        public AudioClip[] footStepSound; // ayak sesi
        public AudioClip jumpSound;// karakter zýplayýnca çýkan ses
        public AudioClip landSound; // yere düþünce çýkan ses
        public float normalHeight;
        public float crouchHeight;

        private Camera camera; //fps kamerasý
        private bool jump; //karakter zýpladý mý 
        private float yRotation; //y ekseninde dönme deðeri
        private Vector2 input; //hareket girdisi
        private Vector3 moveDir = Vector3.zero;
        private CharacterController characterController;
        private CollisionFlags collisionFlags; //karakterin nesnelere çarpmasý
        private bool previouslyGrounded; //yerde miydi
        private Vector3 originalCameraPosition;
        private float stepCycle; //döngü deðeri
        private float nextStep; //
        private bool jumping; //zýpladý mý 
        private AudioSource audioSource;
        private Animator animator;


        private void Start()
        {
            //gerekli tanýmlamalarý yaptým
            characterController = GetComponent<CharacterController>();
            camera = Camera.main;
            originalCameraPosition = camera.transform.localPosition;
            m_FovKick.Setup(camera); //karakterin bakýþ açýsý
            m_HeadBob.Setup(camera, stepInterval);
            stepCycle = 0f;
            nextStep = stepCycle/2f;
            jumping = false;
            audioSource = GetComponent<AudioSource>();
			m_MouseLook.Init(transform , camera.transform);
            animator = GetComponent<Animator>();
        }
        
        private void Update() //karakterin zýplamasýný ve eðilmesini saðlýyor
        {
            RotateView(); //karakter zýplarken etraýna bakar
            //zýplama durumunun kaçýrýlmadýðýndan emin olmak için baraya bakar
            if (!jump) //karakter zýpalamadýysa
            {
                jump = CrossPlatformInputManager.GetButtonDown("Jump"); //zýpla
            }

            if (!previouslyGrounded && characterController.isGrounded) //karakter yere düþtüðü an
            {
                StartCoroutine(jumpBob.DoBobCycle());
                PlayLandingSound();
                moveDir.y = 0f;
                jumping = false;
            }
            if (!characterController.isGrounded && !jumping && previouslyGrounded) //karakter yerdeyse
            {
                moveDir.y = 0f;  //hareket ekseninde y'si 0
            }

            previouslyGrounded = characterController.isGrounded;

            //Eðilme
            if (Input.GetKeyDown(KeyCode.LeftControl))
            {
                characterController.height = crouchHeight;
            }

            if (Input.GetKeyUp(KeyCode.LeftControl))
            {
                characterController.height = normalHeight;
            }
        }
        private void PlayLandingSound() //yürüme ayak sesi
        {
            audioSource.clip = landSound;
            audioSource.Play();
            nextStep = stepCycle + .5f;
        }


        private void FixedUpdate() //karakterin hareket ederken zýplamasýný saðlýyor
        {
            float speed;
            GetInput(out speed); //karakterin hareket etmesi için girdi almasýný saðlar
            // karakterin baktýðý noktaya ilerler
            Vector3 desiredMove = transform.forward*input.y + transform.right*input.x;

            // karakter zemine bastýðýný anlamasý gerekiyor
            RaycastHit hitInfo;
            Physics.SphereCast(transform.position, characterController.radius, Vector3.down, out hitInfo,
                               characterController.height/2f, Physics.AllLayers, QueryTriggerInteraction.Ignore); //karakterimin içindeki SphereCast karakterimle ayný fizikse deðerleri almalý
            desiredMove = Vector3.ProjectOnPlane(desiredMove, hitInfo.normal).normalized;

            moveDir.x = desiredMove.x*speed; //hýzla birlikte x yönünde hareket et
            moveDir.z = desiredMove.z*speed; //hýzla birlikte y yönünde hareket et


            if (characterController.isGrounded) //karakter yere düþerken
            {
                moveDir.y = -stickToGroundForce; //yer çekimine karþý koyar

                if (jump) //karakter zýplýyorsa
                {
                    moveDir.y = jumpSpeed;
                    PlayJumpSound();
                    jump = false;
                    jumping = true;
                }
            }
            else
            {
                moveDir += Physics.gravity*gravityMultiplier*Time.fixedDeltaTime; //karakterin hareket etmesini saðlar ve deltatime sayesinde normal zamanda gider olmasaydý uçardý
            }
            collisionFlags = characterController.Move(moveDir*Time.fixedDeltaTime);

            ProgressStepCycle(speed); //karakter adým atýyor hýzý ile
            UpdateCameraPosition(speed); //karakter adým atarken görüntüyü günceller

            m_MouseLook.UpdateCursorLock();
        }


        private void PlayJumpSound() //zýplama sesi
        {
            audioSource.clip = jumpSound;
            audioSource.Play();
        }


        private void ProgressStepCycle(float speed) //karakterin adýmlarýný bir döngü içerisinde atmasý
        {
            if (characterController.velocity.sqrMagnitude > 0 && (input.x != 0 || input.y != 0))
            {
                stepCycle += (characterController.velocity.magnitude + (speed*(isWalking ? 1f : runStepLenghten)))*
                             Time.fixedDeltaTime;
            }

            if (!(stepCycle > nextStep))
            {
                return;
            }

            nextStep = stepCycle + stepInterval; //bir sonraki adým adým döngüsü ve adým aralýðýnýn toplamý

            PlayFootStepAudio(); //karakter yürürken ayak sesi
        }


        private void PlayFootStepAudio() //ayak sesi
        {
            if (!characterController.isGrounded)
            {
                return;
            }
            int n = Random.Range(1, footStepSound.Length); // birinci indisden ayak sesi dizisinin uzunluðu arasýnda bir ses seç
            //ayak sesi hep tekrar ediyor 
            audioSource.clip = footStepSound[n];
            audioSource.PlayOneShot(audioSource.clip);
            
            footStepSound[n] = footStepSound[0]; //seçtiðin sesi 0'a at ki bir daha seçme
            footStepSound[0] = audioSource.clip;
        }


        private void UpdateCameraPosition(float speed) //her adýmda kamerayý günceller
        {
            Vector3 newCameraPosition;
            if (!useHeadBob)
            {
                return;
            }

            if (characterController.velocity.magnitude > 0 && characterController.isGrounded) //karakterin hýzý varsa ve yerdeyse
            {
                camera.transform.localPosition = //bulunduðu pozisyonu günceller
                    m_HeadBob.DoHeadBob(characterController.velocity.magnitude +
                                      (speed*(isWalking ? 1f : runStepLenghten)));
                newCameraPosition = camera.transform.localPosition;
                newCameraPosition.y = camera.transform.localPosition.y - jumpBob.Offset();
            }
            else
            {
                newCameraPosition = camera.transform.localPosition; //kameranýn yeni position'u
                newCameraPosition.y = originalCameraPosition.y - jumpBob.Offset();
            }
            camera.transform.localPosition = newCameraPosition;
        }


        private void GetInput(out float speed) //karakterin saða sola hareket etmesini saðlar
        {
            float horizontal = CrossPlatformInputManager.GetAxis("Horizontal"); //a - d tuþlarý
            float vertical = CrossPlatformInputManager.GetAxis("Vertical"); // w - s tuþlarý

            bool waswalking = isWalking;

            isWalking = !Input.GetKey(KeyCode.LeftShift); //koþma

            speed = isWalking ? walkSpeed : runSpeed; //hýz yürüme hýzýna eþitse yürü deðilse koþ
            input = new Vector2(horizontal, vertical);

            if (input.sqrMagnitude > 1) //girdiler toplamýnýn karekökü 1'den fazlaysa
            {
                input.Normalize(); //normalize et
            }
        }


        private void RotateView() //Mouse hareketi
        {
            m_MouseLook.LookRotation (transform, camera.transform);
        }
    }
}
