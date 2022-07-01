using UnityEngine;
using Mirror;
using Cinemachine;
public class playerMovement : NetworkBehaviour
{
    [Header("Permanant")]
    public GameObject bendPos;
    public CinemachineVirtualCamera headCam;
    public Camera viewmodelCam;
     public GameObject animCam;
    public GameObject[] firstPerson;
    public GameObject[] thirdPerson;
    public int layer_first;
    public int layer_third;
    public int layer_inv;
    public GameObject[] viewmodels;
    [Header("Speed")]
    public float walkSpeed;
    public float runSpeed;
    public float jumpSpeed;
    public float crouchSpeed;
    public float gravity;
    [Header("Readonly")]
    [HideInInspector]
    [SyncVar]
    public float velocityMagnatude;
    //Private
    CharacterController character;
    [SyncVar]
    string animClip = "stand";
    [SyncVar]
    Quaternion bendRot = Quaternion.identity;
    float Yvelocity;

    void Start()
    {
        //Setup cc and disable collisions (use collider)
        character = GetComponent<CharacterController>();
        character.detectCollisions = false;
        //Determine first or third person model
        if (hasAuthority)
        {
            SetAllLayer(firstPerson, layer_first);
            SetAllLayer(thirdPerson, layer_inv);
            SetAllLayer(viewmodels, layer_first);
        }
        else
        {
            SetAllActive(firstPerson, false);
            SetAllLayer(thirdPerson, layer_third);
            SetAllLayer(viewmodels, layer_third);
        }
        //Determine if local camera
        if (hasAuthority)
        {
            headCam.m_Priority = 10;
        }
        else
        {
            headCam.m_Priority = -10;
        }
        //Determine to use viewmodel camera
        viewmodelCam.gameObject.SetActive(hasAuthority);
    }

    void LateUpdate () {
        if (!GetComponent<NetworkIdentity>().hasAuthority) {return;}
        if(GetComponent<playerWeapons>().isAiming) {
            animCam.transform.localPosition = new Vector3(0,0.6f,0);
        }
    }

    void Update()
    {
        if (GetComponent<NetworkIdentity>().hasAuthority)
        {
            //Sync magnitude
            UpdateMagnitude(character.velocity.magnitude);
            //Get raw move input
            Vector3 raw = Vector3.zero;
            raw.x = Input.GetAxisRaw("Horizontal");
            raw.z = Input.GetAxisRaw("Vertical");
            raw.y = 0;
            raw.Normalize();
            raw = transform.TransformDirection(raw);
            //Sum gravity
            if (!character.isGrounded)
            {
                Yvelocity -= Physics.gravity.magnitude * gravity * Time.deltaTime;
            }else {
                 Yvelocity = -character.stepOffset / Time.deltaTime;
            }
            //Use appropriate speed
            float sel = 0f;
            if (Input.GetKeyDown(KeyCode.Space) && character.isGrounded)
            {
                Yvelocity = jumpSpeed;
            }
            if (Input.GetKey(KeyCode.LeftControl))
            {
                sel = crouchSpeed;

            }
            else if (Input.GetKey(KeyCode.LeftShift) && !GetComponent<playerWeapons>().isAiming)
            {
                sel = runSpeed;
            }
            else
            {
                sel = walkSpeed;
            }
            //Multiply vector by scalar
            raw *= sel;
            raw.y = Yvelocity;
            //Change model according to stance
            if (Input.GetKey(KeyCode.LeftControl))
            {
                character.height = 1.6f;
                character.center = new Vector3(0, 1.2f, 0);
            }
            else
            {
                character.height = 2f;
                character.center = new Vector3(0, 1f, 0);
            }
            //Apply movement
            character.Move(raw * Time.deltaTime);
            //Animation spaghetti
            if (!character.isGrounded)
            {
                UpdateAnimation("jump");
            }
            else
            {
                if (Input.GetKey(KeyCode.LeftControl))
                {
                    if (character.velocity.magnitude > 0.1f)
                    {
                        UpdateAnimation("crouch_walk");
                    }
                    else
                    {
                        UpdateAnimation("crouch_idle");
                    }
                }
                else
                {
                    if (character.velocity.magnitude > 0.1f)
                    {
                        if (Input.GetKey(KeyCode.LeftShift) && !GetComponent<playerWeapons>().isAiming)
                        {
                            if (Input.GetAxisRaw("Horizontal") > .1f)
                            {
                                UpdateAnimation("stand_runR");
                            }
                            else if (Input.GetAxisRaw("Horizontal") < -.1f)
                            {
                                UpdateAnimation("stand_runL");
                            }
                            else
                            {
                                UpdateAnimation("stand_run");
                            }
                        }
                        else
                        {
                            if (Input.GetAxisRaw("Horizontal") > .1f)
                            {
                                UpdateAnimation("stand_walkR");
                            }
                            else if (Input.GetAxisRaw("Horizontal") < -.1f)
                            {
                                UpdateAnimation("stand_walkL");
                            }
                            else
                            {
                                UpdateAnimation("stand_walk");
                            }
                        }
                    }
                    else
                    {
                        UpdateAnimation("stand");
                    }
                }
            }
            //Apply local animation
            GetComponent<Animation>().CrossFade(animClip, 0.1f);
            bendRot = bendPos.transform.rotation;
            UpdateBendRot(bendRot);
        }
        else
        {
            //Apply server animation
            GetComponent<Animation>().CrossFade(animClip, 0.1f);
            bendPos.transform.rotation = Quaternion.Lerp(bendPos.transform.rotation, bendRot, Time.deltaTime * 5f);
        }
    }
    //Commands to sync vars
    [Command]
    void UpdateAnimation(string newAni)
    {
        animClip = newAni;
    }
    [Command]
    void UpdateBendRot(Quaternion newA)
    {
        bendRot = newA;
    }
    [Command]
    void UpdateMagnitude(float newA)
    {
        velocityMagnatude = newA;
    }
    //Set all models visible or not
    static void SetAllActive(GameObject[] obs, bool bo)
    {
        foreach (GameObject gam in obs)
        {
            if (gam.GetComponent<SkinnedMeshRenderer>())
            {
                if (bo)
                {
                    gam.GetComponent<SkinnedMeshRenderer>().shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On;
                }
                else
                {
                    gam.GetComponent<SkinnedMeshRenderer>().shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.ShadowsOnly;
                }
            }
            else
            {
                gam.SetActive(bo);
            }
        }
    }
    //Set all model layers
    void SetAllLayer(GameObject[] obs, int layer)
    {
        foreach (GameObject g in obs)
        {
            SetLayerRecursively(g, layer);
        }
    }

    void SetLayerRecursively(GameObject obj, int newLayer)
    {
        if (null == obj)
        {
            return;
        }

        obj.layer = newLayer;

        foreach (Transform child in obj.transform)
        {
            if (null == child)
            {
                continue;
            }
            SetLayerRecursively(child.gameObject, newLayer);
        }
    }

    //Assign this player to owner
    public override void OnStartAuthority()
    {
        foreach (connected_client man in FindObjectsOfType<connected_client>())
        {
            if (man.GetComponent<NetworkIdentity>().connectionToClient == GetComponent<NetworkIdentity>().connectionToClient)
            {
                man.myPlayer = gameObject;
            }
        }
        base.OnStartAuthority();
    }
}
