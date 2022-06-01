using System.Collections;
using System.Drawing.Imaging;
using System.IO;
using System.Text;

namespace SSH_Tool
{

    public partial class Form1 : Form
    {
        string[] Header = { "SHPS", "SHPP", "SHPI", "SHPX", "SHPM" };
        int filelenght;
        string EAGLVersion;
        int numbersoffiles;
        string filesnames;
        int filessoffset;
        int valuetotal;
        int imagew;
        int imageh;
        int imagelenght;
        string blocksize;
        //imagedata calculations imagew x imageh for 8bpp - imagew x imageh /2 for 4bpp
        int paloffset;
        //16 colors = 4bpp; 256 colors = 8bpp;
        int bpp;
        int palblocksize;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
        private void ParsetoData()
        {
            string str = Convert.ToString(filesnames);
            List<int> intList = new List<int>();
            DataGridViewRow dataGridViewRow = new DataGridViewRow();
            foreach (int num in str)
            {
                dataGridViewRow.CreateCells(this.dataGridView1);
                dataGridViewRow.Cells[0].Value = filesnames;
                dataGridViewRow.Cells[1].Value = filessoffset;
                dataGridViewRow.Cells[2].Value = blocksize;
                dataGridViewRow.Cells[3].Value = imagew;
                dataGridViewRow.Cells[4].Value = imageh;
                dataGridViewRow.Cells[5].Value = paloffset;
                dataGridViewRow.Cells[6].Value = palblocksize;
                dataGridViewRow.Cells[7].Value = bpp;

            }
            this.dataGridView1.Rows.Add(dataGridViewRow);

        }
        private void read(string filename)
        {
            if (dataGridView1.Rows.Count != 0 && dataGridView1.Rows != null)
            {
                dataGridView1.Rows.Clear();
                EAGLVersion = null;
                numbersoffiles = 0;
                filelenght = 0;
            }
            int num1 = 0;
            BinaryReader binaryReader = new BinaryReader((Stream)File.OpenRead(filename));
            int num2 = 0;
            if (num2 <= 4)
            {
                binaryReader.BaseStream.Position = (long)num2;
                num1 = (int)BitConverter.ToInt16(binaryReader.ReadBytes(4), 0);
                int num3 = num2 + 1;
            }
            if (num1.ToString() == "18515")
            {
                for (int i = 4; i <= 4;)
                {
                    binaryReader.BaseStream.Position = i;
                    filelenght = BitConverter.ToInt32(binaryReader.ReadBytes(4), 0);
                    toolStripStatusLabel1.Text = "Size: " + filelenght.ToString();
                    i++;
                }
                for (int i = 8; i <= 8;)
                {
                    binaryReader.BaseStream.Position = i;
                    numbersoffiles = BitConverter.ToInt32(binaryReader.ReadBytes(4), 0);
                    toolStripStatusLabel1.Text = toolStripStatusLabel1.Text + " " + numbersoffiles.ToString() + " Files";
                    i++;
                }
                for (int i = 12; i <= 12;)
                {
                    binaryReader.BaseStream.Position = i;
                    EAGLVersion = System.Text.Encoding.ASCII.GetString(binaryReader.ReadBytes(4));
                    toolStripStatusLabel1.Text = toolStripStatusLabel1.Text + " EAGL Version: " + EAGLVersion;
                    i++;
                }
                for (int i = 16; i <= numbersoffiles * 8 + 8;)
                {
                    binaryReader.BaseStream.Position = i;
                    filesnames = System.Text.Encoding.ASCII.GetString(binaryReader.ReadBytes(4));

                    binaryReader.BaseStream.Seek(i + 4, SeekOrigin.Begin);
                    filessoffset = BitConverter.ToInt32(binaryReader.ReadBytes(4), 0);
                    binaryReader.BaseStream.Position = filessoffset + 1;
                    blocksize = ByteArrayToString(binaryReader.ReadBytes(3));
                    byte[] bytes = Encoding.UTF8.GetBytes(blocksize);
                    int intValue = int.Parse(blocksize, System.Globalization.NumberStyles.HexNumber);
                    byte[] bytesd = BitConverter.GetBytes(intValue);
                    if (BitConverter.IsLittleEndian)
                        Array.Reverse(bytes);
                    blocksize = Functions.ReadInt24(bytesd).ToString();
                    
                    binaryReader.BaseStream.Seek(filessoffset + 4, SeekOrigin.Begin);
                    imagew = BitConverter.ToInt16(binaryReader.ReadBytes(2), 0);
                    imageh = BitConverter.ToInt16(binaryReader.ReadBytes(2), 0);
                    paloffset = filessoffset + int.Parse(blocksize);
                    binaryReader.BaseStream.Position = paloffset + 1;
                    palblocksize = BitConverter.ToUInt16(binaryReader.ReadBytes(3), 0);
                    bpp = BitConverter.ToUInt16(binaryReader.ReadBytes(2), 0);
                    ParsetoData();
                    i += 8;
                }
            }
            else
            {
                MessageBox.Show("This is not SSH File");
                EAGLVersion = null;
                numbersoffiles = 0;
                filelenght = 0;

            }
            binaryReader.Close();


        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }
        public static string ByteArrayToString(byte[] ba)
        {
            StringBuilder hex = new StringBuilder(ba.Length * 2);
            foreach (byte b in ba)
                hex.AppendFormat("{0:x2}", b);
            return hex.ToString();
        }
        
