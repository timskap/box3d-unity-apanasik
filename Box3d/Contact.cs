using System;

namespace Box3d
{
    /// <summary>A persistent contact between two shapes, referenced from contact events.</summary>
    public partial struct Contact : IEquatable<Contact>
    {
        public ContactId Id;

        /// <summary>Contacts are destroyed automatically by simulation and world changes —
        /// always validate ids taken from events before use.</summary>
        public bool IsValid => UnsafeBindings.b3Contact_IsValid(Id);

        public bool Equals(Contact other)
        {
            return Id.Equals(other.Id);
        }

        public override bool Equals(object obj)
        {
            return obj is Contact other && Equals(other);
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }

        public static bool operator ==(Contact left, Contact right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(Contact left, Contact right)
        {
            return !left.Equals(right);
        }
    }
}
