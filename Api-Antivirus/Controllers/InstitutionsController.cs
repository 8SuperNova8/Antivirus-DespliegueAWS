using Api_Antivirus.DTO;
using Api_Antivirus.Interface;
using Api_Antivirus.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace Api_Antivirus.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class InstitutionsController : ControllerBase
    {
        private readonly IInstitutions _service;

        public InstitutionsController(IInstitutions service)
        {
            _service = service;
        }

                [HttpGet]
        public async Task<ActionResult<IEnumerable<InstitutionsResponseDto>>> GetAll()
        {
            var list = await _service.GetAllAsync();
            return Ok(list);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<InstitutionsResponseDto>> Get(int id)
        {
            var dto = await _service.GetByIdAsync(id);
            if (dto == null)
            {
                return NotFound();
            }
            return Ok(dto);
        }

        [HttpPost]
        public async Task<ActionResult<InstitutionsResponseDto>> Create([FromBody] InstitutionsRequestDto dto)
        {   
            if (!IsAdmin()) return Forbid();

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            await _service.CreateAsync(dto);
            return CreatedAtAction(nameof(Get), dto);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> Update(int id, [FromBody] InstitutionsRequestDto dto)
        {   
            if (!IsAdmin()) return Forbid();

            var entity = await _service.GetByIdAsync(id);
            if (entity == null)
            {
                return NotFound();
            }
            return NoContent();
        }

        [HttpDelete]
        public async Task<ActionResult> Delete(int id)
        {   
            if (!IsAdmin()) return Forbid();

            var result = await _service.GetByIdAsync(id);
            if (result == null)
            {
                return NotFound();
            }
            await _service.DeleteAsync(id);
            return NoContent();
        }

        private bool IsAdmin()
        {
            var role = HttpContext.User.Claims.FirstOrDefault(c => c.Type == "rol")?.Value;
            return role == "admin";
        }
    }
}