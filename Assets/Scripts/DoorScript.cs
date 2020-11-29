using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class DoorScript : MonoBehaviour
{
	[SerializeField] LoadingSequence _killedDogSequence;
	[SerializeField] LoadingSequence _savedDogSequence;
	
	public LayerMask playerLayerMask;
	[SerializeField] private AudioEvent openAudioEvent;
	
	private bool isOpened { get; set; }

	public event Action<LoadingSequence> OnDoorOpen;
	
	private void OnTriggerEnter2D(Collider2D other)
	{
		if (((1 << other.gameObject.layer) & playerLayerMask) > 0)
		{
			if (!isOpened)
			{
				var player = other.GetComponent<PlayerScript>();
				if (player.HasKey)
				{
					isOpened = true;
					OpenDoor(player.KilledDog);
				}
			}
		}
	}

	private void OpenDoor(bool killedDog)
	{
		StartCoroutine(OpenDoorEnumerator(killedDog));
	}

	private IEnumerator OpenDoorEnumerator(bool killedDog)
	{
		openAudioEvent.Play();
		var sr = GetComponent<SpriteRenderer>();
		sr.color = new Color(0.9f, 0.9f, 0.9f, 0.5f);
		yield return new WaitForSeconds(1f);
		//todo - go to loading screen
		OnDoorOpen?.Invoke(killedDog ? _killedDogSequence : _savedDogSequence);
	}
}
