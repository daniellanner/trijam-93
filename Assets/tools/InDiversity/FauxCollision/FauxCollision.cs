using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace io.daniellanner.indiversity
{
	public class FauxCollision : MonoBehaviour
	{
		private List<ICollidable> _props = new List<ICollidable>();
		private List<int> _uid = new List<int>();
		private List<int> _toRemove = new List<int>();

		private List<ICollidable> _players = new List<ICollidable>();

		public void SetPlayer(ICollidable player)
		{
			_players.Add(player);
		}

		public void AddProp(ICollidable prop, int unityID)
		{
			_props.Add(prop);
			_uid.Add(unityID);
		}

		public void RemoveProp(int unityID)
		{
			_toRemove.Add(unityID);
		}

		private void Update()
		{
			if (_players == null || _players.Count == 0f)
			{
				return;
			}

			Vector3 propPosition = Vector3.zero;
			float propRadius = 0f;

			float distance = 0f;

			for (int i = _props.Count - 1; i >= 0; i--)
			{
				foreach (var player in _players)
				{
					Vector3 playerPos = player.GetPosition();
					float playerRadius = player.GetRadius();

					var prop = _props[i];
								
					propPosition = prop.GetPosition();
					propRadius = prop.GetRadius();

					//orthographic view
					playerPos.z = 0f;
					propPosition.z = 0f;

					distance = Vector3.SqrMagnitude(playerPos - propPosition);
					if (distance <= Mathf.Pow(playerRadius + propRadius, 2f))
					{
						try
						{
							prop.CollisionWith(player.GetID());
						}
						catch(System.Exception e)
						{
							Debug.LogError("Your obstacle collision resolve method triggered an exception");
							Debug.LogException(e);
							continue;
						}

						try
						{
							player.CollisionWith(prop.GetID());
						}
						catch (System.Exception e)
						{
							Debug.LogError("Your player collision resolve method triggered an exception");
							Debug.LogException(e);
							continue;
						}
						break;
					}
				}
			}

			if (_toRemove.Count == 0)
			{
				return;
			}

			foreach (var it in _toRemove)
			{
				int index = _uid.IndexOf(it);
				if (index >= 0 && index < _uid.Count && Utilities.AllEqualLength(out int count, _uid, _props))
				{
					_uid.RemoveAt(index);
					_props.RemoveAt(index);
				}
				else
				{
					Debug.LogError("Something went terribly wrong when iterating through toRemove in FauxCollision.");
				}
			}

			_toRemove.Clear();
		}
	}
}