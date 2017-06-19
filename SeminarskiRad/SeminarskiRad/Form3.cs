using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Runtime.InteropServices;
using Newtonsoft.Json;


namespace SeminarskiRad
{
    public partial class Form3 : Form
    {
       
     
        StreamReader citanje;                   
        StreamWriter pisanje;
        double zamisljena_temp;
        double temperatura;                     // pretvaranje vrijednosti 0-255 u temp raspona 5-65°

        bool pokrenut_ventilator2 = false;      // ventilator2 (step motor) u početnom stanju ne radi
        bool pokrenut_ventilator = true;       // ventilator (dc motor) u početnom stanju radi

        int brzina_ventilatora1 = 60;
        int brzina_ventilatora12 = 150;
        int brzina_ventilatora123 = 250;
        int zvuk;
        int chartXvalue = 0;

        
        List<TempData> TrenutniPodaci = new List<TempData>();       // Lista koja cuva vrijednosti svakog TempData objekata.
        
        string startTime = "";      // vrijeme pokretanja programa koje se koristi za naziv datoteke

        public Form3()
        {
            InitializeComponent();
            MotorDC.InitMotoBee();              // inicijalizacija DC motora
            MotorStepper.InitStp();             // inicijalizacija step motora
            WASP.InitWasp();                    // inicijalizacija senzora svjetlosti

            this.StartPosition = FormStartPosition.CenterScreen;



        }


        private void Form3_Load(object sender, EventArgs e)
        {
            MotorDC.SetMotors(1, 30, 0, 0, 0, 0, 0, 0, 0);              // pokretanje motora(ventilatora) pri otvaranju forme te ukljucivanje "ledice" (bottun)

            if (pokrenut_ventilator )                 
            {
               
                pokrenut_ventilator = true;

                button4.BackColor = Color.Green;
                timer1boje.Enabled = true;
            }
            

            checkBox1.Checked = false;                              // onemogućavanje svih ručnih opcija jer je uključen automatski mod
            checkBox1.Enabled = false;

            button1.Enabled = false;
            button2.Enabled = false;

            trackBar1.Enabled = false;

            
            startTime = DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss");       // Vrijeme kada je program pokrenut

           
            DirectoryInfo dinfo = new DirectoryInfo(Application.StartupPath);        // popuni listStartTime sa dostupnim datotekama od svakog pokretanja
            FileInfo[] Files = dinfo.GetFiles("*.json");
            

           
            List<ListBoxItem> lbItems = new List<ListBoxItem>();                // Lista koja ce cuvati sve objekte ListBoxItems i na kraju ih dodati u ListBox listStartTime
            foreach (FileInfo file in Files)
            {
                
                string[] FileNamePartsA = file.Name.Replace(".json", "").Split('_');       // Skloni extenziju iz naziva file i razdvoji ga na datum i vrijeme
                
                string[] FileDatePart = FileNamePartsA[0].Split('-');                   // razdvoji datum
              
                string[] FileTimePart = FileNamePartsA[1].Split('-');                     // razdvoji vrijeme

                // dodaj naš objekat ListBoxItem u listu gdje je vrijednost naziv filea a text formatirani datum i vrijeme
                ListBoxItem Item = new ListBoxItem();
                Item.Value = file.Name;
                Item.Text = FileDatePart[2] + "." + FileDatePart[1] + "." + FileDatePart[0] + ". " + FileTimePart[0] + ":" + FileTimePart[1] + ":" + FileTimePart[2];
                lbItems.Add(Item);
            }

            listStartTime.DisplayMember = "Text";
            listStartTime.ValueMember = "Value";
            listStartTime.DataSource = lbItems;
           


          

            // ako zelimo da se file temp.txt isprazni prilikom svakog pokretanja programa otkomentirati ispod kod

            /*FileStream datoteka = new FileStream("temp.txt", FileMode.Truncate, FileAccess.Write);
            datoteka.Close();*/


        }


        private void Form3_FormClosed(object sender, FormClosedEventArgs e)
        {
            Application.Exit();                                                 // pri zatvoranju forme gasimo aplikaciju te motore

            MotorDC.SetMotors(0, 0, 0, 0, 0, 0, 0, 0, 0);
            MotorStepper.StopMotor1(0);

            pokrenut_ventilator = false;
            pokrenut_ventilator2 = false;

            
            var json = JsonConvert.SerializeObject(TrenutniPodaci);     // Serijalizacija TrenutniPodaci liste koja sadrži objekte TempData i spremanje u datoteku sa extenzijom .json

            FileStream dataFS = new FileStream(startTime+".json", FileMode.Create, FileAccess.Write);
            StreamWriter dataSW = new StreamWriter(dataFS);
            dataSW.Write(json);
            dataSW.Close();
        }



