namespace CommonAssets
{
    public abstract record Entity : IEquatable<Entity>
    {
        public Guid Id { get; init; } // init gør den immutable og readonly

        public override int GetHashCode()
        {
            return HashCode.Combine(Id);
        }

        #region indbygget i record
        //public bool Equals(Entity other)
        //{
        //    return other?.Id == Id;
        //}
        //public override bool Equals(object? obj)
        //{
        //    return base.Equals(obj);
        //}

        //public static bool operator !=(Entity left, Entity right)
        //{
        //    return EqualityComparer<Entity>.Default.Equals(left, right);
        //}
        //public static bool operator ==(Entity left, Entity right)
        //{
        //    return EqualityComparer<Entity>.Default.Equals(left, right);
        //}
    }
}
