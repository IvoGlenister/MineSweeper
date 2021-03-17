using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.ComponentModel;
using System.Data;
using System.Drawing;

namespace MineSweeper_0._1
{
    class Cell : Button             //cell class inherits from the button class
    {
        public bool Closed = true;              //  \
        public bool Flagged = false;            //  |
        public bool Flagged_By_Bot = false;     //  |-new varible for the cell class being defined
        public int XCoord;                      //  |  
        public int YCoord;                      //  /
        //public int AdjacentMines;
        public bool closed()            // \
        {                               //  |
            return Closed;              //  |
        }                               //  |
        public bool flagged()           //  |
        {                               //  |
            return Flagged;             //  |
        }                               //  |
        public bool flagged_by_bot()    //  |
        {                               //  |-functions to get the value of the cell variable instead of addressing them directly
            return Flagged_By_Bot;      //  |
        }                               //  |
        public int xcoord()             //  |
        {                               //  |
            return XCoord;              //  |
        }                               //  |
        public int ycoord()             //  |
        {                               //  |
            return YCoord;              //  |
        }                               //  /

        //public int adjacentmines()
        //{
        //    return AdjacentMines;
        //}
    }
}
