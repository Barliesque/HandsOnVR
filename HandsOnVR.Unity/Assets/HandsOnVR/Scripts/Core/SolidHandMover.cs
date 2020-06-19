using UnityEngine;


namespace HandsOnVR
{
	/// <summary>
	/// Controls movement of the solid hands, transitioning between matching the 
	/// movements of the VR controllers and attaching themselves to a grabbed object.
	/// </summary>
	public class SolidHandMover : MonoBehaviour
	{

		[SerializeField] private GameObject _handController;
		private IHandController _controller;
		
		public bool MatchPosition = true;
		public bool MatchRotation = true;

		public IGrabAnchor Anchor;
		[SerializeField] private GrabAttachPoint _attachPoint;

		private Transform _xform;
		private Vector3 _targetPos;
		private Quaternion _targetRot;
		private float _transition;

		private void Start()
		{
			_xform = GetComponent<Transform>();
			_controller = _handController.GetComponent<IHandController>();
		}


		private void FixedUpdate()
		{
			if (_controller == null) return;

			if (Anchor == null)
			{
				_transition *= 0.875f;
				if (_transition <= 0.01f) _transition = 0f;
			} else
			{
				_transition = Mathf.Lerp(_transition, 1f, 0.125f);
			}

			if (Anchor == null && _transition <= 0f)
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
					if (Anchor != null) _targetPos = Anchor.GetPosition(hand) + offsetPos;
					_xform.position = Vector3.Lerp(_controller.Position, _targetPos, _transition);
				}

				if (MatchRotation)
				{
					//_xform.rotation = Anchor.GetRotation(hand);
					if (Anchor != null) _targetRot = Anchor.GetRotation(hand);
					_xform.rotation = Quaternion.Lerp(_controller.Rotation, _targetRot, _transition);
				}

			}
		}


	}
}