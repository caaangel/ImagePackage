using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.IO;

namespace GameTools.ImagePackage
{
    public class ImagePackage : List<Image>
    {
        private string GetExtension(ref FileStream stream)
        {
            int length = GetNumber(ref stream);
            byte[] buffer = new byte[length];
            stream.Read(buffer, 0, length);
            string extension = System.Text.Encoding.Default.GetString(buffer);
            return extension;
        }

        private int GetNumber(ref FileStream stream)
        {
            int result = stream.ReadByte();
            byte[] buffer = new byte[result];
            // int position = unchecked((int)stream.Position);
            stream.Read(buffer, 0, result);
            string str = System.Text.Encoding.Default.GetString(buffer);
            if (int.TryParse(str, out result))
            {
                return result;
            }
            else
                return -1;
        }

        private MemoryStream GetStream(ref FileStream stream)
        {
            int streamLength = GetNumber(ref stream);
            if (streamLength == -1)
            {
                throw new FileLoadException("returned streamLength is -1");
            }

            byte[] buffer = new byte[streamLength];
            if (stream.Read(buffer, 0, streamLength) == streamLength)
            {
                MemoryStream memStream = new MemoryStream(buffer);
                return memStream;
            }
            else
            {
                return null;
            }
        }

        public bool LoadFromFile(string filename)
        {
            Clear();

            if (!File.Exists(filename)) return false;

            FileStream fs = new FileStream(filename, FileMode.Open);
            fs.Seek(0, SeekOrigin.Begin);

            int count = GetNumber(ref fs);

            Image img = null;
            string Extension = null;

            try
            {
                for (int i = 1; i < count; i++)
                {
                    Extension = GetExtension(ref fs);
                    MemoryStream memStream = GetStream(ref fs);
                    img = Image.FromStream(memStream);
                    Add(img);
                }
                fs.Dispose();
            }
            catch (FileLoadException)
            {
                return false;
            }
            return true;
        }
    }
}
