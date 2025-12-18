# ? 密碼數字驗證實作完成

## ?? 完成項目

### 1. 測試程式碼修正
**檔案**: `src\Domain.RentalCar.Tests\Test_DomainRentalCar.cs`

```csharp
[TestMethod]
[DataRow("Test@Password")]
[DataRow("MyPass@word")]
[ExpectedException(typeof(ArgumentException))]
public void Should_ThrowArgumentException_WhenPasswordHasNoDigit(string password)
{
    var result = Password.Create(password);
}
```

? 使用 `[ExpectedException]` 屬性
? 加回第二個測試資料

---

### 2. 密碼驗證邏輯實作
**檔案**: `src\Domain.RentalCarLab1\ValueObjects\Password.cs`

```csharp
// 加入 using System.Linq;

public static Password Create(string plainPassword)
{
    // ... 其他驗證 ...
    
    // 5. 檢查是否包含數字 ? 核心實作
    if (!plainPassword.Any(char.IsDigit))
        throw new ArgumentException("密碼至少包含一個數字", nameof(plainPassword));
    
    // ... 後續驗證 ...
}
```

---

## ?? 測試案例

### TC-D12: 密碼缺少數字

| 測試資料 | 包含數字 | 預期結果 |
|---------|---------|---------|
| `"Test@Password"` | ? 無 | 拋出 ArgumentException |
| `"MyPass@word"` | ? 無 | 拋出 ArgumentException |
| `"Test@1234"` | ? 有 | 通過驗證 |

---

## ?? 實作的驗證規則

```
1. ? 不能為空
2. ? 長度 ? 8
3. ? 包含大寫字母
4. ? 包含小寫字母
5. ? 包含數字         ← 本次實作
6. ? 包含特殊字元
7. ? BCrypt 雜湊      ← 待安裝套件
```

---

## ? 關鍵技術

```csharp
using System.Linq;

// LINQ 檢查是否有數字
if (!plainPassword.Any(char.IsDigit))
    throw new ArgumentException("密碼至少包含一個數字");
```

---

## ?? 建置狀態

```
? 編譯成功
? 無錯誤
? TC-D10~D13 預期通過
? TC-D14 待 BCrypt 套件
```

---

**詳細報告**: `TC-D12_實作完成報告.md`
