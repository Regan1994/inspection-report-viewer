# 主系統新版部署步驟

新版主系統發布包：`InspectionReporting-manager.zip`。

## 在 192.168.0.193 伺服器電腦上

1. 先關閉舊的 `InspectionReporting.exe`。
2. 找到舊程式資料夾，確認裡面有正式資料庫所在的 `instance` 資料夾。先備份整個 `instance` 資料夾。
3. 將 ZIP 解壓縮到新的資料夾，例如 `C:\InspectionReporting-manager-new`。
4. 將舊程式的 `instance` 資料夾內容複製到新資料夾的 `instance`，保留正式資料庫與 backups。
5. 從新資料夾執行 `InspectionReporting.exe`。
6. 在瀏覽器測試 `http://192.168.0.193:5000/manager`。

不要直接用 ZIP 裡的空白／測試 `instance` 取代伺服器正式資料庫，也不要只複製 EXE；這是 onedir 發布包，必須保留整個資料夾。

