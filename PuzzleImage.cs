using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SmilePuzzle
{
    public class PuzzleImage
    {
        public string Name { get; set; }
        public Color N_color { get; set; }
        public Color S_color { get; set; }
        public Color E_color { get; set; }
        public Color W_color { get; set; }
        public bool N_eyes { get; set; }
        public bool S_eyes { get; set; }
        public bool E_eyes { get; set; }
        public bool W_eyes { get; set; }

        public PuzzleImage(string name, Color N, Color E, Color S, Color W, bool N_eyes, bool E_eyes, bool S_eyes, bool W_eyes)
        {
            this.Name = name;

            this.N_color = N;
            this.E_color = E;
            this.S_color = S;
            this.W_color = W;

            this.N_eyes = N_eyes;
            this.E_eyes = E_eyes;
            this.S_eyes = S_eyes;
            this.W_eyes = W_eyes;
        }

        public void Rotate()
        {
            var tmp = this.N_color;
            this.N_color = this.W_color;
            this.W_color = this.S_color;
            this.S_color = this.E_color;
            this.E_color = tmp;

            var tmp_eyes = this.N_eyes;
            this.N_eyes = this.W_eyes;
            this.W_eyes = this.S_eyes;
            this.S_eyes = this.E_eyes;
            this.E_eyes = tmp_eyes;
        }

        public Color[] Colors()
        {
            return new Color[] { this.N_color, this.E_color, this.S_color, this.W_color };
        }

        public bool[] Eyes()
        {
            return new bool[] { this.N_eyes, this.E_eyes, this.S_eyes, this.W_eyes };
        }
    }
}
