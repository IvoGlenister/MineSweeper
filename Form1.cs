using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Reflection;
using System.IO;
using LumenWorks.Framework.IO.Csv;
using Microsoft.VisualBasic;
//using NodaTime;

namespace MineSweeper_0._1
{
    public partial class Form1 : Form
    {
        bool first = true;                                                                          //Global variables that are used throughout the program 
        bool gameend = false;
        bool flagno = true;
        int safesquares = 1;
        int total = 0;
        string grid_seed = "";
        int total2 = 0;
        Dictionary<string, bool> minelayout = new Dictionary<string, bool>();                      //dictionarys are used a lot as a way of tracking values for each cell
        Dictionary<string, bool> flaggedmines = new Dictionary<string, bool>();
        Dictionary<string, bool> usedmines = new Dictionary<string, bool>();
        Dictionary<string, bool> adjacentcells = new Dictionary<string, bool>();
        Dictionary<string, bool> flaggedmines_BOT = new Dictionary<string, bool>();

        TableLayoutPanel grid = new TableLayoutPanel();                                         //creates the grid that the cells will be placed in



        //resourses ---------------------------------------------------------
        Image Cell = Properties.Resources.cell;                                                //resourses defined globaly so all functions and procedures will be able to access them
        Image Cell_mine_clicked = Properties.Resources.Cell_mine_clicked;
        Image Cell_mine = Properties.Resources.Cell_mine;
        Image Cell_flag = Properties.Resources.cell_flag;
        //-------------------------------------------------------------------
        public Form1()
        {
            InitializeComponent();                                  
            gernerateGrid();                                    //initial creating of the grid of cells
        }
        private string createCellName(int x,int y)
        {
            string name = "cell_" + Convert.ToString(x) + "," + Convert.ToString(y);            //function used to convert a set of coordinates to the name of the cell as it is stored in the dictionaries
            return name;
        }
        private void gernerateGrid()                    //function that creates the grid of cells 
        {
            grid = new TableLayoutPanel();                  //assigns the grid object as a new data structure (instead of the one from the last grid generation)
            Random rnd = new Random();
            grid.Location = new Point(12,38);           //places the grid on the form
            grid.Size = new Size(756, 578);             //defines how large the grid is

            grid.ColumnCount = getSize();               //defines the amount of columns in the grid
            grid.RowCount = getSize();                  //defines the amount of rows in the grid
                

            //Dictionary<string, bool> minelayout_reset = new Dictionary<string, bool>();
            //Dictionary<string, bool> flaggedmines_reset = new Dictionary<string, bool>();
            //Dictionary<string, bool> usedmines_reset = new Dictionary<string, bool>();
            //Dictionary<string, bool> adjacentcell_reset = new Dictionary<string, bool>();
            minelayout.Clear();                 //clears all the dictionaries off data from last grid generation
            flaggedmines.Clear();
            usedmines.Clear();
            adjacentcells.Clear();
            flaggedmines_BOT.Clear();
            for (int i = 0; i < grid.ColumnCount; i++)      //for loops to regenerate the dictioanries acording to the size of the new grid
            {
                for (int j = 0; j < grid.RowCount; j++)
                {
                    string cell = createCellName(i, j);     //create name of each cell dynamically
                    minelayout.Add(cell, false);
                    flaggedmines.Add(cell, false);
                    usedmines.Add(cell, false);
                    adjacentcells.Add(cell, false);
                    flaggedmines_BOT.Add(cell, false);
                }
            }

            int mineNo = Convert.ToInt32(grid.RowCount * grid.ColumnCount * Convert.ToDouble(getMineDensity()));    //varaible is asigned to the total number of mines that will be in the grid according to its size and the mine density
            //int mineNo = Math.Round(Convert.ToInt32(No));
            MineRemaining_Box.Text = Convert.ToString(mineNo);        //sets the UI Mine remaing box to the correct value
            safesquares = (grid.ColumnCount * grid.RowCount) - mineNo;      //calculates and assigns how many squares won't be mines
            while (safesquares < 20)
            {
                mineNo--;
                MineRemaining_Box.Text = Convert.ToString(mineNo);
                safesquares = (grid.ColumnCount * grid.RowCount) - mineNo;
            }
            int placed = 0;
            while (placed < mineNo)     //while loop makes random squares in the grid mines untill all of the mines have been placed
            {
                int y = rnd.Next(0, grid.RowCount);
                int x = rnd.Next(0, grid.ColumnCount);
                string cell = createCellName(x, y);
                if (minelayout[cell] == false)
                {
                    minelayout[cell] = true;
                    placed++;
                }
            }


            for (int i = 0; i < grid.ColumnCount; i++)          //for loops that make the grid columns the correct size depending on the amount of columns
            {
                grid.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100 / grid.ColumnCount * 48));
                this.Width = grid.Width + 40;
            }
            for (int j = 0; j < grid.RowCount; j++)             //for loops that make the grid rows the correct size depending on the amount of rows
            {
                grid.RowStyles.Add(new RowStyle(SizeType.Percent, 100 / grid.RowCount * 48));
                this.Height = grid.Height + 90;
            }

