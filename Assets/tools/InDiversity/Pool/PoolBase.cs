using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace io.daniellanner.indiversity
{
	public class PoolBase<T> : MonoBehaviour where T : Component 
	{
		private T[] _objects;
		private int _objectsIdx = 0;

		private bool _isInitialized = false;

		private void OnEnable()
		{
			Initialize();
		}

		private void Initialize()
		{
			var tmp = GetComponentsInChildren<T>().ToList();
			tmp.RemoveAll(it => it == GetComponent<T>()); // GetComponentsInChildren also returns matching components in self
			_objects = tmp.ToArray();

			_isInitialized = true;
		}

		protected Result<T> GetNextObject()
		{
			if(!_isInitialized)
			{
				Initialize();
			}

			if (_objects == null || _objects.Length == 0)
			{
				return Result<T>.FALSE;
			}

			if (_objectsIdx >= _objects.Length)
			{
				_objectsIdx = 0;
			}

			if(_objects[_objectsIdx] == null)
			{
				return Result<T>.FALSE;
			}

			var result = new Result<T>(_objects[_objectsIdx]);

			_objectsIdx++;
			if (_objectsIdx >= _objects.Length)
			{
				_objectsIdx = 0;
			}

			return result.TryMakeSafe();
		}

		public void Cleanup()
		{
			foreach (var it in _objects)
			{
				it.transform.localPosition = Vector3.zero;
			}
		}

		public void ReturnToPool(T obj)
		{
			obj.transform.SetParent(transform);
			obj.transform.localPosition = Vector3.zero;
		}
	}
}