using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using io.daniellanner.indiversity;

[TimelineDescription("game", true)]
public class ShootingStarSpawner : MonoBehaviourInTimeline
{
	[SerializeField]
	private GameObject _yellowStar;
	[SerializeField]
	private GameObject _greenStar;
	[SerializeField]
	private GameObject _pinkStar;

	#region constants
	private const float MAX_REACH = 1.9f;
	#endregion

	private void Awake()
	{

	}

	private void SpawnStar()
	{
		Vector3 spawnPosition = Vector3.zero;

		spawnPosition.y = 8f;
		spawnPosition.x = Random.Range(-MAX_REACH, MAX_REACH);

		int idx = Random.Range(0, 3);
		GameObject toInstantiate;

		switch (idx)
		{
			default:
			case 0:
				toInstantiate = _yellowStar;
				break;
			case 1:
				toInstantiate = _greenStar;
				break;
			case 2:
				toInstantiate = _pinkStar;
				break;
		}

		Instantiate(toInstantiate, spawnPosition, Quaternion.identity)
			?.GetComponent<ShootingStar>()
			?.Spawn();
	}

	public override void EnterTimeline()
	{
		InvokeRepeating("SpawnStar", 1f, 1f);
	}

	public override void ExitTimeline()
	{
		CancelInvoke();
	}
}
