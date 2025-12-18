# ? TC-D12 實作完成：密碼缺少數字驗證

## ?? 實作摘要

### 完成時間
2024 年完成

### 實作的測試案例
**TC-D12**: 驗證失敗 - 密碼缺少數字

---

## ?? 實作內容

### 1. 測試程式碼修正
**檔案**: `src\Domain.RentalCar.Tests\Test_DomainRentalCar.cs`

#### 修正前（有問題的程式碼）
```csharp
[TestMethod]
[DataRow("Test@Password")]
public void Should_ThrowArgumentException_WhenPasswordHasNoDigit(string password)
{
    // Arrange
    var expectedExceptionThrown = false;
    var expectedMessage = "至少包含一個數字";
    
    // Act
    var result = Password.Create(password);

    // Assert - 這個邏輯是錯誤的
    Assert.IsTrue(expectedExceptionThrown,
        "當密碼缺少數字時，應該拋出 ArgumentException");
}
```

**問題分析**:
- ? `expectedExceptionThrown` 永遠是 `false`
- ? 沒有正確捕捉例外
- ? 測試會永遠失敗

#### 修正後（正確的程式碼）
```csharp
[TestMethod]
[DataRow("Test@Password")]
[DataRow("MyPass@word")]
[ExpectedException(typeof(ArgumentException))]
public void Should_ThrowArgumentException_WhenPasswordHasNoDigit(string password)
{
    // Arrange & Act
    var result = Password.Create(password);

    // Assert - 由 ExpectedException 處理
    // 預期拋出包含 "至少包含一個數字" 訊息的 ArgumentException
}
```

**改善點**:
- ? 使用 `[ExpectedException]` 屬性
- ? 自動驗證是否拋出 `ArgumentException`
- ? 加回第二個測試資料 `"MyPass@word"`
- ? 程式碼簡潔且符合 MSTest 慣例

---

### 2. Password.cs 實作
**檔案**: `src\Domain.RentalCarLab1\ValueObjects\Password.cs`

#### 新增的驗證邏輯

```csharp
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

    // 5. 檢查是否包含數字 ? TC-D12 的核心邏輯
    if (!plainPassword.Any(char.IsDigit))
        throw new ArgumentException("密碼至少包含一個數字", nameof(plainPassword));

    // 6. 檢查是否包含特殊字元
    const string specialChars = "!@#$%^&*()_+-=[]{}|;:,.<>?";
    if (!plainPassword.Any(c => specialChars.Contains(c)))
        throw new ArgumentException("密碼至少包含一個特殊字元", nameof(plainPassword));

    // 7. 暫時回傳（待 BCrypt 套件安裝）
    return new Password(plainPassword);
}
```

#### 關鍵技術點

**LINQ 方法使用**:
```csharp
using System.Linq;

// char.IsDigit - .NET Framework 內建方法
if (!plainPassword.Any(char.IsDigit))
    throw new ArgumentException("密碼至少包含一個數字");
```

**工作原理**:
1. `plainPassword.Any()` - 檢查字串中是否有任何字元符合條件
2. `char.IsDigit` - 判斷字元是否為數字 (0-9)
3. `!` - 取反，如果沒有數字則拋出例外

---

## ?? 測試案例分析

### TC-D12: 密碼缺少數字

#### 測試資料 1: `"Test@Password"`
```
字元分析:
  T, e, s, t → 大小寫字母 ?
  @ → 特殊字元 ?
  P, a, s, s, w, o, r, d → 大小寫字母 ?
  數字 → 無 ?

預期結果: 拋出 ArgumentException("密碼至少包含一個數字")
```

#### 測試資料 2: `"MyPass@word"`
```
字元分析:
  M, y, P, a, s, s, w, o, r, d → 大小寫字母 ?
  @ → 特殊字元 ?
  數字 → 無 ?

預期結果: 拋出 ArgumentException("密碼至少包含一個數字")
```

#### 成功案例（對照）: `"Test@1234"`
```
字元分析:
  T, e, s, t → 大小寫字母 ?
  @ → 特殊字元 ?
  1, 2, 3, 4 → 數字 ?

預期結果: 通過驗證，建立 Password 物件
```

---

## ?? 完整的密碼驗證規則

| 規則 | 驗證方法 | 測試案例 | 狀態 |
|------|---------|---------|------|
| 1. 不能為空 | `IsNullOrWhiteSpace` | TC-D08 | ? 已實作 |
| 2. 長度 ? 8 | `.Length` | TC-D09 | ? 已實作 |
| 3. 包含大寫 | `char.IsUpper` | TC-D10 | ? 已實作 |
| 4. 包含小寫 | `char.IsLower` | TC-D11 | ? 已實作 |
| 5. 包含數字 | `char.IsDigit` | **TC-D12** | ? **本次實作** |
| 6. 包含特殊字元 | 自訂字元集 | TC-D13 | ? 已實作 |
| 7. BCrypt 雜湊 | `BCrypt.HashPassword` | TC-D14 | ? 待安裝套件 |

