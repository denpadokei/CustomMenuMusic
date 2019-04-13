using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomMenuMusic.Misc
{
    class ResourceUtil
    {
        public static void ExtractEmbeddedResource(string outputDir, string resourceLocation, List<string> files)
        {
            foreach (string file in files)
            {
                Logger.Log(file);
                using (System.IO.Stream stream = System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceLocation + "." + file))
                {
                    using (System.IO.FileStream fileStream = new System.IO.FileStream(System.IO.Path.Combine(outputDir, file), System.IO.FileMode.Create))
                    {
                        Logger.Log("Writing " + file);
                        for (int i = 0; i < stream.Length; i++)
                        {
                            fileStream.WriteByte((byte)stream.ReadByte());
                        }
                        fileStream.Close();
                    }
                }
            }
        }
    }
}
