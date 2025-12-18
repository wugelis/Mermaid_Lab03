# ? TDD 紅燈階段完成總結

## ?? 完成的工作

### 1. 建立 Password Value Object 骨架
**檔案**: `Domain.RentalCarLab1\ValueObjects\Password.cs`

```csharp
? 已實作:
  - 基本類別結構
  - HashedValue 屬性
  - Create() 工廠方法（骨架，不做驗證）
  - Verify() 方法（拋出 NotImplementedException）
  - Value Object 相等性比較 (Equals, GetHashCode, ==, !=)

? 尚未實作（紅燈階段預期）:
  - 密碼驗證規則（長度、大小寫、數字、特殊字元）
  - BCrypt 雜湊功能
  - 密碼比對功能
```

### 2. 建立 Account Entity
**檔案**: `Domain.RentalCarLab1\Account.cs`

```csharp
? 已實作:
  - Account Entity 基本結構
  - 整合 Password Value Object
  - VerifyPassword() 方法
  - ChangePassword() 方法
  - DDD 封裝原則（私有 setter）
```

### 3. 建立單元測試
**檔案**: `Domain.RentalCar.Tests\Test_DomainRentalCar.cs`

```csharp
? 測試案例:
  - TC-D10: 驗證密碼缺少大寫字母 [2 個測試案例]
  - TC-D11: 驗證密碼缺少小寫字母 [2 個測試案例]
  - TC-D12: 驗證密碼缺少數字 [2 個測試案例]
  - TC-D13: 驗證密碼缺少特殊字元 [2 個測試案例]
  - TC-D14: 驗證 BCrypt 雜湊 [1 個測試案例]

總計: 6 個測試方法 (含多個 DataRow 參數)
```

---

## ?? 建置狀態

```
? 建置成功
   - Domain.RentalCarLab1 專案: 成功編譯
   - Domain.RentalCar.Tests 專案: 成功編譯
   - 無編譯錯誤
```

---

## ?? 測試狀態（預期紅燈）

### 為什麼測試會失敗？

#### TC-D10 ~ TC-D13（驗證測試）
```
狀態: ? 失敗
原因: Password.Create() 目前不做任何驗證
      直接回傳 new Password(plainPassword)
      不會拋出 ArgumentException
```

#### TC-D14（雜湊測試）
```
狀態: ? 失敗
原因: Password.Create() 直接儲存明文密碼
      HashedValue = plainPassword (不是以 "$2" 開頭)
      長度不是 60 字元
```

---

## ?? 測試案例詳細說明

### TC-D10: 密碼缺少大寫字母
```csharp
測試資料:
  - "test@1234"
  - "mypassword@99"

預期行為: 拋出 ArgumentException
實際行為: 正常建立 Password 物件（無驗證）
結果: ? 失敗
```

### TC-D11: 密碼缺少小寫字母
```csharp
測試資料:
  - "TEST@1234"
  - "MYPASSWORD@99"

預期行為: 拋出 ArgumentException
實際行為: 正常建立 Password 物件（無驗證）
結果: ? 失敗
```

### TC-D12: 密碼缺少數字
```csharp
測試資料:
  - "Test@Password"
  - "MyPass@word"

預期行為: 拋出 ArgumentException
實際行為: 正常建立 Password 物件（無驗證）
結果: ? 失敗
```

### TC-D13: 密碼缺少特殊字元
```csharp
測試資料:
  - "Test1234"
  - "MyPassword99"

預期行為: 拋出 ArgumentException
實際行為: 正常建立 Password 物件（無驗證）
結果: ? 失敗
```

### TC-D14: BCrypt 雜湊
```csharp
測試資料: "Test@1234"

預期行為:
  - HashedValue 以 "$2" 開頭
  - HashedValue 長度為 60 字元

實際行為:
  - HashedValue = "Test@1234" (明文)
  - 長度為 9 字元

結果: ? 失敗
```

---

## ??? 專案結構

```
Mermaid_Lab03/
│
├── Domain.RentalCarLab1/              [Domain Layer]
│   ├── Account.cs                      ? Account Entity
│   └── ValueObjects/
│       └── Password.cs                 ? Password Value Object (骨架)
│
├── Domain.RentalCar.Tests/            [Test Layer]
│   ├── Test_DomainRentalCar.cs        ? 單元測試
│   └── packages.config                 ? MSTest 套件
│
└── TDD_紅燈階段_Password_骨架說明.md   ? 說明文件
```

