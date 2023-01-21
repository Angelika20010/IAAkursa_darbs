using System;
using System.Drawing;
using System.Runtime.InteropServices;

namespace kursaDarbs
{
    public class ImageClass
    {

        public PixelRGB[,] img_original;
        public PixelRGB[,] img_edited;

        public ImageClass()
        {
        }

        public void ReadImage(Bitmap bmp)
        {
            img_original = new PixelRGB[bmp.Width, bmp.Height];
            img_edited = new PixelRGB[bmp.Width, bmp.Height];

            //locking so it wouldn't be changed by someone else during our manipulations
            var bmpData = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height),
                                        System.Drawing.Imaging.ImageLockMode.ReadOnly,
                                        bmp.PixelFormat);

            IntPtr ptr = IntPtr.Zero;
            int pixelComponents;
            switch (bmpData.PixelFormat)
            {
                case System.Drawing.Imaging.PixelFormat.Format24bppRgb:
                    pixelComponents = 3;  //  24:8 = 3
                    break;
                case System.Drawing.Imaging.PixelFormat.Format32bppRgb:
                    pixelComponents = 4;  //32:8 = 4
                    break;
                default:
                    pixelComponents = 0;
                    break;
            }

            var row = new byte[bmp.Width * pixelComponents];
            for (int y = 0; y < bmp.Height; y++)
            {
                ptr = bmpData.Scan0 + y * bmpData.Stride;  //.Scan0 - starting point & .Stride - scan width of Bitmap object
                Marshal.Copy(ptr, row, 0, row.Length);
                for (int x = 0; x < bmp.Width; x++)
                {
                    img_original[x, y] = new PixelRGB(row[pixelComponents * x + 2],    // +2 => R
                                                        row[pixelComponents * x + 1],  // +1 => G
                                                        row[pixelComponents * x]);     // 0 => B

                    img_edited[x, y] = new PixelRGB(row[pixelComponents * x + 2],
                                                        row[pixelComponents * x + 1],
                                                        row[pixelComponents * x]);

                }
            }
            //don't forget to unlock bits if locking them
            bmp.UnlockBits(bmpData);
        }

        public Bitmap DrawImage(PixelRGB[,] img)
        {
            IntPtr ptr = IntPtr.Zero;
            var bmp = new Bitmap(img.GetLength(0), img.GetLength(1), System.Drawing.Imaging.PixelFormat.Format24bppRgb);
            var bmpData = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height),
                                System.Drawing.Imaging.ImageLockMode.WriteOnly,
                                bmp.PixelFormat);

            var row = new byte[bmp.Width * 3]; //using 3 because PixelFormat.Format24bppRgb

            for (int y = 0; y < bmp.Height; y++)
            {
                for (int x = 0; x < bmp.Width; x++)
                {
                    row[3 * x + 2] = img[x, y].R;
                    row[3 * x + 1] = img[x, y].G;
                    row[3 * x] = img[x, y].B;
                }
                ptr = bmpData.Scan0 + y * bmpData.Stride;
                Marshal.Copy(row, 0, ptr, row.Length);
            }
            bmp.UnlockBits(bmpData);
            return bmp;
        }
    }
}

