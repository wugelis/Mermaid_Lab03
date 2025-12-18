# ?? AAA (3A) 原則快速參考

## ?? AAA 原則結構

```csharp
[TestMethod]
public void TestName()
{
    // Arrange (安排)
    // 準備測試資料和預期結果
    var expectedValue = "預期值";
    var testData = "測試資料";

    // Act (執行)
    // 執行要測試的方法
    var actualResult = MethodUnderTest(testData);

    // Assert (驗證)
    // 驗證結果是否符合預期
    Assert.AreEqual(expectedValue, actualResult);
}
```

---

## ?? 例外測試的 AAA 模式

### 標準模板

```csharp
[TestMethod]
public void Should_ThrowException_When_InvalidInput()
{
    // Arrange
    var expectedExceptionMessage = "錯誤訊息關鍵字";
    ArgumentException actualException = null;
    var invalidInput = "無效資料";

    // Act
    try
    {
        MethodUnderTest(invalidInput);
    }
    catch (ArgumentException ex)
    {
        actualException = ex;
    }

    // Assert
    Assert.IsNotNull(actualException, "應該拋出例外");
    Assert.IsTrue(
        actualException.Message.Contains(expectedExceptionMessage),
        $"例外訊息應包含 '{expectedExceptionMessage}'"
    );
}
```

---

## ? 好的 AAA 測試範例

```csharp
[TestMethod]
public void Should_ThrowArgumentException_WhenPasswordHasNoDigit()
{
    // Arrange - 準備階段清楚明確
    var expectedExceptionMessage = "至少包含一個數字";
    ArgumentException actualException = null;
    var passwordWithoutDigit = "Test@Password";

    // Act - 執行階段只做一件事
    try
    {
        var result = Password.Create(passwordWithoutDigit);
    }
    catch (ArgumentException ex)
    {
        actualException = ex;
    }

    // Assert - 驗證階段完整且明確
    Assert.IsNotNull(actualException, "應該拋出 ArgumentException");
    Assert.IsTrue(
        actualException.Message.Contains(expectedExceptionMessage),
        $"例外訊息應包含 '{expectedExceptionMessage}'，實際訊息為: {actualException.Message}"
    );
}
```

**優點**:
- ? 三個階段清楚分離
- ? 變數命名有意義
- ? 驗證例外訊息
- ? 提供詳細失敗訊息

---

## ? 不好的測試範例

### 範例 1: 混亂的階段

```csharp
[TestMethod]
public void TestPassword()
{
    var password = "Test@Password";  // Arrange?
    ArgumentException ex = null;
    try
    {
        var result = Password.Create(password);  // Act?
        Assert.Fail("應該拋出例外");  // Assert?
    }
    catch (ArgumentException e)
    {
        ex = e;  // Act?
    }
    Assert.IsNotNull(ex);  // Assert?
}
```

**問題**:
- ? Arrange、Act、Assert 混在一起
- ? 沒有註解標示階段
- ? 變數命名不清楚

---

### 範例 2: 使用 ExpectedException

```csharp
[TestMethod]
[ExpectedException(typeof(ArgumentException))]
public void TestPassword()
{
    var password = "Test@Password";
    var result = Password.Create(password);
}
```

**問題**:
- ? 無法驗證例外訊息
- ? AAA 階段不明確
- ? 失敗訊息不夠詳細

---

## ?? AAA 檢查清單

### Arrange (安排) ?
- [ ] 準備所有測試資料
- [ ] 定義預期結果
- [ ] 初始化必要的物件
- [ ] 設定 Mock 或 Stub（如需要）

### Act (執行) ?
- [ ] 只執行一個被測試的方法
- [ ] 保持簡潔（通常 1-3 行）
- [ ] 捕捉例外（如果測試例外）
- [ ] 儲存結果供 Assert 使用

### Assert (驗證) ?
- [ ] 驗證預期結果
- [ ] 提供清楚的失敗訊息
- [ ] 使用適當的 Assert 方法
- [ ] 驗證所有重要的條件

---

## ?? 常用的 Assert 方法

### 基本斷言
```csharp
// 相等性
Assert.AreEqual(expected, actual, "失敗訊息");
Assert.AreNotEqual(expected, actual);

// Null 檢查
Assert.IsNull(obj);
Assert.IsNotNull(obj, "物件不應為 null");

// 布林值
Assert.IsTrue(condition, "條件應為 true");
Assert.IsFalse(condition, "條件應為 false");
```

### 字串斷言
```csharp
// 包含檢查
Assert.IsTrue(
    actualString.Contains(expectedSubstring),
    $"字串應包含 '{expectedSubstring}'"
);

// 開頭檢查
Assert.IsTrue(
    actualString.StartsWith(prefix),
    $"字串應以 '{prefix}' 開頭"
);
```

### 集合斷言
```csharp
// 集合長度
Assert.AreEqual(expectedCount, actualList.Count);

// 包含元素
CollectionAssert.Contains(actualList, expectedItem);
```

---

## ?? 進階技巧

### 參數化測試 + AAA

```csharp
[TestMethod]
[DataRow("test@1234", "大寫字母")]
[DataRow("TEST@1234", "小寫字母")]
[DataRow("Test@Pass", "數字")]
public void Should_ThrowException_WhenPasswordInvalid(
    string password, 
    string missingElement)
{
    // Arrange
    ArgumentException actualException = null;

    // Act
    try
    {
        Password.Create(password);
    }
    catch (ArgumentException ex)
    {
        actualException = ex;
    }

    // Assert
    Assert.IsNotNull(actualException, 
        $"密碼缺少{missingElement}時應拋出例外");
}
```

---

### Helper Method 保持 AAA 清晰

```csharp
// Helper Method
private ArgumentException CaptureException(string password)
{
    try
    {
        Password.Create(password);
        return null;
    }
    catch (ArgumentException ex)
    {
        return ex;
    }
}

// 測試方法
[TestMethod]
public void Should_ThrowException_WhenPasswordHasNoDigit()
{
    // Arrange
    var expectedMessage = "至少包含一個數字";
    var invalidPassword = "Test@Password";

    // Act
    var actualException = CaptureException(invalidPassword);

    // Assert
    Assert.IsNotNull(actualException);
    Assert.IsTrue(actualException.Message.Contains(expectedMessage));
}
```

---

## ?? AAA vs GWT (Given-When-Then)

AAA 和 GWT 是相似的概念：

| AAA | GWT | 說明 |
|-----|-----|------|
| Arrange | Given | 給定某個條件 |
| Act | When | 當執行某個動作 |
| Assert | Then | 那麼應該得到某個結果 |

兩者都強調測試的可讀性和結構化！

---

## ?? 參考資源

- [Microsoft - Unit testing best practices](https://docs.microsoft.com/en-us/dotnet/core/testing/unit-testing-best-practices)
- [Roy Osherove - The Art of Unit Testing](https://www.manning.com/books/the-art-of-unit-testing-second-edition)

---

**建立日期**: 2024  
**適用框架**: MSTest, NUnit, xUnit  
**適用語言**: C#, .NET Framework 4.8
