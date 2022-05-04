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
    }
}
