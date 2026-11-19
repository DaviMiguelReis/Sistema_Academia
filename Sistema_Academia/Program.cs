using Microsoft.EntityFrameworkCore;
using Sistema_Academia.Data;
using Sistema_Academia.Repositories;
using Sistema_Academia.Repositories.Interfaces;
using Sistema_Academia.Services;
using Sistema_Academia.Services.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

//Conexão com Banco de Dados

var connectionString = builder.Configuration.GetConnectionString("AcademiaConnection")?? throw new InvalidOperationException("A conexão AcademiaConnection não foi configurada.");

builder.Services.AddDbContext<AcademiaContext>(options =>options.UseSqlServer(connectionString));

builder.Services.AddScoped<IAlunoRepository, AlunoRepository>();
builder.Services.AddScoped<IPlanoRepository, PlanoRepository>();
builder.Services.AddScoped<IModalidadeRepository, ModalidadeRepository>();
builder.Services.AddScoped<IMatriculaRepository, MatriculaRepository>();
builder.Services.AddScoped<IPagamentoRepository, PagamentoRepository>();

// Services
builder.Services.AddScoped<IAlunoService, AlunoService>();
builder.Services.AddScoped<IPlanoService, PlanoService>();
builder.Services.AddScoped<IModalidadeService, ModalidadeService>();
builder.Services.AddScoped<IMatriculaService, MatriculaService>();
builder.Services.AddScoped<IPagamentoService, PagamentoService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Alunos}/{action=Index}/{id?}");

app.Run();
