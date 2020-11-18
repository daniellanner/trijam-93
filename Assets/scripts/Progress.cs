using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using io.daniellanner.indiversity;

[TimelineDescription("game", true)]
public class Progress : MonoBehaviourInTimeline
{
	#region properties
	[SerializeField]
	float _reducePerSecond = .1f;
	#endregion

	#region state
	float _currentProgress = 1f;
	#endregion

	private void OnEnable()
	{
		PlayerController.ShootingStarHit += ShootingStarHit;
	}

	private void OnDisable()
	{
		PlayerController.ShootingStarHit -= ShootingStarHit;
	}

	public override void EnterTimeline()
	{
		_currentProgress = 1f;
	}

	public override void ExitTimeline()
	{
		base.ExitTimeline();
	}

	public override void UpdateTimeline(float dt)
	{
		_currentProgress -= _reducePerSecond * dt;

		if(_currentProgress <= 0f)
		{
			// game over
		}

		_currentProgress = Mathf.Clamp01(_currentProgress);

		Vector3 currentScale = transform.localScale;
		currentScale.x = Mathf.Lerp(currentScale.x, _currentProgress * 4.5f, Time.deltaTime);
		transform.localScale = currentScale;
	}

	private void ShootingStarHit(int id)
	{
		id = Mathf.Clamp(id, 0, 7);

		_currentProgress += ((float)id / 7.0f) * 0.35f;
		_currentProgress = Mathf.Clamp01(_currentProgress);
	}
}
