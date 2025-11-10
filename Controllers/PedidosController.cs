using CrudMVCApp.Data;
using CrudMVCApp.Filtros;
using CrudMVCApp.Helpers;
using CrudMVCApp.Models;
using CrudMVCApp.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[FiltroGeneral]
public class PedidosController : Controller
{
    private readonly AppDbContext _context;
    private const string SessionKey = "DetallePedido"; 
    //Es una clave única que identifica un valor almacenado en la sesión del usuario.

    public PedidosController(AppDbContext context)
    {
        _context = context;
    }

    [FiltroAdmin]
    public async Task<IActionResult> Index(string usuarioNombre)
    {
        ViewBag.UsuarioBusqueda = usuarioNombre;

        var pedidosQuery = _context.Pedido
            .Include(p => p.Usuario)  // Cargar relación
            .Include(p => p.Persona)
            .AsQueryable(); //Evita traer datos innecesarios a memoria.
         
        if (!string.IsNullOrEmpty(usuarioNombre)) //IsNullOrEmpty: retorna true si es cadena vacia o null
        {
            pedidosQuery = pedidosQuery.Where(p => p.Usuario.user.Contains(usuarioNombre));
        }

        return View(await pedidosQuery.ToListAsync());
    }

    public async Task<IActionResult> IndexUsuario()
    {
        var usuarioIdStr = HttpContext.Session.GetString("UsuarioId");

        if (string.IsNullOrEmpty(usuarioIdStr))
        {
            // Redirigir al login si no hay sesión activa
            return RedirectToAction("Login", "Login");
        }

        var usuarioId = int.Parse(usuarioIdStr);
        var pedidosUsuario = await _context.Pedido
            .Include(p => p.Persona)
            .Include(p => p.Usuario)
            .Where(p => p.UsuarioId == usuarioId)
            .ToListAsync();

        return View(pedidosUsuario);
    }

    public async Task<IActionResult> Crear(int? personaId)
    {
        var productosActivos = await _context.Producto.Where(p => p.Activo).ToListAsync();
        var clientesActivos = await _context.Persona.Where(p => p.Activo).ToListAsync();

        ViewBag.Clientes = clientesActivos;
        ViewBag.Productos = productosActivos;

        var detalles = HttpContext.Session.GetObject<List<DetallePedidoViewModel>>(SessionKey) ?? new();
        //HttpContext.Session.GetObject<T> para leer un objeto almacenado en la sesión del usuario bajo la clave SessionKey.
        //si retorna null (no hay nada guardado) se asigna una nueva lista

        var model = new PedidoViewModel
        {
            PersonaId = personaId ?? 0,
            UsuarioId = int.Parse(HttpContext.Session.GetString("UsuarioId")),
            // Recupera un valor almacenado previamente en la sesión del usuario bajo la clave "UsuarioId"
            Detalles = detalles
        };

        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> AgregarProducto(int productoId, int cantidad, int personaId)
    {
        if (personaId == 0 || productoId == 0 || cantidad <= 0)
            return RedirectToAction("Crear", new { personaId });

        var producto = await _context.Producto.FindAsync(productoId);
        if (producto == null || producto.Stock < cantidad)
        {
            TempData["Error"] = "Stock insuficiente para este producto.";
            return RedirectToAction("Crear", new { personaId });
        }

        var detalles = HttpContext.Session.GetObject<List<DetallePedidoViewModel>>(SessionKey) ?? new();

        var existente = detalles.FirstOrDefault(p => p.ProductoId == producto.Id);
        if (existente != null)
        {
            if (producto.Stock < (existente.Cantidad + cantidad))
            {
                TempData["Error"] = "No se puede agregar más que el stock disponible.";
                return RedirectToAction("Crear", new { personaId });
            }
            existente.Cantidad += cantidad;
        }
        else
        {
            detalles.Add(new DetallePedidoViewModel
            {
                ProductoId = producto.Id,
                NombreProducto = producto.Nombre,
                PrecioUnitario = producto.PrecioVta,
                Cantidad = cantidad
            });
        }

        HttpContext.Session.SetObject(SessionKey, detalles);
        return RedirectToAction("Crear", new { personaId });
    }

    [HttpPost]
    public async Task<IActionResult> ConfirmarPedido(int personaId)
    {
        var detalles = HttpContext.Session.GetObject<List<DetallePedidoViewModel>>(SessionKey);
        if (detalles == null || detalles.Count == 0)
        {
            TempData["Error"] = "Debe seleccionar un cliente y agregar al menos un producto.";
            return RedirectToAction("Crear", new { personaId });
        }

        var pedido = new Pedido
        {
            PersonaId = personaId,
            UsuarioId = int.Parse(HttpContext.Session.GetString("UsuarioId")),  // FK real
            Fecha = DateTime.Now,
            Detalles = detalles.Select(d => new DetallePedido
            {
                ProductoId = d.ProductoId,
                Cantidad = d.Cantidad,
                PrecioUnitario = d.PrecioUnitario
            }).ToList()
        };

        _context.Pedido.Add(pedido);
        await _context.SaveChangesAsync();
        HttpContext.Session.Remove(SessionKey);

        TempData["Success"] = "Pedido registrado correctamente.";
        return RedirectToAction(HttpContext.Session.GetString("Tipo") == "admin" ? "Index" : "IndexUsuario");
    }

    [HttpPost]
    public IActionResult EliminarProducto(int productoId, int personaId)
    {
        var detalles = HttpContext.Session.GetObject<List<DetallePedidoViewModel>>(SessionKey) ?? new();
        var producto = detalles.FirstOrDefault(p => p.ProductoId == productoId);

        if (producto != null)
        {
            detalles.Remove(producto);
            HttpContext.Session.SetObject(SessionKey, detalles);
        }

        return RedirectToAction("Crear", new { personaId });
    }

    public async Task<IActionResult> Details(int? id)
    {
        if (id == null) return NotFound();

        var pedido = await _context.Pedido
            .Include(p => p.Usuario)  // Cargar usuario
            .Include(p => p.Persona)
            .Include(p => p.Detalles)
            .ThenInclude(d => d.Producto)  // Permite cargar propiedades de las entidades que ya fueron incluidas.
            .FirstOrDefaultAsync(p => p.Id == id);

        if (pedido == null) return NotFound();

        var viewModel = new MostrarDetalleViewModel
        {
            Pedido = pedido,
            Detalles = pedido.Detalles.ToList(),
            DetalleTotal = pedido.Detalles.Select(d => new DetallePedidoViewModel
            {
                ProductoId = d.ProductoId,
                NombreProducto = d.Producto.Nombre,
                Cantidad = d.Cantidad,
                PrecioUnitario = d.PrecioUnitario
            }).ToList()
        };

        return View(viewModel);
    }
}



