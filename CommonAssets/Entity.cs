namespace CommonAssets
{
    /// <summary>
    /// Implementerer explicit IEquatable<Entity> funktioner så vi kan bevare arvestrukturen. 
    /// En simpel record har samme funktion, men arvestrukturen går tabt. Da classes og records ikke kan arve fra hinanden.
    /// </summary>
    public abstract class Entity : IEquatable<Entity>
    {
        public Guid Id { get; init; } // init gør den immutable og readonly

        public override int GetHashCode()
        {
            return HashCode.Combine(Id);
        }

        public bool Equals(Entity other)
        {
            return other?.Id == Id;
        }
        public override bool Equals(object? obj)
        {
            return base.Equals(obj);
        }

        public static bool operator !=(Entity left, Entity right)
        {
            return EqualityComparer<Entity>.Default.Equals(left, right);
        }
        public static bool operator ==(Entity left, Entity right)
        {
            return EqualityComparer<Entity>.Default.Equals(left, right);
        }
    }
}
