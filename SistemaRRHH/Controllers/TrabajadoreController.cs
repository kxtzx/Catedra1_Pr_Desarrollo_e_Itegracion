using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SistemaRRHH.Data;
using SistemaRRHH.Models;
using SistemaRRHH.ViewModels;
using System.Text.Json.Serialization;

namespace SistemaRRHH.Controllers
{
    public class TrabajadoreController : Controller
    {
        private readonly DbRrhhContext _context;

        public TrabajadoreController(DbRrhhContext context)
        {
            _context = context;
        }

        // GET: Trabajadore/ResultadoSueldos
        public async Task<IActionResult> ResultadoSueldos()
        {
            var trabajadores = await _context.Trabajadores.ToListAsync();
            var resultados = CrearResultadosBase(trabajadores);

            return View(resultados);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CargarJson(IFormFile archivo)
        {
            var trabajadores = await _context.Trabajadores.ToListAsync();

            if (archivo == null || archivo.Length == 0)
            {
                ModelState.AddModelError(string.Empty, "Por favor, seleccione un archivo JSON válido.");
                return View("ResultadoSueldos", CrearResultadosBase(trabajadores));
            }

            try
            {
                using var stream = archivo.OpenReadStream();
                var horasList = await JsonSerializer.DeserializeAsync<List<HorasDto>>(stream, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                var horasPorRut = horasList == null
                    ? new Dictionary<string, decimal>(StringComparer.OrdinalIgnoreCase)
                    : horasList
                        .Where(x => !string.IsNullOrWhiteSpace(x.Rut))
                        .GroupBy(x => x.Rut.Trim(), StringComparer.OrdinalIgnoreCase)
                        .ToDictionary(g => g.Key, g => g.Sum(x => x.HorasTrabajadas), StringComparer.OrdinalIgnoreCase);

                var resultados = trabajadores.Select(trabajador =>
                {
                    horasPorRut.TryGetValue(trabajador.Rut, out var horasTrabajadas);

                    var sueldoFinal = trabajador.SueldoBase + (horasTrabajadas * trabajador.ValorHora);

                    return new ResultadoSueldoViewModel
                    {
                        Rut = trabajador.Rut,
                        Nombre = trabajador.Nombre,
                        HorasTrabajadas = horasTrabajadas,
                        SueldoBase = trabajador.SueldoBase,
                        ValorHora = trabajador.ValorHora,
                        SueldoFinal = sueldoFinal
                    };
                }).ToList();

                return View("ResultadoSueldos", resultados);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, "Error al procesar el archivo JSON: " + ex.Message);

                return View("ResultadoSueldos", CrearResultadosBase(trabajadores));
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

        private static List<ResultadoSueldoViewModel> CrearResultadosBase(IEnumerable<Trabajadore> trabajadores)
        {
            return trabajadores.Select(t => new ResultadoSueldoViewModel
            {
                Rut = t.Rut,
                Nombre = t.Nombre,
                HorasTrabajadas = 0m,
                SueldoBase = t.SueldoBase,
                ValorHora = t.ValorHora,
                SueldoFinal = t.SueldoBase
            }).ToList();
        }
    }

    public class HorasDto
    {
        [JsonPropertyName("rut")]
        public string Rut { get; set; } = string.Empty;

        [JsonPropertyName("horas")]
        public decimal HorasTrabajadas { get; set; }
    }
}