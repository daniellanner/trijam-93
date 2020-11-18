using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using io.daniellanner.indiversity;

public class ShootingStar : MonoBehaviour, ICollidable
{
	#region properties
	[SerializeField]
	private GameObject _colorFlow;
	#endregion

	#region constants
	private const float SPEED = 7.5f;
	#endregion

	#region state
	private Vector3 _speed;
	private bool _alive = true;
	private static float _currentLayerOffset = 0f;
	#endregion

	#region cache
	private ParticleSystem _particleSystem;
	private static FauxCollision _collision;
	#endregion

	private void Awake()
	{
		_particleSystem = GetComponentInChildren<ParticleSystem>();

		if(_collision == null)
		{
			_collision = FindObjectOfType<FauxCollision>();
		}
	}

	private void Start()
	{
		_speed = new Vector3(0f, -SPEED, 0f);
		_collision?.AddProp(this, GetInstanceID());
	}

	void Update()
	{
		if(!_alive)
		{
			return;
		}

		transform.Translate(_speed * Time.deltaTime);

		if(transform.position.y <= 0f)
		{
			// reached bottom
			_alive = false;
			_collision?.RemoveProp(GetInstanceID());

			Vector3 pos = transform.position;
			pos.z = 4.5f - _currentLayerOffset;

			_currentLayerOffset += 0.01f;

			Instantiate(_colorFlow, pos, Quaternion.identity)
				?.GetComponent<ColorFlow>()
				?.Expand();

			Invoke("DestroyStar", .5f);
		}
	}

	public string GetID()
	{
		return "star";
	}

	public Vector3 GetPosition()
	{
		return transform.position;
	}

	public float GetRadius()
	{
		return transform.localScale.x;
	}

	public void CollisionWith(string id)
	{
		_collision?.RemoveProp(GetInstanceID());

		_alive = false;
		_particleSystem?.Play();

		Invoke("DestroyStar", .5f);
	}

	public ShootingStar Spawn()
	{
		_alive = true;
		return this;
	}

	private void DestroyStar()
	{
		Destroy(gameObject);
	}
}