---

## ?? TDD 循環進度

```
[? 完成] ?? 紅燈階段
  ├─ [?] 編寫測試案例
  ├─ [?] 建立骨架程式碼
  └─ [?] 確認測試失敗（預期行為）

[? 下一步] ?? 綠燈階段
  ├─ [ ] 實作 Password 驗證邏輯
  ├─ [ ] 實作 BCrypt 雜湊
  ├─ [ ] 實作 Verify 方法
  └─ [ ] 確認所有測試通過

[? 待完成] ?? 重構階段
  ├─ [ ] 優化程式碼結構
  ├─ [ ] 移除重複邏輯
  └─ [ ] 改善可讀性
```

---

## ?? 待辦事項檢查清單

### 立即執行
- [ ] 安裝 BCrypt.Net-Next NuGet 套件
  ```powershell
  Install-Package BCrypt.Net-Next -Version 4.0.3
  ```

### 實作階段（綠燈）
- [ ] 在 `Password.Create()` 加入空值檢查
- [ ] 實作長度驗證（>= 8 字元）
- [ ] 實作大寫字母驗證
- [ ] 實作小寫字母驗證
- [ ] 實作數字驗證
- [ ] 實作特殊字元驗證
- [ ] 使用 BCrypt 雜湊密碼
- [ ] 實作 `Verify()` 方法使用 BCrypt.Verify

### 驗證階段
- [ ] 執行所有測試
- [ ] 確認 6 個測試全部通過（綠燈）
- [ ] 檢查程式碼覆蓋率

---

## ?? 實作提示

### 1. LINQ 查詢範例
```csharp
using System.Linq;

// 檢查大寫字母
if (!plainPassword.Any(char.IsUpper))
    throw new ArgumentException("密碼至少包含一個大寫字母");

// 檢查小寫字母
if (!plainPassword.Any(char.IsLower))
    throw new ArgumentException("密碼至少包含一個小寫字母");

// 檢查數字
if (!plainPassword.Any(char.IsDigit))
    throw new ArgumentException("密碼至少包含一個數字");
```

### 2. BCrypt 使用範例
```csharp
using BCrypt.Net;

// 雜湊密碼
string hashedPassword = BCrypt.HashPassword(plainPassword);

// 驗證密碼
bool isValid = BCrypt.Verify(plainPassword, hashedPassword);
```

### 3. 特殊字元驗證
```csharp
const string specialChars = "!@#$%^&*()_+-=[]{}|;:,.<>?";
if (!plainPassword.Any(c => specialChars.Contains(c)))
    throw new ArgumentException("密碼至少包含一個特殊字元");
```

---

## ?? 驗證方法

### Visual Studio Test Explorer
1. 測試 → 測試總管
2. 執行所有測試
3. 查看失敗原因

### 命令列
```powershell
# 進入測試專案目錄
cd Domain.RentalCar.Tests

# 執行測試
dotnet test --verbosity normal

# 查看詳細結果
dotnet test --logger "console;verbosity=detailed"
```

---

## ?? 參考資源

### 內部文件
- `.github/prompts/UC1.prompt.md` - UC1 需求規格
- `.github/copilot-instructions.md` - 架構指南
- `TDD_紅燈階段_Password_骨架說明.md` - 詳細說明

### 外部資源
- [BCrypt.Net Documentation](https://github.com/BcryptNet/bcrypt.net)
- [MSTest Framework](https://docs.microsoft.com/en-us/dotnet/core/testing/unit-testing-with-mstest)
- [Domain-Driven Design](https://martinfowler.com/bliki/DomainDrivenDesign.html)

---

## ? 成功標準

骨架階段被視為成功，當：
- [?] 建置成功，無編譯錯誤
- [?] 測試案例可以執行
- [?] 所有測試失敗（紅燈）
- [?] 失敗原因符合預期

---

**建立日期**: 2024  
**TDD 階段**: ?? 紅燈（骨架完成）  
**下一步**: ?? 綠燈（實作驗證邏輯）  
**專案**: RentalCar - UC1 帳號註冊
