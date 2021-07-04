using System;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;
using UnityStandardAssets.Utility;
using Random = UnityEngine.Random;

namespace UnityStandardAssets.Characters.FirstPerson
{
    public class FirstPersonController : MonoBehaviour
    {
        public bool isWalking; //karakter y�r�yor mu
        public float walkSpeed; //y�r�me h�z�
        public float runSpeed;  //ko�ma h�z�
        [Range(0f, 1f)] public float runStepLenghten; //bir ad�m�n uzunluk de�eri
        public float jumpSpeed; //z�plama h�z�
        public float stickToGroundForce; //Yere direk d��mesini sa�layan de�er
        public float gravityMultiplier; //Yer �ekimi �arpan�
        public MouseLook m_MouseLook; //mouse hareketi
        public bool useFovKick; //g�r�� alan� vuru�unu kullan�yor mu
        public FOVKick m_FovKick = new FOVKick();
        public bool useHeadBob; //karakter hareket ederken etraf�na bak�yor mu 
        public CurveControlledBob m_HeadBob = new CurveControlledBob();
        public LerpControlledBob jumpBob = new LerpControlledBob();
        public float stepInterval; //ad�m aral���
        public AudioClip[] footStepSound; // ayak sesi
        public AudioClip jumpSound;// karakter z�play�nca ��kan ses
        public AudioClip landSound; // yere d���nce ��kan ses
        public float normalHeight;
        public float crouchHeight;

        private Camera camera; //fps kameras�
        private bool jump; //karakter z�plad� m� 
        private float yRotation; //y ekseninde d�nme de�eri
        private Vector2 input; //hareket girdisi
        private Vector3 moveDir = Vector3.zero;
        private CharacterController characterController;
        private CollisionFlags collisionFlags; //karakterin nesnelere �arpmas�
        private bool previouslyGrounded; //yerde miydi
        private Vector3 originalCameraPosition;
        private float stepCycle; //d�ng� de�eri
        private float nextStep; //
        private bool jumping; //z�plad� m� 
        private AudioSource audioSource;
        private Animator animator;


        private void Start()
        {
            //gerekli tan�mlamalar� yapt�m
            characterController = GetComponent<CharacterController>();
            camera = Camera.main;
            originalCameraPosition = camera.transform.localPosition;
            m_FovKick.Setup(camera); //karakterin bak�� a��s�
            m_HeadBob.Setup(camera, stepInterval);
            stepCycle = 0f;
            nextStep = stepCycle/2f;
            jumping = false;
            audioSource = GetComponent<AudioSource>();
			m_MouseLook.Init(transform , camera.transform);
            animator = GetComponent<Animator>();
        }
        
        private void Update() //karakterin z�plamas�n� ve e�ilmesini sa�l�yor
        {
            RotateView(); //karakter z�plarken etra�na bakar
            //z�plama durumunun ka��r�lmad���ndan emin olmak i�in baraya bakar
            if (!jump) //karakter z�palamad�ysa
            {
                jump = CrossPlatformInputManager.GetButtonDown("Jump"); //z�pla
            }

            if (!previouslyGrounded && characterController.isGrounded) //karakter yere d��t��� an
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

            //E�ilme
            if (Input.GetKeyDown(KeyCode.LeftControl))
            {
                characterController.height = crouchHeight;
            }

            if (Input.GetKeyUp(KeyCode.LeftControl))
            {
                characterController.height = normalHeight;
            }
        }
        private void PlayLandingSound() //y�r�me ayak sesi
        {
            audioSource.clip = landSound;
            audioSource.Play();
            nextStep = stepCycle + .5f;
        }


