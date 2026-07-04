using Equipment.Api.CustomAttributes;
using Equipment.Api.Data;
using Equipment.Api.DTO;
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
            var items = await _context.Equipment
                .Include(e => e.EquipmentType)
                .Include(e => e.EquipmentStatus)
                .Include(e => e.Employee)
                .Select(e => new EquipmentItemDto
                {
                    Id = e.Id,
                    InventoryNumber = e.InventoryNumber,
                    Name = e.Name,
                    Type = e.EquipmentType != null ? e.EquipmentType.Name : string.Empty,
                    SerialNumber = e.SerialNumber,
                    Status = e.EquipmentStatus.Name,
                    ResponsiblePerson = e.Employee != null ? e.Employee.FullName : string.Empty
                })
                .ToListAsync();

            return Ok(items);
        }

        // GET api/equipment/{id} — получить оборудование по ID
        // Доступно: Admin (3), Manager (2)
        [HttpGet("{id}")]
        [RoleAutorize(new[] { 2, 3 })]
        public async Task<IActionResult> GetById(int id)
        {
            var item = await _context.Equipment
                .Include(e => e.EquipmentType)
                .Include(e => e.EquipmentStatus)
                .Include(e => e.Employee)
                .FirstOrDefaultAsync(e => e.Id == id);

            if (item == null)
                return NotFound(new { message = "Оборудование не найдено." });

            var dto = new EquipmentItemDto
            {
                Id = item.Id,
                InventoryNumber = item.InventoryNumber,
                Name = item.Name,
                Type = item.EquipmentType != null ? item.EquipmentType.Name : string.Empty,
                SerialNumber = item.SerialNumber,
                Status = item.EquipmentStatus.Name,
                ResponsiblePerson = item.Employee != null ? item.Employee.FullName : string.Empty
            };

            return Ok(dto);
        }

        // POST api/equipment — создать новое оборудование
        // Доступно: только Admin (3)
        [HttpPost]
        [RoleAutorize(new[] { 3 })]
        public async Task<IActionResult> Create([FromBody] EquipmentItemDto dto)
        {
            var typeId = await GetOrCreateTypeId(dto.Type);
            var statusId = await GetOrCreateStatusId(dto.Status);
            var employeeId = await GetOrCreateEmployeeId(dto.ResponsiblePerson);

            var item = new EquipmentItem
            {
                InventoryNumber = dto.InventoryNumber,
                Name = dto.Name,
                EquipmentTypeId = typeId,
                SerialNumber = dto.SerialNumber,
                EquipmentStatusId = statusId,
                EmployeeId = employeeId
            };

            _context.Equipment.Add(item);
            await _context.SaveChangesAsync();

            dto.Id = item.Id;
            return CreatedAtAction(nameof(GetById), new { id = item.Id }, dto);
        }

        // PUT api/equipment/{id} — обновить оборудование
        // Доступно: только Admin (3)
        [HttpPut("{id}")]
        [RoleAutorize(new[] { 3 })]
        public async Task<IActionResult> Update(int id, [FromBody] EquipmentItemDto dto)
        {
            var existing = await _context.Equipment.FindAsync(id);

            if (existing == null)
                return NotFound(new { message = "Оборудование не найдено." });

            var typeId = await GetOrCreateTypeId(dto.Type);
            var statusId = await GetOrCreateStatusId(dto.Status);
            var employeeId = await GetOrCreateEmployeeId(dto.ResponsiblePerson);

            existing.InventoryNumber = dto.InventoryNumber;
            existing.Name = dto.Name;
            existing.EquipmentTypeId = typeId;
            existing.SerialNumber = dto.SerialNumber;
            existing.EquipmentStatusId = statusId;
            existing.EmployeeId = employeeId;

            await _context.SaveChangesAsync();

            dto.Id = existing.Id;
            return Ok(dto);
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

        #region Helper Methods to maintain 3NF relationships
        private async Task<int?> GetOrCreateTypeId(string typeName)
        {
            if (string.IsNullOrWhiteSpace(typeName)) return null;
            typeName = typeName.Trim();
            var type = await _context.EquipmentTypes.FirstOrDefaultAsync(t => t.Name.ToLower() == typeName.ToLower());
            if (type == null)
            {
                type = new EquipmentType { Name = typeName };
                _context.EquipmentTypes.Add(type);
                await _context.SaveChangesAsync();
            }
            return type.Id;
        }

        private async Task<int> GetOrCreateStatusId(string statusName)
        {
            if (string.IsNullOrWhiteSpace(statusName)) statusName = "В работе";
            statusName = statusName.Trim();
            var status = await _context.EquipmentStatuses.FirstOrDefaultAsync(s => s.Name.ToLower() == statusName.ToLower());
            if (status == null)
            {
                status = new EquipmentStatus { Name = statusName };
                _context.EquipmentStatuses.Add(status);
                await _context.SaveChangesAsync();
            }
            return status.Id;
        }

        private async Task<int?> GetOrCreateEmployeeId(string fullName)
        {
            if (string.IsNullOrWhiteSpace(fullName)) return null;
            fullName = fullName.Trim();
            var employee = await _context.Employees.FirstOrDefaultAsync(e => e.FullName.ToLower() == fullName.ToLower());
            if (employee == null)
            {
                employee = new Employee { FullName = fullName };
                _context.Employees.Add(employee);
                await _context.SaveChangesAsync();
            }
            return employee.Id;
        }
        #endregion
    }
}
