using System.Drawing;
using System;
using System.Windows.Forms;
using System.IO;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Reflection;

namespace SmilePuzzle
{
    partial class MainForm : Form
    {
        private System.ComponentModel.IContainer components = null;
        private Button[] buttons;
        private Button automatButton;
        private RichTextBox matchBox;
        private DataGridView infoGrid;
        string[] imagePositions = { "[0,0]", "[0,1]", "[0,2]", "[1,0]", "[1,1]", "[1,2]", "[2,0]", "[2,1]", "[2,2]" };
        int centerIndex = 4;
        int northIndex = 1;
        int eastIndex = 5;
        int NEIndex = 2;
        int southIndex = 7;
        int SEIndex = 8;
        int westIndex = 3;
        int SWIndex = 6;
        int NWIndex = 0;
        int delay = 100;

        private Image ByteArrayToImage(byte[] byteArray)
        {
            using (MemoryStream ms = new MemoryStream(byteArray))
            {
                return Image.FromStream(ms);
            }
        }

        /// <summary>
        /// Uvolněte všechny používané prostředky.
        /// </summary>
        /// <param name="disposing">hodnota true, když by se měl spravovaný prostředek odstranit; jinak false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Kód generovaný Návrhářem Windows Form

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(610, 340);
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Smile Puzzle";

            InitializeColoredImages();

            buttons = new Button[9];
            for (int i = 0; i < buttons.Length; i++)
            {
                buttons[i] = new Button();
                buttons[i].Size = new System.Drawing.Size(100, 100);
                buttons[i].Location = new System.Drawing.Point((i % rows) * 100, (i / cols) * 100);
                buttons[i].Tag = puzzleImages[i].Name;
                buttons[i].Text = (string)buttons[i].Tag;
                buttons[i].Click += Button_Click; 
                this.Controls.Add(buttons[i]);
            }

            automatButton = new Button();
            automatButton.Size = new Size(100, 30);
            automatButton.Location = new Point(100, 305); 
            automatButton.Text = "SOLVE";
            automatButton.Click += AutomatButton_Click; 
            this.Controls.Add(automatButton);

            matchBox = new RichTextBox();
            matchBox.Size = new Size(300, 100); 
            matchBox.Location = new Point(305, 235); 
            matchBox.ReadOnly = true; 
            this.Controls.Add(matchBox);
            UpdatePositionGrid();

            infoGrid = new DataGridView();
            infoGrid.Size = new Size(300, 225);
            infoGrid.Location = new Point(305, 5);
            infoGrid.RowHeadersVisible = false;
            infoGrid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;  
            infoGrid.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;        
            this.Controls.Add(infoGrid);
        }

