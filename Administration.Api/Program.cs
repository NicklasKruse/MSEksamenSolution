using Administration.Api.Workflows;
using Administration.Api.Workflows.Activities;
using Administration.ContextLibrary;
using Administration.Domain.DomainServices;
using Dapr.Workflow;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//dapr
builder.Services.AddControllers().AddDapr();
builder.Services.AddDaprWorkflow(options =>
{
    options.RegisterWorkflow<AnimalWorkflow>();
    options.RegisterActivity<CreateAnimalActivity>();
    options.RegisterActivity<PublishEventActivity>();
    options.RegisterActivity<NotifyActivity>();

    //Compensating
    options.RegisterActivity<CompensateAnimalCreationActivity>();
    options.RegisterActivity<CompensateEventPublishActivity>();
    options.RegisterActivity<CompensateNotificationActivity>();
});


// ved explicit pubsub 
//builder.Services.AddControllers()
//    .AddDapr(config => config
//    .UseGrpcEndpoint($"http://localhost:{daprGrpcPort}"));

builder.Services.AddDbContext<AdministrationDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});

builder.Services.AddScoped<ISpeciesService, SpeciesService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
