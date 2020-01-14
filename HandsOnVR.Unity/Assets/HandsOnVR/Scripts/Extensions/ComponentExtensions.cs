using System.Collections;
using UnityEngine;


namespace HandsOnVR
{

	static public class ComponentExtensions
	{

		static public T GetComponentInParent<T>(this Component component, bool includeInactive) where T : Component
		{
			var here = component.transform;
			T result = null;
			while (here && !result)
			{
				if (includeInactive || here.gameObject.activeSelf)
				{
					result = here.GetComponent<T>();
				}
				here = here.parent;
			}
			return result;
		}

	}

}