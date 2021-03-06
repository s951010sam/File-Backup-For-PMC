﻿using System;
using System.Globalization;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace WindowsFormsApplication1 {
    public partial class BackUpForm : Form {
        private DateTime nextTime;
        private long targetTime;
        private int textboxcount = 0;
        private int upcount = 0;
        string fileName = "test";
        string sourcePath = @"C:\Data_BackUp";
        string targetPath = @"C:\Data_BackUp\target";
        string tablePath = @"C:\Data_BackUp\table";
        

        public BackUpForm() {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e) {
            main_panel_BackUpSetting.Show();
            main_panel_RecoverData.Hide();

            /***************************************************************/
            //test copy           
            String temp = fileName;
            String time = DateTime.Now.ToString("_yyyyMMdd_HHmmss") + ".txt";
            Console.WriteLine(time);
            
            string sourefile = Path.Combine(sourcePath, fileName);
            string tablefile = Path.Combine(tablePath, fileName);
            fileName += time;
            string targetfile = Path.Combine(targetPath, fileName);
            
            if (!Directory.Exists(tablePath))
            {
                Directory.CreateDirectory(tablePath);
            }

            StreamWriter tableWriter = new StreamWriter(tablefile, true);

            if (!Directory.Exists(sourcePath))
            {
                Console.WriteLine("sourcePath dosen't exist!");
            }

            if (!Directory.Exists(targetPath))
            {
                try
                {
                    Directory.CreateDirectory(targetPath);
                }
                catch { }
            }

            try
            {
                //targetfile += DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");
                File.Copy(sourefile, targetfile, true);
                tableWriter.Write(DateTime.Now.ToString("yyyy/MM/dd (ddd) HH:mm:ss "));
                tableWriter.WriteLine(fileName);
            }
            catch { }
            fileName = temp;
            tableWriter.Close();
            listView1.Clear();
            /***************************************************************/

        }

        private void button2_Click(object sender, EventArgs e) {
            main_panel_BackUpSetting.Hide();
            main_panel_RecoverData.Show();
            String line;
            String[] get = new String[4];
            char[] separator = { ' ' };
                
            string tablefile = Path.Combine(tablePath, fileName);
            StreamReader tableReader = new StreamReader(tablefile);

            InitialColumns();
            listView1.MultiSelect = false;

            while ((line = tableReader.ReadLine()) != null)
            {
                get = line.Split(separator);
                newView(get);
            }
            tableReader.Close();

        }

        private void main_radioButton_daily_CheckedChanged(object sender, EventArgs e) {
            main_comboBox_monthly.Enabled = false;
            main_comboBox_weekly.Enabled = false;
            main_dateTimePicker_daily.Enabled = true;
        }

        private void main_radioButton_weekly_CheckedChanged(object sender, EventArgs e) {
            main_comboBox_weekly.Enabled = true;
            main_comboBox_monthly.Enabled = false;
            main_dateTimePicker_daily.Enabled = true;
        }

        private void main_radioButton_monthly_CheckedChanged(object sender, EventArgs e) {
            main_comboBox_weekly.Enabled = false;
            main_comboBox_monthly.Enabled = true;
            main_dateTimePicker_daily.Enabled = true;
        }

        private void main_radioButton_unused_CheckedChanged(object sender, EventArgs e) {
            main_comboBox_weekly.Enabled = false;
            main_comboBox_monthly.Enabled = false;
            main_dateTimePicker_daily.Enabled = false;
        }

        private void notifyIcon1_MouseDoubleClick(object sender, MouseEventArgs e) {
            this.Show();
            this.WindowState = FormWindowState.Normal;
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e) {
            e.Cancel = true;
            this.Hide();
            Icon.ShowBalloonTip(3000);
        }

        private void 結束ToolStripMenuItem_Click(object sender, EventArgs e) {
            Environment.Exit(Environment.ExitCode);
        }

        private void main_button_Exit_Click(object sender, EventArgs e) {
            Environment.Exit(Environment.ExitCode);
        }

        private void main_button_Save_Click(object sender, EventArgs e) {
            if (main_radioButton_daily.Checked) {
                targetTime = refreshDaily();
            } else if (main_radioButton_weekly.Checked) {
                targetTime = refreshWeekly();
            } else if (main_radioButton_monthly.Checked) {
                targetTime = refreshMonthly();
            }



            //this.Hide();
            //Icon.ShowBalloonTip(3000);
        }

        private long refreshMonthly() {
            string hour = main_dateTimePicker_daily.Text.Substring(1, 2);
            string minute = main_dateTimePicker_daily.Text.Substring(4, 4);
            int hourInt = Convert.ToInt32(hour);
            int minuteInt = Convert.ToInt32(minute);

            DateTime nextTempTime = DateTime.Now.AddMonths(1);
            string nextTimeFormat = nextTempTime.Year.ToString() + " " + nextTempTime.Month.ToString() + " " + main_comboBox_monthly.Text;
            nextTime = DateTime.ParseExact(nextTimeFormat, "yyyy M d", CultureInfo.InvariantCulture);
            TimeSpan nextTimeSpan = nextTime - DateTime.Now;
            return (long)nextTimeSpan.TotalSeconds + deSecond(hourInt, minuteInt, 0, 0);
        }

        private long refreshWeekly() {
            DayOfWeek weekDay = DateTime.Now.DayOfWeek;
            DateTime targetWeekDay;

            if (main_comboBox_weekly.Text.Equals("一")) {
                weekDay = DayOfWeek.Monday;
            } else if (main_comboBox_weekly.Text.Equals("二")) {
                weekDay = DayOfWeek.Tuesday;
            } else if (main_comboBox_weekly.Text.Equals("三")) {
                weekDay = DayOfWeek.Wednesday;
            } else if (main_comboBox_weekly.Text.Equals("四")) {
                weekDay = DayOfWeek.Tuesday;
            } else if (main_comboBox_weekly.Text.Equals("五")) {
                weekDay = DayOfWeek.Friday;
            } else if (main_comboBox_weekly.Text.Equals("六")) {
                weekDay = DayOfWeek.Saturday;
            } else if (main_comboBox_weekly.Text.Equals("日")) {
                weekDay = DayOfWeek.Sunday;
            }

            if (weekDay > DateTime.Now.DayOfWeek) {
                targetWeekDay = getWeekUpOfDate(DateTime.Now, weekDay, 0);
            } else {
                targetWeekDay = getWeekUpOfDate(DateTime.Now, weekDay, 1);
            }

            string hour = main_dateTimePicker_daily.Text.Substring(1, 2);
            string minute = main_dateTimePicker_daily.Text.Substring(4, 4);
            int hourInt = Convert.ToInt32(hour);
            int minuteInt = Convert.ToInt32(minute);

            TimeSpan nextTimeSpan = targetWeekDay - DateTime.Now;
            MessageBox.Show(Convert.ToString((long)nextTimeSpan.TotalSeconds + deSecond(hourInt, minuteInt, Convert.ToInt32(DateTime.Now.Hour), Convert.ToInt32(DateTime.Now.Minute))));
            return (long)nextTimeSpan.TotalSeconds + deSecond(hourInt, minuteInt, 0, 0);
        }

        private long refreshDaily() {
            string hour = main_dateTimePicker_daily.Text.Substring(1, 2);
            string minute = main_dateTimePicker_daily.Text.Substring(4, 4);

            int hourInt = Convert.ToInt32(hour);
            int minuteInt = Convert.ToInt32(minute);
            return deSecond(hourInt, minuteInt, Convert.ToInt32(DateTime.Now.Hour), Convert.ToInt32(DateTime.Now.Minute));
        }

        private long deSecond(int hourInt, int minuteInt, int nowHour, int nowMinute) {
            long targetSecond = Math.Abs(((hourInt * 60 + minuteInt) - (nowHour * 60 + nowMinute))) * 60;
            return targetSecond;
        }

        private DateTime getWeekUpOfDate(DateTime dt, DayOfWeek weekday, int Number) {
            int wd1 = (int)weekday;
            int wd2 = (int)dt.DayOfWeek;
            return wd2 == wd1 ? dt.AddDays(7 * Number) : dt.AddDays(7 * Number - wd2 + wd1);
        }

        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {


        }

        private void button1_Click_return(object sender, EventArgs e)
        {

            /***************************************************************/
            //get data test
            foreach (ListViewItem tmpLstView in listView1.Items)
            {
                if (tmpLstView.Selected == true)
                {
                    textBox1.Text = tmpLstView.SubItems[0].Text + " "
                                  + tmpLstView.SubItems[1].Text + " "
                                  + tmpLstView.SubItems[2].Text + " "
                                  + tmpLstView.SubItems[3].Text;
                }
            }
            /***************************************************************/
            
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            textboxcount =  textBox1.Text.Length;
            if (textboxcount > upcount)//more
            {
                foreach (ListViewItem tmpLstView in listView1.Items)
                {
                    String comper = tmpLstView.SubItems[0].Text + " "
                        //+ tmpLstView.SubItems[1].Text + " "
                                      + tmpLstView.SubItems[2].Text + " "
                                      + tmpLstView.SubItems[3].Text;
                    if (comper.StartsWith(textBox1.Text) == false)
                    {
                        listView1.Items.Remove(tmpLstView);
                    }
                }
                
            }
            else
            {
                listView1.Clear();
                InitialColumns();

                String line;
                String[] get = new String[4];
                char[] separator = {' '};
                string tablefile = Path.Combine(tablePath, fileName);
                StreamReader tableReader = new StreamReader(tablefile);

                while ((line = tableReader.ReadLine()) != null)
                {
                    get = line.Split(separator);

                    line = get[0] + " " + get[2] + " " + get[3];
                    
                    if (line.StartsWith(textBox1.Text) == true)
                    {
                        Console.WriteLine(line);
                        newView(get);
                    }
                }
                tableReader.Close();
            }

            upcount = textboxcount;
            /*
            Console.WriteLine( listView1.Items.Find(textBox1.Text,true));

            ListViewItem foundItem = listView1.FindItemWithText(textBox1.Text, false, 0, true);
            if (foundItem != null)
            {
                listView1.TopItem = foundItem;

            }

            
            /* case 3  can del and recover//要再找辦法 太消耗
            foreach (ListViewItem tmpLstView in listView1.Items)
            {
                String comper = tmpLstView.SubItems[0].Text + " "s
                                //+ tmpLstView.SubItems[1].Text + " "
                                + tmpLstView.SubItems[2].Text + " "
                                + tmpLstView.SubItems[3].Text;
                String line;
                String[] get = new String[3];
                char[] separator = { ' ' };
                string tablefile = Path.Combine(tablePath, fileName);
                StreamReader tableReader = new StreamReader(tablefile);

                while ((line = tableReader.ReadLine()) != null)
                {
                    get = line.Split(separator);

                    line = get[0] + " " + get[2] + " " + get[3];
                    if (line.StartsWith(textBox1.Text) && tmpLstView.SubItems[3].Text.Equals(get[3]) == true)
                    {
                        newView(get);
                    }
                }
                tableReader.Close();

                if (comper.StartsWith(textBox1.Text) == false)
                {
                    listView1.Items.Remove(tmpLstView);
                }
                //listView1.Items.Add(tmpLstView); 
            }
            */
        }


        private void newView(String[] get)
        {
            ListViewItem item = new ListViewItem();
            item.Text = (get[0]);
            item.SubItems.Add(get[1]);
            item.SubItems.Add(get[2]);
            item.SubItems.Add(get[3]);
            listView1.Items.Add(item);
        }

        private void InitialColumns()
        {
            listView1.Columns.Add("Date", 70, HorizontalAlignment.Center);
            listView1.Columns.Add("Week", 50, HorizontalAlignment.Center);
            listView1.Columns.Add("Time", 80, HorizontalAlignment.Center);
            listView1.Columns.Add("File name", 282, HorizontalAlignment.Left);
        }
    }
}
