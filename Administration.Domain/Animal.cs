namespace Administration.Domain
{

    public class Animal : Entity
    {
        public string Name { get; init; } // init gør den immutable og readonly
        public Weight Weight { get; init; }
        public EnumExample EnumExample { get; set; }

        public Guid SpeciesId { get; set; }


        public Animal(Guid id, string name, Weight weight, EnumExample enumExample, Guid speciesId)
        {
            Id = id;
            Name = name;

            Weight = weight;
            EnumExample = enumExample;
            SpeciesId = speciesId;
        }
    }
    public enum EnumExample
    {
        EnumValue1,
        EnumValue2,
    }
}
