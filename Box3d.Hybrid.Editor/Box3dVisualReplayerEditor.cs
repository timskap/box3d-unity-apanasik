using UnityEditor;

namespace Box3d.Hybrid.Editor
{
    /// <summary>Adds a scrub timeline to <see cref="Box3dVisualReplayer"/>.</summary>
    [CustomEditor(typeof(Box3dVisualReplayer))]
    public class Box3dVisualReplayerEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            ReplayTimelineGUI.Draw(this, (Box3dVisualReplayer)target);
        }
    }
}
