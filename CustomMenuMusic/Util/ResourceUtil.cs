using System.Collections.Generic;

namespace CustomMenuMusic.Util
{
    internal class ResourceUtil
    {
        public static void ExtractEmbeddedResource(string outputDir, string resourceLocation, List<string> files)
        {
            foreach (var file in files) {
                Logger.Log(file);
                using (var stream = System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceLocation + "." + file)) {
                    using (var fileStream = new System.IO.FileStream(System.IO.Path.Combine(outputDir, file), System.IO.FileMode.Create)) {
                        Logger.Log("Writing " + file);
                        for (var i = 0; i < stream.Length; i++) {
                            fileStream.WriteByte((byte)stream.ReadByte());
                        }
                        fileStream.Close();
                    }
                }
            }
        }
    }
}
