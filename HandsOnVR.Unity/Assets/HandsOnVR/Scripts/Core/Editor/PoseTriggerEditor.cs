using Barliesque.InspectorTools.Editor;
using UnityEditor;
using UnityEngine;

namespace HandsOnVR.Editor
{

    [CustomEditor(typeof(PoseTrigger), true)]
    public class PoseTriggerEditor : EditorBase<PoseTrigger>
    {
        override protected void CustomInspector(PoseTrigger inst)
        {
            PropertyField("_proximityPose");

            EditorTools.Header("Hands");
            PropertyField("_leftHand");
            PropertyField("_rightHand");

            var collider = inst.GetComponents<Collider>();
            var body = inst.GetComponentInParent<Rigidbody>();

            if (collider.Length == 0)
            {
                EditorTools.HelpBox("A collider is required on this GameObject to enable the PoseTrigger to function.", MessageType.Error);
            }
            else if (collider.Length > 1)
            {
                EditorTools.HelpBox("Only one collider may be used per PoseTrigger.", MessageType.Warning);
            }
            else if (!collider[0].isTrigger)
            {
                EditorTools.HelpBox("The collider must be set to be a trigger.", MessageType.Warning);
            }
            else if (!body)
            {
                EditorTools.HelpBox("A Rigidbody component is required on this or a parent GameObject.", MessageType.Error);
            }

        }
    }
}