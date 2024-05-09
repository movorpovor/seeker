using DAL;
using SeekerAPI.Services;
using SeekHandler;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddTransient<JobRepository, JobRepository>();
builder.Services.AddTransient<DbContext, DbContext>();
builder.Services.AddSingleton<ServiceStateService, ServiceStateService>();


builder.Services.AddTransient<JobRetriever, JobRetriever>();
builder.Services.AddTransient<JobRequestRepository, JobRequestRepository>();



builder.Services.AddCors();

var app = builder.Build();


/* builder.Services.AddTransient<JobRetriever, JobRetriever>();
builder.Services.AddTransient<JobRequestRepository, JobRequestRepository>();
*/

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(x => { x.SwaggerEndpoint("/swagger/v1/swagger.yaml", "Seeker Dashboard API"); });
}

app.UseCors(x => x
    .AllowAnyMethod()
    .AllowAnyHeader()
    .SetIsOriginAllowed(origin => true) // allow any origin
    //.WithOrigins("https://localhost:44351")); // Allow only this origin can also have multiple origins separated with comma
    .AllowCredentials()); // allow credentials

app.UseHttpsRedirection();

app.MapControllers();

app.Run();
