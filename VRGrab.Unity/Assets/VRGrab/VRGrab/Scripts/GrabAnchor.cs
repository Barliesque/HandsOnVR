using UnityEngine;


namespace Barliesque.VRGrab
{
	/// <summary>
	/// A component to refine grabbing of objects.  Multiple anchors can be used to specify a variety of anchor points and orientations.
	/// </summary>
	public class GrabAnchor : MonoBehaviour
	{
		//TODO  Consider adding a public flag that indicates if this anchor is being grabbed
		//TODO  Add a max radius property

		public enum Hand
		{
			None = 0,
			Left = 1,
			Right = 2,
			Either = 3
		}

		public enum Order
		{
			FirstOrSecond = 0,
			FirstOnly = 1,
			SecondOnly = 2
		}

		[Tooltip("Restrict which hand can grab the object from this GrabAnchor?")]
		[SerializeField] Hand _allowHand = Hand.Either;
		public Hand AllowHand { get { return _allowHand; } }

		[Tooltip("Restrict whether this anchor may be grabbed first, or second?")]
		[SerializeField] Order _grabOrder = Order.FirstOrSecond;
		public Order GrabOrder { get { return _grabOrder; } }

		[Tooltip("Should this GrabAnchor use a different Grab Pose than specified by Grabbable?")]
		[SerializeField] bool _overridePose;
		public bool OverridePose { get { return _overridePose; } }

		[Tooltip("A bool parameter name found in the Animator components of the player's hands.  While this object is being grabbed, the specified parameter will be set to true.")]
		[SerializeField] string _grabPose;
		public int GrabPoseID { get; private set; }

		[Tooltip("Override the OrientToHand setting in this object's Grabbable component?")]
		[SerializeField] bool _overrideOrientToHand;
		public bool OverrideOrientToHand { get { return _overrideOrientToHand; } }

		[Tooltip("Should this object be reoriented to meet the hand's orientation?")]
		[SerializeField] bool _orientToHand = true;
		public bool OrientToHand { get { return _orientToHand; } }


		private void Awake()
		{
			if (_overridePose)
			{
				GrabPoseID = Animator.StringToHash(_grabPose);
			}
		}

	}
}