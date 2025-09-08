# Lilhelper

Unity 輔助工具集合（Helpers & Utilities）。

這個套件將常見的實務需求封裝成輕量模組：
- Async：簡單可用的非同步通道與取消語意
- RoutineChaining：把多個協程（IEnumerator）「串起來」的微型流程管線
- Parsing：以 Span 為基礎的字元掃描與 Token 化小工具
- UI：常用的 RectTransform 輔助元件
- Editors：對應的編輯器擴充（Inspector/Context）
- Debugs：好讀的彩色除錯輸出與備忘錄元件
- Objs：常用擴充方法（GameObject/Transform/Component/Linq/String/Rect/...）
- UniTasks：與 Cysharp.UniTask 的簡易流程回傳型別

目標：減少重複樣板、把「小事」寫好用好。

---

## 安裝

此套件以 Unity Package（本地檔案）形式存在：`Packages/lilhelper`

1. 專案 Packages 夾內已有 `lilhelper`；或
2. 於 Package Manager > + > Add package from disk… 指向 `https://github.com/aakwewaanaqa/lilhelper.git`

packages-lock.json 中可見：
```json
"com.lilhelper": { "version": "file:lilhelper" }
```

---

## 套件結構（Assemblies / Modules）

- com.lilhelper（核心）
  - Async, UI, Debugs, Objs 等通用功能
- com.lilhelper.editors（編輯器側）
  - 對應 MonoBehaviour 的 Inspector/Context 介面
- com.lilhelper.parsing（文字解析）
  - Tokenizer、TokenizeUnits、SyntaxTree 等
- com.lilhelper.routinechaining（協程串接）
  - Factory、RoutineResult、串接擴充
- com.lilhelper.unitasks（UniTask 整合）
  - Pipe、PipeReturn
- com.lilhelper.tests（測試）

---

## 模組與範例

### 1) Async 非同步工具
命名空間：`Lilhelper.Async`

重點類型：
- `Ctx`：一次性取消語意（包裝 CancellationTokenSource）
  - 成員重點：`Ct`、`Cancel()`、`ThrowIfCancel()`、`CancelAnew()`、`CancelAnewLink(ref Ctx, CancellationToken)`、`RegisterOnCancel(Action)`
- `Channel<T>`：簡易通道，支援寫入、嘗試讀取、可列舉與超時讀取
  - `Write(T)`、`IEnumerator Read(Action<T> onVal, double timeoutSeconds)`、`bool TryRead(Action<T>)`、`bool TryRead(out T)`、`Clear()`、`Dispose()`
- `OverTimeAlert`：在 `YieldWatching` 上每次 yield 檢查超時，逾時丟 `TimeoutException`
- 介面：`IWriteChannel<T>`、`IReadChannel<T>`

範例：Producer/Consumer（協程）
```csharp
using System.Collections;
using Lilhelper.Async;
using UnityEngine;

public class AsyncExample : MonoBehaviour {
    private Channel<int> _ch;

    private void OnEnable() {
        _ch = new Channel<int>(new Ctx());
        StartCoroutine(Producer());
        StartCoroutine(Consumer());
    }

    private IEnumerator Producer() {
        for (int i = 0; i < 3; i++) {
            _ch.Write(i);
            yield return null;
        }
    }

    private IEnumerator Consumer() {
        int got = -1;
        // 讀取：若暫無資料會 yield，超過 2 秒丟 TimeoutException
        yield return _ch.Read(v => got = v, timeout: 2);
        Debug.Log($"Got: {got}");

        // 嘗試不阻塞讀取
        if (_ch.TryRead(out got)) Debug.Log($"TryRead: {got}");
    }

    private void OnDisable() {
        _ch?.Dispose();
    }
}
```

