using UnityEngine;

static public class LayerMaskExtensions
{

	static public bool Contains(this LayerMask mask, GameObject gameObject)
	{
		return (mask & (1 << gameObject.layer)) != 0;
	}

	static public bool Contains(this LayerMask mask, int layer)
	{
		return (mask & (1 << layer)) != 0;
	}

	static public bool Contains(this LayerMask mask, Collider collider)
	{
		return (mask & (1 << collider.gameObject.layer)) != 0;
	}

}
