# TDD_01 Login方法 - 紅燈測試

## 測試框架
- MSTest V2

## 測試目標
測試 Account 領域物件的 Login 方法

## 測試原則
- 一個 Test Case 只測試一個方法
- 遵循 TDD 紅燈測試階段（先寫測試，後寫實作）
- 不測試 userid 與 password 是否為空值
- 須測試 password 格式驗證

## 密碼規則
密碼必須同時包含：
1. 至少一個數字
2. 至少一個英文字母
3. 至少一個特殊字元（#、$、!、@、^）

## 測試案例

```csharp
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RentalCar.Domain.Entities;
using System;

namespace RentalCar.Tests.Domain
{
    [TestClass]
    public class AccountLoginTests
    {
        // 測試案例 1: 密碼缺少數字 - 應該失敗
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void Login_PasswordWithoutNumber_ShouldThrowException()
        {
            // Arrange
            var account = new Account();
            string userId = "testuser";
            string invalidPassword = "Password#";  // 缺少數字
            
            // Act
            account.Login(userId, invalidPassword);
            
            // Assert is handled by ExpectedException
        }

        // 測試案例 2: 密碼缺少英文字母 - 應該失敗
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void Login_PasswordWithoutLetter_ShouldThrowException()
        {
            // Arrange
            var account = new Account();
            string userId = "testuser";
            string invalidPassword = "12345#";  // 缺少英文字母
            
            // Act
            account.Login(userId, invalidPassword);
            
            // Assert is handled by ExpectedException
        }

        // 測試案例 3: 密碼缺少特殊字元 - 應該失敗
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void Login_PasswordWithoutSpecialChar_ShouldThrowException()
        {
            // Arrange
            var account = new Account();
            string userId = "testuser";
            string invalidPassword = "Password123";  // 缺少特殊字元 (#$!@^)
            
            // Act
            account.Login(userId, invalidPassword);
            
            // Assert is handled by ExpectedException
        }

        // 測試案例 4: 密碼格式正確但使用者不存在 - 應該失敗
        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void Login_UserNotExists_ShouldThrowException()
        {
            // Arrange
            var account = new Account();
            string nonExistentUserId = "nonexistentuser";
            string validPassword = "Password123#";
            
            // Act
            account.Login(nonExistentUserId, validPassword);
            
            // Assert is handled by ExpectedException
        }

        // 測試案例 5: 密碼格式正確但密碼錯誤 - 應該失敗
        [TestMethod]
        [ExpectedException(typeof(UnauthorizedAccessException))]
        public void Login_WrongPassword_ShouldThrowException()
        {
            // Arrange
            var account = new Account();
            account.RegisterAccount("testuser", "CorrectPass123#");
            string userId = "testuser";
            string wrongPassword = "WrongPass456!";
            
            // Act
            account.Login(userId, wrongPassword);
            
            // Assert is handled by ExpectedException
        }

        // 測試案例 6: 使用者帳號與密碼格式都正確 - 應該成功
        [TestMethod]
        public void Login_ValidCredentials_ShouldReturnTrue()
        {
            // Arrange
            var account = new Account();
            string userId = "testuser";
            string validPassword = "ValidPass123#";
            account.RegisterAccount(userId, validPassword);
            
            // Act
            bool result = account.Login(userId, validPassword);
            
            // Assert
            Assert.IsTrue(result);
        }

        // 測試案例 7: 密碼包含多個特殊字元 (#$!@^) - 應該成功
        [TestMethod]
        public void Login_PasswordWithMultipleSpecialChars_ShouldReturnTrue()
        {
            // Arrange
            var account = new Account();
            string userId = "testuser";
            string validPassword = "Pass123#$!@^";
            account.RegisterAccount(userId, validPassword);
            
            // Act
            bool result = account.Login(userId, validPassword);
            
            // Assert
            Assert.IsTrue(result);
        }

        // 測試案例 8: 密碼只包含一個允許的特殊字元 (#) - 應該成功
        [TestMethod]
        public void Login_PasswordWithHashSpecialChar_ShouldReturnTrue()
        {
            // Arrange
            var account = new Account();
            string userId = "testuser";
            string validPassword = "MyPassword1#";
            account.RegisterAccount(userId, validPassword);
            
            // Act
            bool result = account.Login(userId, validPassword);
            
            // Assert
            Assert.IsTrue(result);
        }

        // 測試案例 9: 密碼只包含一個允許的特殊字元 ($) - 應該成功
        [TestMethod]
        public void Login_PasswordWithDollarSpecialChar_ShouldReturnTrue()
        {
            // Arrange
            var account = new Account();
            string userId = "testuser";
            string validPassword = "MyPassword1$";
            account.RegisterAccount(userId, validPassword);
            
            // Act
            bool result = account.Login(userId, validPassword);
            
            // Assert
            Assert.IsTrue(result);
        }

        // 測試案例 10: 密碼包含不允許的特殊字元 - 應該失敗
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void Login_PasswordWithInvalidSpecialChar_ShouldThrowException()
        {
            // Arrange
            var account = new Account();
            string userId = "testuser";
            string invalidPassword = "Password123*";  // * 不是允許的特殊字元
            
            // Act
            account.Login(userId, invalidPassword);
            
            // Assert is handled by ExpectedException
        }
    }
}
```

## 測試案例摘要

| 編號 | 測試案例 | 預期結果 | 驗證項目 |
|------|---------|---------|---------|
| 1 | 密碼缺少數字 | ArgumentException | 密碼格式驗證 |
| 2 | 密碼缺少英文字母 | ArgumentException | 密碼格式驗證 |
| 3 | 密碼缺少特殊字元 | ArgumentException | 密碼格式驗證 |
| 4 | 使用者不存在 | InvalidOperationException | 使用者驗證 |
| 5 | 密碼錯誤 | UnauthorizedAccessException | 密碼驗證 |
| 6 | 有效憑證 | True | 登入成功 |
| 7 | 多個特殊字元 | True | 密碼格式驗證 |
| 8 | 特殊字元 # | True | 密碼格式驗證 |
| 9 | 特殊字元 $ | True | 密碼格式驗證 |
| 10 | 不允許的特殊字元 | ArgumentException | 密碼格式驗證 |

## 注意事項

1. **紅燈階段**: 這些測試目前應該會失敗，因為 Account 類別和 Login 方法尚未實作
2. **測試先行**: 符合 TDD 原則，先定義預期行為
3. **單一職責**: 每個測試案例只測試一個特定情境
4. **密碼規則**: 嚴格驗證密碼必須包含數字、字母和允許的特殊字元 (#$!@^)
5. **例外處理**: 使用不同的例外類型區分不同的錯誤情境

## 下一步驟

進入 TDD 綠燈階段，實作 Account 類別的 Login 方法，使所有測試通過。
