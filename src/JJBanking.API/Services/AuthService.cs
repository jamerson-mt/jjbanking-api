using JJBanking.API.Utils;
using JJBanking.Domain.DTOs;
using JJBanking.Domain.Entities;
using JJBanking.Domain.Interfaces;
using JJBanking.Infra.Context;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using static JJBanking.API.Utils.CpfValidator;

public class AuthService : IAuthService
{
    private readonly UserManager<User> _userManager;
    private readonly BankDbContext _context;

    public AuthService(UserManager<User> userManager, BankDbContext context)
    {
        _userManager = userManager;
        _context = context;
    }

    // Corrigido o tipo de retorno para AuthResponse para bater com a Interface
    public async Task<AuthResponse> RegisterAsync(AccountRegister request)
    {
        using var transaction = await _context.Database.BeginTransactionAsync();

        try
        {
            var user = new User
            {
                UserName = request.Email,
                Email = request.Email,
                FullName = request.FullName,
                Cpf = request.Cpf,
            };

            // validation Password
            if (!PasswordValidator.IsStrong(request.Password))
            {
                throw new Exception("Senha não cumpre os requisitos de complexidade");
            }

            //valida CPF (formato e quantidade de dígitos)
            if (!IsValidCpf(user.Cpf))
            {
                throw new Exception("CPF inválido. Informe um CPF realmente válido.");
            }

            bool cpfExists = await _userManager.Users.AnyAsync(u => u.Cpf == user.Cpf); // Verifica se o CPF já existe no banco de dados
            if (cpfExists)
            {
                throw new Exception("CPF já cadastrado. Informe um CPF diferente.");
            }

            // 1. Cria o usuário
            var result = await _userManager.CreateAsync(user, request.Password); // Cria o usuário com a senha fornecida
            if (!result.Succeeded)
            {
                var error = result.Errors.FirstOrDefault()?.Description ?? "Erro ao criar usuário.";
                throw new Exception(error);
            }

            // 2. Gera número de conta ÚNICO (com check no banco)
            var _random = new Random();
            string accountNumber;
            bool exists;

            do
            {
                accountNumber = $"{_random.Next(1000, 9999)}-{_random.Next(0, 9)}";
                // Verifica se já existe uma conta com esse número no banco
                exists = await _context.Accounts.AnyAsync(a => a.AccountNumber == accountNumber);
            } while (exists);

            // 3. Cria a conta
            var account = new Account(user.Id, 0m, accountNumber);
            _context.Accounts.Add(account);
            await _context.SaveChangesAsync();

            await transaction.CommitAsync(); // Se tudo deu certo, confirma a transação no banco

            // Implementar geração de token JWT aqui, por enquanto retorna um token provisório
            return new AuthResponse("token_provisorio", accountNumber, user.FullName);
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    public async Task<AuthResponse> AuthenticateAsync(string email, string password)
    {
        var user = await _userManager.FindByEmailAsync(email);

        if (user == null || !await _userManager.CheckPasswordAsync(user, password))
        {
            throw new Exception("Credenciais inválidas.");
        }

        // Busca o número da conta vinculado ao usuário
        var account = await _context.Accounts.FirstOrDefaultAsync(a => a.UserId == user.Id);

        var accountNumber = account?.AccountNumber ?? "0000-0";

        // Aqui geraremos o JWT em breve
        return new AuthResponse("token_real_vindo_daqui_a_pouco", accountNumber, user.FullName);
    }
}
