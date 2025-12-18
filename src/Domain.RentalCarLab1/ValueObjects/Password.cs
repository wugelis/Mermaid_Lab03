using System;

namespace Domain.RentalCarLab1.ValueObjects
{
    /// <summary>
    /// Password Value Object - TDD 紅燈階段骨架
    /// 此類別目前只提供最基本的結構，讓測試能夠編譯但會失敗
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
        /// TDD 紅燈階段：目前不做任何驗證，測試應該會失敗
        /// </summary>
        /// <param name="plainPassword">明文密碼</param>
        /// <returns>Password 物件</returns>
        public static Password Create(string plainPassword)
        {
            // TODO: TDD 紅燈階段 - 需要實作以下驗證：
            // 1. 檢查 null 或空字串
            // 2. 檢查長度 (至少 8 字元)
            // 3. 檢查是否包含大寫字母
            // 4. 檢查是否包含小寫字母
            // 5. 檢查是否包含數字
            // 6. 檢查是否包含特殊字元
            // 7. 使用 BCrypt 進行雜湊

            // 暫時只回傳一個簡單的 Password 物件（不做任何驗證和雜湊）
            return new Password(plainPassword);
        }

        /// <summary>
        /// 驗證明文密碼是否與雜湊值匹配
        /// TDD 紅燈階段：未實作
        /// </summary>
        /// <param name="plainPassword">明文密碼</param>
        /// <returns>是否匹配</returns>
        public bool Verify(string plainPassword)
        {
            // TODO: TDD 紅燈階段 - 需要使用 BCrypt.Verify 進行驗證
            throw new NotImplementedException("尚未實作密碼驗證功能");
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
