# ? 測試重構：遵循 AAA (3A) 原則

## ?? 重構摘要

將所有測試案例從使用 `[ExpectedException]` 屬性改為明確的 AAA (Arrange-Act-Assert) 模式。

---

## ?? 重構對比

### ? 重構前（使用 ExpectedException）

```csharp
[TestMethod]
[DataRow("Test@Password")]
[ExpectedException(typeof(ArgumentException))]
public void Should_ThrowArgumentException_WhenPasswordHasNoDigit(string password)
{
    // Arrange & Act
    var result = Password.Create(password);

    // Assert - 由 ExpectedException 處理
}
```

**問題**:
- ? AAA 階段不清楚（Arrange 和 Act 混在一起）
- ? 無法驗證例外訊息內容
- ? 如果其他程式碼拋出 ArgumentException 也會通過
- ? 測試意圖不夠明確

---

### ? 重構後（AAA 原則）

```csharp
[TestMethod]
[DataRow("Test@Password")]
[DataRow("MyPass@word")]
public void Should_ThrowArgumentException_WhenPasswordHasNoDigit(string password)
{
    // Arrange
    var expectedExceptionMessage = "至少包含一個數字";
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
```

**優點**:
- ? 清楚區分 Arrange、Act、Assert 三個階段
- ? 驗證例外訊息內容（確保是正確的驗證失敗）
- ? 提供詳細的失敗訊息
- ? 測試意圖明確易懂

---

## ?? AAA 原則詳解

### ?? Arrange（安排）
準備測試所需的資料和狀態

```csharp
// Arrange
var expectedExceptionMessage = "至少包含一個數字";
ArgumentException actualException = null;
```

**目的**:
- 定義預期的例外訊息
- 準備用於捕捉例外的變數

---

### ? Act（執行）
執行要測試的行為

```csharp
// Act
try
{
    var result = Password.Create(password);
}
catch (ArgumentException ex)
{
    actualException = ex;
}
```

**目的**:
- 呼叫 `Password.Create()` 方法
- 捕捉預期的 ArgumentException
- 保持 Act 階段簡潔（單一職責）

---

### ?? Assert（驗證）
驗證結果是否符合預期

```csharp
// Assert
Assert.IsNotNull(actualException, "應該拋出 ArgumentException");
Assert.IsTrue(
    actualException.Message.Contains(expectedExceptionMessage),
    $"例外訊息應包含 '{expectedExceptionMessage}'，實際訊息為: {actualException.Message}"
);
```

**目的**:
- 確認例外確實被拋出
- 驗證例外訊息內容
- 提供清晰的失敗原因

---

## ?? 所有重構的測試案例

### TC-D10: 密碼缺少大寫字母

```csharp
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
```

---

### TC-D11: 密碼缺少小寫字母

```csharp
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
```

---

### TC-D12: 密碼缺少數字

```csharp
[TestMethod]
[DataRow("Test@Password")]
[DataRow("MyPass@word")]
public void Should_ThrowArgumentException_WhenPasswordHasNoDigit(string password)
{
    // Arrange
    var expectedExceptionMessage = "至少包含一個數字";
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
```

---

### TC-D13: 密碼缺少特殊字元

```csharp
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
```

---

### TC-D14: BCrypt 雜湊（保持不變）

```csharp
[TestMethod]
public void Should_HashPasswordUsingBCrypt_WhenCreated()
{
    // Arrange
    var plainPassword = "Test@1234";

    // Act
    var password = Password.Create(plainPassword);

    // Assert
    Assert.IsTrue(
        password.HashedValue.StartsWith("$2"),
        "BCrypt 雜湊值應該以 '$2' 開頭"
    );
    Assert.AreEqual(
        60,
        password.HashedValue.Length,
        "BCrypt 雜湊值長度應該是 60 字元"
    );
}
```

---

## ?? AAA 原則的好處

### 1. **可讀性提升**
```
? 清楚看到測試的三個階段
? 每個階段的職責明確
? 新成員容易理解測試意圖
```

### 2. **維護性提升**
```
? 修改某個階段不影響其他階段
? 容易定位測試失敗的原因
? 重構時不容易破壞測試
```

### 3. **除錯能力提升**
```
? 失敗訊息更詳細
? 可以看到實際的例外訊息
? 驗證邏輯更明確
```

### 4. **測試更嚴謹**
```
? 驗證例外訊息內容
? 確保是正確的驗證規則失敗
? 避免誤判（其他地方拋出相同例外）
```

---

## ?? 測試執行範例

### 測試通過時
```
? TC-D10: Should_ThrowArgumentException_WhenPasswordHasNoUpperCase
   - "test@1234" → 通過
   - "mypassword@99" → 通過

? TC-D11: Should_ThrowArgumentException_WhenPasswordHasNoLowerCase
   - "TEST@1234" → 通過
   - "MYPASSWORD@99" → 通過

? TC-D12: Should_ThrowArgumentException_WhenPasswordHasNoDigit
   - "Test@Password" → 通過
   - "MyPass@word" → 通過

? TC-D13: Should_ThrowArgumentException_WhenPasswordHasNoSpecialChar
   - "Test1234" → 通過
   - "MyPassword99" → 通過
```

### 測試失敗時（詳細訊息）
```
? TC-D12: Should_ThrowArgumentException_WhenPasswordHasNoDigit
   訊息: 例外訊息應包含 '至少包含一個數字'，實際訊息為: 密碼至少需要 8 個字元
   
   → 可以立即看出是哪個驗證規則出問題
```

---

## ?? 與 ExpectedException 的比較

| 特性 | ExpectedException | AAA 模式 |
|------|------------------|----------|
| 可讀性 | ?? | ????? |
| 驗證訊息 | ? 不支援 | ? 完整支援 |
| 失敗訊息 | ?? | ????? |
| 除錯能力 | ?? | ????? |
| 程式碼行數 | ????? (少) | ??? (多一些) |
| 測試嚴謹度 | ??? | ????? |

---

## ?? 建置狀態

```
? 編譯成功
? 無警告
? 所有測試可執行
? 符合 .NET Framework 4.8 規範
```

---

## ?? 最佳實踐

### ? 應該做的
1. 使用明確的變數名稱（如 `expectedExceptionMessage`）
2. 在 Assert 提供詳細的失敗訊息
3. 驗證例外訊息內容
4. 保持 Act 階段簡潔

### ? 避免做的
1. 在 Act 階段混入驗證邏輯
2. 忽略例外訊息驗證
3. 使用 `[ExpectedException]` 在複雜測試中
4. 過度註解（程式碼應自我說明）

---

## ?? 總結

### 改進項目
- ? 所有測試遵循 AAA 原則
- ? 移除 `[ExpectedException]` 屬性
- ? 加入例外訊息驗證
- ? 提供詳細的失敗訊息
- ? 提升測試可讀性和維護性

### 測試案例狀態
```
? TC-D10: 密碼缺少大寫字母 - AAA 重構完成
? TC-D11: 密碼缺少小寫字母 - AAA 重構完成
? TC-D12: 密碼缺少數字 - AAA 重構完成
? TC-D13: 密碼缺少特殊字元 - AAA 重構完成
? TC-D14: BCrypt 雜湊 - 本就符合 AAA
```

---

**重構日期**: 2024  
**重構目標**: 遵循 AAA (3A) 原則  
**建置狀態**: ? 成功  
**測試品質**: ?????
