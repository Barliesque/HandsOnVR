using UnityEngine;

namespace Barliesque.InspectorTools
{
	public class GizmoTools
	{
		static public void DrawAxes(Vector3 pos, Quaternion rot, float alpha = 1f)
		{
			Gizmos.color = new Color(1, 0, 0, alpha);
			Gizmos.DrawLine(pos, pos + (rot * Vector3.right) * 0.05f);
			Gizmos.color = new Color(0, 0, 1, alpha);
			Gizmos.DrawLine(pos, pos + (rot * Vector3.forward) * 0.05f);
			Gizmos.color = new Color(0, 1, 0, alpha);
			Gizmos.DrawLine(pos, pos + (rot * Vector3.up) * 0.05f);
		}
	}
}