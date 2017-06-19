using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SeminarskiRad
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

            
            this.AcceptButton = button1;        // postavljanje primarnog dugmeta kada se pritisne Enter tipka


            this.StartPosition = FormStartPosition.CenterScreen;
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            Application.Exit();                                                  // zatvaranje aplikacije pri zatvoranju forme                                       
        }

        private void izlazToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void button1_Click(object sender, EventArgs e)                  // provjera imena i sifre te otvaranje nove forme
        {
            if ((textBox1.Text == "Josip Kristo" && textBox2.Text == "lozinka111")||(textBox1.Text == "Marko Bosnjak" && textBox2.Text == "lozinka222"))
            {
                Form3 form3 = new Form3();
                form3.Show();
                this.Hide();

            }
            else

            {
                MessageBox.Show("Pogresan unos podataka!" + "\n" + "Molimo pokusajte ponovo.");

                
                
                    textBox2.Text = "";
                    textBox1.Focus();
                


            }
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)               // omogućavanje pogleda sifre pri klikom na checkbox
        {
            if(checkBox1.Checked == true)
            {
                textBox2.PasswordChar = '\0';

            }
            else 
                if (checkBox1.Checked == false)
            {
                textBox2.PasswordChar = '*';
            }
        }
    }
}
