using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using io.daniellanner.indiversity;

[TimelineDescription("game", true)]
public class PlayerController : MonoBehaviourInTimeline
{
	[SerializeField]
	[Tooltip("Max amount of rotation in Degrees")]
	private float _maxRotation = 90f;

	#region cache
	private Transform[] _hinges;
	private AudioSource _audioSource;
	#endregion
	
	public delegate void ShootingStarHitEvent(int id);
	public static ShootingStarHitEvent ShootingStarHit;

	private void Awake()
	{
		Cursor.visible = false;

		_hinges = GetComponentsInChildren<Transform>();
		_audioSource = GetComponent<AudioSource>();
	}

	public override void EnterTimeline()
	{
	}

	public override void ExitTimeline()
	{
		base.ExitTimeline();
	}

	public override void UpdateTimeline(float dt)
	{
		base.UpdateTimeline(dt);

		float t = Input.mousePosition.x / Screen.width;
		float eulerZ = Mathf.Lerp(_maxRotation, -_maxRotation, t);

		foreach(var it in _hinges)
		{
			Utilities.UpdateEulerRotationZ(it, eulerZ);
		}
	}

	public void Collision(int id)
	{
		_audioSource.pitch = Mathf.Lerp(.9f, 1.5f, (float)id/7f);
		_audioSource.Play();

		ShootingStarHit?.Invoke(id);
	}
}
