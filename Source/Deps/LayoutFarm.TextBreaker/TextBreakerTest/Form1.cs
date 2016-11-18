using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;
using LayoutFarm.TextBreaker; 

namespace TextBreakerTest
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        private void button1_Click(object sender, EventArgs e)
        {
            //break
            EnTextBreaker enTextBreaker = new EnTextBreaker();
            string text = this.textBox1.Text;
            listBox1.Items.Clear();
            char[] textBuffer = text.ToCharArray();
            enTextBreaker.DoBreak(textBuffer, bounds =>
            {
                listBox1.Items.Add(
                    new string(textBuffer, bounds.startIndex, bounds.length));


            });
        }
        private void cmdReadDict_Click(object sender, EventArgs e)
        {

            // LayoutFarm.TextBreaker.ICU.DictionaryData.LoadData("../../../icu58/brkitr/thaidict.dict");
        }

        ThaiDictionaryBreakingEngine thaiDicBreakingEngine;
        LaoDictionaryBreakingEngine laoDicBreakingEngine;
        private void cmdCustomBuild_Click(object sender, EventArgs e)
        {
            CustomBreaker breaker1 = new CustomBreaker();
            //test read dict data line
            //1. load dic
            if (thaiDicBreakingEngine == null)
            {
                var customDic = new CustomDic();
                thaiDicBreakingEngine = new ThaiDictionaryBreakingEngine();
                thaiDicBreakingEngine.SetDictionaryData(customDic);
                customDic.SetCharRange(thaiDicBreakingEngine.FirstUnicodeChar, thaiDicBreakingEngine.LastUnicodeChar);
                customDic.LoadFromTextfile("../../../icu58/brkitr_src/dictionaries/thaidict.txt");
                breaker1.AddBreakingEngine(thaiDicBreakingEngine);
            }
            if (laoDicBreakingEngine == null)
            {
                var customDic = new CustomDic();
                laoDicBreakingEngine = new LaoDictionaryBreakingEngine();
                laoDicBreakingEngine.SetDictionaryData(customDic);
                customDic.SetCharRange(laoDicBreakingEngine.FirstUnicodeChar, laoDicBreakingEngine.LastUnicodeChar);
                customDic.LoadFromTextfile("../../../icu58/brkitr_src/dictionaries/laodict.txt");
                breaker1.AddBreakingEngine(laoDicBreakingEngine);
            }
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