取消語意（Ctx）
```csharp
using System.Threading;
using Lilhelper.Async;

public class CtxExample {
    private Ctx _ctx;

    public void Restart(CancellationToken external) {
        Ctx.CancelAnewLink(ref _ctx, external); // 取消舊的，連動外部 token
    }

    public void Stop() {
        _ctx?.Cancel();
    }
}
```

---

### 2) RoutineChaining 協程串接
命名空間：`Lilhelper.RoutineChaining`

重點：
- `public delegate IEnumerator Factory(Channel<RoutineResult> medium);`
- `RoutineResult { string label; Exception ex; Factory next; bool IsErr; ... }`
- 擴充：`HelperExts.Then(this Factory, Factory next)`、`AsNext(string label)`、`AsErr(Exception)`

使用情境：把多個步驟（協程）以「結果 -> 下一步」的方式串起來。

範例：
```csharp
using System.Collections;
using Lilhelper.Async;
using Lilhelper.RoutineChaining;
using UnityEngine;

public class ChainExample : MonoBehaviour {
    IEnumerator Run(Factory root) {
        using var medium = new Channel<RoutineResult>(new Ctx());
        var current = root;
        var result  = default(RoutineResult);

        while (current != null) {
            // 執行目前步驟
            yield return current(medium);

            // 讀取步驟回傳（可能攜帶 next 或錯誤）
            yield return medium.Read(v => result = v);
            if (result.IsErr) {
                Debug.LogError(result.ex);
                yield break;
            }
            current = result.next; // 移動到下一步
        }
    }

    Factory StepA => medium => Step();
    IEnumerator Step() {
        // ... 做事 ...
        // 宣告下一步
        yield return null;
        // 回報結果給管線
        // 你可以用 AsNext("label") 產生 RoutineResult
        yield return null;
    }

    void Start() {
        // 建立串接（A -> B -> C）
        Factory chain = StepA
            .Then(StepB)
            .Then(StepC);
        StartCoroutine(Run(chain));
    }

    Factory StepB => m => Do("B");
    Factory StepC => m => Do("C");

    IEnumerator Do(string label) {
        // ...
        yield return null;
    }
}
```
提示：`Then` 會在內部讀 `medium` 的結果與錯誤並傳遞 label/next；執行時請像上面示範以一個 `Run` 驅動整條管線。

---

### 3) Parsing 文字解析
命名空間：`Lilhelper.Parsing`、`Lilhelper.Parsing.Tokens`、`Lilhelper.Parsing.SyntaxTree`

重點類型：
- `Tokenizer`：以 `ReadOnlySpan<char>` 掃描字串，提供 `TryHead`、`TryPeek`、`Advance()` 等
- `TokenizeUnits`：常用 Token 組裝單元
  - `DoChar(char)`：匹配單一字元
  - `DoChar(char from, char to)`：匹配區間字元
  - `DoString(string, StringComparison rule)`：匹配字串（可忽略大小寫）
  - `GreedEnds(string ends)`：貪吃直到遇到結尾樣式
- `Parser`：ref struct，作為更高階解析器的起點（目前輕量）

簡單 Token 例子：
```csharp
using Lilhelper.Parsing.Tokens;

var tz   = new Tokenizer("A1");
var readA = TokenizeUnits.DoChar('A');
var readDigit = TokenizeUnits.DoChar('0', '9');

var r1 = readA(ref tz);      // r1.token.content == "A"
var r2 = readDigit(ref tz);  // r2.token.content == "1"
```

---

### 4) UI 元件
命名空間：`Lilhelper.UI`

- `RectTransformHelper`（ExecuteAlways）
  - 欄位/屬性：`Rect`、`Pos`、`SizeDelta`、`PreserveAspect`
  - 搭配 `RectTransformHelperEditor` 可在 Inspector 輕鬆維持寬高比

- `RectTransformSizeSync`（ExecuteAlways）
  - 將自身 `sizeDelta` 依比例同步到目標（預設為父節點 RectTransform）

