using Barliesque.InspectorTools;
using System;
using UnityEngine;


namespace HandsOnVR
{
	/// <summary>
	/// A component to refine grabbing of objects.  Multiple anchors can be used to specify a variety of anchor points and orientations.
	/// </summary>
	[DisallowMultipleComponent]
	public class GrabAnchor : MonoBehaviour, IGrabAnchor
	{

		public enum Order
		{
			FirstOrSecond = 0,
			FirstOnly = 1,
			SecondOnly = 2
		}

		Vector3 _offsetLeft;
		Quaternion _orientLeft;
		Vector3 _offsetRight;
		Quaternion _orientRight;


		[Tooltip("Which hand was this GrabAnchor placed for?")]
		[SerializeField] Hand _primaryHand = Hand.Right;
		public bool SupportsHand(Hand hand)
		{
			return _mirrorForOtherHand || (_primaryHand == hand);
		}

		[Tooltip("Automatically mirror this anchor for use by the other hand?  (Experimental)")]
		[SerializeField] private bool _mirrorForOtherHand = false;
		public bool MirrorForOtherHand => _mirrorForOtherHand;

		[Tooltip("Formula to be used to mirror this anchor for use by the other hand.  (Experimental)")]
		[SerializeField, EnumFlags] private MirrorAction _mirrorActions;
		[Flags]
		public enum MirrorAction
		{
			InvertOffsetX = 1,
			InvertOffsetY = 2,
			InvertOffsetZ = 4,
			InvertRotationX = 8,
			InvertRotationY = 16,
			InvertRotationZ = 32
		}

		[Tooltip("Restrict whether this anchor may be grabbed first, or second?")]
		[SerializeField] Order _grabOrder = Order.FirstOrSecond;
		public Order GrabOrder { get { return _grabOrder; } }

		[Tooltip("Should this GrabAnchor use a different Grab Pose than specified by the Grabbable component?")]
		[SerializeField] bool _overridePose;
		public bool OverridePose { get { return _overridePose; } }

		[Tooltip("A bool parameter name found in the Animator components of the player's hands.  While this object is being grabbed, the specified parameter will be set to true.")]
		[SerializeField] protected string _grabPose;
		public int GrabPoseID { get; private set; }

		[Tooltip("Should this GrabAnchor have a different OrientToHand setting than specified by the Grabbable component?")]
		[SerializeField] bool _overrideOrientToHand;
		public bool OverrideOrientToHand { get { return _overrideOrientToHand; } }

		[Tooltip("Should this object be reoriented to meet the hand's orientation?")]
		[SerializeField] bool _orientToHand = true;
		public bool OrientToHand { get { return _orientToHand; } }


		Transform _xform;
		Transform _grabbableXform;

		public Grabbable Parent => _grabbable;
		Grabbable _grabbable;


		public Vector3 GetPosition(HandsOnVR.Hand hand)
		{
			var offset = (hand == Hand.Left) ? _offsetLeft : _offsetRight;
			return _grabbableXform.position + (_grabbableXform.rotation * offset);
		}

		public Quaternion GetRotation(HandsOnVR.Hand hand)
		{
			var orient = (hand == Hand.Left) ? _orientLeft : _orientRight;
			return _grabbableXform.rotation * orient;
		}

		// TODO  This shouldn't be necessary, at least not here
		public Vector3 GetUp(HandsOnVR.Hand hand)
		{
			return GetRotation(hand) * Vector3.up;
		}


		private void Awake()
		{
			_xform = GetComponent<Transform>();

			if (_overridePose)
			{
				GrabPoseID = Animator.StringToHash(_grabPose);
			}

			// Cache the offset position/rotation of this anchor from the Grabbable transform
			if (_primaryHand == Hand.Right)
			{
				CacheOffsetOrient(ref _offsetRight, ref _orientRight, ref _offsetLeft, ref _orientLeft);
			}
			else
			{
				CacheOffsetOrient(ref _offsetLeft, ref _orientLeft, ref _offsetRight, ref _orientRight);
			}
		}


		[SerializeField] bool _liveUpdate;  //TODO  Show this property at runtime?  ...or, better, simply call Awake via Message whenever the Method changes!
#if UNITY_EDITOR
		private void Update()
		{
			if (Application.isPlaying && _liveUpdate) Awake();
		}
#endif

		private void CacheOffsetOrient(ref Vector3 offset1, ref Quaternion orient1, ref Vector3 offset2, ref Quaternion orient2)
		{
			_grabbable = GetComponentInParent<Grabbable>();
			if (!_grabbable) return;
			_grabbableXform = _grabbable.transform;
			var grot = _grabbableXform.eulerAngles;
			var invGrot = Quaternion.Inverse(_grabbableXform.rotation);
			offset1 = invGrot * (_xform.position - _grabbableXform.position);

			orient1 = invGrot * _xform.rotation;
			var orientEulers = orient1.eulerAngles;

			if (_mirrorForOtherHand)
			{
				offset2 = offset1;
				if ((_mirrorActions & MirrorAction.InvertOffsetX) > 0)
				{
					offset2.x *= -1f;
				}
				if ((_mirrorActions & MirrorAction.InvertOffsetY) > 0)
				{
					offset2.y *= -1f;
				}
				if ((_mirrorActions & MirrorAction.InvertOffsetZ) > 0)
				{
					offset2.z *= -1f;
				}

				var rotX = (_mirrorActions & MirrorAction.InvertRotationX) > 0;
				var rotY = (_mirrorActions & MirrorAction.InvertRotationY) > 0;
				var rotZ = (_mirrorActions & MirrorAction.InvertRotationZ) > 0;
				if (rotX || rotY || rotZ)
				{
					if (rotX)
					{
						orientEulers.x *= -1f;
					}
					if (rotY)
					{
						orientEulers.y *= -1f;
					}
					if (rotZ)
					{
						orientEulers.z *= -1f;
					}
					orient2 = Quaternion.Euler(orientEulers);
				} else
				{
					orient2 = orient1;
				}
			}
		}


#if UNITY_EDITOR
		private void OnDrawGizmosSelected()
		{
			Awake();
			DrawAxes(_primaryHand, 1f);
			if (_mirrorForOtherHand)
			{
				DrawAxes(_primaryHand == Hand.Left ? Hand.Right : Hand.Left, 0.25f);
			}
		}


		private void DrawAxes(Hand hand, float alpha)
		{
			var pos = GetPosition(hand);
			var rot = GetRotation(hand);
			Gizmos.color = new Color(1, 0, 0, alpha);
			Gizmos.DrawLine(pos, pos + (rot * Vector3.right) * 0.05f);
			Gizmos.color = new Color(0, 0, 1, alpha);
			Gizmos.DrawLine(pos, pos + (rot * Vector3.forward) * 0.05f);
			Gizmos.color = new Color(0, 1, 0, alpha);
			Gizmos.DrawLine(pos, pos + (rot * Vector3.up) * 0.05f);
		}
#endif

	}
}