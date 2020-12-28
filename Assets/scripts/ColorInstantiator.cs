using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using io.daniellanner.indiversity;

public class ColorInstantiator : MonoBehaviour
{
	public enum EColor { Pink, Green, Yellow, Empty }

	#region properties
#pragma warning disable 0649
	[SerializeField]
	private GameObject _colorFlowPink;
	[SerializeField]
	private GameObject _colorFlowGreen;
	[SerializeField]
	private GameObject _colorFlowYellow;
#pragma warning restore 0649
	#endregion

	private const float START_Z = 14.5f;
	private const float Z_ADD_OFFSET = -.001f;

	private float _currentZOffset = START_Z;

	public ColorFlow Instantiate(Vector3 position, EColor col, float width, float maxWidth = 0f)
	{
		GameObject tmp;

		switch (col)
		{
			case EColor.Pink:
				tmp = _colorFlowPink;
				break;
			case EColor.Green:
				tmp = _colorFlowGreen;
				break;
			case EColor.Yellow:
				tmp = _colorFlowYellow;
				break;
			case EColor.Empty:
			default:
				return null;
		}

		bool random = !Utilities.NearlyEqual(0f, maxWidth);
		float w = random ? Random.Range(width, maxWidth) : width;

		position.z = _currentZOffset;
		_currentZOffset += Z_ADD_OFFSET;

		return Instantiate(tmp, position, Quaternion.identity)
			?.GetComponent<ColorFlow>()
			?.Expand(w);
	}
}
