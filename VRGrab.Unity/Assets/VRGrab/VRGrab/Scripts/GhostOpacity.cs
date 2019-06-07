using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Barliesque.VRGrab
{

	/// <summary>
	/// Fade ghost hand out as it's position approaches that of the solid version of the hand
	/// </summary>
	public class GhostOpacity : MonoBehaviour
	{
		[SerializeField] SkinnedMeshRenderer _renderer;
		[SerializeField] Transform _handSolid;
		[Tooltip("At this distance or closer, the ghost will be completely transparent.")]
		[SerializeField] float _minDistance = 0.01f;
		[Tooltip("At this distance or further, the ghost will be completely visible.")]
		[SerializeField] float _maxDistance = 0.5f;

		Transform _xform;
		Material _ghostMaterial;
		float _opacityMultiplier;
		int _opacityParam = Shader.PropertyToID("_Opacity");


		void Start()
		{
			_xform = GetComponent<Transform>();
			_opacityMultiplier = _renderer.sharedMaterial.GetFloat(_opacityParam);
			_ghostMaterial = _renderer.material;
		}


		void Update()
		{
			var dist = (_xform.position - _handSolid.position).magnitude;
			var t = Mathf.InverseLerp(_minDistance, _maxDistance, dist);
			_ghostMaterial.SetFloat(_opacityParam, _opacityMultiplier * t);
		}

	}
}