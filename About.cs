using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace StarCraftMapBrowser
{
    public partial class About : Form
    {
        public About()
        {
            InitializeComponent();
            StormLink.Links.Add(5, 8, "https://github.com/ladislav-zezula/StormLib");
            GitLink.Links.Add(0, 6, "https://github.com/gform/StarCraftMapBrowser");
            Mail.Links.Add(12, 6, "mailto:maksym.alokhin@protonmail.com");
            AboutBox.Text = "StarCraft Map Browser v1.0.1";
        }

        private void OKbtn_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void StormLink_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start(e.Link.LinkData as string);
        }

        private void GitLink_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start(e.Link.LinkData as string);
        }

        private void Mail_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start(e.Link.LinkData as string);
        }
    }
}