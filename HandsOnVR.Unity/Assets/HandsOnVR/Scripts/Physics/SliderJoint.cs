using Barliesque.InspectorTools;
using UnityEngine;


namespace HandsOnVR
{
	/// <summary>
	/// At runtime, this component generates a Prismatic Joint -- a ConfigurableJoint with its numerous properties automatically set up to act as a linear sliding joint.
	/// </summary>
	/// https://gamedev.stackexchange.com/questions/129659/how-do-i-configure-a-joint-for-a-sliding-door-in-unity
	[RequireComponent(typeof(Rigidbody))]
	public class SliderJoint : MonoBehaviour, IJoint
	{
		[Tooltip("A reference to another rigidbody this joint connects to.")]
		public Rigidbody ConnectedBody;

		[Tooltip("The axis in local space along which this joint allows movement.")]
		[EnumPopup] public Axis AxisOfMovement = Axis.X;

		[Tooltip("The local position at which this body is connected to the other.")]
		public Vector3 AnchorPosition;

		[Tooltip("The distance in local space the slider can move.")]
		public float Limit;

		[Tooltip("Should the joint allow rotation about the axis of movement?")]
		public ConfigurableJointMotion Rotation = ConfigurableJointMotion.Locked;

		[Tooltip("Lower limit of rotation about the axis of movement.")]
		public float RotationLimitLow;

		[Tooltip("Upper limit of rotation about the axis of movement.")]
		public float RotationLimitHigh;

		ConfigurableJoint _joint;
		Transform _connectedXform;
		Rigidbody _body;


		private void Awake()
		{
			_joint = gameObject.AddComponent<ConfigurableJoint>();
			UpdateJoint();
		}

		public void UpdateJoint()
		{
			_joint.connectedBody = ConnectedBody;
			_connectedXform = ConnectedBody?.transform ?? null;

			_joint.autoConfigureConnectedAnchor = false;

			_joint.xMotion = (HasAxis(Axis.X) ? ConfigurableJointMotion.Limited : ConfigurableJointMotion.Locked);
			_joint.yMotion = (HasAxis(Axis.Y) ? ConfigurableJointMotion.Limited : ConfigurableJointMotion.Locked);
			_joint.zMotion = (HasAxis(Axis.Z) ? ConfigurableJointMotion.Limited : ConfigurableJointMotion.Locked);

			_joint.angularXMotion = (HasAxis(Axis.X) ? Rotation : ConfigurableJointMotion.Locked);
			_joint.angularYMotion = (HasAxis(Axis.Y) ? Rotation : ConfigurableJointMotion.Locked);
			_joint.angularZMotion = (HasAxis(Axis.Z) ? Rotation : ConfigurableJointMotion.Locked);

			_joint.lowAngularXLimit = new SoftJointLimit() { limit = RotationLimitLow };
			_joint.highAngularXLimit = new SoftJointLimit() { limit = RotationLimitHigh };

			var sign = Mathf.Sign(Limit);
			_joint.linearLimit = new SoftJointLimit() { limit = Mathf.Abs(Limit * 0.5f) };

			var axis = (HasAxis(Axis.X) ? Vector3.right : (HasAxis(Axis.Y) ? Vector3.up : Vector3.forward)) * sign;
			_joint.axis = Vector3.right;
			_joint.anchor = AnchorPosition - (axis * Limit * 0.5f);

			_body = GetComponent<Rigidbody>();
			_body.constraints = RigidbodyConstraints.None;
		}


		bool HasAxis(Axis axis)
		{
			return (AxisOfMovement & axis) == axis;
		}

		public Transform ConnectedTo { 
			get => _connectedXform; 
			set {
				_connectedXform = value;
				ConnectedBody = value?.GetComponent<Rigidbody>() ?? null;
				UpdateJoint();
			} 
		}

	}
}