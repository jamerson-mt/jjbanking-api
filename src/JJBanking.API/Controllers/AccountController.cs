using JJBanking.Domain.Entities;
using JJBanking.Infra.Context;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace JJBanking.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AccountsController : ControllerBase
{
    private readonly BankDbContext _context;

    public AccountsController(BankDbContext context)
    {
        _context = context;
    }

    // 🚀 POST: api/accounts
    // CRIA UMA NOVA CONTA
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateAccountRequest request)
    {
        // Validação básica
        if (request.InitialDeposit < 0)
            return BadRequest("O depósito inicial não pode ser negativo.");

        var account = new Account(request.Owner, request.Cpf, request.InitialDeposit);

        //verificar se ja existe uma conta existente com o mesmo CPF
        var existingAccount = await _context.Accounts.FirstOrDefaultAsync(a =>
            a.Cpf == request.Cpf
        );
        if (existingAccount != null)
            return BadRequest("Já existe uma conta vinculada a este CPF.");

        // SE NAO EXISTIR, CRIA A CONTA
        _context.Accounts.Add(account);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetById), new { id = account.Id }, account);
    }

    // 🆔 GET: api/accounts/{id}
    // BUSCA UMA CONTA ESPECIFICA
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var account = await _context.Accounts.FindAsync(id);

        if (account == null)
            return NotFound("Conta não encontrada.");

        return Ok(account);
    }
}

// DTO (Data Transfer Object) para não expor a entidade pura no request
public record CreateAccountRequest(string Owner, string Cpf, decimal InitialDeposit);
