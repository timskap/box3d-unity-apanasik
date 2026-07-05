using UnityEngine;

namespace Box3d.Hybrid
{
    /// <summary>A filter joint: applies no constraint, it just disables collision between this body
    /// and its connected body (they stay in the same simulation island). Handy for bodies that
    /// overlap by design.</summary>
    [AddComponentMenu("Box3d/Box3d Filter Joint")]
    public class Box3dFilterJoint : Box3dJoint
    {
        protected override Joint CreateJoint(BodyId bodyA, BodyId bodyB)
        {
            FilterJointDef def = FilterJointDef.Default;
            def.Base.BodyIdA = bodyA;
            def.Base.BodyIdB = bodyB;
            return World.World.CreateFilterJoint(def);
        }
    }
}
