using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace io.daniellanner.indiversity
{
	public class Utilities
	{
		public static float TWOPI => 6.283185f;

		public static Transform[] GetChildrenWithTag(Transform parent, string tag)
		{
			return GetChildBy(parent, (Transform obj) =>
			{
				return obj.CompareTag(tag);
			});
		}

		public static Transform[] GetChildrenWithName(Transform parent, string name, bool caseInsensitive = true, bool removeWhitespace = false)
		{
			return GetChildBy(parent, (Transform obj) => 
			{
				string a = obj.name;
				string b = name;

				if(caseInsensitive)
				{
					a = a.ToLowerInvariant();
					b = b.ToLowerInvariant();
				}

				if(removeWhitespace)
				{
					a = RemoveWhitespace(a);
					b = RemoveWhitespace(b);
				}

				return a == b;
			});
		}

		private static Transform[] GetChildBy(Transform parent, System.Func<Transform, bool> comp)
		{
			List<Transform> returnContainer = new List<Transform>();

			LinkedList<Transform> queue = new LinkedList<Transform>();
			queue.AddLast(parent);

			while (queue.Count > 0)
			{
				var current = queue.First.Value;
				queue.RemoveFirst();

				for (int i = 0; i < current.childCount; i++)
				{
					var child = current.GetChild(i);

					if (comp.Invoke(child))
					{
						returnContainer.Add(child);
					}

					if (child.childCount > 0)
					{
						queue.AddLast(child);
					}
				}
			}

			return returnContainer.ToArray();
		}

		public static Result<Component> GetComponentIfNullSearchInScene<T>(ref GameObject obj, string name) where T : Component
		{
			if(obj == null || obj.GetComponent(typeof(T)) == null)
			{
				var found = GameObject.FindObjectsOfType<T>();
				if (found == null || found.Length == 0)
				{
					Debug.LogError($"No GameObjects with component of type {typeof(T)} could be found in Scene.");
					return Result<Component>.FALSE;
				}

				var exp = found.Where(it => it.name == name);

				if (exp.Count() == 0)
				{
					Debug.LogError($"No GameObject for {name} could be found in Scene.");
					return Result<Component>.FALSE;
				}

				obj = exp.First().gameObject;
			}

			var comp = obj.GetComponent(typeof(T));
			return new Result<Component>(comp).TryMakeSafe();
		}

		public static string RemoveWhitespace(string str)
		{
			return str
				.Replace(" ", "")
				.Replace("\n", "")
				.Replace("\t", "");
		}

		public static Vector3 RandomRange(Vector3 a, Vector3 b)
		{
			return new Vector3(Random.Range(a.x, b.x), Random.Range(a.y, b.y), Random.Range(a.z, b.z));
		}

		public static Vector2 RandomRange(Vector2 a, Vector2 b)
		{
			return new Vector2(Random.Range(a.x, b.x), Random.Range(a.y, b.y));
		}

		public static Transform UpdatePositionX(Transform transform, float x)
		{
			var tmp = transform.position;
			tmp.x = x;
			transform.position = tmp;

			return transform;
		}

		public static Transform UpdatePositionY(Transform transform, float y)
		{
			var tmp = transform.position;
			tmp.y = y;
			transform.position = tmp;

			return transform;
		}

		public static Transform UpdatePositionZ(Transform transform, float z)
		{
			var tmp = transform.position;
			tmp.z = z;
			transform.position = tmp;

			return transform;
		}

		public static Transform UpdateEulerRotationX(Transform transform, float x)
		{
			var tmp = transform.localEulerAngles;
			tmp.x = x;
			transform.localRotation = Quaternion.Euler(tmp);

			return transform;
		}

		public static Transform UpdateEulerRotationY(Transform transform, float y)
		{
			var tmp = transform.localEulerAngles;
			tmp.y = y;
			transform.localRotation = Quaternion.Euler(tmp);

			return transform;
		}

		public static Transform UpdateEulerRotationZ(Transform transform, float z)
		{
			var tmp = transform.localEulerAngles;
			tmp.z = z;
			transform.localRotation = Quaternion.Euler(tmp);

			return transform;
		}

		public static bool AllEqual(out int min, out int max, params int[] a)
		{
			if(a.Length == 0)
			{
				min = max = 0;
				return true;
			}

			bool same = true;
			min = max = a.First();
			
			foreach (var it in a)
			{
				if(it < min)
				{
					min = it;
					same = false;
				}

				if (it > max)
				{
					max = it;
					same = false;
				}
			}

			return same;
		}

		public static bool AllEqualLength(out int min, params ICollection[] a)
		{
			if (a.Length == 0)
			{
				min = 0;
				return true;
			}

			bool same = true;
			min = a.First().Count;

			foreach (var it in a)
			{
				if (it.Count < min)
				{
					min = it.Count;
					same = false;
				}
			}

			return same;
		}

		public static Vector3 QuadraticBezier(Vector3 start, Vector3 middle, Vector3 end, Delta t)
		{
			Vector3 f1 = t.OneMinusSquared * start;
			Vector3 f2 = 2.0f * t.OneMinus * t * middle;
			Vector3 f3 = t.Squared * end;
			return f1 + f2 + f3;
		}

		public static Vector3 QuadraticBezierDerivative(Vector3 start, Vector3 middle, Vector3 end, Delta t)
		{
			Vector3 f1 = -2f * start * t.OneMinus;
			Vector3 f2 = 2 * middle * (-2 * t + 1);
			Vector3 f3 = 2 * t * end;
			return f1 + f2 + f3;
		}
	}
}
