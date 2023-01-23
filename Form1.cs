using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Net.WebRequestMethods;
using File = System.IO.File;

namespace kursaDarbs
{
    public partial class Metodes : Form
    {
        public ImageClass imageClass = new ImageClass();
        public Metodes()
        {
            InitializeComponent();
            
            openFileDialog1.Title = "Select an image";
            openFileDialog1.Filter = "Image files (*.jpg, *.jpeg, *.jpe, *.jfif, *.png) | *.jpg; *.jpeg; *.jpe; *.jfif; *.png";

            saveFileDialog1.Title = "Save image";
            saveFileDialog1.Filter = "Image files (*.jpg, *.jpeg, *.jpe, *.jfif, *.png) | *.jpg; *.jpeg; *.jpe; *.jfif; *.png";
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            switch (sender.ToString())
            {
                case "Open":
                    {
                        if (openFileDialog1.ShowDialog() == DialogResult.OK)
                        {
                            Image img = Image.FromFile(openFileDialog1.FileName);
                            pictureBox1.Image = img;

                        }
                        break;
                    }
                case "Save":
                    {
                        //jaraksta faila nosaukumu bez formata
                        if (saveFileDialog1.ShowDialog() == DialogResult.OK)
                        {
                            string filename = saveFileDialog1.FileName;
                            pictureBox1.Image.Save(filename + ".jpg", ImageFormat.Jpeg);
                        }
                        break;
                    }
            }
        }


        

        public static string ByteArrayToDecimalString(byte[] ba)
        {
            StringBuilder hex = new StringBuilder();
            string format = "{0}";
            foreach (byte b in ba)
            {
                hex.AppendFormat(format, b);
                format = " {0}";
            }
            return hex.ToString();
        }

        public void SaveImgAsText(Bitmap img, string path)
        {
            ImageClass localImageClass = new ImageClass();
            localImageClass.ReadImage(img);
            var rgb = localImageClass.img_original;
            byte[] res = new byte[img.Width*img.Height*3];
            int index = 0;
            for (int x = 0; x < img.Width; x++)
            {
                for (int y = 0; y < img.Height; y++)
                {
                    res[index] = rgb[x, y].R;
                    index++;
                    res[index] = rgb[x, y].G;
                    index++;
                    res[index] = rgb[x, y].B;
                    index++;
                }
                
            }
            string result = ByteArrayToDecimalString(res);
            File.WriteAllText(path, result);

        }

        private void Huffman_metode_Click(object sender, EventArgs e)
        {
            //Image img = pictureBox1.Image;
            //CompressImageFirstMethod(img);
            Bitmap bmp = (Bitmap)pictureBox1.Image.Clone();
            string codePath = "C:\\RTU\\3kurss\\Attelu apstrade\\bitmap.txt";
            string decodePath = "C:\\RTU\\3kurss\\Attelu apstrade\\test.txt";
            string decodedResultPath = "C:\\RTU\\3kurss\\Attelu apstrade\\decoded.txt";
            SaveImgAsText(bmp, codePath);
            saspiest(codePath);
            decode(decodePath);
            string decompressedImageData = File.ReadAllText(decodedResultPath);
            string[] decompressedRGBArray = decompressedImageData.Split(' ');
            byte[] decompressedByteArray = new byte[bmp.Width * bmp.Height * 3];
            for (int i = 0; i < decompressedByteArray.Length; i++)
            {
                decompressedByteArray[i] = Convert.ToByte(decompressedRGBArray[i]);
            }

            PixelRGB[,] decompressedImg = new PixelRGB[bmp.Width, bmp.Height];
            int index = 0;
            for (int x = 0; x < bmp.Width; x++)
            {
                for (int y = 0; y < bmp.Height; y++)
                {
                    decompressedImg[x,y] = new PixelRGB(decompressedByteArray[index], //  0 => R
                                                    decompressedByteArray[index + 1], // +1 => G 
                                                    decompressedByteArray[index + 2]); // +2 => B
                    index = index + 3; 
                }
            }
            

        }
        
