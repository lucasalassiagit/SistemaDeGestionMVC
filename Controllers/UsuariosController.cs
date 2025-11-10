using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using CrudMVCApp.Data;
using CrudMVCApp.Models;
using CrudMVCApp.Filtros;

namespace CrudMVCApp.Controllers
{
    [FiltroAdmin]
    public class UsuariosController : Controller
    {
        private readonly AppDbContext _context;

        public UsuariosController(AppDbContext context)
        {
            _context = context;
        }

        // GET: Usuarios
        public async Task<IActionResult> Index()
        {
            return View(await _context.Usuario.ToListAsync());
        }

        // GET: Usuarios/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var usuario = await _context.Usuario
                .FirstOrDefaultAsync(m => m.Id == id);
            if (usuario == null)
            {
                return NotFound();
            }

            return View(usuario);
        }

        // GET: Usuarios/Create
        public IActionResult Create()
        {
            return View();
        }

        

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Usuario usuario)
        {
            if (!ModelState.IsValid)
            {
                return View(usuario); 
            }

            var usuarioExistente = _context.Usuario
                .AsEnumerable()
                .FirstOrDefault(u => u.user == usuario.user);

            if (usuarioExistente != null)
            {
                if (!usuarioExistente.Activo)
                {
                    ViewBag.ErrorMessage = "El usuario existe pero está dado de baja.";
                }
                else
                {
                    ViewBag.ErrorMessage = "Usuario existente.";
                }

                return View(usuario); 
            }

            
            usuario.Activo = true;

            _context.Usuario.Add(usuario);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        //GET: Usuarios/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var usuario = await _context.Usuario.FindAsync(id);
            if (usuario == null)
            {
                return NotFound();
            }
            return View(usuario);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Usuario usuario)
        {
            if (id != usuario.Id)
            {
                return NotFound();
            }

            // Obtener el usuario existente de la base de datos
            var usuarioExistente = await _context.Usuario.FindAsync(id);
            if (usuarioExistente == null)
            {
                return NotFound();
            }

            // Proteger al usuario admin
            //
            if (usuarioExistente.user.Equals("admin", StringComparison.OrdinalIgnoreCase))
            {
                return RedirectToAction(nameof(Index));
            }

            // Deshabilitar validación para campos no editables
            ModelState.Remove("Tipo");

            if (ModelState.IsValid)
            {
                try
                {
                    // Actualizar solo los campos permitidos
                    usuarioExistente.user = usuario.user;
                    usuarioExistente.Clave = usuario.Clave;
                   
                    // Marcar solo los campos modificados como modificados
                    _context.Entry(usuarioExistente).Property(x => x.user).IsModified = true;
                    _context.Entry(usuarioExistente).Property(x => x.Clave).IsModified = true;

                    var pedidosDelUsuario = _context.Pedido
                        .Where(p => p.UsuarioId == usuario.Id)
                        .ToList();

                    await _context.SaveChangesAsync();

                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UsuarioExists(usuario.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", $"Error al guardar: {ex.Message}");
                }
            }

            // Si hay errores, mostrar la vista con los errores
            return View(usuarioExistente);
        }

        // GET: Usuarios/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var usuario = await _context.Usuario
                .FirstOrDefaultAsync(m => m.Id == id);
            if (usuario == null)
            {
                return NotFound();
            }

            return View(usuario);
        }

        // POST: Usuarios/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken] //Al enviar el formulario, el token se valida en el servidor. Si no coincide o falta, la solicitud se rechaza.
        //Este atributo asegura que las solicitudes POST, PUT, DELETE, etc., provengan de formularios generados por tu aplicación y no de un origen fraudulento.
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var usuario = await _context.Usuario.FindAsync(id);
            if (usuario != null)
            {
                usuario.Activo = false; 
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool UsuarioExists(int id)
        {
            return _context.Usuario.Any(e => e.Id == id);
        }
    }
}
