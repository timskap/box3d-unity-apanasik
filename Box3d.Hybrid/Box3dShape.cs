using Unity.Mathematics;
using UnityEngine;

namespace Box3d.Hybrid
{
    /// <summary>Base class for shape components, analogous to Unity's Collider. Attaches to the
    /// <see cref="Box3dBody"/> on the same GameObject when that body is created. Friction and
    /// restitution can be changed at runtime; density is baked at creation (it changes mass).</summary>
    public abstract class Box3dShape : MonoBehaviour
    {
        [SerializeField, Min(0f), Tooltip("Density in kg/m³ (mass = density × volume). Baked at creation.")]
        private float Density = 1000f;

        [SerializeField, Range(0f, 1f), Tooltip("Coulomb friction coefficient.")]
        private float Friction = 0.6f;

        [SerializeField, Range(0f, 1f), Tooltip("Bounciness. Only applies above the world's restitution speed threshold (~1 m/s), so gentle settling never bounces.")]
        private float Restitution;

        [SerializeField, Tooltip("Local offset of the shape from the body origin.")]
        private Vector3 Center = Vector3.zero;

        private Shape _shape;

        protected float3 LocalCenter => Center;

        /// <summary>Sets friction, updating the live shape if it exists.</summary>
        public void SetFriction(float value)
        {
            Friction = value;
            if (_shape.IsValid) _shape.SetFriction(value);
        }

        /// <summary>Sets bounciness, updating the live shape if it exists.</summary>
        public void SetRestitution(float value)
        {
            Restitution = value;
            if (_shape.IsValid) _shape.SetRestitution(value);
        }

        /// <summary>A shape definition seeded from this component's material fields.</summary>
        protected ShapeDef BuildDef()
        {
            ShapeDef def = ShapeDef.Default;
            def.Density = Density;
            def.BaseMaterial.Friction = Friction;
            def.BaseMaterial.Restitution = Restitution;
            return def;
        }

        internal void AttachTo(Body body, float3 scale)
        {
            _shape = CreateShape(body, scale);
        }

        /// <summary>Creates the native shape on the given body. <paramref name="scale"/> is the
        /// owning GameObject's lossy scale, to bake into the shape dimensions.</summary>
        protected abstract Shape CreateShape(Body body, float3 scale);

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (!Application.isPlaying || !_shape.IsValid) return;
            _shape.SetFriction(Friction);
            _shape.SetRestitution(Restitution);
        }
#endif
    }
}
