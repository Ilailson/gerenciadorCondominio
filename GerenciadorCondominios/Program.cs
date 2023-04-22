using GerenciadorCondominios.DAL;
using GerenciadorCondominios.BLL.Models;
using GerenciadorCondominios.DAL.Repositorios;
using GerenciadorCondominios.DAL.Interfaces;
using Microsoft.EntityFrameworkCore;
using GerenciadorCondominios.Extensions;
using Microsoft.AspNetCore.Hosting;

var builder = WebApplication.CreateBuilder(args);


// Add services to the container.
builder.Services.AddControllersWithViews();



builder.Services.AddDbContext<Contexto>
    (options => options.UseMySql(
        "server=localhost;initial catalog=GerenciadorCondominiosDB;uid=root;pwd=",
        Microsoft.EntityFrameworkCore.ServerVersion.Parse("8.0.25-mysql")));

builder.Services.AddIdentity<Usuario, Funcao>().AddEntityFrameworkStores<Contexto>();

builder.Services.AddAuthentication();
builder.Services.AddAuthorization();

//injetando serviços para realizar injeção de dependencias.
builder.Services.AddTransient<IUsuarioRepositorio, UsuarioRepositorio>();
builder.Services.AddTransient<IFuncaoRepositorio, FuncaoRepositorio>();
builder.Services.AddTransient<IVeiculoReposito,VeiculoRepositorio>();
builder.Services.AddTransient<IEventoRepositorio, EventoRepositorio>();


//builder.Services.ConfigurarRepositorios();
//builder.Services.ConfigurarCookies();
//builder.Services.ConfigurarNomeUsuario();
//builder.Services.ConfigurarSenhaUsuario();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

 static IHostBuilder CreateHostBuilder(string[] args) =>
    Host.CreateDefaultBuilder(args)
        .ConfigureWebHostDefaults(webBuilder =>
        {
            webBuilder.ConfigureServices(services =>
            {
                services.ConfigurarRepositorios();
                services.ConfigurarCookies();
                services.ConfigurarNomeUsuario();
                services.ConfigurarSenhaUsuario();
            });
           
        });


app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
