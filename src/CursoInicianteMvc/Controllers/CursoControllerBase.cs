using Microsoft.AspNetCore.Mvc;

namespace CursoInicianteMvc.Controllers;

public class CursoControllerBase : Controller
{
    public ViewResult NaoEncontrado() => View();
}