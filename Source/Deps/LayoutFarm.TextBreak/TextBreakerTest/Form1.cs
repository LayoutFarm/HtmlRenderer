using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;
using LayoutFarm.TextBreak;

namespace TextBreakerTest
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
       
        private void cmdReadDict_Click(object sender, EventArgs e)
        {

            // LayoutFarm.TextBreaker.ICU.DictionaryData.LoadData("../../../icu58/brkitr/thaidict.dict");
        }

     
        private void cmdCustomBuild_Click(object sender, EventArgs e)
        {

            CustomBreakerBuilder.DataDir = "../../../icu58/brkitr_src/dictionaries";
            CustomBreaker breaker1 = CustomBreakerBuilder.NewCustomBreaker();
         
            //2. create dictionary based breaking engine 
            // 
            char[] test = this.textBox1.Text.ToCharArray();
            System.Diagnostics.Stopwatch stopWatch = new System.Diagnostics.Stopwatch();
            stopWatch.Start();
            for (int i = 0; i < 1; ++i)
            {
                breaker1.BreakWords(test, 0);
                this.listBox1.Items.Clear();
                foreach (var span in breaker1.GetBreakSpanIter())
                {
                    string s = new string(test, span.startAt, span.len);
                    this.listBox1.Items.Add(span.startAt + " " + s);
                }
            }
            stopWatch.Stop();
            Console.WriteLine(stopWatch.ElapsedMilliseconds);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            //string test1 = "ผู้ใหญ่หาผ้าใหม่ให้สะใภ้ใช้คล้องคอใฝ่ใจเอาใส่ห่อมิหลงใหลใครขอดูจะใคร่ลงเรือใบดูน้ำใสและปลาปูสิ่งใดอยู่ในตู้มิใช่อยู่ใต้ตั่งเตียงบ้าใบถือใยบัวหูตามัวมาให้เคียงเล่าเท่าอย่าละเลี่ยงยี่สิบม้วนจำจงดี";
            //string test1 = "แป้นพิมลาว";
            string test1 = "ແປ້ນພິມລາວ";
            this.textBox1.Text = test1;
        }

        private void cmdIcuTest_Click(object sender, EventArgs e)
        {

        }
        
    }
}
