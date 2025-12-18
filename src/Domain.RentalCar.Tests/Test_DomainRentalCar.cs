using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Domain.RentalCarLab1.ValueObjects;

namespace Domain.RentalCar.Tests
{
    /// <summary>
    /// Password Value Object 的 TDD 測試
    /// 紅燈階段：測試會失敗，因為 Password.Create 尚未實作驗證邏輯
    /// </summary>
    [TestClass]
    public class Test_DomainRentalCar
    {
        [TestMethod]
        public void TestMethod1()
        {

        }

        #region TC-D10: 驗證失敗 - 密碼缺少大寫字母

        [TestMethod]
        [DataRow("test@1234")]
        [DataRow("mypassword@99")]
        public void Should_ThrowArgumentException_WhenPasswordHasNoUpperCase(string password)
        {
            // Arrange
            var expectedExceptionMessage = "至少包含一個大寫字母";
            ArgumentException actualException = null;

            // Act
            try
            {
                var result = Password.Create(password);
            }
            catch (ArgumentException ex)
            {
                actualException = ex;
            }

            // Assert
            Assert.IsNotNull(actualException, "應該拋出 ArgumentException");
            Assert.IsTrue(
                actualException.Message.Contains(expectedExceptionMessage),
                $"例外訊息應包含 '{expectedExceptionMessage}'，實際訊息為: {actualException.Message}"
            );
        }

        #endregion

        #region TC-D11: 驗證失敗 - 密碼缺少小寫字母

        [TestMethod]
        [DataRow("TEST@1234")]
        [DataRow("MYPASSWORD@99")]
        public void Should_ThrowArgumentException_WhenPasswordHasNoLowerCase(string password)
        {
            // Arrange
            var expectedExceptionMessage = "至少包含一個小寫字母";
            ArgumentException actualException = null;

            // Act
            try
            {
                var result = Password.Create(password);
            }
            catch (ArgumentException ex)
            {
                actualException = ex;
            }

            // Assert
            Assert.IsNotNull(actualException, "應該拋出 ArgumentException");
            Assert.IsTrue(
                actualException.Message.Contains(expectedExceptionMessage),
                $"例外訊息應包含 '{expectedExceptionMessage}'，實際訊息為: {actualException.Message}"
            );
        }

        #endregion

        #region TC-D12: 驗證失敗 - 密碼缺少數字

        [TestMethod]
        [DataRow("Test2@Password")]
        [DataRow("MyPass2@word")]
        public void Should_ThrowArgumentException_WhenPasswordHasNoDigit(string password)
        {
            // Arrange
            var expectedExceptionMessage = "至少包含一個數字";
            ArgumentException actualException = null;

            Password actual = Password.Create(password);

            // Act
            try
            {
                var result = Password.Create(password);
            }
            catch (ArgumentException ex)
            {
                actualException = ex;
            }

            // Assert
            Assert.IsNotNull(actualException, "應該拋出 ArgumentException");
            Assert.IsTrue(
                actualException.Message.Contains(expectedExceptionMessage),
                $"例外訊息應包含 '{expectedExceptionMessage}'，實際訊息為: {actualException.Message}"
            );
        }

        #endregion

        #region TC-D13: 驗證失敗 - 密碼缺少特殊字元

        [TestMethod]
        [DataRow("Test1234")]
        [DataRow("MyPassword99")]
        public void Should_ThrowArgumentException_WhenPasswordHasNoSpecialChar(string password)
        {
            // Arrange
            var expectedExceptionMessage = "至少包含一個特殊字元";
            ArgumentException actualException = null;

            // Act
            try
            {
                var result = Password.Create(password);
            }
            catch (ArgumentException ex)
            {
                actualException = ex;
            }

            // Assert
            Assert.IsNotNull(actualException, "應該拋出 ArgumentException");
            Assert.IsTrue(
                actualException.Message.Contains(expectedExceptionMessage),
                $"例外訊息應包含 '{expectedExceptionMessage}'，實際訊息為: {actualException.Message}"
            );
        }

        #endregion

        #region TC-D14: 密碼雜湊 - 使用 BCrypt

        [TestMethod]
        public void Should_HashPasswordUsingBCrypt_WhenCreated()
        {
            // Arrange
            var plainPassword = "Test@1234";

            // Act
            var password = Password.Create(plainPassword);

            // Assert
            // 檢查 BCrypt 雜湊前綴 (以 "$2" 開頭)
            Assert.IsTrue(
                password.HashedValue.StartsWith("$2"),
                "BCrypt 雜湊值應該以 '$2' 開頭"
            );

            // 檢查 BCrypt 固定長度 (60 字元)
            Assert.AreEqual(
                60,
                password.HashedValue.Length,
                "BCrypt 雜湊值長度應該是 60 字元"
            );
        }

        #endregion
    }
}
