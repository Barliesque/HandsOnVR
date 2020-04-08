using System.Collections.Generic;
using Barliesque.InspectorTools.Editor;
using UnityEditor;
using UnityEngine;


namespace HandsOnVR.Editor
{
	[CustomEditor(typeof(Sensor))]
	public class SensorEditor : EditorBase<Sensor>
	{
		private readonly List<Collider> _colliders = new List<Collider>();
		
		override protected void CustomInspector(Sensor inst)
		{
			inst.GetComponents<Collider>(_colliders);
			bool body = inst.GetComponent<Rigidbody>();
			bool gotTrigger = false;
			bool gotCollision = false;
			for (int i = 0; i < _colliders.Count; i++)
			{
				gotTrigger |= (_colliders[i].isTrigger);
				gotCollision |= (!_colliders[i].isTrigger);
			}

			if (!body || _colliders.Count == 0)
			{
				EditorTools.HelpBox("Sensor can only respond if Colliders and a Rigidbody are on <b>this</b> GameObject.", MessageType.Error);
			}
			else
			{
				if (gotTrigger && gotCollision)
				{
					EditorTools.HelpBox("This Sensor will invoke all Trigger and Collision events because there is a mix of solid and trigger colliders.", MessageType.Info);
				}
				else
				{
					if (gotTrigger)
					{
						EditorTools.HelpBox("This Sensor will invoke events: <b>OnEnterTrigger</b>, <b>OnExitTrigger</b> and <b>OnStayTrigger</b>", MessageType.Info);
					}
					else if (gotCollision)
					{
						EditorTools.HelpBox("This Sensor will invoke events: <b>OnEnterCollision</b>, <b>OnExitCollision</b> and <b>OnStayCollision</b>", MessageType.Info);
					} 
				}
			}
		}
	}
}