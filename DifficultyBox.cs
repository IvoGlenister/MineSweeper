using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;

namespace MineSweeper_0._1
{
    class DifficultyBox
    {
        public static void ShowDialog()//function called when the difficulty button is clicked
        {
            Form prompt = new Form()        //creates a new from
            {
                Width = 400,                                        //  \
                Height = 200,                                       //  |
                FormBorderStyle = FormBorderStyle.FixedDialog,      //  |-variables that desribe the new from
                Text = "Diffuclty",                                 //  |
                StartPosition = FormStartPosition.CenterScreen,     //  /

            };
            Label textLabel = new Label() { Left = 50, Top = 30, Text = "Mine Density:(e.g. 0.1,0.6)" };                                            //  \
            Label textLabe2 = new Label() { Left = 200, Top = 30, Text = "Grid Size:" };                                                            //  |
            TextBox textBox1 = new TextBox() { Left = 50, Top = 50, Width = 100, Text = "Mine Density" };                                           //  |
            TextBox textBox2 = new TextBox() { Left = 200, Top = 75, Width = 100, Text = "10 by 10", ReadOnly = true };                             //  |- creating new items for the form
            TrackBar trackBar1 = new TrackBar() { Left = 200, Top = 50, Width = 100, Maximum = 20, Minimum = 6, TickFrequency = 2, Value = 10 };    //  |
            Button confirmation = new Button() { Text = "Ok", Left = 125, Width = 100, Top = 125, DialogResult = DialogResult.OK };                 //  |
            Button reset = new Button() { Text = "Reset to Defualt", Left = 80, Width = 100, Top = 0, DialogResult = DialogResult.OK };             //  /
            confirmation.Click += (sender, e) => end(sender, e, prompt, textBox1.Text,trackBar1.Value);        //event manager for the OK button   
            //reset.Click += (sender, e) => { textBox1.Text = "Mine Density"; };                  
            trackBar1.Scroll += (sender, e) => scroll(sender, e, trackBar1.Value, textBox2);                   //event manager for track bar
            textBox1.Size = new System.Drawing.Size(100, 15);                                             
            textLabel.Size = new Size(150, 15);                                                                
            prompt.Controls.Add(textBox1);              //  \
            //prompt.Controls.Add(reset);               //  |
            prompt.Controls.Add(confirmation);          //  |
            prompt.Controls.Add(textBox2);              //  |-adds the items to the form
            prompt.Controls.Add(trackBar1);             //  |
            prompt.Controls.Add(textLabel);             //  |
            prompt.Controls.Add(textLabe2);             //  |
            prompt.AcceptButton = confirmation;         //  /
            prompt.Show();      //shows the form to the user
        }

        private static void scroll(object sender, EventArgs e, int value, TextBox textBox2) //event manager for track bar that lets the user drag the track bar to alter the grid size value
        {
            textBox2.Text = value + " by " + value;
        }

        private static void end(object sender, EventArgs e,Form prompt,string density, int size)        //event manager for OK button
        {
            string path_Density = @"C:\Users\ivogl\Desktop\Solver_MineSweeper\obj\Debug\mineDensity.txt";   
            File.WriteAllText(path_Density, density);                       //save the writtten mine density to it's external file
            string path_oldSize = @"C:\Users\ivogl\Desktop\Solver_MineSweeper\obj\Debug\OLD_gridSize.txt";
            string path_Size = @"C:\Users\ivogl\Desktop\Solver_MineSweeper\obj\Debug\gridSize.txt";
            File.WriteAllText(path_oldSize,File.ReadAllText(path_Size));        //saves the grids old size to it's external file
            File.WriteAllText(path_Size, Convert.ToString(size));               //saves the grids new size to it's external files

            prompt.Close();     //closes the form
        }
    }
}
