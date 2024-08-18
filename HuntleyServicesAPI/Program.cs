using HuntleyServicesAPI.Configuration;
using HuntleyServicesAPI.Configuration.Options;
using HuntleyWeb.Application.Configuration;

var builder = WebApplication.CreateBuilder(args);

//var configuration = builder.Configuration;
IConfiguration configuration = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    //.AddJsonFile("appsettings.json", optional: false)
    .AddEnvironmentVariables()
    .Build();

var appInsights = configuration.GetSection("ApplicationInsights");

var appInsightsInstrumentationKey = appInsights.GetValue<string>("InstumentationKey");
    
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddApplicationInsightsTelemetry(appInsightsInstrumentationKey);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var applicationConfiguration = new ApplicationConfiguration();

var mailOptions = MailOptionsCreator.Create(applicationConfiguration);
var bookingRatesDbOptions = BookingsRatesDbOptionsCreator.Create(applicationConfiguration);
var bookingsDbOptions = BookingsDbOptionsCreator.Create(applicationConfiguration);

builder.Services.AddApplication(mailOptions);
builder.Services.AddBookingRateRepository(bookingRatesDbOptions);
builder.Services.AddBookingsRepository(bookingsDbOptions);
builder.Services.AddFunctionServices();

var app = builder.Build();

// Configure the HTTP request pipeline JH.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();
