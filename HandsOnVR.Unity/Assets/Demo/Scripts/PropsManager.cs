using UnityEngine;

namespace HandsOnVR.Demo
{
	public class PropsManager : MonoBehaviour
	{
		private class TransformState
		{
			public Vector3 Position;
			public Quaternion Rotation;
		}

		private Grabbable[] _props;
		private TransformState[] _initialState;
		private bool _keepReset;

		private void Start()
		{
			_props = GetComponentsInChildren<Grabbable>();
			_initialState = new TransformState[_props.Length];
			for (int i = 0; i < _props.Length; i++)
			{
				var prop = _props[i].GetComponent<Transform>();
				_initialState[i] = new TransformState()
				{
					Position = prop.position,
					Rotation = prop.rotation
				};
			}
		}

		public void ResetProps()
		{
			for (int i = 0; i < _props.Length; i++)
			{
				var prop = _props[i].Body;
				prop.position = _initialState[i].Position;
				prop.rotation = _initialState[i].Rotation;
				prop.velocity = Vector3.zero;
				prop.angularVelocity = Vector3.zero;
			}
		}

		public void ResetAndHold()
		{
			for (int i = 0; i < _props.Length; i++)
			{
				var prop = _props[i].Body;
				prop.velocity = Vector3.zero;
				prop.angularVelocity = Vector3.zero;
				prop.isKinematic = true;
				prop.position = _initialState[i].Position;
				prop.rotation = _initialState[i].Rotation;
			}
		}

		public void Release()
		{
			for (int i = 0; i < _props.Length; i++)
			{
				var prop = _props[i].Body;
				prop.position = _initialState[i].Position;
				prop.rotation = _initialState[i].Rotation;
				prop.isKinematic = false;
			}
		}
	}
}