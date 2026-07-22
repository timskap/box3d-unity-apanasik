using UnityEditor;
using UnityEngine;

namespace Box3D.Hybrid.Editor
{
    /// <summary>Rope inspector, modeled on Source 2's Hammer cable workflow: the Scene view always
    /// shows the settled hang while editing (move either end and it re-drapes), **Simulate in
    /// Editor** animates it live, **Bake Current Shape** freezes the curve for Baked mode, and
    /// **Make Dynamic** reverts. When End Point is empty the far end gets a draggable handle.</summary>
    [CustomEditor(typeof(Box3DRope))]
    public class Box3DRopeEditor : UnityEditor.Editor
    {
        private Vector3[] _nodes;
        private Vector3[] _previous;
        private bool _simulating;
        private Vector3 _lastStart;
        private Vector3 _lastEnd;

        private Box3DRope Rope => (Box3DRope)target;

        private void OnEnable()
        {
            EditorApplication.update += OnEditorUpdate;
            Undo.undoRedoPerformed += RefreshPreview;
            RefreshPreview();
        }

        private void OnDisable()
        {
            EditorApplication.update -= OnEditorUpdate;
            Undo.undoRedoPerformed -= RefreshPreview;
            _simulating = false;
        }

        public override void OnInspectorGUI()
        {
            EditorGUI.BeginChangeCheck();
            DrawDefaultInspector();
            if (EditorGUI.EndChangeCheck() && !_simulating) RefreshPreview();

            if (Application.isPlaying) return;
            EditorGUILayout.Space();

            if (Rope.CurrentMode == Box3DRope.RopeMode.Dynamic)
            {
                bool simulate = GUILayout.Toggle(_simulating,
                    _simulating ? "■ Stop Editor Simulation" : "▶ Simulate in Editor", GUI.skin.button);
                if (simulate != _simulating)
                {
                    _simulating = simulate;
                    if (!_simulating) RefreshPreview();
                }
                if (GUILayout.Button("Bake Current Shape"))
                {
                    Undo.RecordObject(Rope, "Bake Box3D Rope");
                    Rope.Bake(_nodes ?? Rope.ComputeSettledPoints());
                    EditorUtility.SetDirty(Rope);
                    _simulating = false;
                    RefreshPreview();
                }
                EditorGUILayout.HelpBox("Dynamic: simulated in game as capsule segments + ball joints. " +
                    "Bake to freeze the current hang into a static cable instead.", MessageType.None);
            }
            else
            {
                if (GUILayout.Button("Make Dynamic (clear bake)"))
                {
                    Undo.RecordObject(Rope, "Un-bake Box3D Rope");
                    Rope.ClearBake();
                    EditorUtility.SetDirty(Rope);
                    RefreshPreview();
                }
                EditorGUILayout.HelpBox("Baked: a frozen curve — no simulation in game, optional static collision.",
                    MessageType.None);
            }
        }

        private void OnSceneGUI()
        {
            if (Application.isPlaying) return;

            // A draggable far-end handle when no End Point transform is assigned.
            SerializedProperty endPoint = serializedObject.FindProperty("EndPoint");
            if (!endPoint.objectReferenceValue)
            {
                SerializedProperty offset = serializedObject.FindProperty("EndOffset");
                Vector3 world = Rope.transform.TransformPoint(offset.vector3Value);
                EditorGUI.BeginChangeCheck();
                world = Handles.PositionHandle(world, Quaternion.identity);
                if (EditorGUI.EndChangeCheck())
                {
                    offset.vector3Value = Rope.transform.InverseTransformPoint(world);
                    serializedObject.ApplyModifiedProperties();
                }
            }

            // Re-drape when either endpoint is moved.
            if (!_simulating && (_lastStart != Rope.StartWorld || _lastEnd != Rope.EndWorld))
            {
                RefreshPreview();
            }
        }

        private void OnEditorUpdate()
        {
            if (!_simulating || Application.isPlaying || !Rope) return;
            if (_nodes == null || _nodes.Length != RopeNodeCount()) RefreshPreview();

            // Two verlet steps per editor tick: lively but stable, endpoints tracked live.
            for (int i = 0; i < 2; i++)
            {
                Box3DRope.StepCurve(_nodes, _previous, Rope.StartWorld, Rope.EndWorld,
                    Rope.SettledSegmentLength(), Rope.SceneGravity(), 1f / 60f);
            }
            Rope.ApplyToLine(_nodes);
            SceneView.RepaintAll();
        }

        private int RopeNodeCount()
        {
            return serializedObject.FindProperty("Segments").intValue + 1;
        }

        private void RefreshPreview()
        {
            if (!Rope || Application.isPlaying) return;
            _lastStart = Rope.StartWorld;
            _lastEnd = Rope.EndWorld;

            if (Rope.CurrentMode == Box3DRope.RopeMode.Baked && Rope.HasBake)
            {
                Rope.ApplyToLine(Rope.BakedToWorld());
                return;
            }
            _nodes = Rope.ComputeSettledPoints();
            _previous = (Vector3[])_nodes.Clone();
            Rope.ApplyToLine(_nodes);
        }
    }
}
