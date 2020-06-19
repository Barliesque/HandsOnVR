using UnityEngine;


namespace HandsOnVR
{

	/// <summary>
	/// When the player's hand comes within range of an object (defined by a trigger collider) this component changes the hand's pose.
	/// </summary>
	public class PoseTrigger : MonoBehaviour
    {

		[Tooltip("A bool parameter name found in the Animator components of the player's hands.  While the player's hand is near this object, the specified parameter will be set to true.")]
		[SerializeField] private string _proximityPose;
		public string ProximityPose => _proximityPose;

		private int _proximityPoseID;
		public int ProximityPoseID
		{
			get => _proximityPoseID;
			private set => _proximityPoseID = value;
		}

		[Tooltip("Should the right hand respond to this PoseTrigger?")]
		[SerializeField] private bool _rightHand = true;

		[Tooltip("Should the left hand respond to this PoseTrigger?")]
		[SerializeField] private bool _leftHand = true;

		public Collider Collider { get; private set; }


		private void Awake()
		{
			Collider = GetComponent<Collider>();
			ProximityPoseID = Animator.StringToHash(_proximityPose);
		}

		public bool SupportsHand(Hand hand)
		{
			return (hand == Hand.Left) ? _leftHand : _rightHand;
		}


		//TODO  The Grabber needs to gather trigger colliders as it does for Grabbables, responding with the same logic
		//TODO  The editor of this component should show a warning info box that it requires a trigger collider to function

	}

}