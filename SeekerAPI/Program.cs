using System.Text.Json.Serialization;
using DAL;
using Interfaces;
using SeekerAPI;
using SeekerAPI.Services;
using SeekHandler;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddControllers().AddJsonOptions( options =>
{
    options.JsonSerializerOptions.Converters.Add( new JsonStringEnumConverter() );
});;
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddTransient<IJobRepository, JobRepository>();
builder.Services.AddTransient<DbContext, DbContext>();
builder.Services.AddTransient<FilterRepository, FilterRepository>();
builder.Services.AddSingleton<IServiceStateService, ServiceStateService>();


builder.Services.AddTransient<JobRetriever, JobRetriever>();
builder.Services.AddTransient<JobRequestRepository, JobRequestRepository>();



builder.Services.AddCors();

builder.Services.AddSwaggerGen(c => {
    c.UseInlineDefinitionsForEnums();
});

var app = builder.Build();

/* builder.Services.AddTransient<JobRetriever, JobRetriever>();
builder.Services.AddTransient<JobRequestRepository, JobRequestRepository>();
*/

app.UseSwagger();
app.UseSwaggerUI(x => { x.SwaggerEndpoint("/swagger/v1/swagger.yaml", "Seeker Dashboard API"); });

app.UseCors(x => x
    .AllowAnyMethod()
    .AllowAnyHeader()
    .SetIsOriginAllowed(origin => true) // allow any origin
    //.WithOrigins("https://localhost:44351")); // Allow only this origin can also have multiple origins separated with comma
    .AllowCredentials()); // allow credentials

app.UseHttpsRedirection();

app.MapControllers();

app.Run();
