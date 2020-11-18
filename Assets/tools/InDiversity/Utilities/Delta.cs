using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace io.daniellanner.indiversity
{
	public class Delta
	{
		private float _t = 0f;
		private IInterpolationMethod _interpolationMethod;

		public Delta(float t)
		{
			_t = t;
		}

		public Delta InterpolationMethod(IInterpolationMethod interpolationMethod)
		{
			_interpolationMethod = interpolationMethod;
			return this;
		}

		public static implicit operator Delta(double d) => new Delta((float)d);
		public static implicit operator Delta(float f) => new Delta(f);

		public static implicit operator float(Delta delta) => delta.Value;
		public static implicit operator double(Delta delta) => delta.Value;

		public float Value => _t;
		public float OneMinus => 1f - _t;

		public float Squared => _t * _t;
		/// <summary>
		/// (1-x)^2
		/// </summary>
		public float OneMinusSquared => OneMinus * OneMinus;

		public float Smooth => _interpolationMethod.Interpolate(_t);
	}
}
