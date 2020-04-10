using UnityEngine;

namespace HandsOnVR
{
	public class GrabAttachPoint : MonoBehaviour
	{
		[SerializeField] private Grabber _grabber;
		public Grabber Grabber => _grabber;
		[SerializeField] private bool _visualize = false;

		private Transform _xform;
		public Vector3 Position => _xform.position;
		public Quaternion Rotation => _xform.rotation;
		public Vector3 EulerAngles => _xform.eulerAngles;


		private void Awake()
		{
			_xform = GetComponent<Transform>();
		}


		private void OnDrawGizmos()
		{
			if (!_visualize) return;
			
			var pos = transform.position;
			Gizmos.color = Color.red;
			Gizmos.DrawLine(pos, pos + transform.right * 0.05f);
			Gizmos.color = Color.blue;
			Gizmos.DrawLine(pos, pos + transform.forward * 0.05f);
			Gizmos.color = Color.green;
			Gizmos.DrawLine(pos, pos + transform.up * 0.05f);
			Gizmos.color = Color.magenta;
			Gizmos.DrawWireSphere(pos, 0.005f);
		}
	}

}