範例：
```csharp
// 於 Inspector 新增元件：
// Add Component -> Lilhelper/UI/Rect Transform Helper
// 勾選 IsPreserveAspect 後編輯 SizeDelta 會自動維持比例
```

---

### 5) Editors 編輯器擴充
命名空間：`Lilhelper.Editors`

- `RectTransformHelperEditor`：
  - 顯示 Image 與 Rect/Pos 資訊
  - 提供 `IsPreserveAspect` 切換，維持 SizeDelta 比例編輯

---

### 6) Debugs 除錯工具
命名空間：`Lilhelper.Debugs`

- `DebugExts`
  - 彩色輸出：`TypeColor`、`MethodColor`
  - `F(this Type, string methodName, params object[] args)`：格式化輸出
  - `FuncThrow(this Type, string methodName, Exception ex = null)`：輸出後擲出例外
- `Memo`（MonoBehaviour）
  - 啟用時以 `typeof(Memo).F(gameObject.name, msg)` 輸出備忘訊息

範例：
```csharp
using System;
using Lilhelper.Debugs;

public class DebugExample {
    public void Foo() {
        typeof(DebugExample).F(nameof(Foo), 123, "abc");
        try {
            throw new InvalidOperationException("oops");
        } catch (Exception ex) {
            typeof(DebugExample).FuncThrow(nameof(Foo), ex);
        }
    }
}
```

---

### 7) Objs 常用擴充
命名空間：`Lilhelper.Objs`

- `Exts`：
  - `IsNull()`、`IsExists()` 判斷 Unity 對象是否存在
  - `EnsureComp<T>(...)` 系列，確保 GameObject/Transform/Component 擁有 T 組件
  - `Instantiate(this GameObject, Transform parent = null)` 包一層簡化
  - `Out<T>(this T self, out T t)` 快速把鏈式結果存出
- `CtExts.ThrowIfCancel(this CancellationToken, Action onThrow = null)`：一致化取消檢查
- `LinqExts`：`ToIndexDict()`、`OutIndexDict(...)`、`OutArray(...)`
- `StringExts.ToLowerCamelCase()`：以 `_` 或空白進行 camel 化
- `RectExts.SplitV(height)`、`SplitH(width)`：Rect 分割

小範例：
```csharp
using Lilhelper.Objs;
using UnityEngine;

public class ObjsExample : MonoBehaviour {
    void Start() {
        gameObject.EnsureComp(out Rigidbody rb);
        gameObject.EnsureCompAct((BoxCollider bc) => bc.isTrigger = true, out _);

        var copy = gameObject.Instantiate(transform);

        var rect = new Rect(0, 0, 100, 50);
        var (top, bottom) = rect.SplitV(20);
    }
}
```

---

### 8) UniTasks 流程回傳
命名空間：`UniTasks`

- `public delegate UniTask<PipeReturn> Pipe();`
- `readonly struct PipeReturn { Exception Ex; Pipe Then; bool IsFaulty; bool IsEnd; Continue(); }`

範例：
```csharp
using System;
using Cysharp.Threading.Tasks;
using UniTasks;

public static class PipeExample {
    static Pipe StepA => async () => {
        await UniTask.Delay(100);
        return new PipeReturn(then: StepB);
    };

    static Pipe StepB => async () => {
        await UniTask.Yield();
        return new PipeReturn(); // 結束
    };

    public static async UniTask Run() {
        var cur = StepA;
        while (cur != null) {
            var r = await cur();
            if (r.IsFaulty) throw r.Ex;
            if (r.IsEnd) break;
            cur = r.Then;
        }
    }
}
```

---

## 測試
- `com.lilhelper.tests` 含 Parsing 相關測試，可作為用法參考。

---

## 小結
Lilhelper 聚焦在「少即是多」：
- 小而精的 API，解決常見痛點
- 協程、取消、UI、除錯、解析等場景即取即用

若你有擴充需求或發現問題，歡迎直接在專案內新增／調整模組，或撰寫對應測試補強！
