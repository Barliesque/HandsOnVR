using Barliesque.InspectorTools;
using System;
using UnityEngine;
using UnityEngine.Serialization;

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

		private Vector3 _offsetLeft;
		private Quaternion _orientLeft;
		private Vector3 _offsetRight;
		private Quaternion _orientRight;


		[Tooltip("Which hand was this GrabAnchor placed for?")]
		[SerializeField, SingleSelection] private Hand _primaryHand = Hand.Right;
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
		[SerializeField] private Order _grabOrder = Order.FirstOrSecond;
		public Order GrabOrder { get { return _grabOrder; } }

		[Tooltip("Should this GrabAnchor use a different Grab Pose than specified by the Grabbable component?")]
		[SerializeField] private bool _overrideGrabPose;
		public bool OverrideGrabPose { get { return _overrideGrabPose; } }

		[Tooltip("A bool parameter name found in the Animator components of the player's hands.  While this object is being grabbed, the specified parameter will be set to true.")]
		[SerializeField] protected string _grabPose;
		public int GrabPoseID { get; private set; }

		[Tooltip("Should this GrabAnchor use a different Proximity Pose than specified by the Grabbable component?")]
		[SerializeField] private bool _overrideProximityPose;
		public bool OverrideProximityPose { get { return _overrideProximityPose; } }

		[Tooltip("A bool parameter name found in the Animator components of the player's hands.  While the player's hand is near this object, the specified parameter will be set to true.")]
		[SerializeField] private string _proximityPose;
		public int ProximityPoseID { get; private set; }


		[Tooltip("Should this GrabAnchor have a different OrientToHand setting than specified by the Grabbable component?")]
		[SerializeField] private bool _overrideOrientToHand;
		public bool OverrideOrientToHand { get { return _overrideOrientToHand; } }

		[Tooltip("Should this object be reoriented to meet the hand's orientation?")]
		[SerializeField] private bool _orientToHand = true;
		public bool OrientToHand { get { return _orientToHand; } }


		private Transform _xform;
		private Transform _grabbableXform;

		public Grabbable Grabbable => _grabbable;
		private Grabbable _grabbable;


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

			if (_overrideGrabPose)
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


		[SerializeField] private bool _liveUpdate;  //TODO  Show this property at runtime?  ...or, better, simply call Awake via Message whenever the Method changes!
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
			if (!_grabbableXform) return;
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
			GizmoTools.DrawAxes(pos, rot, alpha);
		}
#endif

		[Obsolete("For internal use only.  Use Grabber.Grab() instead.")]
		public bool TryForceGrab(Grabber grabber, out IGrabAnchor anchor) => Grabbable.ForceGrab(this, grabber, out anchor);

	}
}