            for (int i = 0; i < grid.ColumnCount; i++)      //for loop that generates the cells themselves
            {
                for (int j = 0; j < grid.RowCount; j++)
                {
                    Cell button = new Cell();           //Cells are a child object of the Button class with inherited properties
                    button.Name = string.Format(createCellName(i, j));
                    button.BackgroundImage = Image.FromFile(@"C:\Users\ivogl\Desktop\Solver_MineSweeper\obj\Debug\resourses\cell.png");    //sets the cell to the correct image 
                    button.Dock = DockStyle.Fill;                                                 //   \
                    button.Width = 100 / grid.ColumnCount * 48;                                   //   |
                    button.Height = 100 / grid.RowCount * 48;                                     //   |
                    button.BackgroundImageLayout = ImageLayout.Stretch;                           //   |
                    button.Padding = new Padding(0);                                              //   |- assigings cell properties so they look corrrect inside of the grid
                    button.Margin = new Padding(0);                                               //   |
                    button.FlatStyle = FlatStyle.Flat;                                            //   |
                    button.FlatAppearance.BorderSize = 0;                                         //   |
                    button.Click += (sender2, e2) => Cell_Click(sender2, e2, button);             //   /
                    button.XCoord = i;
                    button.YCoord = j;
                    System.Windows.Forms.ToolTip ToolTip1 = new System.Windows.Forms.ToolTip();    
                    ToolTip1.SetToolTip(button, Convert.ToString(button.Name));      //gives the cells a tooltip that shows their names this is mostly for error testing

                    grid.Controls.Add(button, i, j);        //placed the created cell in the grid in the correct spot

                }
                Controls.Add(grid);     //places the grid in the form
                //grid_seed = Convert.ToString(getSize()) + "_";
                //string seedCHAR = "";
                //foreach (KeyValuePair<string, bool> item in minelayout)
                //{
                //    if (minelayout[item.Key])
                //    {
                //        seedCHAR = seedCHAR + "1";
                //    }
                //    else
                //    {
                //        seedCHAR = seedCHAR + "0";
                //    }
                //    if (seedCHAR.Length == 8)
                //    {
                //        string ascii = null;
                //        for (i = 0; i < 8; i += 8)
                //        {
                //            ascii += Convert.ToChar(seedCHAR.Substring(i, 1));
                //        }
                //        grid_seed = grid_seed + ascii;
                //    }
                //}
            }
            //MessageBox.Show(Convert.ToString(grid.RowCount*grid.ColumnCount));
        }
        private int getSize()       //function that gets the size of the grid from an external file(x and y are sthe same as the grid is always square)
        {
            string path = @"C:\Users\ivogl\Desktop\Solver_MineSweeper\obj\Debug\gridSize.txt";
            int size = Convert.ToInt32(File.ReadAllText(path));
            return size;
        }
        private void viewallmines()     //function the is called at the end of the game to open all of the cells and dispaly which ones where mines and which weren't
        {
            int rowCount  = getSize();
            int columnCount = getSize();
            for (int i = 0; i < columnCount; i++)       //nested for loops that itterate through each cell:
            {
                for (int j = 0; j < rowCount; j++)
                {
                    string cell = createCellName(i, j);
                    if (flaggedmines[cell])                 //if the cell is flagged, its reset to a flag image
                    {
                        Cell btn = grid.Controls.OfType<Cell>().Where(x => x.Name == cell).FirstOrDefault();
                        btn.BackgroundImage = Cell_flag;
                    }
                    else if (minelayout[cell] == false)     //if the cell is safe the adjacentmine() function is called
                    {
                        Cell btn = grid.Controls.OfType<Cell>().Where(x => x.Name == cell).FirstOrDefault();
                        numadjacentmines(btn,true);   //this function get the number of adjacent mines to a cell and if the second called variable is true sets the cells background to the correct number (empty to 8)
                    }
                    else
                    {
                        Cell btn = grid.Controls.OfType<Cell>().Where(x => x.Name == cell).FirstOrDefault();  //if the cell is a mine its set to the mine image
                        btn.BackgroundImage = Cell_mine;
                    }
                }
            }
        }
        private void floodfill(Cell name)      //recursive flood fill algorithm for when a safe cell with no adjacent mines is opened
        {
            int rowCount = getSize();
            int columnCount = getSize();
            if (!name.flagged())            //only works if the cell isn;t flagged
            {

                int adjacent = numadjacentmines(name,true);  //finds the number of adjcent mines, also set the image of the cell to the correct number
                if ((adjacent == 0) && !usedmines[name.Name])   //if the cell has 0 adjacent mines:
                {
                    string cellname = name.Name;
                    usedmines[cellname] = true;     //adds the cell to this dictioanry so the algorithm won't try to open this cell again
                    bool skipN = false;   // \
                    bool skipS = false;   // |-variables used if the cell is at an edge so some adjacent cells dont actually exist
                    bool skipE = false;   // |
                    bool skipW = false;   // /

                    int index = cellname.IndexOf(',');          //finds the cells corrdinates from it's name
                    string x = cellname.Substring(5, index - 5);
                    string y = cellname.Substring(index + 1);

                    int N = Convert.ToInt32(y) - 1;   // \
                    int E = Convert.ToInt32(x) + 1;   // |-gets a North,South,East,West from those coordinates
                    int S = Convert.ToInt32(y) + 1;   // |
                    int W = Convert.ToInt32(x) - 1;   // /
                    if (N < 0)                 // \
                    {                          // |
                        skipN = true;          // |
                        N++;                   // |
                    }                          // |
                    if (E >= columnCount)      // |
                    {                          // |
                        skipE = true;          // |
                        E--;                   // |
                    }                          // |- if the cell is on an edge ignore cells that don't exist
                    if (S >= rowCount)         // |
                    {                          // |
                        skipS = true;          // |
                        S--;                   // |
                    }                          // |
                    if (W < 0)                 // |
                    {                          // |
                        skipW = true;          // |
                        W++;                   // |
                    }                          // /
                    //MessageBox.Show(Convert.ToString(N), Convert.ToString(S));
                    string NW = createCellName(W, N);                   // \
                    string NN = createCellName(Convert.ToInt32(x), N);  // |
                    string NE = createCellName(E, N);                   // |
                    string WW = createCellName(W, Convert.ToInt32(y));  // |-gets the name of all cells that are adjacent to the first cell
                    string EE = createCellName(E, Convert.ToInt32(y));  // |
                    string SW = createCellName(W, S);                   // |
                    string SS = createCellName(Convert.ToInt32(x), S);  // |
                    string SE = createCellName(E, S);                   // /
                    if (!skipN && !skipW)                                               //all these if statements are checking if the cell is off the edge of the grid
                    {
                        Cell btn = grid.Controls.OfType<Cell>().Where(X => X.Name == NW).FirstOrDefault();      //take cell name as a string and assings the variable as a cell itself 
                        floodfill(btn);                                                                         //recursivly call this function for the new cell
                    }
                    if (!skipN)
                    {
                        Cell btn = grid.Controls.OfType<Cell>().Where(X => X.Name == NN).FirstOrDefault();      //take cell name as a string and assings the variable as a cell itself 
                        floodfill(btn);                                                                         //recursivly call this function for the new cell
                    }
                    if (!skipN && !skipE)
                    {
                        Cell btn = grid.Controls.OfType<Cell>().Where(X => X.Name == NE).FirstOrDefault();      //take cell name as a string and assings the variable as a cell itself
                        floodfill(btn);                                                                         //recursivly call this function for the new cell
                    }
                    if (!skipW)
                    {
                        Cell btn = grid.Controls.OfType<Cell>().Where(X => X.Name == WW).FirstOrDefault();      //take cell name as a string and assings the variable as a cell itself
                        floodfill(btn);                                                                         //recursivly call this function for the new cell
                    }
                    if (!skipE)
                    {
                        Cell btn = grid.Controls.OfType<Cell>().Where(X => X.Name == EE).FirstOrDefault();      //take cell name as a string and assings the variable as a cell itself
                        floodfill(btn);                                                                         //recursivly call this function for the new cell
                    }
                    if (!skipS && !skipW)
                    {
                        Cell btn = grid.Controls.OfType<Cell>().Where(X => X.Name == SW).FirstOrDefault();      //take cell name as a string and assings the variable as a cell itself
                        floodfill(btn);                                                                         //recursivly call this function for the new cell
                    }
                    if (!skipS)
                    {
                        Cell btn = grid.Controls.OfType<Cell>().Where(X => X.Name == SS).FirstOrDefault();      //take cell name as a string and assings the variable as a cell itself
                        floodfill(btn);                                                                         //recursivly call this function for the new cell
                    }
                    if (!skipS && !skipE)
                    {
                        Cell btn = grid.Controls.OfType<Cell>().Where(X => X.Name == SE).FirstOrDefault();      //take cell name as a string and assings the variable as a cell itself
                        floodfill(btn);                                                                         //recursivly call this function for the new cell
                    }
                    //Cell Btn = grid.Controls.OfType<Cell>().Where(X => X.Name == "cell_xy").FirstOrDefault();
                    //System.Windows.Forms.ToolTip ToolTip1 = new System.Windows.Forms.ToolTip();
                    //ToolTip1.SetToolTip(grid.Controls.OfType<Cell>().Where(X => X.Name == SE).FirstOrDefault(), Convert.ToString(grid.Controls.OfType<Cell>().Where(X => X.Name == SE).FirstOrDefault().closed()));
                }
            }
        }
        private int numadjacentmines(Cell name,bool change)     //finds the number of adjacent mines to the cell passed into the function
        {
            int rowCount = getSize();            //THIS FUNCTION IS VERY SIMILAR TO THE FLOOD FILL FUNCTION ABOVE PLEASE REFER TO ITS COMMENTS FOR CLARIFICATION
            int columnCount = getSize();         //THIS FUNCTION IS VERY SIMILAR TO THE FLOOD FILL FUNCTION ABOVE PLEASE REFER TO ITS COMMENTS FOR CLARIFICATION
            int adjacent = 0;                    //THIS FUNCTION IS VERY SIMILAR TO THE FLOOD FILL FUNCTION ABOVE PLEASE REFER TO ITS COMMENTS FOR CLARIFICATION
            string cellname = name.Name;         //THIS FUNCTION IS VERY SIMILAR TO THE FLOOD FILL FUNCTION ABOVE PLEASE REFER TO ITS COMMENTS FOR CLARIFICATION

            bool skipN = false;
            bool skipS = false;
            bool skipE = false;
            bool skipW = false;

            Cell btn = grid.Controls.OfType<Cell>().Where(X => X.Name == cellname).FirstOrDefault();
            btn.Closed = false;

            string x = Convert.ToString(btn.xcoord());
            string y = Convert.ToString(btn.ycoord());

            int N = Convert.ToInt32(y) - 1;
            int E = Convert.ToInt32(x) + 1;
            int S = Convert.ToInt32(y) + 1;
            int W = Convert.ToInt32(x) - 1;
            if (N < 0)
            {
                skipN = true;
                N++;
            }
            if (E >= columnCount)
            {
                skipE = true;
                E--;
            }
            if (S >= rowCount)
            {
                skipS = true;
                S--;
            }
            if (W < 0)
            {
                skipW = true;
                W++;
            }
            //MessageBox.Show(Convert.ToString(N), Convert.ToString(S));
            string NW = createCellName(W, N);
            string NN = createCellName(Convert.ToInt32(x), N);
            string NE = createCellName(E, N);
            string WW = createCellName(W, Convert.ToInt32(y));
            string EE = createCellName(E, Convert.ToInt32(y));
            string SW = createCellName(W, S);
            string SS = createCellName(Convert.ToInt32(x), S);
            string SE = createCellName(E, S);
            if ((!skipN && !skipW) && minelayout[NW])
            {
                adjacent++;
                //MessageBox.Show(NW);
            }
            if (!skipN &&(minelayout[NN]))
            {
                adjacent++;
                //MessageBox.Show(NN);
            }
            if ((!skipN && !skipE) && minelayout[NE])
            {
                adjacent++;
                //MessageBox.Show(NE);
            }
            if (!skipW && minelayout[WW])
            {
                adjacent++;
                //MessageBox.Show(WW);
            }
            if (!skipE && minelayout[EE])
            {
                adjacent++;
                //MessageBox.Show(EE);
            }
            if ((!skipS && !skipW) && minelayout[SW])
            {
                adjacent++;
                //MessageBox.Show(SW);
            }
            if (!skipS && minelayout[SS])
            {
                adjacent++;
                //MessageBox.Show(SS);
            }
            if ((!skipS && !skipE) && minelayout[SE])
            {
                adjacent++;
                //MessageBox.Show(SE);
            }
            //MessageBox.Show(Convert.ToString(adjacent));
            Image empty = Image.FromFile(@"C:\Users\ivogl\Desktop\Solver_MineSweeper\obj\Debug\resourses\cell_empty.png"); // \
            Image one = Image.FromFile(@"C:\Users\ivogl\Desktop\Solver_MineSweeper\obj\Debug\resourses\1.png");            // |
            Image two = Image.FromFile(@"C:\Users\ivogl\Desktop\Solver_MineSweeper\obj\Debug\resourses\2.png");            // |
            Image three = Image.FromFile(@"C:\Users\ivogl\Desktop\Solver_MineSweeper\obj\Debug\resourses\3.png");          // |
            Image four = Image.FromFile(@"C:\Users\ivogl\Desktop\Solver_MineSweeper\obj\Debug\resourses\4.png");           // |- resourses for each of the cell numbers depending on the No. of adjacent cells
            Image five = Image.FromFile(@"C:\Users\ivogl\Desktop\Solver_MineSweeper\obj\Debug\resourses\5.png");           // |
            Image six = Image.FromFile(@"C:\Users\ivogl\Desktop\Solver_MineSweeper\obj\Debug\resourses\6.png");            // |
            Image seven = Image.FromFile(@"C:\Users\ivogl\Desktop\Solver_MineSweeper\obj\Debug\resourses\7.png");          // |
            Image eight = Image.FromFile(@"C:\Users\ivogl\Desktop\Solver_MineSweeper\obj\Debug\resourses\8.png");          // /
            if (change)     //if the funcion call wanted the cell's image to be changed
            {
                name.Closed = false;    
                if (adjacent == 0)                  //  \
                {                                   //  |
                    name.BackgroundImage = empty;   //  |
                }                                   //  |   
                else if (adjacent == 1)             //  |
                {                                   //  |
                    name.BackgroundImage = one;     //  |
                }                                   //  |
                else if (adjacent == 2)             //  |
                {                                   //  |
                    name.BackgroundImage = two;     //  |
                }                                   //  |
                else if (adjacent == 3)             //  |
                {                                   //  |
                    name.BackgroundImage = three;   //  |
                }                                   //  |
                else if (adjacent == 4)             //  |
                {                                   //  |- changes the image of the cell depending on the No. of adjacent mines
                    name.BackgroundImage = four;    //  |
                }                                   //  |
                else if (adjacent == 5)             //  |
                {                                   //  |
                    name.BackgroundImage = five;    //  |
                }                                   //  |
                else if (adjacent == 6)             //  |
                {                                   //  |
                    name.BackgroundImage = six;     //  |
                }                                   //  |
                else if (adjacent == 7)             //  |
                {                                   //  |
                    name.BackgroundImage = seven;   //  |
                }                                   //  |
                else if (adjacent == 8)             //  |
                {                                   //  |
                    name.BackgroundImage = eight;   //  |
                }                                   //  /
                                    

                if (adjacent != 0)      //if the cell is adjacent to mines:
                {
                    adjacentcells[name.Name] = true;        //sets it's va;lue to true in the adjacent to mines dictioanry (used in the 'AI' solver algorithm)
                }
            }
            return adjacent;    //returns the number of adjacent mines
        }
        private List<string> findadjacentcell_closed(Cell name)     //finds the number of adjacent cells that are closed (mine or otherwise)
        {
            int rowCount = getSize();                       //THIS FUNCTION IS VERY SIMILAR TO THE FLOOD FILL FUNCTION ABOVE PLEASE REFER TO ITS COMMENTS FOR CLARIFICATION
            int columnCount = getSize();                    //THIS FUNCTION IS VERY SIMILAR TO THE FLOOD FILL FUNCTION ABOVE PLEASE REFER TO ITS COMMENTS FOR CLARIFICATION
            List<string> adjacent = new List<string>();     //THIS FUNCTION IS VERY SIMILAR TO THE FLOOD FILL FUNCTION ABOVE PLEASE REFER TO ITS COMMENTS FOR CLARIFICATION
            string cellname = name.Name;                    //THIS FUNCTION IS VERY SIMILAR TO THE FLOOD FILL FUNCTION ABOVE PLEASE REFER TO ITS COMMENTS FOR CLARIFICATION
            bool skipN = false;
            bool skipS = false;
            bool skipE = false;
            bool skipW = false;

            Cell btn = grid.Controls.OfType<Cell>().Where(X => X.Name == cellname).FirstOrDefault();

            string x = Convert.ToString(btn.xcoord());
            string y = Convert.ToString(btn.ycoord());

            int N = Convert.ToInt32(y) - 1;
            int E = Convert.ToInt32(x) + 1;
            int S = Convert.ToInt32(y) + 1;
            int W = Convert.ToInt32(x) - 1;
            if (N < 0)
            {
                skipN = true;
                N++;
            }
            if (E >= columnCount)
            {
                skipE = true;
                E--;
            }
            if (S >= rowCount)
            {
                skipS = true;
                S--;
            }
            if (W < 0)
            {
                skipW = true;
                W++;
            }
            //MessageBox.Show(Convert.ToString(N), Convert.ToString(S));
            string one = createCellName(W, N);
            string two = createCellName(Convert.ToInt32(x), N);
            string three = createCellName(E, N);
            string four = createCellName(W, Convert.ToInt32(y));
            string five = createCellName(E, Convert.ToInt32(y));
            string six = createCellName(W, S);
            string seven = createCellName(Convert.ToInt32(x), S);
            string eight = createCellName(E, S);
            Cell NW = grid.Controls.OfType<Cell>().Where(X => X.Name == one).FirstOrDefault();
            Cell NN = grid.Controls.OfType<Cell>().Where(X => X.Name == two).FirstOrDefault();
            Cell NE = grid.Controls.OfType<Cell>().Where(X => X.Name == three).FirstOrDefault();
            Cell WW = grid.Controls.OfType<Cell>().Where(X => X.Name == four).FirstOrDefault();
            Cell EE = grid.Controls.OfType<Cell>().Where(X => X.Name == five).FirstOrDefault();
            Cell SW = grid.Controls.OfType<Cell>().Where(X => X.Name == six).FirstOrDefault();
            Cell SS = grid.Controls.OfType<Cell>().Where(X => X.Name == seven).FirstOrDefault();
            Cell SE = grid.Controls.OfType<Cell>().Where(X => X.Name == eight).FirstOrDefault();
            if ((!skipN && !skipW) && NW.closed())
            {
                adjacent.Add(one);
            }
            if (!skipN && NN.closed())
            {
                adjacent.Add(two);
            }
            if ((!skipN && !skipE) && NE.closed())
            {
                adjacent.Add(three);
            }
            if (!skipW && WW.closed())
            {
                adjacent.Add(four);
            }
            if (!skipE && EE.closed())
            {
                adjacent.Add(five);
            }
            if (!skipS && !skipW && SW.closed())
            {
                adjacent.Add(six);
            }
            if (!skipS && SS.closed())
            {
                adjacent.Add(seven);
            }
            if ((!skipS && !skipE) && SE.closed())
            {
                adjacent.Add(eight);
            }
            return adjacent;

        }
        private List<string> findadjacentcells(Cell name)
        {
            int rowCount = getSize();
            int columnCount = getSize();
            List<string> adjacent = new List<string>();
            string cellname = name.Name;
            bool skipN = false;
            bool skipS = false;
            bool skipE = false;
            bool skipW = false;



            Cell btn = grid.Controls.OfType<Cell>().Where(X => X.Name == cellname).FirstOrDefault();

            string x = Convert.ToString(btn.xcoord());
            string y = Convert.ToString(btn.ycoord());

            int N = Convert.ToInt32(y) - 1;
            int E = Convert.ToInt32(x) + 1;
            int S = Convert.ToInt32(y) + 1;
            int W = Convert.ToInt32(x) - 1;
            if (N < 0)
            {
                skipN = true;
                N++;
            }
            if (E >= columnCount)
            {
                skipE = true;
                E--;
            }
            if (S >= rowCount)
            {
                skipS = true;
                S--;
            }
            if (W < 0)
            {
                skipW = true;
                W++;
            }
            //MessageBox.Show(Convert.ToString(N), Convert.ToString(S));
            string one = createCellName(W, N);
            string two = createCellName(Convert.ToInt32(x), N);
            string three = createCellName(E, N);
            string four = createCellName(W, Convert.ToInt32(y));
            string five = createCellName(E, Convert.ToInt32(y));
            string six = createCellName(W, S);
            string seven = createCellName(Convert.ToInt32(x), S);
            string eight = createCellName(E, S);
            Cell NW = grid.Controls.OfType<Cell>().Where(X => X.Name == one).FirstOrDefault();
            Cell NN = grid.Controls.OfType<Cell>().Where(X => X.Name == two).FirstOrDefault();
            Cell NE = grid.Controls.OfType<Cell>().Where(X => X.Name == three).FirstOrDefault();
            Cell WW = grid.Controls.OfType<Cell>().Where(X => X.Name == four).FirstOrDefault();
            Cell EE = grid.Controls.OfType<Cell>().Where(X => X.Name == five).FirstOrDefault();
            Cell SW = grid.Controls.OfType<Cell>().Where(X => X.Name == six).FirstOrDefault();
            Cell SS = grid.Controls.OfType<Cell>().Where(X => X.Name == seven).FirstOrDefault();
            Cell SE = grid.Controls.OfType<Cell>().Where(X => X.Name == eight).FirstOrDefault();
            if (!skipN && !skipW)
            {
                adjacent.Add(one);
            }
            if (!skipN)
            {
                adjacent.Add(two);
            }
            if (!skipN && !skipE)
            {
                adjacent.Add(three);
            }
            if (!skipW)
            {
                adjacent.Add(four);
            }
            if (!skipE)
            {
                adjacent.Add(five);
            }
            if (!skipS && !skipW)
            {
                adjacent.Add(six);
            }
            if (!skipS)
            {
                adjacent.Add(seven);
            }
            if (!skipS && !skipE)
            {
                adjacent.Add(eight);
            }
            return adjacent;

        }   //unused function from a previouse itteration

        private double getMineDensity()     //function that gets the mine density of the grid from an external file
        {
            double Density = 0.0;
            string path = @"C:\Users\ivogl\Desktop\Solver_MineSweeper\obj\Debug\mineDensity.txt";
            try                                         //validation satement, if the value entered by the player into the files is invalid a default value (0.1) is used
            {
                Density = Convert.ToDouble(File.ReadAllText(path));
            }
            catch
            {
                Density = 0.1;
            }
            return Density;
        }
        private void WritetoBoard(string data)  //function that saves a won games data in an external file
        {
            string path = @"C:\Users\ivogl\Desktop\Solver_MineSweeper\obj\Debug\LeaderBoard.csv";

            File.AppendAllText(path , data);
            MessageBox.Show("saved to file"); //UI message letting the player know that their data has been saved to an external file
        }
        private void checkwin()             //function to check if the player has won
        {
            int total = getSize() * getSize();                      //  \
            int mines = Convert.ToInt32(total * getMineDensity());  //  |-finding the total number of squares,mines and safe sqaures
            int safe = total - mines;                               //  /
            int count = 0;
            foreach (KeyValuePair<string, bool> square in minelayout)   //for each loop, itterates through every cell in grid
            {
                Cell check = grid.Controls.OfType<Cell>().Where(x => x.Name == square.Key).FirstOrDefault();
                if (!check.Closed)                //if the cell is open the count is incremented
                {
                    count++;
                }
            }
            if (count == safe)  //if the count = the total number of safge squares the game is won:
            {
                gameend = true;  //variable used to stop other function happening after the game has ended
                MessageBox.Show("You Won!");
                DialogResult dialogResult = MessageBox.Show("Do you want to save your score to an external file?", "Save game?", MessageBoxButtons.YesNo);      //asks the player if they want to safe there game in an external file
                if (dialogResult == DialogResult.Yes)       //if they do
                {
                    string playerName = Interaction.InputBox("Please enter your name for the leader board (letter's only)", "Player name?", "name");  //gets the players prefered name
                    playerName = playerName.ToUpper();
                    string newName = "";
                    char[] alphabet = new char[26] { 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z' };
                    for (int i = 0; i < playerName.Length; i++)     //if the player enters a ',' because the file is saved as a CSV they would overwrite other data in the csv file
                    {                                               // this removes any commas from the player's name
                        if ((alphabet.Contains(playerName[i])))
                        {
                            newName += playerName[i];
                        }
                    }
                    //MessageBox.Show(newName);
                    //MessageBox.Show(total.ToString());
                    Random rnd = new Random();
                    string key = "";
                    for (int i = 0; i < 75; i++)
                    {
                        int temp = rnd.Next(65, 90);
                        key = key + Convert.ToChar(temp);
                    }
                    string Time = Clock_Box.Text;                                                                                               //  \
                    string gridSize = getSize() + " by " + getSize();                                                                           //  |
                    string mineDensity = Convert.ToString(getMineDensity());                                                                    //  |
                    string score = Convert.ToString(((Convert.ToInt32(getSize()) / (total + 1))) * 1000 * getMineDensity() + total);            //  /- getting other data about the game that must be saved with the playername                                                                                      
                    string data = encrypt(newName, key )+ "," + Time + "," + gridSize + "," + mineDensity + "," + score + "," + grid_seed + "," + key + "\n";             //each item is encrypted with the encrypt() function and then concatonated
                    //string display = "size: " + gridSize + ", " + "score: " + score;
                    //MessageBox.Show(display);
                    WritetoBoard(data);   //calls a function that saves that data to the file
                }
            }
        }
        public static string encrypt(string plaintext, string wordshift)      //vernum encryption function
        {
            char letter;
            string ciphertext = "";
            //string wordshift = "";
            Random random = new Random();
            plaintext = plaintext.ToUpper();
            Console.WriteLine("The random Key, created specifically for this Encoding is: " + wordshift);       //outputs the key
            for (int i = 0; i < plaintext.Length; i++)      //for loop that itterates through the plaintext and key and shifts the plaintext approprietly
            {
                int shift = Convert.ToInt32(wordshift[i]) - 64;
                letter = Convert.ToChar(Convert.ToInt32(plaintext[i]) + shift);
                if (Convert.ToInt32(letter) > 90)        //stops characters from being punctuation
                {
                    letter = Convert.ToChar(Convert.ToInt32(letter) - 26);
                }
                ciphertext = ciphertext + letter;
            }
            //MessageBox.Show(ciphertext);
            return ciphertext;      //returns final value
    }
        private void Cell_Click(object sender, EventArgs e, Cell name)      //Primary function in the code deals with when any cell is clicked on
        {
            //MessageBox.Show(Convert.ToString(findadjacentcell_closed(name).Count));
            //MessageBox.Show(string.Join(",", findadjacentcell_closed(name)));
            //MessageBox.Show("CLOSED:"+Convert.ToString(name.closed())+"FLAGGED:"+ Convert.ToString(name.flagged()));
            //MouseEventArgs MS = (MouseEventArgs)e;
            //if (MS.Clicks >= 2)
            //{
            //    MessageBox.Show("Flag :)");
            //}
            int rowCount = getSize();
            int columnCount = getSize();
            Random rnd = new Random();
            if (first && name.closed() && flagno)                   //if this is the first time a cell is clicked:
            {
                List<string> adjacentCells = findadjacentcell_closed(name);
                minelayout[name.Name] = false;
                Cell btn = grid.Controls.OfType<Cell>().Where(X => X.Name == name.Name).FirstOrDefault();
                foreach (string U in adjacentCells)                     //this algorithm removes all mines in a 3 by 3 grid around the clicked cell and places them elsewhere in the grid
                {                                                       //while this does simplify the game, it makes it much more satisfying, and fun, to play
                    //MessageBox.Show(U);
                    bool placed = false;
                    while (!placed)                 //while the removed mine hasn't been placed
                    {
                        while (minelayout[U])       //while the cell still has a mine in it
                        {
                            int x = btn.xcoord();       //cells x coord
                            int y = btn.ycoord();       //cells y coord
                            int Y = rnd.Next(0, rowCount);  //random new y coord
                            int X = rnd.Next(0, columnCount); //random new x coord
                            if (Y == y)             //  \
                            {                       //  |
                                Y = Y + 2;          //  | 
                            }                       //  |
                            else if (Y == y - 1)    //  |
                            {                       //  |  
                                Y--;                //  |
                            }                       //  |
                            else if (Y == y + 1)    //  |
                            {                       //  |
                                Y++;                //  |- if the new X or Y coord is adjacent to the starting coord it's adjusted
                            }                       //  |
                            if (X == x)             //  |
                            {                       //  |
                                X = X + 2;          //  |
                            }                       //  |
                            else if (X == x - 1)    //  |
                            {                       //  |
                                X--;                //  |
                            }                       //  |
                            else if (X == x + 1)    //  |
                            {                       //  |
                                X++;                //  /
                            }
                            string New = createCellName(X, Y);
                            if (!minelayout[New])       //if the new cell isn't already a mine
                            {
                                minelayout[New] = true;     //new cell becomes a mine
                                minelayout[U] = false;      //old cell is no longer a mine
                            }
                        }
                        placed = true;  //the mine was placed, the loop may end
                    }

                }
                first = false;      //the first click has happened
                timer1.Interval = 1000; //timer is set
                timer1.Start();         //time is started
            }
            if (flagno&&!gameend&&!name.flagged())      //if the click isn't adding flags,the game hasn't ended and the cell isn't already falgged 
            {
                name.Closed = false;
                if (!minelayout[name.Name]) //if the cell insn't a mine
                {
                    floodfill(name);            //open the cell using the flood fill algorithm
                    int openedcells = 0;
                    for (int i = 0; i < columnCount; i++)       //for loop that looks at every cell
                    {
                        for (int j = 0; j < rowCount; j++)
                        {
                            string cell = createCellName(i, j);
                            Cell btn = grid.Controls.OfType<Cell>().Where(x => x.Name == cell).FirstOrDefault();
                            if (!btn.closed())
                            {
                                openedcells++;          //if the cell is open varoable is incremented
                            }
                        }
                    }
                    //MessageBox.Show(Convert.ToString(openedcells));
                    if (openedcells == safesquares)     //if the open cells are equale to the total safe cells the game may be won
                    {
                        checkwin();  //certify the game is won
                    }
                }
                else
                {
                    gameend = true;     //if the cell wasn't safe it was a mine, the game is lost
                    viewallmines();     //display all cells
                    name.BackgroundImage = Cell_mine_clicked;  //set the clicked cell to a unique identifing image
                }
            }
            else        //in this case the click should be creating/removing flags
            {
                if (name.Closed && !gameend && !name.Flagged /*&& flagno*/)  //if the click is adding flags
                {
                    //MessageBox.Show("Trying to add flag");
                    name.BackgroundImage = Cell_flag;  //flag the cell
                    name.Flagged = true;    //update variables
                    name.Closed = true;     // ''
                    MineRemaining_Box.Text = Convert.ToString(Convert.ToInt32(MineRemaining_Box.Text)-1);  //update display box
                    if (minelayout[name.Name]) //if the cell is also a mine
                    {
                        flaggedmines[name.Name] = true;  //add the cell to the falged mine dictionary
                    }
                }
                else if (name.Closed && !gameend && name.Flagged && !flagno)  //if the click is removing flags
                {
                    name.BackgroundImage = Cell;        //update variables
                    name.Flagged = false;               //  ""
                    name.Flagged_By_Bot = false;        //  ""
                    name.Closed = true;                 //  ""
                    MineRemaining_Box.Text = Convert.ToString(Convert.ToInt32(MineRemaining_Box.Text) + 1); //update display box
                    if (minelayout[name.Name])      //if the cell is also a mine
                    {
                        flaggedmines[name.Name] = false;   //remove the cell from the flagged mine dictionary
                        //flaggedmines_BOT[name.Name] = false;      <- even if the player removes the flag, the Solver still knows that it flagged the mine
                    }
                }
            }





        }
        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }//useless
        private void grid_Paint(object sender, PaintEventArgs e)
        {

        }//useless
        private void Form1_Load(object sender, EventArgs e)  //called when the form is loaded
        {
            Flag_Button.BackgroundImage = Properties.Resources.flag_no;   //sets the 'flag or not flag' button to the negative possition
            //string path = @"C:\Users\ivogl\Desktop\Solver_MineSweeper\obj\Debug\gridSize.txt";
            //int size = Convert.ToInt32(File.ReadAllText(path));
            //grid.Width = 100 / size * 48;
            //grid.Height = 100 / size * 48;

        }
        private void Flag_Button_Click(object sender, EventArgs e)      //when the flag button is clicked:
        {
            if (flagno)     //if flags arn't being placed by clicks
            {
                Flag_Button.BackgroundImage = Properties.Resources.flag_yes;        //change button image to possitive
                flagno = false;     //clicks now place flags
            }
            else if (!flagno)  //if flags are being placed by clicks
            {
                Flag_Button.BackgroundImage = Properties.Resources.flag_no;     //change button image to negative
                flagno = true;  //clicks no longer place flages
            }
        }
        private void displayTime()  //updates the timer box once a second 
        {
            int min = total / 60;       //finds the number of minutes from the total time
            int sec = total % 60;       //finds the number of seconds from the total time
            string displaymin = Convert.ToString(min);      //turns this data into a string
            string displaysec = Convert.ToString(sec);      // ""

            if (min < 10)       //if the time is less than 10 min add a 0 onto the front
            {
                displaymin = "0" + displaymin;
            }
            if (sec < 10)       //if the time is less than 10 sec add a 0 to the front of the seconds 
            {
                displaysec = "0" + displaysec;
            }
            Clock_Box.Text = displaymin + ":" + displaysec;     //update the box with the new values
        }
        private void timer1_Tick(object sender, EventArgs e)//timer that causes the timer to tick 
        {
            if (!gameend) //if the game hasn't ended
            {
                total++;        //timer ticks
                displayTime();  //update time display
            }
            else
            {
                timer1.Stop();  //else stop the clock
            }
        }
        private void Difficulty_Button_Click(object sender, EventArgs e) //opens a new dialog for the difficulty form class
        {
            DifficultyBox.ShowDialog();
            Difficulty_Button.Enabled = false;      //disables the button and starts a timer
            timer2.Interval = 1000;
            timer2.Start();
            //Enabled = false;
        }

        public void Restart_Button_Click(object sender, EventArgs e) //ends the game and starts a new one
        {
            MessageBox.Show("Loading......\nthis may take a few seconds");      //lets the player know this may take a few seconds
            string path_oldSize = @"C:\Users\ivogl\Desktop\Solver_MineSweeper\obj\Debug\OLD_gridSize.txt";     //
            int Old_size = Convert.ToInt32(File.ReadAllText(path_oldSize));
            first = true;       //  \
            gameend = false;    //  |
            flagno = true;      //  |-resets all the global varaibles
            safesquares = 1;    //  |
            total = 0;          //  /

            timer1.Stop();  //  \
            displayTime();  //  / stops and dispalys time
            grid.Dispose(); // removes the old grod

            gernerateGrid();        //calls the generate grid function to create a new grid
        }

        private void LeaderBoard_Button_Click(object sender, EventArgs e) //opens a new dialog for the leader Board form class
        {
            LeaderBoard.ShowDialog();
            LeaderBoard_Button.Enabled = false;     //disables the button and starts a timer
            timer2.Interval = 1000;
            timer2.Start();
        }

        private void updateDisplay()    //updates the copunter for the amount of remaining mines
        {
            int count = 0;
            foreach (KeyValuePair<string, bool> flag in flaggedmines)       //for all cells
            {
                Cell check = grid.Controls.OfType<Cell>().Where(x => x.Name == flag.Key).FirstOrDefault();
                if (check.flagged() || check.flagged_by_bot())      //if the cell is flagged, either by the player of the 'AI'
                {
                    count++; //incremetn the count 
                }
            }
            MineRemaining_Box.Text = (Convert.ToInt32(grid.RowCount * grid.ColumnCount * Convert.ToDouble(getMineDensity())) - count).ToString(); //set the remaining box to the total - count
        }

        private void Solve()  //AI solver for the minesweeper game
        {
            Dictionary<string, bool> temp = new Dictionary<string, bool>();  //creates a tempory dictionay 
            if (!flagno)
            {
                Flag_Button.PerformClick();     //if the place flag button is clicked turn it off
            }
            if (first)      //if the grid hasn't been opened yet
            {
                //MessageBox.Show(":)");
                int Size = getSize();
                int middle = Size / 2;
                string cell = createCellName(middle, middle);  //find the centre cell of the grid
                Cell btn = grid.Controls.OfType<Cell>().Where(x => x.Name == cell).FirstOrDefault();
                if (btn.flagged())
                {
                    btn.Flagged = false;        //if the cell hasn't been opened yet all squares are safe so the flag can be removed
                }
                btn.PerformClick(); //open the centre cell
                first = false;  //the first move has been made
                foreach (KeyValuePair<string, bool> item in flaggedmines)       //add to the tempory dictioany all data from the flagged mine dicitionay
                {
                    temp.Add(item.Key, item.Value);     
                }
            }
            foreach (KeyValuePair<string,bool> flagg in flaggedmines)  //for every cell
            {
                Cell safe = grid.Controls.OfType<Cell>().Where(x => x.Name == flagg.Key).FirstOrDefault();
                if (findadjacentcell_closed(safe).Count == 0 && safe.flagged() && !safe.flagged_by_bot()) //if the cell is closed, has no adjacent cells and wasn't flagged by this bot 
                {                                                                                         //it must be safe
                    safe.Flagged = false;   
                    temp[safe.Name] = false;     //remove any flags from the cell
                    safe.PerformClick();        //open the cell
                }
            }
            foreach (KeyValuePair<string, bool> item in flaggedmines)  //deflags all cell flagged by a human that the bot didn't also flag
            {
                if (item.Value && !flaggedmines_BOT[item.Key])
                {
                    Cell deflag = grid.Controls.OfType<Cell>().Where(x => x.Name == item.Key).FirstOrDefault();
                    deflag.Flagged = false;
                    temp[deflag.Name] = false;
                }
            }
            foreach (KeyValuePair<string, bool> item in temp) //updates the flaggedmines dictionary from the temproy one now all chages have been made
            {
                flaggedmines[item.Key] = item.Value;
            }
            bool change = true;   
            while (change)          //this while loop continues as long as the algorithm made a change in the last itteration, in this way it will continue until it can't do anything else or its has won the game
            {
                change = false;
                foreach (KeyValuePair<string, bool> item in adjacentcells)      //looks at all cells adjacent to mines
                {                                                       
                    Cell btn = grid.Controls.OfType<Cell>().Where(X => X.Name == item.Key).FirstOrDefault();

                    if (item.Value && (findadjacentcell_closed(btn).Count == numadjacentmines(btn, false)))     // if the number of adjacent mines = the number of adjacent closed cells all of them must be mines
                    {
                        //MessageBox.Show(Convert.ToString(Convert.ToString(findadjacentcell_closed(btn).Count) + " , " +Convert.ToString(numadjacentmines(btn, false))));
                        List<string> adjacent = findadjacentcell_closed(btn);
                        for (int i = 0; i < adjacent.Count; i++)   //takes each cell adjacent to the first cell
                        {
                            Cell mine = grid.Controls.OfType<Cell>().Where(x => x.Name == adjacent[i]).FirstOrDefault();
                            if (!mine.flagged_by_bot())     //if the hassn't already been flagged by the bot
                            {
                                mine.BackgroundImage = Cell_flag;           //the bot flags the cell
                                mine.Flagged = true;                        //  ""
                                mine.Flagged_By_Bot = true;                 //  ""
                                mine.Closed = true;                         //  ""
                                flaggedmines_BOT[mine.Name] = true;         //  ""
                                MineRemaining_Box.Text = Convert.ToString(Convert.ToInt32(MineRemaining_Box.Text) - 1); //updates flagged mines box
                                //continue;
                                change = true; //a change has been made so the outer loop should continue for at least 1 more itteration
                                this.Refresh(); //updates the playes visable view with the new flags
                                break;
                            }
                        }
                    }
                }
                Dictionary<string, bool> temp1 = new Dictionary<string, bool>();
                foreach (KeyValuePair<string, bool> item in adjacentcells)  //temporary dictioany for adjacent cells
                {
                    temp1.Add(item.Key, item.Value);
                }
                foreach (KeyValuePair<string, bool> item in temp1) //looks at all cell adjacent to mines
                {
                    if (item.Value)      
                    {
                        Cell btn = grid.Controls.OfType<Cell>().Where(X => X.Name == item.Key).FirstOrDefault();                //takes the cells name's as a string and converts them into variables of type cell
                        int value = numadjacentmines(btn, false);   //finds the amount of adjacent mines
                        int adjacentFlags = 0;
                        List<string> adjacentClosed = findadjacentcell_closed(btn);     
                        foreach (string cell in adjacentClosed)           //finds the amount of adjacent cells that are flagged   
                        {
                            if (flaggedmines_BOT[cell] == true)
                            {
                                adjacentFlags++;           
                            }
                        }
                        if (adjacentFlags == value)     //if adjacent mines = flaggedmines then all other adjacent squares must be safe
                        {
                            foreach (string cell in findadjacentcell_closed(btn))       //iterates through all adjacent cells and trys to open them
                            {
                                Cell safe = grid.Controls.OfType<Cell>().Where(x => x.Name == cell).FirstOrDefault();
                                if (!safe.flagged_by_bot())  //as long as the cell isn't flagged, then there is no point in trying to open it
                                {
                                    //safe.BackgroundImage = Cell_mine_clicked;
                                    safe.PerformClick();
                                    //continue;
                                    change = true; //a change has been made so the outer loop should continue for at least 1 more itteration
                                    Refresh();//updates the playes visable view with the now open cell
                                    break;
                                }
                            }
                        }
                    }
                }
            }
        }

        private void Solve_Button_Click(object sender, EventArgs e) //called when the player clicks on the solve grid button
        {
            if (!gameend)//as long as the game hasn't ended yet
            {
                Solve();        //calls the solve algorithm
                updateDisplay(); //updates how many mines have been flagged in the diplay box
            }
        }

        private void timer2_Tick(object sender, EventArgs e)
        {
            total2++;
            if (total2 == 5)
            {
                LeaderBoard_Button.Enabled = true;
                Difficulty_Button.Enabled = true;
                total2 = 0;
                timer2.Stop();
            }
        }

        private void instructions_Button_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Hello and welcome to this implimentation of minesweeper.\n\n" +
                "The game functions like any other version but it also has several extra additions:\n\n" +
                ">To add or remove a flag to/from a mine the flag button in the top left must be togled\n" +
                ">If you click the difficulty button you may chose the grid size and the mine density\n" +
                "\tThe grid size must be a decimal between 0.0 and 1.0, any invalid input will be treated as default\n" +
                ">If The Leader Board Button is clicked a leaderboard will be disaplyed. Any time a player wins a game they will be offered the option to add thier name to this board\n" +
                ">Finaly if the attempt solve button is pressed an inbuilt algorithm will atempt to solve the current grid from it's current possition\n" +
                "\tSometimes the algorithm will be succesful\n" +
                "\tOthertimes if it can't work any further the player will be able to continue where the algorithm left off\n\n" +
                "                                               HAVE FUN!");

        }
    }
}