        private void FixedUpdate() //karakterin hareket ederken z�plamas�n� sa�l�yor
        {
            float speed;
            GetInput(out speed); //karakterin hareket etmesi i�in girdi almas�n� sa�lar
            // karakterin bakt��� noktaya ilerler
            Vector3 desiredMove = transform.forward*input.y + transform.right*input.x;

            // karakter zemine bast���n� anlamas� gerekiyor
            RaycastHit hitInfo;
            Physics.SphereCast(transform.position, characterController.radius, Vector3.down, out hitInfo,
                               characterController.height/2f, Physics.AllLayers, QueryTriggerInteraction.Ignore); //karakterimin i�indeki SphereCast karakterimle ayn� fizikse de�erleri almal�
            desiredMove = Vector3.ProjectOnPlane(desiredMove, hitInfo.normal).normalized;

            moveDir.x = desiredMove.x*speed; //h�zla birlikte x y�n�nde hareket et
            moveDir.z = desiredMove.z*speed; //h�zla birlikte y y�n�nde hareket et


            if (characterController.isGrounded) //karakter yere d��erken
            {
                moveDir.y = -stickToGroundForce; //yer �ekimine kar�� koyar

                if (jump) //karakter z�pl�yorsa
                {
                    moveDir.y = jumpSpeed;
                    PlayJumpSound();
                    jump = false;
                    jumping = true;
                }
            }
            else
            {
                moveDir += Physics.gravity*gravityMultiplier*Time.fixedDeltaTime; //karakterin hareket etmesini sa�lar ve deltatime sayesinde normal zamanda gider olmasayd� u�ard�
            }
            collisionFlags = characterController.Move(moveDir*Time.fixedDeltaTime);

            ProgressStepCycle(speed); //karakter ad�m at�yor h�z� ile
            UpdateCameraPosition(speed); //karakter ad�m atarken g�r�nt�y� g�nceller

            m_MouseLook.UpdateCursorLock();
        }


        private void PlayJumpSound() //z�plama sesi
        {
            audioSource.clip = jumpSound;
            audioSource.Play();
        }


        private void ProgressStepCycle(float speed) //karakterin ad�mlar�n� bir d�ng� i�erisinde atmas�
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

            nextStep = stepCycle + stepInterval; //bir sonraki ad�m ad�m d�ng�s� ve ad�m aral���n�n toplam�

            PlayFootStepAudio(); //karakter y�r�rken ayak sesi
        }


        private void PlayFootStepAudio() //ayak sesi
        {
            if (!characterController.isGrounded)
            {
                return;
            }
            int n = Random.Range(1, footStepSound.Length); // birinci indisden ayak sesi dizisinin uzunlu�u aras�nda bir ses se�
            //ayak sesi hep tekrar ediyor 
            audioSource.clip = footStepSound[n];
            audioSource.PlayOneShot(audioSource.clip);
            
            footStepSound[n] = footStepSound[0]; //se�ti�in sesi 0'a at ki bir daha se�me
            footStepSound[0] = audioSource.clip;
        }


        private void UpdateCameraPosition(float speed) //her ad�mda kameray� g�nceller
        {
            Vector3 newCameraPosition;
            if (!useHeadBob)
            {
                return;
            }

            if (characterController.velocity.magnitude > 0 && characterController.isGrounded) //karakterin h�z� varsa ve yerdeyse
            {
                camera.transform.localPosition = //bulundu�u pozisyonu g�nceller
                    m_HeadBob.DoHeadBob(characterController.velocity.magnitude +
                                      (speed*(isWalking ? 1f : runStepLenghten)));
                newCameraPosition = camera.transform.localPosition;
                newCameraPosition.y = camera.transform.localPosition.y - jumpBob.Offset();
            }
            else
            {
                newCameraPosition = camera.transform.localPosition; //kameran�n yeni position'u
                newCameraPosition.y = originalCameraPosition.y - jumpBob.Offset();
            }
            camera.transform.localPosition = newCameraPosition;
        }


        private void GetInput(out float speed) //karakterin sa�a sola hareket etmesini sa�lar
        {
            float horizontal = CrossPlatformInputManager.GetAxis("Horizontal"); //a - d tu�lar�
            float vertical = CrossPlatformInputManager.GetAxis("Vertical"); // w - s tu�lar�

            bool waswalking = isWalking;

            isWalking = !Input.GetKey(KeyCode.LeftShift); //ko�ma

            speed = isWalking ? walkSpeed : runSpeed; //h�z y�r�me h�z�na e�itse y�r� de�ilse ko�
            input = new Vector2(horizontal, vertical);

            if (input.sqrMagnitude > 1) //girdiler toplam�n�n karek�k� 1'den fazlaysa
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
