using Administration.Domain.Entities;
using Administration.Domain.ValueObjects;
using CommonAssets;
using Dapr.Workflow;

namespace Administration.Api.Workflows.Activities
{
    public class CreateAnimalActivity : WorkflowActivity<CreateAnimalRequest, Animal>
    {
        public override async Task<Animal> RunAsync(WorkflowActivityContext context, CreateAnimalRequest request)
        {

            var newAnimal = new Animal(
                id: Guid.NewGuid(),
                name: request.Animal.Name,
                Category: request.Animal.Category,
                speciesId: request.SpeciesId
            );
            newAnimal.SetWeight(new Weight(request.Animal.Weight.Value));

            return newAnimal;
        }
    }
    public record CreateAnimalRequest(Animal Animal, SpeciesId SpeciesId);
}
