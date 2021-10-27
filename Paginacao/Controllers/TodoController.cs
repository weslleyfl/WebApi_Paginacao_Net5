using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Paginacao.Data;
using Paginacao.Filter;
using Paginacao.Model;
using Paginacao.Wrappers;

namespace Paginacao.Controllers
{
    // https://codewithmukesh.com/blog/pagination-in-aspnet-core-webapi/

    [Route("api/v1/todos")]
    [ApiController]
    public class TodoController : ControllerBase
    {
        private readonly AppDbContext _context;

        public TodoController(AppDbContext context)
        {
            _context = context;
        }


        [HttpGet("load")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> LoadAsync()
        {

            if (await _context.Todos?.CountAsync() > 0)
            {
                return NoContent();
            }

            for (int i = 0; i < 1348; i++)
            {
                var todo = new Todo()
                {
                    Id = i + 1,
                    Done = false,
                    CreatedAt = DateTime.Now,
                    Title = $"Tarefa numero {i}"
                };

                await _context.AddAsync(todo).ConfigureAwait(false);
            }

            await _context.SaveChangesAsync().ConfigureAwait(false);

            return Ok();
        }

        /// <summary>
        /// Com paginação um pouco mais completa
        /// GET: api/Todos
        /// </summary>
        /// <param name="filter">PaginationFilter object</param>
        [HttpGet()]
        [ProducesDefaultResponseType]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(PagedResponse<>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetAsync([FromQuery] PaginationFilter filter)
        {

            var validarFiltro = new PaginationFilter(filter.PageNumber, filter.PageSize);
            var todos = await _context
                .Todos
                .AsNoTracking()
                .Skip((filter.PageNumber - 1) * filter.PageSize)
                .Take(filter.PageSize)
                .ToListAsync();

            return Ok(new PagedResponse<List<Todo>>(todos, filter.PageNumber, filter.PageSize));

        }

        // [HttpGet("lista-todos/{skip:int}/{take:int}")] // vai aparecer na rota tipo [FromRoute] lista-todos/1/10
        [HttpGet("list-todos")] // [FromQuery] - fica assim: list-todos?skip=1&take=25" 
        public async Task<ActionResult<IEnumerable<Todo>>> GetTodos([FromQuery] int skip = 1, [FromQuery] int take = 25)
        {
            if (take > 100)
            {
                return BadRequest(new { Error = "Valor maximo para retorno da api e 100 registros" });
            }

            var total = await _context.Todos.CountAsync();

            var todos = await _context
               .Todos
               .AsNoTracking()
               .Skip((skip - 1) * take)
               .Take(take)
               .ToListAsync();

            //return Ok(new Response<List<Todo>>(todos)); // retorno simples
            return Ok(new PagedResponse<List<Todo>>(todos, skip, take)
            {
                TotalRecords = total
            });
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Todo>> GetTodo(int id)
        {
            var todo = await _context.Todos.FindAsync(id);

            if (todo == null)
            {
                return NotFound();
            }

            return todo;
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutTodo(int id, Todo todo)
        {
            if (id != todo.Id)
            {
                return BadRequest();
            }

            _context.Entry(todo).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TodoExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        [HttpPost]
        public async Task<ActionResult<Todo>> PostTodo(Todo todo)
        {
            _context.Todos.Add(todo);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetTodo", new { id = todo.Id }, todo);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTodo(int id)
        {
            var todo = await _context.Todos.FindAsync(id);
            if (todo == null)
            {
                return NotFound();
            }

            _context.Todos.Remove(todo);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool TodoExists(int id)
        {
            return _context.Todos.Any(e => e.Id == id);
        }
    }
}