        private void RLE_COMPRESS_Click(object sender, EventArgs e)
        {
            

            Bitmap bmp = (Bitmap)pictureBox1.Image.Clone();

            byte[] compressedImage = RLEEncode(bmp) ;

                if (saveFileDialog1.ShowDialog() == DialogResult.OK)
                {
                    File.WriteAllBytes(saveFileDialog1.FileName , compressedImage);
                }
            

        }

        private void rle_decompress_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                byte[] compressedImage = File.ReadAllBytes(openFileDialog1.FileName);

                Image image = RLEDecode(compressedImage);

                pictureBox1.Image = image;

                if (saveFileDialog1.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        image.Save(saveFileDialog1.FileName);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("An error occurred while saving the image: " + ex.Message);
                    }
                }
            }
        }

        private void JPG_METODE_Click(object sender, EventArgs e)
        {
            

            Bitmap bmp = (Bitmap)pictureBox1.Image.Clone();
            CompressImage(bmp,70);
           
        }

       

        private void fileToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        //-----------------------------------------------------------
        // Huffman metode
        //-----------------------------------------------------------
        private void saveToFile(String outputFilePath, Dictionary<Char?, int> frequencies, String bits)
        {
            try
            {
                var stream = File.OpenWrite(outputFilePath);
                BinaryWriter os = new BinaryWriter(stream);
                os.Write(frequencies.Count);
                foreach (Char character in frequencies.Keys)
                {
                    os.Write(character);
                    os.Write(frequencies[character]);
                }
                int compressedSizeBits = bits.Length;
                BitArray bitArray = new BitArray(compressedSizeBits);
                for (int i = 0; i < bits.Length; i++)
                {
                    bitArray.set(i, bits.ElementAt(i) != '0' ? 1 : 0);
                }

                os.Write(compressedSizeBits);
                os.Write(bitArray.bytes, 0, bitArray.getSizeInBytes());
                os.Flush();
                os.Close();

            }
            catch (FileNotFoundException e)
            {
                Debug.WriteLine(e.ToString());
            }
            catch (IOException e)
            {
                Debug.WriteLine(e.ToString());
            }
        }


        private Dictionary<Char?, int> countFrequency(String text)
        {
            Dictionary<Char?, int> freqMap = new Dictionary<Char?, int>();
            for (int i = 0; i < text.Length; i++)
            {
                Char? c = text.ElementAt(i);
                if (freqMap.ContainsKey(c))
                {
                    freqMap[c]++;
                } else
                {
                    freqMap.Add(c, 1);
                }
            }
            return freqMap;
        }
        private CodeTreeNode huffman(List<CodeTreeNode> codeTreeNodes)
        {
            while (codeTreeNodes.Count > 1)
            {
                codeTreeNodes.Sort();

                CodeTreeNode left = (CodeTreeNode)codeTreeNodes[codeTreeNodes.Count - 1];
                codeTreeNodes.RemoveAt(codeTreeNodes.Count - 1);
                CodeTreeNode right = (CodeTreeNode)codeTreeNodes[codeTreeNodes.Count - 1];
                codeTreeNodes.RemoveAt(codeTreeNodes.Count - 1);

                CodeTreeNode parent = new CodeTreeNode(null, right.weight + left.weight, left, right);
                codeTreeNodes.Add(parent);
            }
            return (CodeTreeNode)codeTreeNodes[0];
        }


        private String huffmanDecode(String encoded, CodeTreeNode tree)
        {
            StringBuilder decoded = new StringBuilder();

            CodeTreeNode node = tree;
            for (int i = 0; i < encoded.Length; i++)
            {
                node = encoded.ElementAt(i).Equals('0') ? node.left : node.right;
                if (node.content != null)
                {
                    decoded.Append(node.content);
                    node = tree;
                }
            }
            return decoded.ToString();
        }

        public void saspiest(String filePath)
        {
            try
            {
                String content = File.ReadAllText(filePath).ToString();
                //System.out.println(content);

                Dictionary<Char?, int> frequencies = countFrequency(content);

                //Set<Character> keys = frequencies.keySet();
                //for(Character key : keys){
                // System.out.println( key + "----" + frequencies.get(key) );
                //}

                //System.out.println("Code table:");


                // generating a list of tree leaves
                List<CodeTreeNode> codeTreeNodes = new List<CodeTreeNode>();
                foreach (Char? c in frequencies.Keys)
                {
                    codeTreeNodes.Add(new CodeTreeNode(c, frequencies[c]));
                }

                // building a code tree using the Huffman algorithm

                CodeTreeNode tree = huffman(codeTreeNodes);

                // generate a table of prefix codes for encoded characters using a code tree
                Dictionary<Char?, string> codes = new Dictionary<Char?, string>();
                foreach (Char? c in frequencies.Keys)
                {
                    codes.Add(c, tree.getCodeForCharacter(c, ""));
                }


                StringBuilder encoded = new StringBuilder();
                for (int i = 0; i < content.Length; i++)
                {
                    encoded.Append(codes[content.ElementAt(i)]);
                }
                String compressedtext = "C:\\RTU\\3kurss\\Attelu apstrade\\test.txt";
                saveToFile(compressedtext, frequencies, encoded.ToString());//exports as binary file

            }
            catch (IOException e)
            {
                Debug.WriteLine(e.ToString());
            }
        }

        // loading compressed information and frequency table from a file
        private void loadFromFile(String inputFilePath, Dictionary<Char?, int> frequencies, StringBuilder bits)
        {
            try
            {
                var stream = File.OpenRead(inputFilePath);
                BinaryReader os = new BinaryReader(stream);
                int frequencyTableSize = os.ReadInt32();
                for (int i = 0; i < frequencyTableSize; i++)
                {
                    frequencies.Add(os.ReadChar(), os.ReadInt32());
                }
                int dataSizeBits = os.ReadInt32();
                BitArray bitArray = new BitArray(dataSizeBits);
                os.Read(bitArray.bytes, 0, bitArray.getSizeInBytes());
                os.Close();

                for (int i = 0; i < bitArray.size; i++)
                {
                    bits.Append(bitArray.get(i) != 0 ? 1 : 0);
                }

            }
            catch (FileNotFoundException e)
            {
                Debug.WriteLine(e.ToString());
            }
            catch (IOException e)
            {
                Debug.WriteLine(e.ToString());
            }
        }

        public void decode(String filename)
        {
            try
            {
                Dictionary<Char?, int> frequencies2 = new Dictionary<Char?, int>();
                StringBuilder encoded2 = new StringBuilder();
                List<CodeTreeNode> codeTreeNodes = new List<CodeTreeNode>();
                // extracting compressed information from a file
                loadFromFile(filename, frequencies2, encoded2);

                // tile generation and construction of the Huffman code tree based on the frequency table of the compressed file
                foreach (Char? c in frequencies2.Keys)
                {
                    codeTreeNodes.Add(new CodeTreeNode(c, frequencies2[c]));
                }
                CodeTreeNode tree2 = huffman(codeTreeNodes);

                // decoding back the original information from the compressed
                String decoded = huffmanDecode(encoded2.ToString(), tree2);


                // saving decoded information to file
                String decompressedtext = "C:\\RTU\\3kurss\\Attelu apstrade\\decoded.txt";
                File.WriteAllText(decompressedtext, decoded);
            }
            catch (IOException e)
            {
                Debug.WriteLine(e.ToString());
            }

        }

        //-----------------------------------------------------------
        // RLE metode
        //-----------------------------------------------------------

        private byte[] RLEEncode(Image image)
        {
            byte[] imageBytes = ImageToByteArray(image);

            List<byte> compressedBytes = new List<byte>();
            int count = 1;
            byte currentByte = imageBytes[0];

            for (int i = 1; i < imageBytes.Length; i++)
            {
                if (currentByte == imageBytes[i])
                {
                    count++;
                }
                else
                {
                    compressedBytes.Add((byte)count);
                    compressedBytes.Add(currentByte);

                    count = 1;
                    currentByte = imageBytes[i];
                }
            }

            compressedBytes.Add((byte)count);
            compressedBytes.Add(currentByte);

            return compressedBytes.ToArray();
        }

        private Image RLEDecode(byte[] compressedImage)
        {
            List<byte> imageBytes = new List<byte>();

            for (int i = 0; i < compressedImage.Length; i += 2)
            {
                int count = compressedImage[i];
                byte currentByte = compressedImage[i + 1];

                for (int j = 0; j < count; j++)
                {
                    imageBytes.Add(currentByte);
                }
            }

            return ByteArrayToImage(imageBytes.ToArray());
        }

        private byte[] ImageToByteArray(Image image)
        {
            using (var ms = new MemoryStream())
            {
                image.Save(ms, image.RawFormat);
                return ms.ToArray();
            }
        }
        private Image ByteArrayToImage(byte[] byteArray)
        {
            using (var ms = new MemoryStream(byteArray))
            {
                return Image.FromStream(ms);
            }
        }

        //-----------------------------------------------------------
        // JPG metode
        //-----------------------------------------------------------
        //-----------------------------------------------------------
        // Treša metode
        //-----------------------------------------------------------

        //šī rinda deklarē metodi "CompressImage", kas izmanto divas ievades: attēla objektu un veselu skaitli "kvalitāte". Tas atgriež virkni.
        private string CompressImage(Image Image, int Quality)
        {   //: šī rinda izveido jaunu bitkartes objektu no ievades attēla objekta
            using (Bitmap mybitmap = new Bitmap(@Image))
            {   //izsauc palīgmetodi "GetEncoder" un kā ievadi tiek nodota Jpeg attēla formātā. Tas piešķir atgriezto kodeku mainīgajam "jpegEncoder".
                ImageCodecInfo jpgEncoder = GetEncoder(ImageFormat.Jpeg);
                
                System.Drawing.Imaging.Encoder myEncoder = System.Drawing.Imaging.Encoder.Quality;
                //izveido jaunu EncoderParameter objektu, kas kā ievadi izmanto mainīgo "myEncoder" un "kvalitātes".
                EncoderParameter myEncoderParameter = new EncoderParameter(myEncoder, Quality);
                //izveido jaunu EncoderParameters objektu ar vienu parametru.
                EncoderParameters myEncoderParameters = new EncoderParameters(1);
               // šī rinda piešķir iepriekš izveidoto "encoderParameter" objektu pirmajam (un vienīgajam) objekta "encoderParameters" parametram.
                myEncoderParameters.Param[0] = myEncoderParameter;
                //direktorijas ceļš
                String ResultDirectory = "C:\\Users\\PC\\Desktop";
                
                //Šajā rindā apvieno rezultātu direktoriju ar saspiestā faila nosaukumu, gala rezultāts ir ceļš, kurā tiks saglabāts saspiestais attēls.
                string ResultPath = @ResultDirectory + "\\" + Path.GetFileNameWithoutExtension("Compressed") + ".JPEG";

                mybitmap.Save(ResultPath, jpgEncoder, myEncoderParameters);
                //šī rinda atgriež galīgo "resultPath", kurā tiek saglabāts saspiestais attēls.
                return ResultPath;
            }
        }
        //šī rinda deklarē palīgmetodi "GetEncoder", kas kā ievadi izmanto ImageFormat objektu. Tas atgriež ImageCodecInfo objektu.
        private ImageCodecInfo GetEncoder(ImageFormat format)
        {   //rinda izveido ImageCodecInfo objektu masīvu, izsaucot metodi GetImageEncoders.
            ImageCodecInfo[] codecs = ImageCodecInfo.GetImageDecoders();
            //šī rinda sāk cilpu, kas atkārtojas caur katru kodeku "kodeku" masīvā.
            foreach (ImageCodecInfo codec in codecs)
            {   //šī rindiņa pārbauda, vai pašreizējā kodeka FormatID ir vienāds ar ievades formāta Guid.
                if (codec.FormatID == format.Guid)
                {
                    return codec;
                }
            }
            return null;
        }


       

        
    }
}
