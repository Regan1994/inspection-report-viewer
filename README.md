# 校機報工報表閱讀器

給業務經理使用的唯讀報表入口啟動器。閱讀器不執行 Flask、不連線 SQLite，也不保存主系統資料；它只負責開啟主系統提供的報表專用入口。

## 目前狀態

目前 repository 從空白開始建立，第一階段先完成 Windows 啟動器骨架：

- 點擊 `ReportViewer.exe` 後開啟主系統 `/manager`
- 優先使用 Edge App Mode，其次使用 Chrome App Mode
- 找不到上述瀏覽器時退回預設瀏覽器
- 可用同目錄的 `ReportViewer.json` 指定主機網址
- 也支援命令列 `--url`，方便部署測試
- 不提供使用者設定頁面、資料修改或本機資料庫

主系統目前已新增 GET `/manager` 報表頁面；它直接讀取主系統正式 SQLite 資料，不由閱讀器建立第二份資料庫。

## 執行設定

將 `ReportViewer.json.example` 複製為 `ReportViewer.json`，再填入主機 IP：

```json
{
  "serverUrl": "http://192.168.1.20:5000/manager"
}
```

也可以直接測試：

```powershell
ReportViewer.exe --url http://192.168.1.20:5000/manager
```

沒有設定檔時，預設網址為 `http://127.0.0.1:5000/manager`。

## 建置

開發機需安裝 .NET 8 SDK。Windows 發布命令：

```powershell
./build.ps1
```

輸出位於 `publish/win-x64/`，可將整個資料夾交付給使用者；使用者不需要安裝 Python、SQLite 或 .NET Runtime。

## MVP 邊界

閱讀器只會開啟報表入口。查詢、校機狀態、開始／結束時間、人員、明細、缺料與停工資料都必須由主系統的唯讀入口提供。

「實際校機工時」目前先保留既有主系統的顯示方式，不在閱讀器端重新計算，也不加入停工扣除或缺料扣除邏輯。這項需求先記錄，待實際使用者確認有需要時再處理。

