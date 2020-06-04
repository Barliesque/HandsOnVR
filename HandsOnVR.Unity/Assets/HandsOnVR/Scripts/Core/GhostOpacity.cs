using UnityEngine;


namespace HandsOnVR
{

	/// <summary>
	/// Fade ghost hand out as it's position approaches that of the solid version of the hand
	/// </summary>
	public class GhostOpacity : MonoBehaviour
	{
		[SerializeField] private SkinnedMeshRenderer _renderer;
		[SerializeField] private Transform _handSolid;
		[Tooltip("At this distance or closer, the ghost will be completely transparent.")]
		[SerializeField] private float _minDistance = 0.01f;
		[Tooltip("At this distance or further, the ghost will be completely visible.")]
		[SerializeField] private float _maxDistance = 0.5f;

		private Transform _xform;
		private Material _ghostMaterial;
		private float _opacityMultiplier;
		private readonly int _opacityParam = Shader.PropertyToID("_Opacity");

		public float Stretch { get; private set; }

		private void Start()
		{
			_xform = GetComponent<Transform>();
			_opacityMultiplier = _renderer.sharedMaterial.GetFloat(_opacityParam);
			_ghostMaterial = _renderer.material;
		}


		private void Update()
		{
			var dist = (_xform.position - _handSolid.position).magnitude;
			Stretch = Mathf.InverseLerp(_minDistance, _maxDistance, dist);
			var alpha = _opacityMultiplier * Stretch;
			_ghostMaterial.SetFloat(_opacityParam, alpha);
			_renderer.enabled = (alpha > 0.0001f);
		}

	}
}