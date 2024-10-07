using System;
using System.Drawing;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace SmilePuzzle
{
    public partial class MainForm : Form
    {
        private Image[] images;
        private PuzzleImage[] puzzleImages;
        private int firstSelected = -1; 
        private int rows = 3;
        private int cols = 3;
        private int maxMatch = 48;

        public MainForm()
        {
            InitializeComponent(); 
            LoadImages();    
            InitializePuzzle(); 
            InitializeColorGrid();
            CheckResult();
        }

        private void LoadImages()
        {
            images = new Image[]
            {
                ByteArrayToImage(Properties.Resources.smajlik_A1),
                ByteArrayToImage(Properties.Resources.smajlik_A2),
                ByteArrayToImage(Properties.Resources.smajlik_A3),
                ByteArrayToImage(Properties.Resources.smajlik_B1),
                ByteArrayToImage(Properties.Resources.smajlik_B2),
                ByteArrayToImage(Properties.Resources.smajlik_B3),
                ByteArrayToImage(Properties.Resources.smajlik_C1),
                ByteArrayToImage(Properties.Resources.smajlik_C2),
                ByteArrayToImage(Properties.Resources.smajlik_C3)
            };
        }

        private void InitializeColoredImages()
        {
            puzzleImages = new PuzzleImage[]
            {
                // false = smile, true = eyes
                new PuzzleImage("A1", Color.Red, Color.Yellow, Color.Red, Color.Green, false, false, true, true),     // smajlik_A1
                new PuzzleImage("A2", Color.Blue, Color.Yellow, Color.Blue, Color.Green, true, true, false, false),   // smajlik_A2
                new PuzzleImage("A3", Color.Red, Color.Yellow, Color.Blue, Color.Yellow, true, true, false, false),   // smajlik_A3
                new PuzzleImage("B1", Color.Red, Color.Blue, Color.Green, Color.Red, false, true, true, false),       // smajlik_B1
                new PuzzleImage("B2", Color.Blue, Color.Green, Color.Red, Color.Yellow, true, true, false, false),    // smajlik_B2
                new PuzzleImage("B3", Color.Blue, Color.Yellow, Color.Red, Color.Green, false, false, true, true),    // smajlik_B3
                new PuzzleImage("C1", Color.Blue, Color.Green, Color.Yellow, Color.Blue, false, false, true, true),   // smajlik_C1
                new PuzzleImage("C2", Color.Blue, Color.Red, Color.Blue, Color.Yellow, true, false, false, true),     // smajlik_C2
                new PuzzleImage("C3", Color.Yellow, Color.Red, Color.Green, Color.Green, true, false, false, true),   // smajlik_C3
            };
        }

        private void InitializePuzzle()
        {
            for (int i = 0; i < buttons.Length; i++)
            {
                buttons[i].BackgroundImage = images[i];
                buttons[i].BackgroundImageLayout = ImageLayout.Stretch;
            }
        }

        private void Button_Click(object sender, EventArgs e)
        {
            Button clickedButton = sender as Button;
            string clickedImageIdentifier = clickedButton.Tag.ToString();

            if (firstSelected == -1)
            {
                firstSelected = Array.IndexOf(buttons, clickedButton); 
            }
            else
            {
                int secondSelected = Array.IndexOf(buttons, clickedButton); 
                SwapImages(firstSelected, secondSelected);
                firstSelected = -1; 
            }
        }

        private void SwapImages(int index1, int index2)
        {
            Image tempImage = buttons[index1].BackgroundImage;
            buttons[index1].BackgroundImage = buttons[index2].BackgroundImage;
            buttons[index2].BackgroundImage = tempImage;

            object tempTag = buttons[index1].Tag;
            buttons[index1].Tag = buttons[index2].Tag;
            buttons[index2].Tag = tempTag;

            var tempPuzzleImage = puzzleImages[index1];
            puzzleImages[index1] = puzzleImages[index2];
            puzzleImages[index2] = tempPuzzleImage;

            UpdatePositionGrid();
            UpdateColorGrid();
            CheckResult();
        }

        private void RotateImage(int index)
        {
            Image selectedImage = buttons[index].BackgroundImage;
            Bitmap bitmap = new Bitmap(selectedImage);
            bitmap.RotateFlip(RotateFlipType.Rotate90FlipNone);
            buttons[index].BackgroundImage = bitmap;

            puzzleImages[index].Rotate();
            UpdatePositionGrid();
            UpdateColorGrid();
            CheckResult();
        }

        private void CheckResult()
        {
            PuzzleImage prevRow = null, nextRow = null, prevCol = null, nextCol = null;
            int colorMatch = 0;
            int faceMatch = 0;

            for (int row = 0; row < rows; row++) 
            {
                for (int col = 0; col < cols; col++)
                {
                    int currentIndex = row * cols + col;
                    var current = puzzleImages[currentIndex];
                    if (currentIndex > 0) prevCol = puzzleImages[currentIndex - 1];
                    if (currentIndex > 2) prevRow = puzzleImages[currentIndex - 3];
                    if (currentIndex < 6) nextRow = puzzleImages[currentIndex + 3];
                    if (currentIndex < 8) nextCol = puzzleImages[currentIndex + 1];

                    switch (row)
                    {
                        case 0:
                            if (current.S_color == nextRow.N_color) colorMatch++;
                            if (current.S_eyes != nextRow.N_eyes) faceMatch++;
                            break;
                        case 1:
                            if (current.N_color == prevRow.S_color) colorMatch++;
                            if (current.S_color == nextRow.N_color) colorMatch++;
                            if (current.S_eyes != prevRow.N_eyes) faceMatch++;
                            if (current.S_eyes != nextRow.N_eyes) faceMatch++;
                            break;
                        case 2:
                            if (current.N_color == prevRow.S_color) colorMatch++;
                            if (current.N_eyes != prevRow.S_eyes) faceMatch++;
                            break;
                    }

                    switch(col)
                    {
                        case 0:
                            if (current.E_color == nextCol.W_color) colorMatch++;
                            if (current.E_eyes != nextCol.W_eyes) faceMatch++;
                            break;
                        case 1:
                            if (current.E_color == nextCol.W_color) colorMatch++;
                            if (current.W_color == prevCol.E_color) colorMatch++;
                            if (current.E_eyes != nextCol.W_eyes) faceMatch++;
                            if (current.W_eyes != prevCol.E_eyes) faceMatch++;
                            break;
                        case 2:
                            if (current.W_color == prevCol.E_color) colorMatch++;
                            if (current.W_eyes != prevCol.E_eyes) faceMatch++;
                            break;
                    }
                }
            }

            var matchResult = colorMatch + faceMatch;
            string message = "";
            var index = 0;
            foreach (var button in buttons)
            {
                index++;
                message = message + button.Tag.ToString() + " ";
                if (index % cols == 0) message = message + "\n";
            }
            string colorMessage = "Center N = " + puzzleImages[centerIndex].N_color.Name +", E = " + puzzleImages[centerIndex].E_color.Name +", S = " + puzzleImages[centerIndex].S_color.Name +", W = " + puzzleImages[centerIndex].W_color.Name; ;
            matchBox.Text = "MATCH BOX: " + matchResult.ToString() + "/" + maxMatch + "\n\n" + message + "\n" + colorMessage;

            if (matchResult == maxMatch) throw new PuzzleSolvedException();
        }
    }

    public class PuzzleSolvedException : Exception
    {
        public PuzzleSolvedException() : base("Puzzle solved successfully.") { }
    }
}