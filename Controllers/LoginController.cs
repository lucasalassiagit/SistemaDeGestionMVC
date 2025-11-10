using CrudMVCApp.Data;
using CrudMVCApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Session;
using Microsoft.EntityFrameworkCore;

namespace CrudMVCApp.Controllers
{
    public class LoginController : Controller
    {
        private readonly AppDbContext _db;

        public LoginController(AppDbContext db)
        {
            _db = db;
        }


        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }
        

        [HttpPost]
        [ValidateAntiForgeryToken] // Protege contra ataques CSRF (Cross-Site Request Forgery)
                                   // Se valida el token que se genera desde la vista
        public IActionResult Login(Usuario usuario)
        {

            // Deshabilitar validación requerida para Tipo solo para esta acción
            ModelState.Remove("Tipo");

            if (!ModelState.IsValid)
            {
                return View();
            }

            var dbUsuario = _db.Usuario
                .AsEnumerable() // Cargar todos los usuarios en memoria y se compara mayusculas y minúsculas
                .FirstOrDefault(u => u.user == usuario.user && u.Clave == usuario.Clave);

            
            if (dbUsuario != null && dbUsuario.Activo == true)
            {
                HttpContext.Session.SetString("Usuario", dbUsuario.user);
                HttpContext.Session.SetString("Tipo", dbUsuario.Tipo);
                HttpContext.Session.SetString("UsuarioId", dbUsuario.Id.ToString());
                //HttpContext.Session.SetString: acepta solo string por eso el Id se convierte;

                return RedirectToAction("Index", "Home");
            }

            ViewBag.ErrorMessage = "Usuario o contraseña incorrecto";
            return View();

        }



        [HttpGet]
        public ActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index", "Home");
        }
    }
}
