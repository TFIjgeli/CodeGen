using CodeGen.Application.DatabaseEntity.Queries.GetAllTables;
using CodeGen.Application.DatabaseEntity.Queries.GetTablesInfo;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CodeGen.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DatabaseController : ControllerBase
    {
        private readonly IMediator _mediator;

        public DatabaseController(IMediator mediator)
        {
            this._mediator = mediator;
        }

        /// <summary>
        /// Get All Tables Query
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("table/get")]
        public async Task<ActionResult<List<GetAllTablesQueryDto>>> GetAllTables(GetAllTablesQuery query)
        {
            var response = await _mediator.Send(query);

            if (response.Error)
                return BadRequest(response.ModelStateError);

            return Ok(response.Data);
        }


        /// <summary>
        /// Get tables information
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("table/info/get")]
        public async Task<ActionResult<List<GetTablesInfoQueryDto>>> GetTablesInfo(GetTablesInfoQuery query)
        {
            var response = await _mediator.Send(query);

            if (response.Error)
                return BadRequest(response.ModelStateError);

            return Ok(response.Data);
        }
    }
}
