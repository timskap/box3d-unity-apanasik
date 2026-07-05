using UnityEditor;
using UnityEngine;

namespace Box3d.Hybrid.Editor
{
    /// <summary>Shared inspector for the mesh-sourced shapes: draws the default fields, then warns
    /// if the assigned mesh is missing or not Read/Write enabled (the common runtime-empty trap).</summary>
    public abstract class MeshSourceShapeEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            var mesh = serializedObject.FindProperty("Mesh").objectReferenceValue as Mesh;
            if (!mesh)
            {
                EditorGUILayout.HelpBox("Assign a mesh for this shape.", MessageType.Info);
            }
            else if (!mesh.isReadable)
            {
                EditorGUILayout.HelpBox(
                    $"'{mesh.name}' is not Read/Write enabled, so the shape will be empty at runtime. " +
                    "Enable Read/Write in the mesh's import settings.", MessageType.Warning);
            }
        }
    }

    [CustomEditor(typeof(Box3dMeshShape))]
    public class Box3dMeshShapeEditor : MeshSourceShapeEditor { }

    [CustomEditor(typeof(Box3dHullShape))]
    public class Box3dHullShapeEditor : MeshSourceShapeEditor { }
}
