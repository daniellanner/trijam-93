using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using io.daniellanner.indiversity;

[TimelineDescription("game", true)]
public class PlayerController : MonoBehaviourInTimeline
{
	#region constants
	private const int NUMBER_OF_COLORABLES = 7;
	#endregion

	#region properties
#pragma warning disable 0649
	[SerializeField]
	[Tooltip("Max amount of rotation in Degrees")]
	private float _maxRotation = 90f;

	[SerializeField]
	private Material _yellowColor;
	[SerializeField]
	private Material _greenColor;
	[SerializeField]
	private Material _pinkColor;
	[SerializeField]
	private Material _neutralColor;
#pragma warning restore 0649
	#endregion

	#region members
	private List<ColorInstantiator.EColor> _colorQueue = new List<ColorInstantiator.EColor>();
	#endregion

	#region cache
	private Transform[] _hinges;
	private MeshRenderer[] _hingesRenderer;
	private AudioSource _audioSource;
	private static ColorInstantiator _colorInit;
	#endregion

	public delegate void ShootingStarHitEvent(int id);
	public static ShootingStarHitEvent ShootingStarHit;

	private void Awake()
	{
		Cursor.visible = false;

		_hinges = GetComponentsInChildren<Transform>();
		_hingesRenderer = GetComponentsInChildren<MeshRenderer>();
		_audioSource = GetComponent<AudioSource>();

		for (int i = 0; i < NUMBER_OF_COLORABLES; i++)
		{
			_colorQueue.Add(ColorInstantiator.EColor.Empty);
		}

		if (_colorInit == null)
		{
			_colorInit = FindObjectOfType<ColorInstantiator>();
		}
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

	public void Collision(int playerID, string color)
	{
		_audioSource.pitch = Mathf.Lerp(.9f, 1.5f, (float)playerID/7f);
		_audioSource.Play();

		ShootingStarHit?.Invoke(playerID);

		// this would color in the hinges of the tentacle and pop the color of the biggest
		// tentacle to color in the bit below the player
		// I turned out not to like it, maybe in the future with some fine tuning
		// for now everything is set up but I disabled it

		/*
		// hacky, need to adjust collision id to be more generic
		if (!System.Enum.TryParse(color, out ColorInstantiator.EColor col))
		{
			return;
		}

		if (playerID >= NUMBER_OF_COLORABLES || playerID < 0)
		{
			// hit with tip, no push back
			return;
		}

		var nextPop = _colorQueue[0];

		for (int i = 0; i <= playerID; i++)
		{
			if (i == playerID)
			{
				_colorQueue[i] = col;
			}
			else
			{
				_colorQueue[i] = _colorQueue[i + 1];
			}
		}

		for (int i = 0;
			i < NUMBER_OF_COLORABLES &&
			i < _hingesRenderer.Length &&
			i < _colorQueue.Count; i++)
		{
			switch (_colorQueue[i])
			{
				case ColorInstantiator.EColor.Pink:
					_hingesRenderer[i].material = _pinkColor;
					break;
				case ColorInstantiator.EColor.Green:
					_hingesRenderer[i].material = _greenColor;
					break;
				case ColorInstantiator.EColor.Yellow:
					_hingesRenderer[i].material = _yellowColor;
					break;
				case ColorInstantiator.EColor.Empty:
				default:
					_hingesRenderer[i].material = _neutralColor;
					break;
			}
		}

		_colorInit?.Instantiate(transform.position, nextPop, 1.25f);
		*/
	}
}
