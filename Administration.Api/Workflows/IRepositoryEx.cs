using Administration.Domain.Entities;


namespace Administration.Api.Workflows
{
    /// <summary>
    /// Repo eksempel
    /// </summary>
    public interface IRepositoryEx
    {
        Task<Animal> GetByIdAsync(Guid id);
        Task DeleteAsync(Guid id);

        Task<Animal> CreateAsync(Animal animal);// til CreateAnimalActivity
    }
}


