using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using SistemaRRHH.Data;
using SistemaRRHH.Models;

namespace SistemaRRHH.Controllers
{
    public class TrabajadoreController : Controller
    {
        private readonly DbRrhhContext _context;

        public TrabajadoreController(DbRrhhContext context)
        {
            _context = context;
        }

        // ==========================================
        // PROCESAMIENTO DE ARCHIVO JSON (SUELDOS)
        // ==========================================

        [HttpPost]
        public async Task<IActionResult> CargarHoras(IFormFile archivo)
        {
            // Validar que el usuario haya seleccionado un archivo
            if (archivo == null || archivo.Length == 0)
            {
                ModelState.AddModelError("", "Por favor, seleccione un archivo JSON válido.");
                return View("Index", await _context.Trabajadores.ToListAsync());
            }

            try
            {
                using var stream = archivo.OpenReadStream();
                var horasList = await System.Text.Json.JsonSerializer.DeserializeAsync<List<HorasDto>>(stream);

                // Lista para almacenar los resultados calculados
                var resultados = new List<ResultadoSueldoViewModel>();

                if (horasList != null)
                {
                    foreach (var item in horasList)
                    {
                        // Buscamos al trabajador por su RUT
                        var trabajador = await _context.Trabajadores.FirstOrDefaultAsync(t => t.Rut == item.Rut);
                        
                        if (trabajador != null)
                        {
                            // Fórmula: Sueldo Base + (Horas Trabajadas * Valor Hora)
                            decimal sueldoMes = trabajador.SueldoBase + (item.HorasTrabajadas * trabajador.ValorHora);
                            
                            // Guardamos la información procesada
                            resultados.Add(new ResultadoSueldoViewModel
                            {
                                Rut = trabajador.Rut,
                                Nombre = trabajador.Nombre,
                                SueldoFinal = sueldoMes
                            });
                        }
                    }
                }

                // Envía los resultados a la vista "ResultadoSueldos.cshtml"
                return View("ResultadoSueldos", resultados);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Error al procesar el archivo JSON: " + ex.Message);
                return View("Index", await _context.Trabajadores.ToListAsync());
            }
        }

        // ==========================================
        // VISTAS CRUD AUTOGENERADAS
        // ==========================================

        // GET: Trabajadore
        public async Task<IActionResult> Index()
        {
            return View(await _context.Trabajadores.ToListAsync());
        }

        // GET: Trabajadore/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var trabajadore = await _context.Trabajadores
                .FirstOrDefaultAsync(m => m.Id == id);
            if (trabajadore == null)
            {
                return NotFound();
            }

            return View(trabajadore);
        }

        // GET: Trabajadore/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Trabajadore/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Rut,Nombre,SueldoBase,ValorHora")] Trabajadore trabajadore)
        {
            if (ModelState.IsValid)
            {
                _context.Add(trabajadore);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(trabajadore);
        }

        // GET: Trabajadore/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var trabajadore = await _context.Trabajadores.FindAsync(id);
            if (trabajadore == null)
            {
                return NotFound();
            }
            return View(trabajadore);
        }

        // POST: Trabajadore/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Rut,Nombre,SueldoBase,ValorHora")] Trabajadore trabajadore)
        {
            if (id != trabajadore.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(trabajadore);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TrabajadoreExists(trabajadore.Id))
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
            return View(trabajadore);
        }

        // GET: Trabajadore/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var trabajadore = await _context.Trabajadores
                .FirstOrDefaultAsync(m => m.Id == id);
            if (trabajadore == null)
            {
                return NotFound();
            }

            return View(trabajadore);
        }

        // POST: Trabajadore/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var trabajadore = await _context.Trabajadores.FindAsync(id);
            if (trabajadore != null)
            {
                _context.Remove(trabajadore);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TrabajadoreExists(int id)
        {
            return _context.Trabajadores.Any(e => e.Id == id);
        }
    }

    // ==========================================
    // CLASES AUXILIARES (MODELOS DE TRANSFERENCIA)
    // ==========================================

    // Mapea la estructura del archivo JSON entrante
    public class HorasDto
    {
        public string Rut { get; set; } = string.Empty;
        public decimal HorasTrabajadas { get; set; }
    }

    // Mapea los datos finales calculados que se verán en la vista
    public class ResultadoSueldoViewModel
    {
        public string Rut { get; set; } = string.Empty;
        public string Nombre { get; set; } = string.Empty;
        public decimal SueldoFinal { get; set; }
    }
}