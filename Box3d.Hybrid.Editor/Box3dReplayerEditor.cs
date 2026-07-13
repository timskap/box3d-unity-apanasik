using UnityEditor;

namespace Box3d.Hybrid.Editor
{
    /// <summary>Adds a scrub timeline to <see cref="Box3dReplayer"/>.</summary>
    [CustomEditor(typeof(Box3dReplayer))]
    public class Box3dReplayerEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            ReplayTimelineGUI.Draw(this, (Box3dReplayer)target);
        }
    }
}