---

## ?? 測試執行結果

### 預期測試狀態

```
? TC-D10: Should_ThrowArgumentException_WhenPasswordHasNoUpperCase
   - "test@1234" → 拋出 ArgumentException ?
   - "mypassword@99" → 拋出 ArgumentException ?

? TC-D11: Should_ThrowArgumentException_WhenPasswordHasNoLowerCase
   - "TEST@1234" → 拋出 ArgumentException ?
   - "MYPASSWORD@99" → 拋出 ArgumentException ?

? TC-D12: Should_ThrowArgumentException_WhenPasswordHasNoDigit
   - "Test@Password" → 拋出 ArgumentException ?
   - "MyPass@word" → 拋出 ArgumentException ?

? TC-D13: Should_ThrowArgumentException_WhenPasswordHasNoSpecialChar
   - "Test1234" → 拋出 ArgumentException ?
   - "MyPassword99" → 拋出 ArgumentException ?

? TC-D14: Should_HashPasswordUsingBCrypt_WhenCreated
   - 原因: BCrypt.Net-Next 套件尚未安裝
   - HashedValue 不是以 "$2" 開頭
```

---

## ?? TDD 進度

```
[?] ?? 紅燈階段 - 測試失敗（已完成）
[?] ?? 綠燈階段 - TC-D12 實作完成
[?] ?? 綠燈階段 - TC-D14 待完成（需安裝 BCrypt）
[  ] ?? 重構階段 - 優化程式碼
```

---

## ?? 程式碼品質檢查

### ? 已遵循的最佳實踐

1. **DDD Value Object 模式**
   - ? 不可變性 (Immutable)
   - ? 私有建構子
   - ? 工廠方法 (Create)
   - ? 相等性比較實作

2. **驗證邏輯**
   - ? 防禦性程式設計 (Guard Clauses)
   - ? 有意義的錯誤訊息
   - ? 參數名稱包含在例外中

3. **程式碼可讀性**
   - ? 使用 LINQ 簡化邏輯
   - ? 註解清楚說明驗證步驟
   - ? 驗證順序由簡單到複雜

4. **測試設計**
   - ? AAA 模式 (Arrange-Act-Assert)
   - ? 參數化測試 (DataRow)
   - ? 使用 ExpectedException 屬性

---

## ?? 下一步

### 立即任務
- [ ] 安裝 BCrypt.Net-Next 套件
  ```powershell
  Install-Package BCrypt.Net-Next -Version 4.0.3
  ```

### 完成 TC-D14
在 `Password.Create()` 中啟用 BCrypt 雜湊：
```csharp
// 7. 使用 BCrypt 進行雜湊
string hashedPassword = BCrypt.Net.BCrypt.HashPassword(plainPassword);
return new Password(hashedPassword);
```

在 `Password.Verify()` 中實作驗證：
```csharp
public bool Verify(string plainPassword)
{
    if (string.IsNullOrWhiteSpace(plainPassword))
        return false;

    return BCrypt.Net.BCrypt.Verify(plainPassword, HashedValue);
}
```

---

## ?? 參考資源

### 相關檔案
- `src\Domain.RentalCarLab1\ValueObjects\Password.cs` - 實作程式碼
- `src\Domain.RentalCar.Tests\Test_DomainRentalCar.cs` - 測試程式碼
- `快速開始_綠燈階段.md` - 實作指南

### .NET Framework 4.8 相容性
- ? `System.Linq` - 支援
- ? `char.IsDigit` - 支援
- ? `[ExpectedException]` - MSTest 支援

---

## ? 成果總結

### TC-D12 實作成果
- ? 測試程式碼已修正
- ? 數字驗證邏輯已實作
- ? 建置成功，無編譯錯誤
- ? 測試應該通過（綠燈）

### 技術亮點
1. **正確使用 LINQ**: `plainPassword.Any(char.IsDigit)`
2. **MSTest 最佳實踐**: `[ExpectedException]` 屬性
3. **清晰的錯誤訊息**: "密碼至少包含一個數字"
4. **完整的驗證鏈**: 6 個驗證規則有序執行

---

**實作日期**: 2024  
**TDD 階段**: ?? 綠燈（TC-D12 完成）  
**建置狀態**: ? 成功  
**測試狀態**: ? 預期通過
