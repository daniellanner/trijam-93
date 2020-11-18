using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using io.daniellanner.indiversity;

public class PlayerCollidable : MonoBehaviour, ICollidable
{
	#region properties
	[SerializeField]
	private float _radius = .5f;
	[SerializeField]
	private int _id = 0;
	#endregion

	#region cache
	private PlayerController _controller;
	#endregion

	private void Awake()
	{
		_controller = FindObjectOfType<PlayerController>();
		FindObjectOfType<FauxCollision>()?.SetPlayer(this);
	}

	public void CollisionWith(string id)
	{
		_controller?.Collision(_id);
	}

	public string GetID()
	{
		return "player";
	}

	public Vector3 GetPosition()
	{
		return transform.position;
	}

	public float GetRadius()
	{
		return _radius;
	}
}
