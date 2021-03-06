﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace rec003WaveReading
{
    public partial class Form1 : Form
    {

        bool fileOpenOK = false;
        string openFileNameWav = null;
        byte[] data;
        public Form1()
        {
            InitializeComponent();
        }

        private void openFileName_Click(object sender, EventArgs e)
        {
            DialogResult dialogResult = openFileDialog1.ShowDialog();
            openFileDialog1.Filter = "wav files(*.wav)|*.wav|All files (*.*)|*.*";
            if (dialogResult == System.Windows.Forms.DialogResult.OK)
            {
                openFileNameWav = openFileDialog1.FileName;
                textBox1.Text = openFileNameWav;
                dms("Set WavFile :" + openFileNameWav);
                fileOpenOK = true;
            }
            else
            {
                fileOpenOK = false;
                dms("No file specified.");
            }
        }



        private void button1_Click(object sender, EventArgs e)
        {
            if (fileOpenOK == false || openFileNameWav == null || openFileNameWav == "") {
                dms("Plz Specify the file.");
                return;
            }

            byte[] datas = data_Get(openFileNameWav);
            if (datas == null)
            {
                dms("Don't open file.");

                return;
            }
            DataParser dataParser = new DataParser(datas);
            dms(dataParser.debug());
        }




        private byte[] data_Get(string File_path)
        {
            try
            {
                FileStream fileStream = new FileStream(File_path, FileMode.Open, FileAccess.Read);
                byte[] data = new byte[fileStream.Length];
                fileStream.Read(data, 0, data.Length);
                dms("Get data size =" + data.Length + " byte.");
                // クローズ処理要らないの？
                return data;
            }
            catch
            {
                dms("File Read error.");
                return null;
            }
        }


        private void dms(string msg)
        {
            textBox2.AppendText("\r\n" + msg);
        }

    }

    public class DataParser{
        bool data_set_ok = false;               // error flag
        string debug_msg;
        byte[] origin_data;
        byte[] data_RIFF = new byte[4];         // RIFF                         4byte "RIFF"
        int data_file_size;                     // ALL file size                4byte  -> num
        byte[] file_type = new byte[4];         // WAVE                         4byte "WAVE"

        byte[] fmt_T = new byte[4];             // fmt Chunk                    4byte "fmt "
        byte[] fmt_Tbyte = new byte[4];         // fmt Chunk byte 16:linearPCM  4byte 10 00 00 00   <- little endian
        byte[] fmt_id = new byte[2];            // fmt ID linearPMC:1           2byte 01 00         <- little endian
        byte[] fmt_channel = new byte[2];       // fmt Channel mono:1 stereo:2  2byte 0? 00         <- little endian
        byte[] fmt_sample_rete = new byte[4];   // fmt Sampling Rete            4byte 44.1kHz : 44100 : 44 AC 00 00  <- little endian
        byte[] fmt_data_rete = new byte[4];     // fmt Data Rete (Byte/sec)     4byte Stereo 16bit: 44100 * 2 * 2 = 176400
        byte[] fmt_block_size = new byte[4];    // fmt block size               4byte stereo 16bit: 2*2 = 4 04 00
        byte[] fmt_sample_bit_num = new byte[2];    // fmt one sample          2byte WAV Format 16bit 10 00
        byte[] fmt_no_use01 = new byte[2];      // non use (premiamu)           2byte 
        byte[] fmt_no_use02 = new byte[4];      // non use                      nouse
        byte[] data_T = new byte[4];            // data CHunk                   4byte "data"
        byte[] data_max_size = new byte[4];     // data byte num                4byte
        byte[] main_data_byte;                  // data strim                   ?byte max data saize

        public Func<string, byte[]> Data_Get { get; }

        public DataParser()
        {
            this.origin_data = null;
        }

        public DataParser(byte[] origin_data)
        {
            this.data_set_ok = setOriginData(origin_data);
            parse();
        }


        public bool setOriginData(byte[] origin_data)
        {
            try
            {
                this.origin_data = origin_data;
            }
            catch
            {
                return false;
            }
            return true;
        }

        public bool getSetingFlag()
        {
            return data_set_ok;
        }

        private void parse()
        {
            try
            {
                int data_num = 0;
                byte test = origin_data[0];
                debug_msg = origin_data.Length.ToString();
                Array.Copy(origin_data, data_num, data_RIFF, 0, 4);  // RIFF copy


                byte[] tmpbyte = new byte[4];
                Array.Copy(origin_data, (data_num += 4), tmpbyte, 0, 4);  // data file size copy
                for(int a = 0; a < 4; a++)
                {
                    data_file_size |= tmpbyte[a] << (a * 8);
                }
                //                debug_msg = data_file_size.ToString();


                Array.Copy(origin_data, (data_num += file_type.Length), file_type, 0, file_type.Length);  // get file tipe "WAVE"
                debug_msg = System.Text.Encoding.ASCII.GetString(file_type);

                debug_msg = null;
                string[] forintest = { "i", "ro", "ha?" };
                foreach (string tmpmozi in forintest)
                {
                    debug_msg += tmpmozi;
                }

/*                string[] forintest2 = { "ni", "ho", "he?" };
                foreach (string* tmpmozi2 in &forintest)
                {
                    debug_msg += *tmpmozi2;
                }

                byte[] foo = { fmt_T, fmt_Tbyte, fmt_id, fmt_channel };
                foreach (byte[]* hensui in &hennsuuireire)
                {
                    debug_msg = (string)hennsuuireire;
                }*/
            }
            catch
            {
                debug_msg = "error parse.";
            }

        }
        #region a
        public string debug()
        {
//            item mikan = new item("mikan", 1, "c:\\mikan.png", "c:\\mikans.png", new string[] { "小さなミカンを見つけた！", "体力が１０回復する" });
//            debug_msg = mikan.itme_name;
            return debug_msg;
        }

        #endregion

    }
}


public class item
{
    public string itme_name { get; } = "noitem";    // アイテム名
    public int item_num { get; set; }               // アイテムの数
    public string item_picture_file { get;}         // アイテムの画像
    public string items_picture_file { get;}        // アイテムが複数時の画像
    private string[] item_documents { get;}         // アイテムの説明

    public item()
    {

    }

    public item(string iname, int inum, string ipct, string ispct, string[] idoc)
    {
        this.itme_name = iname;
        this.item_num = inum;
        this.item_picture_file = ipct;
        this.items_picture_file = ispct;
        this.item_documents = idoc;
    }

}

/*
            item mikan = new item("mikan", 1, "c:\\mikan.png", "c:\\mikans.png", new string[] {"小さなミカンを見つけた！" , "体力が１０回復する"});
            item ringo = new item("ringo", 1, "c:\\ringo.png", "c:\\ringos.png", new string[] { "青いリンゴをみつけた！", "毒状態が治るかもしれない"});

 */
