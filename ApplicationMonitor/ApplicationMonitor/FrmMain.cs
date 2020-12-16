using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using ApplicationMonitor.Library;

namespace ApplicationMonitor
{
    public partial class FrmMain : Form
    {
        public FrmMain()
        {
            InitializeComponent();
        }

        private void btnConnect_Click(object sender, EventArgs e)
        {
            Thread thread = new Thread(AsyncSocketServer.StartListening);
            thread.Start();
        }

        private void tmrUpdate_Tick(object sender, EventArgs e)
        {
            //Application.DoEvents();



            if (AsyncSocketServer.eventList.Count > 0)
            {
                dgvMain.DataSource = AsyncSocketServer.eventList;
                
                
            }
                

            
            if (ApplicationMonitor.Library.AsyncSocketServer.eventList.Count > 0)
            {
                textBox1.Clear();

                foreach (var l in AsyncSocketServer.eventList)
                {
                    textBox1.Text += l.EventDate.ToString() + "\t" + 
                                     l.PanelId.ToString() + "\t" + 
                                     l.AccountId.ToString() + "\t" + 
                                     l.EventId.ToString() + "\t" + 
                                     l.EventDescription + "\t" + 
                                     l.PartitionId.ToString() + "\t" + 
                                     l.ZoneId.ToString() + "\t" + 
                                     l.UserId.ToString() + "\t" +
                                     Environment.NewLine;
                }
            }
                //textBox1.Text += ApplicationMonitor.Library.AsyncSocketServer.eventList.OrderByDescending(x => x.EventDate).FirstOrDefault().EventDate.ToString() + "\n"
        }
    }
}