        private async void AutomatButton_Click(object sender, EventArgs e)
        {
            automatButton.Text = "I AM SOLVING";
            automatButton.Enabled = false;
            List<PuzzleImage> puzzles = new List<PuzzleImage>();
            foreach (var puzzleImage in puzzleImages)
            {
                puzzles.Add(puzzleImage);
            }

            List<int> ignoreIndex;
            List<int> ignoreIndexNorth = new List<int>() { centerIndex };
            List<int> ignoreIndexEast = new List<int>() { centerIndex, northIndex };
            List<int> ignoreIndexNorthEast = new List<int>() { centerIndex, northIndex, eastIndex};
            List<int> ignoreIndexSouth = new List<int>() { centerIndex, northIndex, eastIndex, NEIndex};
            List<int> ignoreIndexSouthEast = new List<int>() { centerIndex, northIndex, eastIndex, NEIndex, southIndex };
            List<int> ignoreIndexWest = new List<int>() { centerIndex, northIndex, eastIndex, NEIndex, southIndex, SEIndex };
            List<int> ignoreIndexSouthWest = new List<int>() { centerIndex, northIndex, eastIndex, NEIndex, southIndex, SEIndex, westIndex };
            List<int> ignoreIndexNorthWest = new List<int>() { centerIndex, northIndex, eastIndex, NEIndex, southIndex, SEIndex, westIndex, SWIndex};

            try
            {
                foreach (var puzzle in puzzles)
                {
                    string imageName = puzzle.Name;
                    int currentIndex = FindImageIndexByName(imageName);
                    if (currentIndex == centerIndex)
                        continue;
                    else
                    {
                        SwapImages(currentIndex, centerIndex);
                        await Task.Delay(delay);
                    }

                    // North
                    ignoreIndex = ignoreIndexNorth;
                    List<string> northOptions = FindOptions(puzzle.N_color, puzzle.N_eyes, ignoreIndex);
                    int northRepeat = 0;

                    foreach (var northOption in northOptions)
                    {
                        ignoreIndex = ignoreIndexNorth;
                        int northOptionIndex = FindImageIndexByName(northOption);
                        if (northOptionIndex == northIndex)
                            northRepeat++;

                        await AutomationSwapAsync(northOptionIndex, northIndex, northRepeat);
                        northRepeat = await AutomationRotateAsync(puzzle.N_color, northIndex, 2, northRepeat);

                        // East
                        ignoreIndex = ignoreIndexEast;
                        List<string> eastOptions = FindOptions(puzzle.E_color, puzzle.E_eyes, ignoreIndex);
                        int eastRepeat = 0;

                        foreach (var eastOption in eastOptions)
                        {
                            ignoreIndex = ignoreIndexEast;
                            int eastOptionIndex = FindImageIndexByName(eastOption);
                            if (eastOptionIndex == eastIndex)
                                eastRepeat++;

                            await AutomationSwapAsync(eastOptionIndex, eastIndex, eastRepeat);
                            eastRepeat = await AutomationRotateAsync(puzzle.E_color, eastIndex, 3, eastRepeat);

                            // NorthEast
                            var NpuzzleEcolor = puzzleImages[1].E_color;
                            var EpuzzleNcolor = puzzleImages[5].N_color;

                            ignoreIndex = ignoreIndexNorthEast;
                            List<string> NEOptions = FindCornerOptions(EpuzzleNcolor, puzzleImages[5].N_eyes, NpuzzleEcolor, puzzleImages[1].E_eyes, ignoreIndex);
                            int northEastRepeat = 0;

                            foreach (var NEOption in NEOptions)
                            {
                                ignoreIndex = ignoreIndexNorthEast;
                                int NEOptionIndex = FindImageIndexByName(NEOption);
                                if (NEOptionIndex == NEIndex)
                                    northEastRepeat = northEastRepeat + GetCornerRepeat(NEIndex, NpuzzleEcolor, EpuzzleNcolor);

                                await AutomationSwapAsync(NEOptionIndex, NEIndex, northEastRepeat);
                                northEastRepeat = await AutomationRotateAsync(NpuzzleEcolor, 3, EpuzzleNcolor, 2, NEIndex, northEastRepeat);

                                // South
                                ignoreIndex = ignoreIndexSouth;
                                List<string> southOptions = FindOptions(puzzle.S_color, puzzle.W_eyes, ignoreIndex);
                                int southRepeat = 0;

                                foreach (var southOption in southOptions)
                                {
                                    ignoreIndex = ignoreIndexSouth;
                                    int southOptionIndex = FindImageIndexByName(southOption);
                                    if (southOptionIndex == southIndex)
                                        southRepeat++;

                                    await AutomationSwapAsync(southOptionIndex, southIndex, southRepeat);
                                    southRepeat = await AutomationRotateAsync(puzzle.S_color, southIndex, 0, southRepeat);

                                    // SouthEast
                                    var SpuzzleEcolor = puzzleImages[7].E_color;
                                    var EpuzzleScolor = puzzleImages[5].S_color;

                                    ignoreIndex = ignoreIndexSouthEast;
                                    List<string> SEOptions = FindCornerOptions(SpuzzleEcolor, puzzleImages[7].E_eyes, EpuzzleScolor, puzzleImages[5].S_eyes, ignoreIndex);
                                    int southEastRepeat = 0;

                                    foreach (var SEOption in SEOptions)
                                    {
                                        ignoreIndex = ignoreIndexSouthEast;
                                        int SEOptionIndex = FindImageIndexByName(SEOption);
                                        if (SEOptionIndex == SEIndex)
                                            southEastRepeat = southEastRepeat + GetCornerRepeat(SEIndex, SpuzzleEcolor, EpuzzleScolor);

                                        await AutomationSwapAsync(SEOptionIndex, SEIndex, southEastRepeat);
                                        southEastRepeat = await AutomationRotateAsync(SpuzzleEcolor, 3, EpuzzleScolor, 0, SEIndex, southEastRepeat);

                                        // West
                                        ignoreIndex = ignoreIndexWest;
                                        List<string> westOptions = FindOptions(puzzle.W_color, puzzle.W_eyes, ignoreIndex);
                                        int westRepeat = 0;

                                        foreach (var westOption in westOptions)
                                        {
                                            ignoreIndex = ignoreIndexWest;
                                            int westOptionIndex = FindImageIndexByName(westOption);
                                            if (westOptionIndex == westIndex)
                                                westRepeat++;

                                            await AutomationSwapAsync(westOptionIndex, westIndex, westRepeat);
                                            westRepeat = await AutomationRotateAsync(puzzle.W_color, westIndex, 1, westRepeat);

                                            // SouthWest
                                            var SpuzzleWcolor = puzzleImages[7].W_color;
                                            var WpuzzleScolor = puzzleImages[3].S_color;

                                            ignoreIndex = ignoreIndexSouthWest;
                                            List<string> SWOptions = FindCornerOptions(WpuzzleScolor, puzzleImages[3].S_eyes, SpuzzleWcolor, puzzleImages[7].W_eyes, ignoreIndex);
                                            int southWestRepeat = 0;

                                            foreach (var SWOption in SWOptions)
                                            {
                                                ignoreIndex = ignoreIndexSouthWest;
                                                int SWOptionIndex = FindImageIndexByName(SWOption);
                                                if (SWOptionIndex == SWIndex)
                                                    southWestRepeat = southWestRepeat + GetCornerRepeat(SWIndex, WpuzzleScolor, SpuzzleWcolor);

                                                await AutomationSwapAsync(SWOptionIndex, SWIndex, southWestRepeat);
                                                southWestRepeat = await AutomationRotateAsync(SpuzzleWcolor, 1, WpuzzleScolor, 0, SWIndex, southWestRepeat);

                                                // NorthWest
                                                var NpuzzleWcolor = puzzleImages[1].W_color;
                                                var WpuzzleNcolor = puzzleImages[3].N_color;

                                                ignoreIndex = ignoreIndexNorthWest;
                                                List<string> NWOptions = FindCornerOptions(NpuzzleWcolor, puzzleImages[1].W_eyes, WpuzzleNcolor, puzzleImages[3].N_eyes, ignoreIndex);
                                                int northWestRepeat = 0;

                                                foreach (var NWOption in NWOptions)
                                                {
                                                    ignoreIndex = ignoreIndexNorthWest;
                                                    int NWOptionIndex = FindImageIndexByName(NWOption);
                                                    if (NWOptionIndex == NWIndex)
                                                        northWestRepeat = northWestRepeat + GetCornerRepeat(NWIndex, NpuzzleWcolor, WpuzzleNcolor);

                                                    await AutomationSwapAsync(NWOptionIndex, NWIndex, northWestRepeat);
                                                    northWestRepeat = await AutomationRotateAsync(NpuzzleWcolor, 1, WpuzzleNcolor, 2, NWIndex, northWestRepeat);
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (PuzzleSolvedException)
            {
                automatButton.Text = "SOLVE AGAIN";
                automatButton.Enabled = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred: " + ex.Message);
            }
        }

        private int GetCornerRepeat(int index, Color color1, Color color2)
        {
            int repeat = 0;
            Color[] colors = puzzleImages[index].Colors();
            var clrsCount1 = colors.Count(c => c == color1);
            var clrsCount2 = colors.Count(c => c == color2);
            if (clrsCount1 > 1 || clrsCount2 > 1)
            {
                repeat--;
                for (int i = 0; i < colors.Length; i++)
                {
                    if (colors[i] == color1)
                    {
                        int colorIndex = i;
                        if (colorIndex == 3)
                            colorIndex = -1;

                        if (colors[colorIndex + 1] == color2)
                            repeat++;
                    }
                }
            }
            return repeat;
        }

        private async Task<int> AutomationRotateAsync(Color color1, int colorIndex, Color color2, int colorIndex2, int index, int repeat)
        {
            int resultRepeat = repeat;
            await RotationAsync(color1, index, colorIndex);
            await RotationAsync(color2, index, colorIndex2);

            if (resultRepeat > 0)
            {
                RotateImage(index);
                await Task.Delay(delay);
                await RotationAsync(color1, index, colorIndex);
                await RotationAsync(color2, index, colorIndex2);
                resultRepeat--;
            }
            return resultRepeat;
        }

        private async Task<int> AutomationRotateAsync(Color color, int index, int colorIndex, int repeat)
        {
            int resultRepeat = repeat;
            await RotationAsync(color, index, colorIndex);

            if (resultRepeat > 0)
            {
                RotateImage(index);
                await Task.Delay(delay);
                await RotationAsync(color, index, colorIndex);
                resultRepeat--;
            }
            return resultRepeat;
        }

        private async Task RotationAsync(Color color1, int index, int colorIndex)
        {
            Color color = puzzleImages[index].Colors()[colorIndex];
            while (color != color1)
            {
                RotateImage(index);
                await Task.Delay(delay);
                color = puzzleImages[index].Colors()[colorIndex];
                CheckResult();
            }
        }

        private async Task AutomationSwapAsync(int index1, int index2, int repeat)
        {
            if (repeat == 0)
            {
                SwapImages(index1, index2);
                await Task.Delay(delay);
            }
        }

        private List<string> FindOptions(Color color, bool eyes, List<int> ignoreIndex)
        {
            List<string> options = new List<string>();

            foreach (var puzzle in puzzleImages)
            {
                if (ignoreIndex.Contains(FindImageIndexByName(puzzle.Name))) continue;
                var colors = puzzle.Colors();

                for (int i = 0; i < colors.Length; i++)
                {
                    if (colors[i] == color && puzzle.Eyes()[i] != eyes)
                        options.Add(puzzle.Name);
                }
            }
            return options;
        }

        private List<string> FindCornerOptions(Color color1, bool eyes1, Color color2, bool eyes2, List<int> ignoreIndex)
        {
            List<string> options = new List<string>();
            int colorIndex;

            foreach (var puzzle in puzzleImages)
            {
                if (ignoreIndex.Contains(FindImageIndexByName(puzzle.Name))) continue;
                var colors = puzzle.Colors();
                var eyes = puzzle.Eyes();

                for (int i = 0; i < colors.Length; i++)
                {
                    if (colors[i] == color1 && eyes[i] != eyes1)
                    {
                        colorIndex = i + 1;
                        if (i == colors.Length-1)
                            colorIndex = 0;

                        if (colors[colorIndex] == color2 && eyes[colorIndex] != eyes2)
                            options.Add(puzzle.Name);
                    }
                }
            }
            return options;
        }

        private int FindImageIndexByName(string imageName)
        {
            for (int i = 0; i < buttons.Length; i++)
            {
                if (GetImageName(i) == imageName) return i;
            }
            return -1;
        }

        private void InitializeColorGrid()
        {
            infoGrid.Columns.Clear();
            infoGrid.Columns.Add("Position", "[row,col]");
            infoGrid.Columns.Add("Image", "Image");
            infoGrid.Columns.Add("North", "N");
            infoGrid.Columns.Add("East", "E");
            infoGrid.Columns.Add("South", "S");
            infoGrid.Columns.Add("West", "W");

            foreach (DataGridViewColumn column in infoGrid.Columns)
            {
                column.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            }

            UpdateColorGrid();
        }

        private void UpdateColorGrid()
        {
            infoGrid.Rows.Clear();
            for (int row = 0; row < rows; row++)
            {
                for (int col = 0; col < cols; col++)
                {
                    int index = row * cols + col;  
                    int rowIndex = infoGrid.Rows.Add();
                    DataGridViewRow dataRow = infoGrid.Rows[rowIndex];

                    dataRow.Cells[0].Value = imagePositions[index];
                    dataRow.Cells[1].Value = GetImageName(index);
                    dataRow.Cells[2].Value = puzzleImages[index].N_eyes ? "eyes" : "smile";
                    dataRow.Cells[3].Value = puzzleImages[index].E_eyes ? "eyes" : "smile";
                    dataRow.Cells[4].Value = puzzleImages[index].S_eyes ? "eyes" : "smile";
                    dataRow.Cells[5].Value = puzzleImages[index].W_eyes ? "eyes" : "smile";
                    dataRow.Cells[2].Style.BackColor = puzzleImages[index].N_color;
                    dataRow.Cells[3].Style.BackColor = puzzleImages[index].E_color;
                    dataRow.Cells[4].Style.BackColor = puzzleImages[index].S_color;
                    dataRow.Cells[5].Style.BackColor = puzzleImages[index].W_color; 
                }
            }
        }

        private string GetImageName(int position)
        {
            return buttons[position].Tag.ToString();
        }

        private void UpdatePositionGrid()
        {
            foreach (var button in buttons)
            {
                button.Text = button.Tag.ToString();
            }
        }
        #endregion
    }
}