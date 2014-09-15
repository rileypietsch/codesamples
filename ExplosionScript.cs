using UnityEngine;
using System.Collections;

public class ExplosionScript : MonoBehaviour
{

		//Public Ivars
		public float 
				targetRadius = 2.0f, 
				knockbackForce = 50f;
		public AudioClip 
				explodeSFX;

		//Private Ivars
		float 
				particleLifetime, 
				deltaRadiusPerFrame,
				timeOfCreation, 
				timeOfReverse;
		bool 	isGrowing = true,
				hasHitPlayer;
		
		void Start ()
		{
				particleLifetime = this.GetComponent<ParticleSystem> ().startLifetime;
				timeOfCreation = Time.time;
				GetComponent<SphereCollider> ().radius = 0;
				audio.PlayOneShot (explodeSFX);
		}

		/** Grows and then shrinks a circular collider in coordination with associated explosion effect */
		void FixedUpdate ()
		{
				if (isGrowing)
				{
						GetComponent<SphereCollider> ().radius = Mathf.Lerp (GetComponent<SphereCollider> ().radius, targetRadius, (Time.time - timeOfCreation));
						if (Mathf.Abs (GetComponent<SphereCollider> ().radius - targetRadius) < .01f)
						{
								isGrowing = false;
								timeOfReverse = Time.time;
						}
				}
				else
				{
						GetComponent<SphereCollider> ().radius = Mathf.Lerp (GetComponent<SphereCollider> ().radius, 0f, (Time.time - timeOfReverse));
				}
				if (Time.time - timeOfCreation > particleLifetime)
				{
						Destroy (gameObject);
				}
		}

		void OnTriggerEnter (Collider col)
		{
				if (col.gameObject.tag == "player1" && !hasHitPlayer)
				{
						hasHitPlayer = true;
						col.GetComponent<_Player> ().AddImpact ((col.transform.position - this.transform.position).normalized, (float)knockbackForce);
				}
		}
}
