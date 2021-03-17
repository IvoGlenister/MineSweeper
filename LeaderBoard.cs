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
using LumenWorks.Framework.IO.Csv;

namespace MineSweeper_0._1
{
    class LeaderBoard
    {
        public class SearchParameters //new class that hold the parameters of each entry into the table 
        {
            public string name { get; set; }
            public string time { get; set; }
            public string size { get; set; }
            public string density { get; set; }
            public string score { get; set; }
            public string seed { get; set; }
            public string key { get; set; }
        }
        public static string Decrypt(string ciphertext , string wordshift)     //vernum decryption function
        {
            char letter;
            string plaintext = "";
            ciphertext = ciphertext.ToUpper();
            for (int i = 0; i < ciphertext.Length; i++)     //for loop that itterates through the cipherand key and shifts the plaintext approprietly
            {
                int shift = Convert.ToInt32(wordshift[i]-64);
                letter = Convert.ToChar(Convert.ToInt32(ciphertext[i]) - shift);
                if (Convert.ToInt32(letter) < 65)       //stops characters from being punctuation
                {
                    letter = Convert.ToChar(Convert.ToInt32(letter) + 26);
                }
                plaintext = plaintext + letter;
            }
            return plaintext;       //returns final value
        }
        public static void ShowDialog()  //function called when the show leaderBoard button is clicked
        {
            Form Board = new Form()     //create a new form
            {
                //Width = 700,
                //Height = 300,
                FormBorderStyle = FormBorderStyle.Sizable,
                Text = "Leader Board",
                StartPosition = FormStartPosition.CenterScreen,
            };
            TableLayoutPanel tableLayoutPanel = new TableLayoutPanel();     // \
            tableLayoutPanel.AutoSize = true;                               //  |
            Label Label_PlayerName = new Label();                           //  |
            Label Label_TimeTaken = new Label();                            //  |
            Label Label_GridSize= new Label();                              //  |-creating new elements for the form
            Label Label_MineDensity= new Label();                           //  |
            Label Label_Score= new Label();                                 //  |
            Label Label_GridSeed= new Label();                              //  /
            //VScrollBar vScrollBar1 = new VScrollBar();

            tableLayoutPanel.Name = "TableLayoutPanel1";                    // \
            tableLayoutPanel.Size = new System.Drawing.Size(700, 700);      //  |
            tableLayoutPanel.TabIndex = 0;                                  //  |
                                                                            //  |
                                                                            //  |
            Label_PlayerName.Text = "Player Name";                          //  |- asinging values to the new elements
            Label_TimeTaken.Text = "Time Taken";                            //  |
            Label_GridSize.Text = "Grid Size";                              //  |
            Label_MineDensity.Text = "Mine Density";                        //  |
            Label_Score.Text = "Score";                                     //  |
            Label_GridSeed.Text = "Grid Seed";                              // /


            Board.AutoScroll = true;        //adds a scrollbar to the form


            Board.Controls.Add(tableLayoutPanel);                                           //  \
            //Board.Controls.Add(vScrollBar1);                                              //  |
            tableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 52));            //  |
            tableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 30));            //  |
            tableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 20));       //  |
            tableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 20));       //  |
            tableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 20));       //  |
            tableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 20));       //  |
            tableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 20));       //  |
            //tableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 17));       //  |- adding the new elemnts to the form
                                                                                            //  |
                                                                                            //  |    
            tableLayoutPanel.Controls.Add(Label_PlayerName, 0, 0);                          //  |
            tableLayoutPanel.Controls.Add(Label_TimeTaken, 1, 0);                           //  |
            tableLayoutPanel.Controls.Add(Label_GridSize, 2, 0);                            //  |
            tableLayoutPanel.Controls.Add(Label_MineDensity, 3, 0);                         //  |
            tableLayoutPanel.Controls.Add(Label_Score, 4, 0);                               //  |
            //tableLayoutPanel.Controls.Add(Label_GridSeed, 5, 0);                            //  /

            //Retrieving Data---------------------------------------------------
            string path = @"C:\Users\ivogl\Desktop\Solver_MineSweeper\obj\Debug\LeaderBoard.csv";
            DataTable table = new DataTable();      //create a new object called a DataTable
                using (var csvReader = new CsvReader(new StreamReader(File.OpenRead(path)), true))  //reads the data from the leaderbaord file
                {
                    table.Load(csvReader);
                }

            List<SearchParameters> searchParameters = new List<SearchParameters>(); //cerates an object of the searchParameter class
            //MessageBox.Show(Convert.ToString(table.Rows.Count));
            for (int i = 0; i < table.Rows.Count; i++)          //for loop that assings a value to each variable in the search parameter class for each piece of data from the file 
            {
                searchParameters.Add(new SearchParameters
                {
                    name = table.Rows[i][0].ToString(),
                    time = table.Rows[i][1].ToString(),
                    size = table.Rows[i][2].ToString(),
                    density = table.Rows[i][3].ToString(),
                    score = table.Rows[i][4].ToString(),
                    seed = table.Rows[i][5].ToString(),
                    key = table.Rows[i][6].ToString()
                });
            }
            //
            //
            //---------------------------------------------------------------------
            //
            //
            searchParameters = searchParameters.OrderBy(value => value.score).ToList();   //sorts each entry by score

            for (int i = 0; i < searchParameters.Count; i++)
            {
                string key = searchParameters[i].score;
                searchParameters[i].name = Decrypt(searchParameters[i].name, searchParameters[i].key);
            }

                for (int i = 0; i < searchParameters.Count; i++)//for each item in the search paramter object:
            {
                tableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 30));  //add a new row to the grid the data will be stored in
                Board.Width = tableLayoutPanel.Width + 50; //widen the grid slightly to accomodate
                if (tableLayoutPanel.Height < 700)
                {
                    Board.Height = tableLayoutPanel.Height;
                }
                else
                {
                    Board.Height = 700;
                }
                Label PlayerName = new Label();     //  \
                Label TimeTaken = new Label();      //  |
                Label GridSize = new Label();       //  |
                Label MineDensity = new Label();    //  |- creates new lable for each item in the search parameters
                Label Score = new Label();          //  |
                //Label GridSeed = new Label();       //  /

                PlayerName.Text = searchParameters[i].name;         //  \
                TimeTaken.Text = searchParameters[i].time;          //  |
                GridSize.Text = searchParameters[i].size;           //  |-assigns each label a value from the seach parameters
                MineDensity.Text = searchParameters[i].density;     //  |
                Score.Text = searchParameters[i].score;             //  |
                //GridSeed.Text = searchParameters[i].seed;           //  /

                tableLayoutPanel.Controls.Add(PlayerName, 0, i+1);      //  \
                tableLayoutPanel.Controls.Add(TimeTaken, 1, i+1);       //  |
                tableLayoutPanel.Controls.Add(GridSize, 2, i+1);        //  |
                tableLayoutPanel.Controls.Add(MineDensity, 3, i+1);     //  |-add each label to the board (grid)
                tableLayoutPanel.Controls.Add(Score, 4, i+1);           //  |
                //tableLayoutPanel.Controls.Add(GridSeed, 5, i+1);        //  /

            }

            //-----------------------------------------------------------------


            Board.Show();       //shows the leader board to the player;
        }
    }
}
