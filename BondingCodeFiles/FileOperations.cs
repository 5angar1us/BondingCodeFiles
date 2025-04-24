using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BondingCodeFiles
{
    internal class FileOperations
    {

        public static bool IsFileLocked(string targetPath)
        {
            if (File.Exists(targetPath) == false) return false;


            FileStream stream = null;
            try
            {
                stream = new FileStream(targetPath, FileMode.Open, FileAccess.Read, FileShare.None);
                return false; // Файл не заблокирован
            }
            catch (IOException)
            {
                return true; // Файл заблокирован
            }
            finally
            {
                stream?.Close();
            }
        }
    }
}
