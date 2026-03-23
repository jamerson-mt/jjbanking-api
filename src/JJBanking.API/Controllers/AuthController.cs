using JJBanking.Domain.DTOs;
using JJBanking.Domain.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace JJBanking.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Tags("Autenticação")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    /// <summary>
    /// Registra um novo usuário e cria uma conta bancária vinculada.
    /// </summary>
    /// <param name="request">Dados de cadastro (Email, Senha, Nome, CPF)</param>
    /// <returns>Retorna o Token JWT e o Número da Conta gerado.</returns>
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] AccountRegister request)
    {
        // O Controller apenas valida se o modelo é válido e chama o Service
        if (!ModelState.IsValid)
            return BadRequest(ModelState); // 

        try
        {
            // O Service é quem tem a lógica de negócio, o Controller só orquestra
            var result = await _authService.RegisterAsync(request);
            return CreatedAtAction(nameof(Register), new { id = result.AccountNumber }, result); // 201 Created com localização do recurso
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    // POST: /api/auth/login
    /// <summary>
    /// Realiza o login do usuário e retorna um Token JWT para autenticação nas próximas requisições.
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            // Chama a autenticação que verifica e-mail e senha
            var result = await _authService.AuthenticateAsync(request.Email, request.Password);
            return Ok(result);
        }
        catch (Exception ex)
        {
            // 401 Unauthorized é mais apropriado para falhas de login
            return Unauthorized(new { error = ex.Message });
        }
    }
}
