using System;
using Domain.RentalCarLab1.ValueObjects;

namespace Domain.RentalCarLab1
{
    /// <summary>
    /// Account Entity (Aggregate Root)
    /// 代表租車系統的用戶帳戶
    /// </summary>
    public class Account
    {
        /// <summary>
        /// 帳戶唯一識別碼
        /// </summary>
        public Guid AccountId { get; private set; }

        /// <summary>
        /// 用戶名稱
        /// </summary>
        public string Username { get; private set; }

        /// <summary>
        /// 密碼 (使用 Value Object)
        /// </summary>
        public Password Password { get; private set; }

        /// <summary>
        /// 帳戶建立時間
        /// </summary>
        public DateTime CreatedAt { get; private set; }

        /// <summary>
        /// 私有建構子 - 符合 DDD 封裝原則
        /// </summary>
        private Account()
        {
            // For ORM
        }

        /// <summary>
        /// 建立新帳戶
        /// </summary>
        /// <param name="accountId">帳戶ID</param>
        /// <param name="username">用戶名</param>
        /// <param name="password">密碼 Value Object</param>
        public Account(Guid accountId, string username, Password password)
        {
            if (accountId == Guid.Empty)
                throw new ArgumentException("帳戶 ID 不能為空", nameof(accountId));

            if (string.IsNullOrWhiteSpace(username))
                throw new ArgumentException("用戶名不能為空", nameof(username));

            if (password == null)
                throw new ArgumentNullException(nameof(password));

            AccountId = accountId;
            Username = username;
            Password = password;
            CreatedAt = DateTime.UtcNow;
        }

        /// <summary>
        /// 驗證密碼
        /// </summary>
        /// <param name="plainPassword">明文密碼</param>
        /// <returns>密碼是否正確</returns>
        public bool VerifyPassword(string plainPassword)
        {
            return Password.Verify(plainPassword);
        }

        /// <summary>
        /// 變更密碼
        /// </summary>
        /// <param name="newPassword">新密碼 Value Object</param>
        public void ChangePassword(Password newPassword)
        {
            if (newPassword == null)
                throw new ArgumentNullException(nameof(newPassword));

            Password = newPassword;
        }
    }
}
