using Mapster;
using Microsoft.AspNetCore.Mvc;
using Tesis.Application.DTOs.Proceso;
using Tesis.DataAcces.Repository.IRepository;
using Tesis.Domain.Models;
using Tesis.Shared;

namespace Tesis.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProcesoController : ControllerBase
    {

        private readonly IUnitOfWorks _unitOfWorks;
        private readonly ILogger<ProcesoController> _logger;
        public ProcesoController(IUnitOfWorks unitOfWorks, ILogger<ProcesoController> logger)
        {
            _unitOfWorks = unitOfWorks;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProcesoModel>>> GetAll()
        {
            _logger.LogDebug("Obteniendo todos los procesos ....");

            IEnumerable<ProcesoModel> AllProcess = await _unitOfWorks.Proceso.GetAllWithIncludes(includeProperties: "Indicadores");

            AllProcess.AllProcesosConsoleMessage();

            if (!AllProcess.Any() || AllProcess is null)
            {
                _logger.LogWarning("No existe ningun proceso en la Base de Datos");
                return NoContent();
            }

            var procesoDto = AllProcess.Adapt<List<ProcesoDTO>>();

            _logger.LogDebug("Termino la Obtencion de todos los procesos de la Base de Datos");
            return Ok(procesoDto);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ProcesoModel>> GetById(int id)
        {
            ProcesoModel Proceso = await _unitOfWorks.Proceso.GetWithIncludes(p => p.Id == id, includeProperties: "Indicadores");

            if (Proceso is null)
            {
                _logger.LogWarning("El Proceso:{id} no existe en la Base de Datos",id);
                return NotFound("Proceso no encontrado");
            }

            var procesoDto = Proceso.Adapt<ProcesoDTO>();


            _logger.LogInformation("Proceso Obtenido Correctamente\n" +
                "Id:{id}\n" +
                "Nombre:{Nombre}\n" +
                "Evaluacion:{evaluacion}",Proceso.Id,Proceso.Nombre,Proceso.Evaluacion);

            return Ok(procesoDto);
        }

        [HttpPost("create")]
        public async Task<IActionResult> Add(ProcesoUpsertDTO newProcesodTO)
        {
            _logger.LogDebug("Iniciando Creacion de Proceso");

            if (newProcesodTO is null)
            {
                _logger.LogWarning("No puede crear un Proceso vacio/nulo");
                return BadRequest("No puede crear un Proceso nulo");
            }

            ProcesoModel Proceso = new ProcesoModel
            {
               Nombre = newProcesodTO.Nombre
            };

            _unitOfWorks.Proceso.Add(Proceso);

            Proceso.ProcesoCreadoConsoleMessage();
            _logger.LogInformation("Proceso Creado Correctamente");

            await _unitOfWorks.SaveAsync();
            _logger.LogInformation("Proceso Guardado Correctamente");
            return CreatedAtAction(nameof(GetById), new { id = Proceso.Id }, Proceso);
        }

        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            // Load the process with related indicators
            ProcesoModel Proceso = await _unitOfWorks.Proceso.GetWithIncludes(p => p.Id == id, includeProperties: "Indicadores");

            if (Proceso is null)
                return NotFound($"El Proceso con {id} no existe");

            // Safely iterate over indicators using a for loop
            var indicadoresList = Proceso.Indicadores.ToList(); 

            // Unlink todos los indicadores
            Proceso.Indicadores.ToList().ForEach(indicador =>
            {
                indicador.ProcesoId = null; // Unlink
                _unitOfWorks.Indicador.Update(indicador); // Update in memory
            });

            // Guardar cambios en indicadores Unlinked
            await _unitOfWorks.SaveAsync();

            //Eliminar el Proceso
            _unitOfWorks.Proceso.Remove(Proceso);
            await _unitOfWorks.SaveAsync();

            return NoContent();
        }

        [HttpPut("update/{id}")]
        public async Task<IActionResult> Update(int id, ProcesoUpsertDTO procesoDto)
        {
            _logger.LogDebug("Editando Proceso....");

            ProcesoModel Proceso = await _unitOfWorks.Proceso.GetWithIncludes(p => p.Id == id, includeProperties: "Indicadores");

            if (Proceso is null)
            {
                _logger.LogWarning("El Proceso:{} no existe en la Base de Datos",id);
                return NotFound($"El Proceso con id:{id} no existe");
            }

            Proceso.Nombre = procesoDto.Nombre; 

            _unitOfWorks.Proceso.Update(Proceso);
            _logger.LogDebug("Guardando Datos de Edicion del Proceso:{id} en Base de Datos....",id);
            _unitOfWorks.Save();
            _logger.LogDebug("Datos del Proceso Editado Guardado Correctamente");
            return NoContent();
        }


        [HttpDelete("delete-all")]
        public async Task<IActionResult> DeleteAllProcesos()
        {
            IEnumerable<ProcesoModel> AllProcesos = await _unitOfWorks.Proceso.GetAll();

            if (AllProcesos is null) return NoContent();

            _unitOfWorks.Proceso.RemoveRange(AllProcesos);
            _unitOfWorks.Save();

            return NoContent();

        }
    }
}
