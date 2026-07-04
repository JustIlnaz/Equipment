using Equipment.Api.CustomAttributes;
using Equipment.Api.Data;
using Equipment.Api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Equipment.Api.Controllers
{
    [ApiController]
    [Route("api/equipment")]
    public class EquipmentController : ControllerBase
    {
        private readonly AppDbContext _context;

        public EquipmentController(AppDbContext context)
        {
            _context = context;
        }

        // GET api/equipment — получить список всего оборудования
        // Доступно: Admin (3), Manager (2), User (1)
        [HttpGet]
        [RoleAutorize(new[] { 1, 2, 3 })]
        public async Task<IActionResult> GetAll()
        {
            var items = await _context.Equipment.ToListAsync();
            return Ok(items);
        }

        // GET api/equipment/{id} — получить оборудование по ID
        // Доступно: Admin (3), Manager (2)
        [HttpGet("{id}")]
        [RoleAutorize(new[] { 2, 3 })]
        public async Task<IActionResult> GetById(int id)
        {
            var item = await _context.Equipment.FindAsync(id);

            if (item == null)
                return NotFound(new { message = "Оборудование не найдено." });

            return Ok(item);
        }

        // POST api/equipment — создать новое оборудование
        // Доступно: только Admin (3)
        [HttpPost]
        [RoleAutorize(new[] { 3 })]
        public async Task<IActionResult> Create([FromBody] EquipmentItem item)
        {
            item.Id = 0;
            _context.Equipment.Add(item);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById), new { id = item.Id }, item);
        }

        // PUT api/equipment/{id} — обновить оборудование
        // Доступно: только Admin (3)
        [HttpPut("{id}")]
        [RoleAutorize(new[] { 3 })]
        public async Task<IActionResult> Update(int id, [FromBody] EquipmentItem item)
        {
            var existing = await _context.Equipment.FindAsync(id);

            if (existing == null)
                return NotFound(new { message = "Оборудование не найдено." });

            existing.InventoryNumber = item.InventoryNumber;
            existing.Name = item.Name;
            existing.Type = item.Type;
            existing.SerialNumber = item.SerialNumber;
            existing.Status = item.Status;
            existing.ResponsiblePerson = item.ResponsiblePerson;

            await _context.SaveChangesAsync();

            return Ok(existing);
        }

        // DELETE api/equipment/{id} — удалить оборудование
        // Доступно: только Admin (3)
        [HttpDelete("{id}")]
        [RoleAutorize(new[] { 3 })]
        public async Task<IActionResult> Delete(int id)
        {
            var item = await _context.Equipment.FindAsync(id);

            if (item == null)
                return NotFound(new { message = "Оборудование не найдено." });

            _context.Equipment.Remove(item);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Оборудование удалено." });
        }
    }
}
