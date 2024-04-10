# Snowflake Generator by Cellular Automata and Marching Cube
---
PROJECT: Snowflake
AUTHOR: 310465008 李芸蓁 yclee@arch.nycu.edu.tw
DATE: 2022/01/27
DESCRIPTION: 使用 Cell Automata 生成雪花，並以 Marching Triangles 將雪花生成過程視覺化

![image](https://github.com/yunchen-lee/IAR5262_gh_Snowflake_CA-MarchingCube/blob/main/report/snowflake_demoXnspeed.gif)

---
## A. 檔案名稱與內容
### a1 作業檔：
	a1-1 2022_0127_MarchingCube.gh
	a1-2 IAR5262_MarchingCube.dll
### a2 原始碼：
	a2-1 HexNode.cs 雪花生成網點
	a2-2 HexGrid.cs 雪花生成網格
	a2-3 TriPrismUnit.cs 三角柱堆砌單元
	a2-4 PrismGrid.cs 三角柱網格
### a4 影片與照片：
	a4-1 2022_0127_snowflake_demo.mp4 生成記錄影片
	a4-2 2022_0127_snowflake_report.pdf 實驗紀錄


---
## B. 演算法
### b1 Cell Automata - snowflake 生成：
class HexNode 描述三角形網格中每一個網點的狀態，class HexGrid 參考 Reiter, C. (2005) 以 cell automata 機制模仿雪花生成時的擴散作用，HexNode 中的 water 紀錄網點的含水量，如果含水量超過 1 則 frozen。擴散機制中有三個參數 alpha、beta、gamma，調整這三個參數初始值可以改變雪花生成的結果，在過程中更改參數則可以變化出各種雪花特徵組合。alpha 為擴散速度，beta 表示邊界傳入水量，gamma 表示 receptive node 吸收的背景水量，每回合計算完後同時更新新的含水量，因此雪花完全對稱。

### b2 Marching Triangles - snowflake 視覺化：
根據雪花的分子結構 - 六角柱堆砌(Triangular-hexagonal prismatic honeycomb)，class PrismGrid 以 class TriPrismUnit 單一三角柱組成三層三角柱堆砌網格，中間單層做雪花生成，形成雪花單一封閉 mesh 實體。


---
## C. 操作方法
### c1 DLL：使用 IAR5262_MarchingCube.dll

### c2 Input：
reset：初始化
size：網格大小
alpha：擴散速率(diffusion coefficient)
beta：邊界傳入水量(background level)
gamma：背景水量(vapour addition)
displayPeriod：marching triangle 更新週期(每計算 n 次更新一次 field)

### c3 Output：
out：顯示狀態(Reset：初始化狀態/End：snowflake生成完成)
snowflake：list of mesh -> joined mesh

### c4 使用步驟：
1. 關閉 timmer，並設定 reset = true
2. 設定初始值(預設值 size:30, alpha:1, beta: 0.9, gamma: 0, displayPeriod: 75, timmer: 10ms)
3. 設定 reset = false
4. 開啟 timmer
5. out 顯示 End 時表示生成完畢

* 生成中變更參數
1. 關閉 timmer (reset 維持 false)
2. 更改 alpha/beta/gamma
3. 開啟 timmer


---
## D. 困難與待嘗試
困難 - cell automata 需很清楚每個步驟執行的順序，一開始沒有注意遇到雪花生長不對稱的問題
待嘗試 - 加上Z方向的擴散作用機制


---
## E. 參考資料
* [1] Kenneth G. Libbrecht (2019). Snow Crystals.
* [2] Li, J., & Schaposnik, L. P. (2016). Interface control and snow crystal growth. Physical Review E, 93(2), 023302. https://doi.org/10.1103/PhysRevE.93.023302
* [3] Libbrecht, K. G. (2005). The physics of snow crystals. Reports on Progress in Physics, 68(4), 855–895. https://doi.org/10.1088/0034-4885/68/4/R03
* [4] Reiter, C. (2005). A local cellular model for snow crystal growth. Chaos, Solitons & Fractals, 23(4), 1111–1119. https://doi.org/10.1016/S0960-0779(04)00374-1


