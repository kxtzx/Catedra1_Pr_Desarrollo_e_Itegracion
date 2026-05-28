using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SistemaMantencion.Data;
using SistemaMantencion.Models;
using System.Text.Json;
using System.Text;

namespace SistemaMantencion.Controllers
{
    public class MantencionesController : Controller
    {
        private readonly DbVehiculosContext _context;

        public MantencionesController(DbVehiculosContext context)
        {
            _context = context;
        }

        // GET: Mantenciones
        public async Task<IActionResult> Index()
        {
            var dbVehiculosContext = _context.Mantenciones.Include(m => m.Vehiculo);
            return View(await dbVehiculosContext.ToListAsync());
        }

        // GET: Mantenciones/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var mantencione = await _context.Mantenciones
                .Include(m => m.Vehiculo)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (mantencione == null)
            {
                return NotFound();
            }

            return View(mantencione);
        }

        // GET: Mantenciones/Create
        public IActionResult Create()
        {
            ViewData["VehiculoId"] = new SelectList(_context.Vehiculos, "Id", "Id");
            return View();
        }

        public IActionResult ExportarHorasJson()
{
            var horas = _context.Mantenciones
                .GroupBy(m => m.RutMecanico)
                .Select(g => new {
                    rut = g.Key,
                    horas_trabajadas = g.Sum(m => m.HorasTrabajadas)
                }).ToList();

            var json = System.Text.Json.JsonSerializer.Serialize(horas);
            var bytes = System.Text.Encoding.UTF8.GetBytes(json);
            return File(bytes, "application/json", "horas_trabajadas.json");
        }

        // POST: Mantenciones/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,VehiculoId,FechaMantencion,RutMecanico,HorasTrabajadas")] Mantencione mantencione)
        {
            if (ModelState.IsValid)
            {
                _context.Add(mantencione);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["VehiculoId"] = new SelectList(_context.Vehiculos, "Id", "Id", mantencione.VehiculoId);
            return View(mantencione);
        }

        // GET: Mantenciones/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var mantencione = await _context.Mantenciones.FindAsync(id);
            if (mantencione == null)
            {
                return NotFound();
            }
            ViewData["VehiculoId"] = new SelectList(_context.Vehiculos, "Id", "Id", mantencione.VehiculoId);
            return View(mantencione);
        }

        // POST: Mantenciones/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,VehiculoId,FechaMantencion,RutMecanico,HorasTrabajadas")] Mantencione mantencione)
        {
            if (id != mantencione.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(mantencione);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MantencioneExists(mantencione.Id))
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
            ViewData["VehiculoId"] = new SelectList(_context.Vehiculos, "Id", "Id", mantencione.VehiculoId);
            return View(mantencione);
        }

        // GET: Mantenciones/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var mantencione = await _context.Mantenciones
                .Include(m => m.Vehiculo)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (mantencione == null)
            {
                return NotFound();
            }

            return View(mantencione);
        }

        // POST: Mantenciones/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var mantencione = await _context.Mantenciones.FindAsync(id);
            if (mantencione != null)
            {
                _context.Mantenciones.Remove(mantencione);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool MantencioneExists(int id)
        {
            return _context.Mantenciones.Any(e => e.Id == id);
        }
    }
}
