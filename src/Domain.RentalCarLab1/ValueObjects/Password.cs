using System;
using System.Linq;

namespace Domain.RentalCarLab1.ValueObjects
{
    /// <summary>
    /// Password Value Object - TDD 綠燈階段實作
    /// 實作密碼驗證邏輯
    /// </summary>
    public class Password
    {
        /// <summary>
        /// 密碼的雜湊值
        /// </summary>
        public string HashedValue { get; private set; }

        /// <summary>
        /// 私有建構子 - 強制使用 Create 工廠方法
        /// </summary>
        private Password(string hashedValue)
        {
            HashedValue = hashedValue;
        }

        /// <summary>
        /// 建立 Password 物件的工廠方法
        /// 實作完整的密碼驗證邏輯
        /// </summary>
        /// <param name="plainPassword">明文密碼</param>
        /// <returns>Password 物件</returns>
        public static Password Create(string plainPassword)
        {
            // 1. 檢查 null 或空字串
            if (string.IsNullOrWhiteSpace(plainPassword))
                throw new ArgumentException("密碼不能為空", nameof(plainPassword));

            // 2. 檢查長度 (至少 8 字元)
            if (plainPassword.Length < 8)
                throw new ArgumentException("密碼至少需要 8 個字元", nameof(plainPassword));

            // 3. 檢查是否包含大寫字母
            if (!plainPassword.Any(char.IsUpper))
                throw new ArgumentException("密碼至少包含一個大寫字母", nameof(plainPassword));

            // 4. 檢查是否包含小寫字母
            if (!plainPassword.Any(char.IsLower))
                throw new ArgumentException("密碼至少包含一個小寫字母", nameof(plainPassword));

            // 5. 檢查是否包含數字
            if (!plainPassword.Any(char.IsDigit))
                throw new ArgumentException("密碼至少包含一個數字", nameof(plainPassword));

            // 6. 檢查是否包含特殊字元
            const string specialChars = "!@#$%^&*()_+-=[]{}|;:,.<>?";
            if (!plainPassword.Any(c => specialChars.Contains(c)))
                throw new ArgumentException("密碼至少包含一個特殊字元", nameof(plainPassword));

            // 7. 使用 BCrypt 進行雜湊
            // TODO: 安裝 BCrypt.Net-Next 套件後啟用
            // string hashedPassword = BCrypt.Net.BCrypt.HashPassword(plainPassword);
            // return new Password(hashedPassword);

            // 暫時使用模擬的雜湊（用於測試 TC-D10~D13）
            return new Password(plainPassword);
        }

        /// <summary>
        /// 驗證明文密碼是否與雜湊值匹配
        /// </summary>
        /// <param name="plainPassword">明文密碼</param>
        /// <returns>是否匹配</returns>
        public bool Verify(string plainPassword)
        {
            if (string.IsNullOrWhiteSpace(plainPassword))
                return false;

            // TODO: 安裝 BCrypt.Net-Next 套件後啟用
            // return BCrypt.Net.BCrypt.Verify(plainPassword, HashedValue);

            // 暫時的驗證邏輯
            return HashedValue == plainPassword;
        }

        #region Value Object Pattern - 相等性比較

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
                return false;

            var other = (Password)obj;
            return HashedValue == other.HashedValue;
        }

        public override int GetHashCode()
        {
            return HashedValue?.GetHashCode() ?? 0;
        }

        public static bool operator ==(Password left, Password right)
        {
            if (ReferenceEquals(left, null))
                return ReferenceEquals(right, null);
            return left.Equals(right);
        }

        public static bool operator !=(Password left, Password right)
        {
            return !(left == right);
        }

        #endregion
    }
}