        private void timer1_Tick(object sender, EventArgs e)           // pokretanje timer1 sa otvaranjem forme 
        {
            zvuk = SenzorZvuka.DohvatiInput();

            label11.Text = Convert.ToString(zvuk);                             // ispisavanje vrijednosti 0 ili 1 sa senzora zvuka

            temperatura = (0.2353*zamisljena_temp)+5;                   // pretvaranje vrijednosti 0-255 u raspon temp. od 5-65
            zamisljena_temp = WASP.ProcitajInput();             
            label15.Text = temperatura.ToString();                      // ispisavanje vrijednosti temp. u labelu
            
            if (!checkBox2.Checked)                                     // ako je ukljucen automatski mod prati se koja je temperatura te u ovisnosti od odredene temp. ventilator se postavlja na odredenu brzinu

            
            {
                if (temperatura >= 5 && temperatura <= 20)
                {
                    
                    pokrenut_ventilator = true;
                    MotorDC.SetMotors(1, brzina_ventilatora1, 0, 0, 0, 0, 0, 0, 0);
                    trackBar1.Value = brzina_ventilatora1;
                    label12.Text = brzina_ventilatora1.ToString();
                    button4.BackColor = Color.Green;
                    timer1boje.Enabled = true;
                }

                if(temperatura >= 21 && temperatura <= 40)
                {
                    pokrenut_ventilator = true;
                    MotorDC.SetMotors(1, brzina_ventilatora12, 0, 0, 0, 0, 0, 0, 0);
                    trackBar1.Value = brzina_ventilatora12;
                    label12.Text = brzina_ventilatora12.ToString();
                    button4.BackColor = Color.Green;
                    timer1boje.Enabled = true;

                }
                if (temperatura >= 41 && temperatura <= 65)
                {
                    pokrenut_ventilator = true;
                    MotorDC.SetMotors(1, brzina_ventilatora123, 0, 0, 0, 0, 0, 0, 0);
                    trackBar1.Value = brzina_ventilatora123;
                    label12.Text = brzina_ventilatora123.ToString();
                    button4.BackColor = Color.Green;
                    timer1boje.Enabled = true;

                }


            }
        
        }

        private void timer2_Tick(object sender, EventArgs e)            // drugi timer koji sluzi za zapisivanje temp. u datoteku cijelo vrijeme svakih 5 sek.
        {
            string temp_zapis = Convert.ToString(temperatura);
          
            string datum = DateTime.Now.ToString();                     // spremanje trenutnog datuma i temp u string
            string temp_datum = datum + " " + temp_zapis + " °";        // spremanje datuma i temp. sa razmakom u string

                                                                        // ne koristimo globalni filestream jer onda ne bi znali jeli otvoren već prije jer ćemo u isto vrijeme i pisati i čitati te bi se javile greške

            FileStream datotekaWR = new FileStream("temp.txt", FileMode.Append, FileAccess.Write);      // Pri svakom ticku timera otvara se novi filestream ali u Append modu jer Open raadi truncate pa u fileu uvijek ima samo jedna linija zapisana.
            pisanje = new StreamWriter(datotekaWR);
            pisanje.WriteLine(temp_datum);
            pisanje.Close();                           // zatvaranje streamwritera da bi se sadrzaj nasao u fileu te se zatvara i filestream pa se pri iducem prolazu ponovo otvara  

          
           
            TrenutniPodaci.Add(new TempData() { datum = datum, temperatura = temp_zapis });       // Dodavanje objekta TempData u listu TrenutniPodaci svakih 5 sek
        }

