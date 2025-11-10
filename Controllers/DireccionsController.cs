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
    [FiltroGeneral]
    public class DireccionsController : Controller
    {
        private readonly AppDbContext _context;

        public DireccionsController(AppDbContext context)
        {
            _context = context;
        }

        // GET: Direccions
        public async Task<IActionResult> Index()
        {
            var appDbContext = _context.Direccion.Include(d => d.Persona);
            return View(await appDbContext.ToListAsync());
        }

        // GET: Direccions/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var direccion = await _context.Direccion
                .Include(d => d.Persona)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (direccion == null)
            {
                return NotFound();
            }

            return View(direccion);
        }

        // GET: Direccions/Create
        public IActionResult Create()
        {
            // Llamamos a nuestro método helper
            PoblarDropdownPersonas();
            return View();
        }

        // POST: Direccions/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Calle,Ciudad,CodigoPostal,PersonaId")] Direccion direccion)
        {
            if (ModelState.IsValid)
            {
                _context.Add(direccion);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            PoblarDropdownPersonas(direccion.PersonaId);
            return View(direccion);
        }

        // GET: Direccions/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var direccion = await _context.Direccion.FindAsync(id);
            if (direccion == null)
            {
                return NotFound();
            }

            // --- CAMBIO ---
            // Llamamos a nuestro método helper
            PoblarDropdownPersonas(direccion.PersonaId);
            return View(direccion);
        }

        // POST: Direccions/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Calle,Ciudad,CodigoPostal,PersonaId")] Direccion direccion)
        {
            if (id != direccion.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(direccion);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DireccionExists(direccion.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }

            // --- CAMBIO ---
            // Volvemos a poblar el dropdown si el modelo no es válido
            PoblarDropdownPersonas(direccion.PersonaId);
            return View(direccion);
        }

        // GET: Direccions/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var direccion = await _context.Direccion
                .Include(d => d.Persona)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (direccion == null)
            {
                return NotFound();
            }

            return View(direccion);
        }

        // POST: Direccions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var direccion = await _context.Direccion.FindAsync(id);
            if (direccion != null)
            {
                _context.Direccion.Remove(direccion);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool DireccionExists(int id)
        {
            return _context.Direccion.Any(e => e.Id == id);
        }

        // --- MÉTODO HELPER AGREGADO ---
        // Esta es la buena práctica para tu portafolio
        private void PoblarDropdownPersonas(object personaSeleccionada = null)
        {
            // 1. Creamos una consulta eficiente.
            //    Usamos tu modelo Persona (con Nombre y Apellido)
            var personasQuery = from p in _context.Persona
                                orderby p.Apellido, p.Nombre
                                select new
                                {
                                    Id = p.Id,
                                    NombreCompleto = p.Nombre + " " + p.Apellido
                                };

            // 2. Creamos el SelectList correctamente
            //    Value = "Id"
            //    Text = "NombreCompleto"
            ViewData["PersonaId"] = new SelectList(personasQuery.AsNoTracking(), "Id", "NombreCompleto", personaSeleccionada);
        }
    }
}