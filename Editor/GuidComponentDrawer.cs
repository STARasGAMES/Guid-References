using UnityEditor;

namespace SaG.GuidReferences.Editor
{
    [CustomEditor(typeof(GuidComponent))]
    public class GuidComponentDrawer : UnityEditor.Editor
    {
        private GuidComponent guidComp;

        public override void OnInspectorGUI()
        {
            if (guidComp == null)
            {
                guidComp = (GuidComponent)target;
            }
       
            // Draw label
            EditorGUILayout.LabelField("Guid:", guidComp.GetGuid().ToString());
        }
    }
}