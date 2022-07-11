using APSS.Domain.Entities;
using APSS.Domain.Repositories;
using APSS.Domain.Repositories.Extensions;
using APSS.Domain.Services;

using System.Security.Cryptography;
using System.Text;

namespace APSS.Application.App;

public sealed class AccountsService : IAccountsService
{
    #region Fields

    private readonly IPermissionsService _permissionsSvc;
    private readonly IUnitOfWork _uow;

    #endregion Fields

    #region Public Constructors

    /// <summary>
    /// Default constructor
    /// </summary>
    /// <param name="permissionsSvc">The permissions managment service</param>
    public AccountsService(IUnitOfWork uow, IPermissionsService permissionsSvc)
    {
        _uow = uow;
        _permissionsSvc = permissionsSvc;
    }

    #endregion Public Constructors

    #region Public Methods

    /// <summary>
    /// Computes the sha256 hash of a string
    /// </summary>
    /// <param name="password">The password to hash</param>
    /// <returns>The hash</returns>
    public static string HashPassword(string password)
    {
        using var sha256 = SHA256.Create();

        var hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));

        return String.Join(string.Empty, hashBytes.Select(b => b.ToString("x2")));
    }

    /// <inheritdoc/>
    public async Task<Account> CreateAccountAsync(
        long superUserAccountId,
        long userId,
        string holderName,
        string password,
        PermissionType permissions)
    {
        var superAccount = await _permissionsSvc
            .ValidatePermissionsAsync(superUserAccountId, userId, permissions);

        var user = await _uow.Users.Query().FindAsync(userId);
        var account = new Account
        {
            User = user,
            HolderName = holderName,
            PasswordHash = HashPassword(password),
            Permissions = permissions,
            AddedBy = superAccount.User,
        };

        _uow.Accounts.Add(account);
        await _uow.CommitAsync();

        return account;
    }

    #endregion Public Methods
}