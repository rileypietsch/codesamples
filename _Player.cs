using UnityEngine;
using System.Collections;

public class _Player : MonoBehaviour
{

		//PUBLIC IVARS*******************************

		public GameObject 		
				basicBullet,
				mainCam;
		public float				
				accel_Ground,
				accel_Air,
				maxSpeed_Ground,
				maxSpeed_Air,
				jumpDelay = .2f,
				fireDelay = .5f,
				jumpForce;
		public bool
				isGrounded;
		public AudioClip
				fireSFX, jumpSFX;



		//PRIVATE IVARS******************************

		enum MoveState
		{
				Ground,
				Air,
				Stun,
				GroundIdle,
				Jump,
				Test
		}

		MoveState 
				playerMoveState;
		Vector3 
				moveDir;
		float						
				forwardInput,
				lateralInput,
				jumpInput,
				jumpDelayCounter,
				fireDelayCounter;

		//**********************************************************************

		void Start ()
		{
				playerMoveState = MoveState.Ground;
		}

		void FixedUpdate ()
		{
				#region Input Management
				forwardInput = (float)Input.GetAxis ("Forward");
				lateralInput = (float)Input.GetAxis ("Lateral");
				jumpInput = (float)Input.GetAxis ("Jump");
				#endregion
				

				//print (rigidbody.velocity);
				#region Determine State
				if (isGrounded == false)
						playerMoveState = MoveState.Air;
				else if (forwardInput != 0 || lateralInput != 0)
						playerMoveState = MoveState.Ground;
				else if (forwardInput * lateralInput == 0 && isGrounded)
						playerMoveState = MoveState.GroundIdle;
				#endregion

				#region Firing Check
				if (Input.GetMouseButtonDown (0) && fireDelayCounter < 0) {
						fireDelayCounter = fireDelay;
						Fire ();
				}
				#endregion

				#region Jump
				if (jumpInput != 0 && isGrounded && jumpDelayCounter < 0) {
						playerMoveState = MoveState.Jump;
						jumpDelayCounter = jumpDelay;
				}
				jumpDelayCounter -= Time.fixedDeltaTime;
				#endregion
				
				#region State machine

				switch (playerMoveState) {
				case MoveState.Ground: //********************************************
						rigidbody.drag = 10f;
						moveDir = new Vector3 (lateralInput, 0, forwardInput).normalized;
						if (rigidbody.velocity.magnitude < maxSpeed_Ground && isGrounded && jumpDelayCounter < 0) {
								rigidbody.AddRelativeForce (moveDir * accel_Ground);
						}
						break;

				case MoveState.Air: //********************************************
						rigidbody.drag = 0f;
						moveDir = new Vector3 (lateralInput, 0, forwardInput).normalized;

						/** This section curbs the movement so air strafing is possible but player input can still steer player without breaching max speed */
						if (Mathf.Abs (transform.InverseTransformDirection (rigidbody.velocity).x) > maxSpeed_Air) {
								if (Mathf.Sign (lateralInput) == Mathf.Sign (transform.InverseTransformDirection (rigidbody.velocity).x)) {
										moveDir.x = 0;
								}
						}

						if (Mathf.Abs (transform.InverseTransformDirection (rigidbody.velocity).z) > maxSpeed_Air) {
								if (Mathf.Sign (forwardInput) == Mathf.Sign (transform.InverseTransformDirection (rigidbody.velocity).z)) {
										moveDir.z = 0;
								}
						}
						rigidbody.AddRelativeForce (moveDir * accel_Air);
						break;

				case MoveState.Stun: //********************************************
			
						break;

				case MoveState.GroundIdle: //********************************************
						rigidbody.drag = 10f;
						break;

				case MoveState.Jump: //********************************************
						rigidbody.drag = 0f;
						rigidbody.AddRelativeForce (new Vector3 ((lateralInput * jumpForce) / 2f, jumpForce, (forwardInput * jumpForce) / 2f), ForceMode.Impulse);
						break;

				default: //********************************************
						break;

	
				}
				#endregion

				moveDir = Vector3.zero;
				fireDelayCounter -= Time.fixedDeltaTime;
				transform.Translate (0, 0, .0000000001f); //Necessary for Unity's handling of adding force to a rigidbody that has a collider grow into it if it's not moving
		}

		void Fire ()
		{
				GameObject tempGO = Instantiate (basicBullet, transform.position, Quaternion.identity) as GameObject;
				tempGO.GetComponent<BasicBullet> ().SetStartData (transform.position, mainCam.transform.rotation);
				audio.PlayOneShot (fireSFX);
		}
		
		/** Adds force to player from radial bullet impact explosion */
		public void AddImpact (Vector3 dir, float impactMag)
		{
				rigidbody.AddForce (dir * impactMag, ForceMode.Impulse);
		}
		
		/** Quick and easy ground detection for prototype */
		void OnCollisionEnter (Collision col)
		{
				if (col.gameObject.tag == "floor")
						isGrounded = true;
		}

		void OnCollisionExit (Collision col)
		{
				if (col.gameObject.tag == "floor")
						isGrounded = false;

		}
}
