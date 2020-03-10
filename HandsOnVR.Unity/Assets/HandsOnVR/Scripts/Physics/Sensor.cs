using UnityEngine;



namespace HandsOnVR
{

	/// <summary>
	/// Enables handling of collision events.
	/// </summary>
	public class Sensor : MonoBehaviour
	{

		public delegate void TriggerHandler(Sensor sensor, Collider other);
		public delegate void CollisionHandler(Sensor sensor, Collision collision);

		public event TriggerHandler OnEnterTrigger;
		public event TriggerHandler OnStayTrigger;
		public event TriggerHandler OnExitTrigger;

		public event CollisionHandler OnEnterCollision;
		public event CollisionHandler OnStayCollision;
		public event CollisionHandler OnExitCollision;


		private void OnTriggerEnter(Collider other)
		{
			OnEnterTrigger?.Invoke(this, other);
		}

		private void OnTriggerStay(Collider other)
		{
			OnStayTrigger?.Invoke(this, other);
		}

		private void OnTriggerExit(Collider other)
		{
			OnExitTrigger?.Invoke(this, other);
		}

		private void OnCollisionEnter(Collision collision)
		{
			OnEnterCollision?.Invoke(this, collision);
		}

		private void OnCollisionStay(Collision collision)
		{
			OnStayCollision?.Invoke(this, collision);
		}

		private void OnCollisionExit(Collision collision)
		{
			OnExitCollision?.Invoke(this, collision);
		}

	}

}