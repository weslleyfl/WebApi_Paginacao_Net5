using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Paginacao.Data;
using Paginacao.Filter;
using Paginacao.Helpers;
using Paginacao.Model;
using Paginacao.Wrappers;
using Paginacao.Services;

namespace Paginacao.Controllers
{
    [Route("api/v1/customers")]
    [ApiController]
    public class CustomerController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IUriService _uriService;

        public CustomerController(AppDbContext context, IUriService uriService)
        {
            _context = context;
            _uriService = uriService;
        }


        [HttpGet("")]
        public async Task<IActionResult> GetAll([FromQuery] PaginationFilter filter)
        {
            var route = Request.Path.Value;
            var validFilter = new PaginationFilter(filter.PageNumber, filter.PageSize);
            var pagedData = await _context.Customers
                .AsNoTracking()
                .Skip((validFilter.PageNumber - 1) * validFilter.PageSize) // Pula um certo conjunto de registros
                .Take(validFilter.PageSize) // Devolver apenas a quantidade necessária de dados
                .ToListAsync();

            var totalRecords = await _context.Customers.AsNoTracking().CountAsync();
            var pagedReponse = PaginationHelper.CreatePagedReponse<Customer>(
                pagedData,
                validFilter, 
                totalRecords, 
                _uriService, 
                route);
            
            return Ok(pagedReponse);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var customer = await _context.Customers
                .AsNoTracking()               
                .FirstOrDefaultAsync(p => p.Id == id);

            return Ok(new Response<Customer>(customer));
        }
    }
}
