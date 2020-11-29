using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class DoorScript : MonoBehaviour
{
	public LayerMask playerLayerMask;
	[SerializeField] private AudioEvent openAudioEvent;
	
	private bool isOpened { get; set; }

	public UnityAction OnDoorOpen;
	
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
					OpenDoor();
				}
			}
		}
	}

	private void OpenDoor()
	{
		StartCoroutine(OpenDoorEnumerator());
	}

	private IEnumerator OpenDoorEnumerator()
	{
		openAudioEvent.Play();
		var sr = GetComponent<SpriteRenderer>();
		sr.color = new Color(0.9f, 0.9f, 0.9f, 0.5f);
		yield return new WaitForSeconds(1f);
		//todo - go to loading screen
		OnDoorOpen?.Invoke();
	}
}
