using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using io.daniellanner.indiversity;

public class ColorFlow : MonoBehaviour
{
	public ColorFlow Expand(float x)
	{
		Vector3 to = Vector3.one;
		to.x = x;
		to.y = 10f;

		var anim = new CoroutineTransformScale(transform, Vector3.zero, to)
			.SetInterpolation(new EaseInterpolation(3f))
			.SetDuration(1f);

		StartCoroutine(anim.GetIEnumerator());

		return this;
	}
}
