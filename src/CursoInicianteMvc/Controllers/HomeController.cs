using System.Diagnostics;
using CursoInicianteMvc.Data;
using Microsoft.AspNetCore.Mvc;
using CursoInicianteMvc.Models;
using Microsoft.EntityFrameworkCore;

namespace CursoInicianteMvc.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly CursoInicianteContexto _contexto;

    public HomeController(ILogger<HomeController> logger, CursoInicianteContexto contexto)
    {
        _logger = logger;
        _contexto = contexto;
    }

    public async Task<IActionResult> Index()
    {
        var inicio = new InicioViewModel()
        {
            QuantidadePessoas = await _contexto.Pessoa.CountAsync(),
            QuantidadeTarafas = await _contexto.Tarefa.CountAsync()
        };
        
        return View(inicio);
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}