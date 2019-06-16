using UnityEngine;


namespace Barliesque.VRGrab
{
	/// <summary>
	/// A component to refine grabbing of objects.  Multiple anchors can be used to specify a variety of anchor points and orientations.
	/// </summary>
	public class GrabAnchor : MonoBehaviour
	{
		[SerializeField] Hand _hand = Hand.Either;
		public enum Hand
		{
			Left = 1,
			Right = 2,
			Either = 3
		}

		//TODO  Add option to override Grabbable's GrabPose
		//TODO  Add option to override Grabbable's OrientToHand setting

		[Tooltip("A bool parameter name found in the Animator components of the player's hands.  While this object is being grabbed, the specified parameter will be set to true.")]
		[SerializeField] string _grabPose;
		public int GrabPoseID { get; private set; }

		[SerializeField] bool _orientToHand = true;
		public bool OrientToHand { get { return _orientToHand; } }

	}
}