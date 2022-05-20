using CodeGen.Application.DatabaseEntity.Queries.GenerateTableSelect;
using CodeGen.Application.DynamicCrud.Command.CreateTable;
using CodeGen.Application.DynamicCrud.Command.DeleteTable;
using CodeGen.Application.DynamicCrud.Command.UpdateTable;
using CodeGen.Application.DynamicCrud.Queries;
using CodeGen.Application.DynamicCrud.Queries.GetTableById;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Collections.Generic;
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
        /// Search
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="currentPage"></param>
        /// <param name="pageSize"></param>
        /// <param name="searchQuery"></param>
        /// <param name="joinQuery"></param>
        /// <param name="filterQuery"></param>
        /// <param name="tableFields"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("{tableName}")]
        public async Task<ActionResult<object>> Get(string tableName, int? currentPage, int? pageSize, [FromQuery]string searchQuery, [FromQuery]string joinQuery, [FromQuery]string filterQuery, [FromQuery]string tableFields)
        {
            var response = await _mediator.Send(new GetTableQuery(tableName, currentPage, pageSize, searchQuery, joinQuery, filterQuery, tableFields));

            if (response.Error)
                return BadRequest(response.ModelStateError);

            return Ok(response.Data);
        }

        // <summary>
        /// SELECT GET
        /// </summary>
        /// <param name="query"></param>
        /// <param name="tableName"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("{tableName}/get")]
        public async Task<ActionResult<string>> GenerateQuery([FromBody] TableSelectPost query, [FromRoute] string tableName)
        {

            var response = await _mediator.Send(new GetTableQuery(tableName,
                                                                  query.CurrentPage,
                                                                  query.PageSize,
                                                                  JsonConvert.SerializeObject(query.SearchQuery),
                                                                  JsonConvert.SerializeObject(query.JoinQuery),
                                                                  JsonConvert.SerializeObject(query.FilterQuery),
                                                                  JsonConvert.SerializeObject(query.TableFields)
                                                                  ));

            if (response.Error)
                return BadRequest(response.ModelStateError);

            return Ok(response.Data);
        }


        /// <summary>
        /// Get by Id
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("{tableName}/{id:int}")]
        public async Task<ActionResult<object>> Get(string tableName, int id)
        {
            var response = await _mediator.Send(new GetTableByIdQuery(id, tableName));

            if (response.Error)
                return BadRequest(response.ModelStateError);

            return Ok(response.Data);
        }


        /// <summary>
        /// Create 
        /// </summary>
        /// <param name="command"></param>
        /// <param name="tableName"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("{tableName}")]
        public async Task<ActionResult<object>> Create(CreateTableCommand command, string tableName)
        {
            command.TableName = tableName;
            var response = await _mediator.Send(command);

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
        public async Task<ActionResult<bool>> Update(UpdateTableCommand command, string tableName, int id)
        {
            command.Id = id;
            command.TableName = tableName;
            var response = await _mediator.Send(command);

            if (response.Error)
                return BadRequest(response.ModelStateError);

            return Ok(response.Data);
        }


        /// <summary>
        /// Delete
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete]
        [Route("{tableName}/{id:int}")]
        public async Task<ActionResult<bool>> Delete(string tableName, int id)
        {
            var response = await _mediator.Send(new DeleteTableCommand(id, tableName));

            if (response.Error)
                return BadRequest(response.ModelStateError);

            return Ok(response.Data);
        }
    }
}
