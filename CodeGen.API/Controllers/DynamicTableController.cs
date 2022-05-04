using CodeGen.Application.DynamicCrud.Command.UpdateTable;
using CodeGen.Application.DynamicCrud.Queries;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace CodeGen.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DynamicTableController : ControllerBase
    {
        private readonly IMediator _mediator;

        public DynamicTableController(IMediator mediator)
        {
            this._mediator = mediator;
        }

        /// <summary>
        /// Get 
        /// </summary>
        /// <param name="tableName"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("{tableName}")]
        public async Task<ActionResult<object>> Get(string tableName)
        {
            var response = await _mediator.Send(new GetTableQuery(tableName));

            if (response.Error)
                return BadRequest(response.ModelStateError);

            return Ok(response.Data);
        }


        /// <summary>
        /// Update
        /// </summary>
        /// <param name="command"></param
        /// <param name="tableName"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("{tableName}/{id:int}")]
        public async Task<ActionResult<object>> Update(UpdateTableCommand command, string tableName, int id)
        {
            command.Id = id;
            command.TableName = tableName;
            var response = await _mediator.Send(command);

            if (response.Error)
                return BadRequest(response.ModelStateError);

            return Ok(response.Data);
        }
    }
}
