using UnityEngine;


namespace Barliesque.VRGrab
{
	/// <summary>
	/// Copies the world position and/or orientation from another transform to the local transform.
	/// </summary>
	[ExecuteInEditMode]
	public class MatchTransform : MonoBehaviour
	{

		[SerializeField] Transform _copyFrom;
		[SerializeField] bool _matchPosition = true;
		[SerializeField] bool _matchRotation = true;

		Transform _xform;

		private void Start()
		{
			_xform = GetComponent<Transform>();
		}

		void Update()
		{
			if (_copyFrom == null)
			{
				return;
			}

			if (_matchPosition)
			{
				_xform.position = _copyFrom.position;
			}

			if (_matchRotation)
			{
				_xform.rotation = _copyFrom.rotation;
			}
		}


	}
}