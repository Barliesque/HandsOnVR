using UnityEngine;

namespace Barliesque.InspectorTools
{
	static public class GizmoTools
	{
		static private readonly Mesh _quad = new Mesh()
		{
			vertices = new[]
			{
				new Vector3(-0.5f, 0.5f, 0f),
				new Vector3(0.5f, 0.5f, 0f),
				new Vector3(0.5f, -0.5f, 0f),
				new Vector3(-0.5f, -0.5f, 0f),
			},
			normals = new[]
			{
				Vector3.back, Vector3.back, Vector3.back, Vector3.back
			},
			triangles = new[]
			{
				0, 1, 2, 0, 2, 3
			},
			uv = new[]
			{
				new Vector2(0, 1),
				new Vector2(1, 1),
				new Vector2(1, 0),
				new Vector2(0, 0)
			}
		};

		static private readonly Vector3[] _cubeSides =
		{
			Vector3.right, Vector3.up, Vector3.forward, Vector3.left, Vector3.down, Vector3.back
		};

		static private readonly Color[] _axisColors =
		{
			Color.red * 0.75f, Color.green * 0.5f, Color.blue * 0.75f, Color.red, Color.green, Color.blue
		};


		static public void DrawAxes(Vector3 pos, Quaternion rot, float alpha = 1f)
		{
			Gizmos.color = new Color(1, 0, 0, alpha);
			Gizmos.DrawLine(pos, pos + (rot * Vector3.right) * 0.05f);
			Gizmos.color = new Color(0, 1, 0, alpha);
			Gizmos.DrawLine(pos, pos + (rot * Vector3.up) * 0.05f);
			Gizmos.color = new Color(0, 0, 1, alpha);
			Gizmos.DrawLine(pos, pos + (rot * Vector3.forward) * 0.05f);
		}

		static public void DrawAxes(Vector3 pos, Quaternion rot, float alphaX, float alphaY, float alphaZ)
		{
			Gizmos.color = new Color(1, 0, 0, alphaX);
			Gizmos.DrawLine(pos, pos + (rot * Vector3.right) * 0.05f);
			Gizmos.color = new Color(0, 1, 0, alphaY);
			Gizmos.DrawLine(pos, pos + (rot * Vector3.up) * 0.05f);
			Gizmos.color = new Color(0, 0, 1, alphaZ);
			Gizmos.DrawLine(pos, pos + (rot * Vector3.forward) * 0.05f);
		}


		static public void DrawRotatedCube(Vector3 pos, Quaternion rot, Vector3 size)
		{
			var scale = new[]
			{
				new Vector3(size.x, size.y, 1f),
				new Vector3(size.z, size.y, 1f),
				new Vector3(size.x, size.z, 1f)
			};
			for (var i = 0; i < 6; i++)
			{
				var side = _cubeSides[i];
				var orientation = rot * Quaternion.LookRotation(side);

				var r = side;
				r.Scale(size);
				var radius = Mathf.Abs(r.x + r.y + r.z) * 0.5f;

				Gizmos.DrawMesh(_quad, pos + (orientation * (Vector3.back * radius)), orientation, scale[i % 3]);
			}
		}


		static public void DrawRotatedCube(Vector3 pos, Quaternion rot, float size, bool useAxisColors = false)
		{
			var radius = size * 0.5f;
			var scale = Vector3.one * size;

			for (var i = 0; i < _cubeSides.Length; i++)
			{
				var side = _cubeSides[i];
				if (useAxisColors) Gizmos.color = _axisColors[i];
				var orientation = rot * Quaternion.LookRotation(side);

				Gizmos.DrawMesh(_quad, pos + (orientation * (Vector3.back * radius)), orientation, scale);
			}
		}
	}
}