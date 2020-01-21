using UnityEngine;


namespace HandsOnVR
{
	/// <summary>
	/// Controls movement of the solid hands, transitioning between matching the 
	/// movements of the VR controllers and attaching themselves to a grabbed object.
	/// </summary>
//	[ExecuteInEditMode]
	public class SolidHandMover : MonoBehaviour
	{

		[SerializeField] HandController _controller;
		[Range(0, 1)] public float Transition;
		public bool MatchPosition = true;
		public bool MatchRotation = true;

		public IGrabAnchor Anchor;
		[SerializeField] GrabAttachPoint _attachPoint;

		private Transform _xform;


		private void Start()
		{
			_xform = GetComponent<Transform>();
		}


		void Update()
		{
			if (_controller == null) return;

			if (Anchor == null)
			{
				if (MatchRotation)
				{
					_xform.rotation = _controller.Rotation;
				}
				if (MatchPosition)
				{
					_xform.position = _controller.Position;
				}
			} else
			{
				var hand = _controller.Hand;
				if (MatchPosition)
				{
					var offsetPos = _xform.position - _attachPoint.Position;
					//_xform.position = Anchor.GetPosition(hand) + offsetPos;
					var targetPos = Anchor.GetPosition(hand) + offsetPos;
					_xform.position = Vector3.Lerp(_controller.Position, targetPos, Transition);
				}

				if (MatchRotation)
				{
					//_xform.rotation = Anchor.GetRotation(hand);
					var targetRot = Anchor.GetRotation(hand);
					_xform.rotation = Quaternion.Lerp(_controller.Rotation, targetRot, Transition);
				}

			}
		}


	}
}