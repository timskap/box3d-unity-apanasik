using System.Collections.Generic;
using Box3d.Hybrid;
using UnityEngine;

/// <summary>Test harness for the MonoBehaviour component layer: buttons spawn primitives carrying
/// <see cref="Box3dBody"/> + a shape component (added at runtime), which fall onto the scene's
/// static floor. Proves the hybrid layer works both scene-authored (the floor) and runtime-added.</summary>
public class HybridSpawner : MonoBehaviour
{
    [SerializeField, Tooltip("Material applied to spawned objects (random tint per object).")]
    private Material BaseMaterial;

    [SerializeField, Range(1, 30), Tooltip("Objects added per button press.")]
    private int SpawnCount = 5;

    [SerializeField, Range(0f, 1f), Tooltip("Bounciness of spawned objects. Restitution combines as max, so this alone makes them bounce off the floor.")]
    private float Bounciness = 0.5f;

    private readonly List<GameObject> _spawned = new List<GameObject>();

    private void Spawn(PrimitiveType primitive, bool box)
    {
        for (int i = 0; i < SpawnCount; i++)
        {
            // Inactive during setup so Box3dBody.Awake (which gathers shapes) runs AFTER the
            // shape component is added.
            GameObject visual = GameObject.CreatePrimitive(primitive);
            visual.SetActive(false);
            Destroy(visual.GetComponent<Collider>());
            visual.transform.position = new Vector3(
                Random.Range(-3f, 3f), Random.Range(6f, 11f), Random.Range(-3f, 3f));
            visual.transform.rotation = Random.rotation;

            if (BaseMaterial)
            {
                visual.GetComponent<MeshRenderer>().material =
                    new Material(BaseMaterial) { color = Color.HSVToRGB(Random.value, 0.55f, 1f) };
            }

            Box3dShape shape = box
                ? visual.AddComponent<Box3dBoxShape>()
                : (Box3dShape)visual.AddComponent<Box3dSphereShape>();
            shape.SetRestitution(Bounciness); // set before the body bakes the shape on activation
            visual.AddComponent<Box3dBody>();

            visual.SetActive(true);
            _spawned.Add(visual);
        }
    }

    private void Clear()
    {
        foreach (GameObject go in _spawned)
        {
            if (go) Destroy(go); // Box3dBody.OnDisable destroys the native body
        }
        _spawned.Clear();
    }

    private void OnGUI()
    {
        GUILayout.BeginArea(new Rect(10f, 10f, 220f, 235f), GUI.skin.box);
        GUILayout.Label("<b>Component layer test</b>", new GUIStyle(GUI.skin.label) { richText = true });

        GUILayout.Label($"Count per press: {SpawnCount}");
        SpawnCount = (int)GUILayout.HorizontalSlider(SpawnCount, 1f, 30f);

        GUILayout.Label($"Bounciness: {Bounciness:F2}");
        Bounciness = GUILayout.HorizontalSlider(Bounciness, 0f, 1f);

        GUILayout.Space(6f);
        if (GUILayout.Button($"Spawn {SpawnCount} spheres")) Spawn(PrimitiveType.Sphere, box: false);
        if (GUILayout.Button($"Spawn {SpawnCount} boxes")) Spawn(PrimitiveType.Cube, box: true);
        if (GUILayout.Button("Clear")) Clear();

        GUILayout.FlexibleSpace();
        GUILayout.Label($"Live objects: {_spawned.Count}");
        GUILayout.EndArea();
    }
}
