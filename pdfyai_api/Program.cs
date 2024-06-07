using Microsoft.EntityFrameworkCore;
using pdfyai.Modules;
using pdfyai_api.Data;
using pdfyai_api.Extensions;
using pdfyai_api.Modules;

var builder = WebApplication.CreateBuilder(args);





builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"));
});



builder.Services.AddAuthServices(builder.Configuration);
builder.Services.AddAppServices(builder.Configuration);



builder.Services.AddSwaggerGen();

builder.Services.AddAuthorization();




builder.Services.AddEndpointsApiExplorer();




builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.WithOrigins("http://localhost:4200")
        .AllowAnyMethod()
        .AllowAnyHeader()
        .AllowCredentials();
    });
});

var app = builder.Build();



// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors();
app.UseAuthentication();
app.UseAuthorization();





app.AddPdfModule(builder.Configuration);

app.AddAuthModule(builder.Configuration);

app.AddChatModule();
app.AddEmailModule();


app.AddPaymentModule(builder.Configuration);


app.Run();


