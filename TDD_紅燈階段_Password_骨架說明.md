# TDD 紅燈階段 - Password Value Object 骨架程式碼

## ?? 目錄結構

```
Mermaid_Lab03/
├── Domain.RentalCarLab1/
│   ├── Account.cs                    ? 已建立
│   └── ValueObjects/
│       └── Password.cs               ? 已建立 (骨架)
└── Domain.RentalCar.Tests/
    └── Test_DomainRentalCar.cs       ? 已建立測試
```

## ?? TDD 紅燈階段說明

### 目前狀態
所有測試案例已建立，但 **預期會失敗**，因為 `Password.Create()` 方法尚未實作驗證邏輯。

### 已建立的檔案

#### 1. `Domain.RentalCarLab1\ValueObjects\Password.cs`
- ? 基本類別結構
- ? `HashedValue` 屬性
- ? `Create()` 工廠方法（骨架）
- ? `Verify()` 方法（骨架）
- ? Value Object 相等性比較
- ? **尚未實作**：所有驗證邏輯
- ? **尚未實作**：BCrypt 雜湊

#### 2. `Domain.RentalCarLab1\Account.cs`
- ? Account Entity 基本結構
- ? 整合 Password Value Object
- ? `VerifyPassword()` 方法
- ? `ChangePassword()` 方法

#### 3. `Domain.RentalCar.Tests\Test_DomainRentalCar.cs`
- ? TC-D10: 驗證密碼缺少大寫字母
- ? TC-D11: 驗證密碼缺少小寫字母
- ? TC-D12: 驗證密碼缺少數字
- ? TC-D13: 驗證密碼缺少特殊字元
- ? TC-D14: 驗證 BCrypt 雜湊

## ?? 待實作功能 (綠燈階段)

在 `Password.Create()` 方法中需要實作：

### 1. 空值驗證
```csharp
if (string.IsNullOrWhiteSpace(plainPassword))
    throw new ArgumentException("密碼不能為空", nameof(plainPassword));
```

### 2. 長度驗證
```csharp
if (plainPassword.Length < 8)
    throw new ArgumentException("密碼至少需要 8 個字元", nameof(plainPassword));
```

### 3. 大寫字母驗證
```csharp
if (!plainPassword.Any(char.IsUpper))
    throw new ArgumentException("密碼至少包含一個大寫字母", nameof(plainPassword));
```

### 4. 小寫字母驗證
```csharp
if (!plainPassword.Any(char.IsLower))
    throw new ArgumentException("密碼至少包含一個小寫字母", nameof(plainPassword));
```

### 5. 數字驗證
```csharp
if (!plainPassword.Any(char.IsDigit))
    throw new ArgumentException("密碼至少包含一個數字", nameof(plainPassword));
```

### 6. 特殊字元驗證
```csharp
string specialChars = "!@#$%^&*()_+-=[]{}|;:,.<>?";
if (!plainPassword.Any(c => specialChars.Contains(c)))
    throw new ArgumentException("密碼至少包含一個特殊字元", nameof(plainPassword));
```

### 7. BCrypt 雜湊
```csharp
// 需要安裝 BCrypt.Net-Next NuGet 套件
string hashedPassword = BCrypt.Net.BCrypt.HashPassword(plainPassword);
return new Password(hashedPassword);
```

## ?? 必要的 NuGet 套件

### Domain.RentalCarLab1 專案
```powershell
Install-Package BCrypt.Net-Next -Version 4.0.3
```

### Domain.RentalCar.Tests 專案
```powershell
# 已安裝
- MSTest.TestAdapter 2.2.10
- MSTest.TestFramework 2.2.10

# 建議安裝 (可選)
Install-Package FluentAssertions -Version 6.12.0
```

## ?? 執行測試

### Visual Studio
1. 開啟 Test Explorer (測試 → 測試總管)
2. 點選 "執行所有測試"
3. **預期結果**：所有測試應該 **失敗** ?

### 命令列
```powershell
cd Domain.RentalCar.Tests
dotnet test
```

### 預期測試結果（紅燈）

```
測試執行摘要
  總計: 6
  通過: 0
  失敗: 6

? TC-D10: Should_ThrowArgumentException_WhenPasswordHasNoUpperCase
   原因: 沒有拋出 ArgumentException

? TC-D11: Should_ThrowArgumentException_WhenPasswordHasNoLowerCase
   原因: 沒有拋出 ArgumentException

? TC-D12: Should_ThrowArgumentException_WhenPasswordHasNoDigit
   原因: 沒有拋出 ArgumentException

? TC-D13: Should_ThrowArgumentException_WhenPasswordHasNoSpecialChar
   原因: 沒有拋出 ArgumentException

? TC-D14: Should_HashPasswordUsingBCrypt_WhenCreated
   原因: HashedValue 不是以 "$2" 開頭
```

## ?? TDD 流程

### 目前階段：?? 紅燈
- [x] 建立測試案例
- [x] 建立骨架程式碼
- [x] 確認測試失敗

### 下一階段：?? 綠燈
- [ ] 實作 `Password.Create()` 驗證邏輯
- [ ] 實作 BCrypt 雜湊
- [ ] 實作 `Password.Verify()` 方法
- [ ] 確認所有測試通過

### 最後階段：?? 重構
- [ ] 優化程式碼結構
- [ ] 移除重複邏輯
- [ ] 改善可讀性
- [ ] 保持測試綠燈

## ?? 相關文件

- [UC1 需求規格](.github/prompts/UC1.prompt.md)
- [Hexagonal Architecture 指南](.github/copilot-instructions.md)
- [TDD 測試設計](.github/prompts/UC1.prompt.md#-tdd-紅燈測試案例設計)

## ?? 已知問題

### 專案引用
測試專案需要引用 Domain 專案。如果 Visual Studio 未自動添加引用，請手動執行：

**方案 1: 使用 Visual Studio**
1. 右鍵點選 `Domain.RentalCar.Tests` 專案
2. 加入 → 參考
3. 勾選 `Domain.RentalCarLab1`
4. 確定

**方案 2: 手動編輯 .csproj**
在 `Domain.RentalCar.Tests.csproj` 加入：
```xml
<ItemGroup>
  <ProjectReference Include="..\Domain.RentalCarLab1\Domain.RentalCarLab1.csproj" />
</ItemGroup>
```

## ?? 下一步

1. **安裝 BCrypt.Net-Next 套件**
   ```powershell
   Install-Package BCrypt.Net-Next -Version 4.0.3
   ```

2. **實作驗證邏輯**
   - 在 `Password.cs` 中實作所有驗證規則

3. **執行測試**
   - 確認測試從紅燈變為綠燈

4. **重構**
   - 優化程式碼品質

---

**建立時間**: 2024
**TDD 階段**: ?? 紅燈
**狀態**: 骨架程式碼已完成，準備進入綠燈階段
