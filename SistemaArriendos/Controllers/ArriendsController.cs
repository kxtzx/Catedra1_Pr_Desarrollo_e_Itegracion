using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SistemaArriendos.Data;
using SistemaArriendos.Models;

namespace SistemaArriendos.Controllers
{
    public class ArriendsController : Controller
    {
        private readonly DbVehiculosContext _context;

        public ArriendsController(DbVehiculosContext context)
        {
            _context = context;
        }

        private void CargarCombos(int? clienteId = null, int? vehiculoId = null)
        {
            ViewData["ClienteId"] = new SelectList(_context.Clientes, "Id", "Nombre", clienteId);
            ViewData["VehiculoId"] = new SelectList(
                _context.Vehiculos.Where(v => v.Estado == "Disponible"),
                "Id",
                "Patente",
                vehiculoId);
        }

        // GET: Arriends
        public async Task<IActionResult> Index()
        {
            var dbVehiculosContext = _context.Arriendos.Include(a => a.Cliente).Include(a => a.Vehiculo);
            return View(await dbVehiculosContext.ToListAsync());
        }

        // GET: Arriends/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var arriendo = await _context.Arriendos
                .Include(a => a.Cliente)
                .Include(a => a.Vehiculo)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (arriendo == null)
            {
                return NotFound();
            }

            return View(arriendo);
        }

        // GET: Arriends/Create
        public IActionResult Create()
        {
            CargarCombos();
            return View();
        }

        // POST: Arriends/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]

        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,VehiculoId,ClienteId,FechaInicio,FechaFin,PrecioDiario,PrecioTotal")] Arriendo arriendo)
        {
            CargarCombos(arriendo.ClienteId, arriendo.VehiculoId);
            ModelState.Remove(nameof(Arriendo.Cliente));
            ModelState.Remove(nameof(Arriendo.Vehiculo));

            var vehiculo = await _context.Vehiculos.FindAsync(arriendo.VehiculoId);

            if (vehiculo == null)
            {
                ModelState.AddModelError("", "El vehículo seleccionado no existe.");
                return View(arriendo);
            }

            if (vehiculo.Estado != "Disponible")
            {
                ModelState.AddModelError("", $"Operación denegada: El vehículo actualmente se encuentra en estado: '{vehiculo.Estado}'.");
                return View(arriendo);
            }

            if (arriendo.FechaFin.DayNumber < arriendo.FechaInicio.DayNumber)
            {
                ModelState.AddModelError("", "La fecha de fin no puede ser menor a la fecha de inicio.");
                return View(arriendo);
            }

            int dias = arriendo.FechaFin.DayNumber - arriendo.FechaInicio.DayNumber;
            if (dias == 0) dias = 1;

            arriendo.PrecioTotal = dias * arriendo.PrecioDiario;

            if (ModelState.IsValid)
            {
                vehiculo.Estado = "Arrendado";
                _context.Update(vehiculo);

                _context.Add(arriendo);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            return View(arriendo);
        }

        // GET: Arriends/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var arriendo = await _context.Arriendos.FindAsync(id);
            if (arriendo == null)
            {
                return NotFound();
            }
            ViewData["ClienteId"] = new SelectList(_context.Clientes, "Id", "Id", arriendo.ClienteId);
            ViewData["VehiculoId"] = new SelectList(_context.Vehiculos, "Id", "Id", arriendo.VehiculoId);
            return View(arriendo);
        }

        // POST: Arriends/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,VehiculoId,ClienteId,FechaInicio,FechaFin,PrecioDiario,PrecioTotal")] Arriendo arriendo)
        {
            if (id != arriendo.Id)
            {
                return NotFound();
            }

            ModelState.Remove(nameof(Arriendo.Cliente));
            ModelState.Remove(nameof(Arriendo.Vehiculo));

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(arriendo);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ArriendoExists(arriendo.Id))
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
            ViewData["ClienteId"] = new SelectList(_context.Clientes, "Id", "Id", arriendo.ClienteId);
            ViewData["VehiculoId"] = new SelectList(_context.Vehiculos, "Id", "Id", arriendo.VehiculoId);
            return View(arriendo);
        }

        // GET: Arriends/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var arriendo = await _context.Arriendos
                .Include(a => a.Cliente)
                .Include(a => a.Vehiculo)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (arriendo == null)
            {
                return NotFound();
            }

            return View(arriendo);
        }

        // POST: Arriends/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var arriendo = await _context.Arriendos.FindAsync(id);
            if (arriendo != null)
            {
                _context.Arriendos.Remove(arriendo);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ArriendoExists(int id)
        {
            return _context.Arriendos.Any(e => e.Id == id);
        }
    }
}
