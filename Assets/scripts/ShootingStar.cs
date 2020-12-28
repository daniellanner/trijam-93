using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using io.daniellanner.indiversity;

public class ShootingStar : MonoBehaviour, ICollidable
{
	#region properties
	#pragma warning disable 0649
	[SerializeField]
	private ColorInstantiator.EColor _color;
	#pragma warning restore 0649
	#endregion

	#region constants
	private const float SPEED = 7.5f;
	#endregion

	#region state
	private Vector3 _speed;
	private bool _alive = true;
	#endregion

	#region cache
	private ParticleSystem _particleSystem;
	private static FauxCollision _collision;
	private static ColorInstantiator _colorInit;
	#endregion

	private void Awake()
	{
		_particleSystem = GetComponentInChildren<ParticleSystem>();

		if(_collision == null)
		{
			_collision = FindObjectOfType<FauxCollision>();
		}

		if (_colorInit == null)
		{
			_colorInit = FindObjectOfType<ColorInstantiator>();
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
			_colorInit?.Instantiate(transform.position, _color, .25f, 1f);

			Invoke("DestroyStar", .5f);
		}
	}

	public string GetID()
	{
		return _color.ToString();
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
