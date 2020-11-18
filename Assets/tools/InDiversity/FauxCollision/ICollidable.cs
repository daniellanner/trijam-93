using UnityEngine;

namespace io.daniellanner.indiversity
{
	public interface ICollidable
	{
		Vector3 GetPosition();
		float GetRadius();
		string GetID();
		void CollisionWith(string id);
	}
}
