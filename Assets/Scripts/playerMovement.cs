using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using Cinemachine;
public class playerMovement : NetworkBehaviour
{

	public GameObject bendPos;
	public CinemachineVirtualCamera headCam;
	public float walkSpeed;
	public float runSpeed;
	public float jumpSpeed;
	public float crouchSpeed;
	public float gravity;
	CharacterController character;
	[SyncVar]
	string animClip = "stand";
	[SyncVar]
	Quaternion bendRot = Quaternion.identity;
	float Yvelocity;

	[SyncVar]
	public float velocityMagnatude;

	public GameObject[] firstPerson;
	public GameObject[] thirdPerson;
	// Start is called before the first frame update
	void Start()
	{
		character = GetComponent<CharacterController>();
		character.detectCollisions = false;

		SetAllActive(firstPerson, hasAuthority);
		SetAllActive(thirdPerson, !hasAuthority);
		if(hasAuthority) {
			headCam.m_Priority = 10;
		}else {
			headCam.m_Priority = -10;
		}
		

	}

	// Update is called once per frame
	void Update()
	{
		if(GetComponent<NetworkIdentity>().hasAuthority)
		{
			UpdateMagnitude(character.velocity.magnitude);
			Vector3 raw = Vector3.zero;
			raw.x = Input.GetAxisRaw("Horizontal");
			raw.z = Input.GetAxisRaw("Vertical");
			raw.y = 0;
			raw.Normalize();

			raw = transform.TransformDirection(raw);

			if(!character.isGrounded)
			{
				Yvelocity -= Physics.gravity.magnitude * gravity * Time.deltaTime;
			}
			float sel = 0f;
			if(Input.GetKeyDown(KeyCode.Space) && character.isGrounded)
			{
				Yvelocity = jumpSpeed;
			}
			if(Input.GetKey(KeyCode.LeftControl))
			{
				sel = crouchSpeed;

			}
			else if(Input.GetKey(KeyCode.LeftShift))
			{
				sel = runSpeed;
			}
			else
			{
				sel = walkSpeed;
			}

			raw *= sel;
			raw.y = Yvelocity;

			if(Input.GetKey(KeyCode.LeftControl))
			{
				character.height = 1.25f;
			//	character.center = new Vector3(0, 0.875f, 0);
			}
			else
			{
				character.height = 1.75f;
			//	character.center = new Vector3(0, 0.625f, 0);
			}


			character.Move(raw * Time.deltaTime);


			if(!character.isGrounded)
			{
				UpdateAnimation("jump");
			}
			else
			{

				if(Input.GetKey(KeyCode.LeftControl))
				{
					if(character.velocity.magnitude > 0.1f)
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

					if(character.velocity.magnitude > 0.1f)
					{
						if(Input.GetKey(KeyCode.LeftShift))
						{
							if(Input.GetAxisRaw("Horizontal") > .1f)
							{
								UpdateAnimation("stand_runR");
							}
							else if(Input.GetAxisRaw("Horizontal") < -.1f)
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
							if(Input.GetAxisRaw("Horizontal") > .1f)
							{
								UpdateAnimation("stand_walkR");
							}
							else if(Input.GetAxisRaw("Horizontal") < -.1f)
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


			//GetComponent<Animation>().CrossFade(animClip, 0.1f);
			bendRot = bendPos.transform.rotation;
			UpdateBendRot(bendRot);
		}
		else
		{
			//GetComponent<Animation>().CrossFade(animClip, 0.1f);
			bendPos.transform.rotation = Quaternion.Lerp(bendPos.transform.rotation, bendRot, Time.deltaTime * 5f);
		}

	}
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

	void SetAllActive(GameObject[] obs, bool bo)
	{
		
		foreach(GameObject gam in obs)
		{
			if(gam.GetComponent<SkinnedMeshRenderer>())
			{
				if(bo)
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

	public override void OnStartAuthority()
	{
		foreach(connected_client man in FindObjectsOfType<connected_client>())
		{
			if(man.GetComponent<NetworkIdentity>().connectionToClient == GetComponent<NetworkIdentity>().connectionToClient)
			{
				man.myPlayer = this;
			}
		}
		base.OnStartAuthority();
	}

}