        private void button3_Click(object sender, EventArgs e)          
        {
            richTextBox1.Clear();                                   // brišemo prethodni sadržaj box-a

            DateTime datumOd = dateTimePicker1.Value.Date;
            DateTime datumDo = dateTimePicker2.Value.Date;

            String linija;

            
            FileStream datotekaRD = new FileStream("temp.txt", FileMode.Open, FileAccess.Read);       // otvaranje novog filestream-a da bi citanje radilo 
            citanje = new StreamReader(datotekaRD);

            while (!citanje.EndOfStream)                // citanje svih linija datoteke te usporedivanje datuma sa odabranim i zapisivanje u richtextbox
            {
                linija = citanje.ReadLine();
                String[] niz = linija.Split();
                DateTime datum = DateTime.Parse(niz[0]);

                if ((datum >= datumOd) && (datum <= datumDo))
                {
                    richTextBox1.Text += linija + "\n";
                }
            }

            
            citanje.Close();  // zatvaramo streamreader a samim tim i filestream koji je korišten
            
        }


        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox2.Checked)                              //  paljenje ručnog moda označavanjem checkbox-a te omogućavanje kontrola
            {
                checkBox1.Enabled = true;
                button1.Enabled = true;
                button2.Enabled = true;
                trackBar1.Enabled = true;
                label17.Text = "Pokrenut je ručni način upravljanja!";
            }
            else
            {
                checkBox1.Enabled = false;
                checkBox1.Checked = false;
                button1.Enabled = false;
                button2.Enabled = false;
                trackBar1.Enabled = false;
                label17.Text = "Pokrenut je automatski način upravljanja!";
            }
        }



        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            int x = trackBar1.Value;                                // Mijenjanje brzine ventilatora sa trackbar-om te ispisivanje brzine
            if (pokrenut_ventilator)
            {
                   MotorDC.SetMotors(1, x, 0, 0, 0, 0, 0, 0, 0);
            }

            label12.Text = Convert.ToString(x);


        }

        private void button1_Click(object sender, EventArgs e)
        {
            MotorDC.SetMotors(1, 100, 0, 0, 0, 0, 0, 0, 0);         // paljenje ventilatora te postavljanje "ledice" u zeleno

            pokrenut_ventilator = true;

            button4.BackColor = Color.Green;
            timer1boje.Enabled = true;
        }

        private void button2_Click(object sender, EventArgs e)        // gašenje ventilatora i postavljanje "ledice" u crveno
        {
            MotorDC.SetMotors(0, 0, 0, 0, 0, 0, 0, 0, 0);

            pokrenut_ventilator = false;

            button4.BackColor = Color.Red;
            timer1boje.Enabled = false;
        }

      

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (pokrenut_ventilator)                                        // mijenjanje smjera vrtnje ventilatora te provjera ako je već bio upaljen
            {
                if (checkBox1.Checked == true)
                {
                    MotorDC.SetMotors(0, 0, 1, 50, 0, 0, 0, 0, 0);
                }

                else
                {

                    MotorDC.SetMotors(1, 50, 0, 0, 0, 0, 0, 0, 0);
                }
            }
        }

        private void timer1boje_Tick(object sender, EventArgs e)        // timer za blinkanje "ledice" zeleno kad je ventilator upaljen
                                                                        // ovaj timer ne radi stalno već ga pozivamo u gornjim funkcijama
        {
            if (button4.BackColor == Color.Green)
            {
                button4.BackColor = Color.Gray;

            }
            else if (button4.BackColor == Color.Gray)
            {
                button4.BackColor = Color.Green;

            }
        }

      

        private void label12_Click(object sender, EventArgs e)
        {

        }

        private void timer3Zvuk_Tick(object sender, EventArgs e)
        {
           

            if (SenzorZvuka.DohvatiInput() == 0)                    // hvatamo signal sa senzora zvuka te ako je buka prevelika odnosno ako senzor daje 0 pali se dodatni ventilator za hlađenje te "ledica"
            {
                
              
              
                MotorStepper.RunMotor1(16000, 10, 1, 0);
                pokrenut_ventilator2 = true;
                button5.BackColor = Color.Green;
                timer1boje.Enabled = true;
             
          
                

            }
            else
            {
                MotorStepper.StopMotor1(0);
                pokrenut_ventilator2 = false;
                button5.BackColor = Color.Red;
                timer1boje.Enabled = false;
            }
        }

        private void timerGraf_Tick(object sender, EventArgs e)         // timer za crtanje grafa temperature u realnom vremenu
        {
            Console.WriteLine("Temperatura:"+temperatura.ToString());
            
          
            chartXvalue++;
            chart1.Series[0].Points.AddXY(chartXvalue, temperatura);
            chart1.ChartAreas[0].Axes[0].Maximum = chartXvalue;


            chart1.Refresh();           // osvjezi prikaz grafa
        }

        private void btnShowData_Click(object sender, EventArgs e)
        {
            string file = listStartTime.SelectedValue.ToString();
           
           
            chart2.Series[0].Points.Clear();         // obrisi postojece vrijednosti grafa 2

                                                                                            // procitaj odabranu datoteku i deserijaliziraj podatke
            FileStream fileFS = new FileStream(file, FileMode.Open, FileAccess.Read);       // otvaranje novog filestream-a da bi citanje radilo 
            StreamReader fileSR = new StreamReader(fileFS);
            string json = fileSR.ReadToEnd();

            List<TempData> Lista = new List<TempData>();
           
            Lista = JsonConvert.DeserializeObject<List<TempData>>(json);         // Deserijalizacija stringa pročitanog iz datoteke
            

            int x = 1;
            foreach(TempData data in Lista)
            {
                chart2.Series[0].Points.AddXY(x, Convert.ToDouble(data.temperatura));
                x++;
            }
            chart2.Refresh();
        }

        private void label5_Click(object sender, EventArgs e)
        {

        }
    }

    // Objekat u koji se spremaju podaci za serijalizaciju i deserijalizaciju
    public class TempData
    {
        public string datum { get; set; }
        public string temperatura { get; set; }
    }

    // ListBox item objekat koji nam omogucava da spremimo vrijednost svake stavke koja je drugacija od prikazanog teksta
    public class ListBoxItem
    {
        public string Value { get; set; }
        public string Text { get; set; }
    }
}