        private void openToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                read(openFileDialog1.FileName);
            }
        }
        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            BinaryReader binaryReader = new BinaryReader((Stream)File.OpenRead(openFileDialog1.FileName));
            string filenames = dataGridView1.Rows[dataGridView1.CurrentRow.Index].Cells["Column1"].Value.ToString();
            if (dataGridView1.Columns[e.ColumnIndex].Name == "Column6")
            {


                string width = dataGridView1.Rows[dataGridView1.CurrentRow.Index].Cells["Column3"].Value.ToString();
                string height = dataGridView1.Rows[dataGridView1.CurrentRow.Index].Cells["Column9"].Value.ToString();
                string lenght = dataGridView1.Rows[dataGridView1.CurrentRow.Index].Cells["Column8"].Value.ToString();
                string off = dataGridView1.Rows[dataGridView1.CurrentRow.Index].Cells["Column2"].Value.ToString();
                string paloff = dataGridView1.Rows[dataGridView1.CurrentRow.Index].Cells["Column10"].Value.ToString();
                string palblock = dataGridView1.Rows[dataGridView1.CurrentRow.Index].Cells["Column11"].Value.ToString();
                saveFileDialog1.FileName = filenames;
                saveFileDialog1.Filter = "RAW Image Files | *.raw";
                saveFileDialog1.DefaultExt = "raw";
                if (saveFileDialog1.ShowDialog() == DialogResult.OK)
                {


                    int leng = int.Parse(lenght);
                    int h = int.Parse(height);
                    int w = int.Parse(width);
                    int o = int.Parse(off);
                    int palpff = int.Parse(paloff);
                    int palb = int.Parse(palblock);
                    int size;
                    if (leng < w * h)
                    {
                        size = w * h / 2;

                    }
                    else
                    {
                        size = w * h;
                    }

                    if (o <= palpff + palb)
                    {
                        try
                        {
                            string fileName = saveFileDialog1.FileName;
                            binaryReader.BaseStream.Position = o;
                            byte[] bytes = binaryReader.ReadBytes(size + palb);
                            File.WriteAllBytes(fileName, bytes);
                        }
                        catch
                        {
                            MessageBox.Show("error");
                            // recover from exception
                        }

                    }

                }
                binaryReader.Close();
            }
            if (dataGridView1.Columns[e.ColumnIndex].Name == "Column7")
            {

                int off = Convert.ToInt32(dataGridView1.Rows[dataGridView1.CurrentRow.Index].Cells["Column2"].Value);
                int lenght = Convert.ToInt32(dataGridView1.Rows[dataGridView1.CurrentRow.Index].Cells["Column8"].Value);
                int width = Convert.ToInt32(dataGridView1.Rows[dataGridView1.CurrentRow.Index].Cells["Column3"].Value);
                int height = Convert.ToInt32(dataGridView1.Rows[dataGridView1.CurrentRow.Index].Cells["Column9"].Value);
                if (saveFileDialog1.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        for (int i = off; i >= lenght; i++)
                        {
                            binaryReader.BaseStream.Position = off;
                            string fileName = saveFileDialog1.FileName;
                            byte[] bytes = binaryReader.ReadBytes(lenght);

                            saveFileDialog1.FileName = filenames;
                            File.WriteAllBytes(fileName, UnSwizzle4(bytes, width, height, 0));
                            binaryReader.Close();
                        }
                    }
                    catch
                    {

                    }

                }
            }

        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }
        public static byte[] UnSwizzle4(byte[] buffer, int width, int height, int where)
        {
            // HUGE THANKS TO:
            // L33TMasterJacob for finding the information on unswizzling 4-bit textures
            // Dageron for his 4-bit unswizzling code; he's truly a genius!
            //
            // Source: https://gta.nick7.com/ps2/swizzling/unswizzle_delphi.txt

            byte[] InterlaceMatrix = {
        0x00, 0x10, 0x02, 0x12,
        0x11, 0x01, 0x13, 0x03,
    };

            int[] Matrix = { 0, 1, -1, 0 };
            int[] TileMatrix = { 4, -4 };

            var pixels = new byte[width * height];
            var newPixels = new byte[width * height];

            var d = 0;
            var s = where;

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < (width >> 1); x++)
                {
                    var p = buffer[s++];

                    pixels[d++] = (byte)(p & 0xF);
                    pixels[d++] = (byte)(p >> 4);
                }
            }

            // not sure what this was for, but it actually causes issues
            // we can just use width directly without issues!
            //var mw = width;

            //if ((mw % 32) > 0)
            //    mw = ((mw / 32) * 32) + 32;

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    var oddRow = ((y & 1) != 0);

                    var num1 = (byte)((y / 4) & 1);
                    var num2 = (byte)((x / 4) & 1);
                    var num3 = (y % 4);

                    var num4 = ((x / 4) % 4);

                    if (oddRow)
                        num4 += 4;

                    var num5 = ((x * 4) % 16);
                    var num6 = ((x / 16) * 32);

                    var num7 = (oddRow) ? ((y - 1) * width) : (y * width);

                    var xx = x + num1 * TileMatrix[num2];
                    var yy = y + Matrix[num3];

                    var i = InterlaceMatrix[num4] + num5 + num6 + num7;
                    var j = yy * width + xx;

                    newPixels[j] = pixels[i];
                }
            }

#if UNSWIZZLE_TO_4BIT
    var result = new byte[width * height];

    s = 0;
    d = 0;

    for (int y = 0; y < height; y++)
    {
        for (int x = 0; x < (width >> 1); x++)
            result[d++] = (byte)((newPixels[s++] & 0xF) | (newPixels[s++] << 4));
    }
    return result;
#else
            // return an 8-bit texture
            return newPixels;
#endif
        }
        
        public static decimal ByteArrayToDecimal(byte[] src)
        {
            using (MemoryStream stream = new MemoryStream(src))
            {
                using (BinaryReader reader = new BinaryReader(stream))
                {
                    return reader.ReadDecimal();
                }
            }
        }
        public static byte[] DecimalToByteArray(decimal src)
        {
            using (MemoryStream stream = new MemoryStream())
            {
                using (BinaryWriter writer = new BinaryWriter(stream))
                {
                    writer.Write(src);
                    return stream.ToArray();
                }
            }
        }
        private void button1_Click(object sender, EventArgs e)
        {

        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AboutBox1 aboutBox1 = new AboutBox1();
            aboutBox1.Show();
        }
        private void button1_Click_1(object sender, EventArgs e)
        {
           
            
    }
    }
}