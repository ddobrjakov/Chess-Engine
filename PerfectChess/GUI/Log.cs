using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PerfectChess
{
    public class Log : ListView
    {
        public Log() : base()
        {
            this.DoubleBuffered = true;
            this.MouseMove += Log_MouseMove;
            this.MouseLeave += Log_MouseLeave;
            this.BackColor = NoHoverColor;
            this.View = View.Details;//List;// Tile;
        }

        private void Log_MouseLeave(object sender, EventArgs e)
        {
            HoveredItem = null;
        }

        private ListViewItem _HoveredItem;
        public ListViewItem HoveredItem
        {
            get
            {
                return _HoveredItem;
            }
            private set
            {
                if (_HoveredItem == null && value == null) return;

                //Гасим, если меняем на null
                if (value == null)
                {
                    UnHover(_HoveredItem);
                    _HoveredItem = null;
                    return;
                } 
                //Включаем, если меняем с null-a
                if (_HoveredItem == null)
                {
                    _HoveredItem = value;
                    Hover(_HoveredItem);
                    return;
                }
                //Выключаем и включаем
                UnHover(_HoveredItem);
                _HoveredItem = value;
                Hover(_HoveredItem);
            }
        }
        private void Hover(ListViewItem Item)
        {
            Item.BackColor = HoverColor;
        }
        private void UnHover(ListViewItem Item)
        {
            Item.BackColor = NoHoverColor;
        }
        private void Log_MouseMove(object sender, MouseEventArgs e)
        {
            Point localPoint = e.Location;
            ListViewItem Item = this.GetItemAt(localPoint.X, localPoint.Y);
            HoveredItem = Item;
        }









        private System.Drawing.Color NoHoverColor = System.Drawing.Color.FromArgb(38, 38, 38);
        private System.Drawing.Color HoverColor = System.Drawing.Color.FromArgb(56, 147, 232);
    }
}
