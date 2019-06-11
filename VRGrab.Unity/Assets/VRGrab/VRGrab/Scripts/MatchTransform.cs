using UnityEngine;


namespace Barliesque.VRGrab
{
	/// <summary>
	/// Copies the world position and/or orientation from another transform to the local transform.
	/// </summary>
	[ExecuteInEditMode]
	public class MatchTransform : MonoBehaviour
	{

		public Transform Target;
		public bool MatchPosition = true;
		public bool MatchRotation = true;
		[Range(0,1)] public float Transition;
		public Transform SecondTarget;

		Transform _xform;

		private void Start()
		{
			_xform = GetComponent<Transform>();
		}

		void Update()
		{
			if (Target == null)
			{
				return;
			}

			if (SecondTarget == null)
			{
				if (MatchPosition)
				{
					_xform.position = Target.position;
				}
				if (MatchRotation)
				{
					_xform.rotation = Target.rotation;
				}
			} else
			{
				if (MatchPosition)
				{
					_xform.position = Vector3.Lerp(Target.position, SecondTarget.position, Transition);
				}
				if (MatchRotation)
				{
					_xform.rotation = Quaternion.Lerp(Target.rotation, SecondTarget.rotation, Transition);
				}
			}
		}


	}
}