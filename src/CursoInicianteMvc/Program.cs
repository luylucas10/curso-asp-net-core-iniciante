using CursoInicianteMvc.Data;
using CursoInicianteMvc.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews().AddRazorRuntimeCompilation();

builder.Services.AddDbContext<CursoInicianteContexto>(o =>
{
    o.UseSqlServer(builder.Configuration.GetConnectionString("Principal"));
    o.EnableDetailedErrors();
    o.EnableSensitiveDataLogging();
});

builder.Services.AddScoped<IPessoaRepository, PessoaRepository>();
builder.Services.AddScoped<IPessoaService, PessoaService>();

builder.Services.AddScoped<ITarefaRepository, TarefaRepository>();
builder.Services.AddScoped<ITarefaService, TarefaService>();

builder.Services.AddScoped<ISubtarefaService, SubtarefaService>();
builder.Services.AddScoped<ISubtarefaRepository, SubtarefaRepository>();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();
app.MapControllerRoute(name: "mvc", pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();