using APSS.Domain.Entities;
using APSS.Domain.Repositories;
using APSS.Domain.ValueTypes;

namespace APSS.Domain.Services
{
    public interface IAccountService
    {
        #region public Methods

        /// <summary>
        /// Asynchronously adds a new account
        /// </summary>
        /// <param name="holderName">The account owner name</param>
        /// <param name="nationalId">The national Id of the owner</param>
        /// <param name="phoneNumber">The phone number of the owner</param>
        /// <param name="socialStatus">The social status of the owner</param>
        /// <param name="job">The job of the accout holder</param>
        /// <param name="user">The user which the account belongs to</param>
        /// <param name="addedBy">The user who added the account</param>
        /// <param name="permissions"> the permessions that the account have</param>
        /// <returns></returns>
        Task<Account> AddAccountAsync(
            long accountId,
            string holderName,
            string? nationalId,
            string? phoneNumber,
            SocialStatus socialStatus,
            string? job,
            User user,
            User addedBy,
            PermissionType permissions

            );

        /// <summary>
        /// gets the account 
        /// </summary>
        /// <param name="accountId"> the account who tries to get the account</param>
        /// <param name="userAccountId">the account to be searched</param>
        /// <returns></returns>
        Task<IQueryBuilder<Account>> GetAccount(long accountId, long userAccountId);

        /// <summary>
        /// updates the account 
        /// </summary>
        /// <param name="accountId"> the account who updates the user account</param>
        /// <param name="userAccountId">the account to update</param>
        /// <returns></returns>
        Task<Account> UpdateAccountAsync(long accountId, long userAccountId);
        /// <summary>
        /// delete the account
        /// </summary>
        /// <param name="accountId">the account who deletes the user account </param>
        /// <param name="userAccountId">The account to be deleted</param>
        /// <returns></returns>
        Task DeleteAccountAsync(long accountId, long userAccountId);

        #endregion
    }
}
