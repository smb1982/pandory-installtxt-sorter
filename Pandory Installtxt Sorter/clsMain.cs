using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Globalization;
using System.Text.RegularExpressions;

namespace Pandory_Install_Sort
{
    internal class clsMain
    {
        string strInstallTXT;
        public void Start()
        {
            showIntro();
            strInstallTXT = getInstallTXT();
            SortedDictionary<string, List<string>> dictTitlethenRom = new SortedDictionary<string, List<string>>();
            dictTitlethenRom = createDictionary();

            StringBuilder sb = new StringBuilder();

            foreach(KeyValuePair<string,List<string>> kvPair in dictTitlethenRom)
            {
                if(kvPair.Value.Count > 1)
                {   
                    foreach(string value in kvPair.Value)
                    {
                        sb.AppendLine(value);
                    }
                }
                else
                {
                    sb.AppendLine(kvPair.Value[0]);
                }
            }

            string strEXT = randomText();
            string strInstallBaseDir = strInstallTXT.TrimEnd(Path.GetFileName(strInstallTXT).ToCharArray());

            File.Move(strInstallBaseDir + "install.txt", strInstallBaseDir + "install.bak_" + strEXT);
            Console.WriteLine(centerText("\"install.txt\" backed up to \"install.bak_" + strEXT + "\" ... Done!"));

            string strOut = sb.ToString().TrimEnd('\r', '\n');

            File.WriteAllText(strInstallTXT, strOut);
            Console.WriteLine(centerText("New sorted \"install.txt\" written ... Done!"));
        }

        private SortedDictionary<string, List<string>> createDictionary()
        {
            SortedDictionary<string, List<string>> dictTitlethenRom = new SortedDictionary<string, List<string>>();

            string strInstallBaseDir = strInstallTXT.TrimEnd(Path.GetFileName(strInstallTXT).ToCharArray());

            string strText = NormalizeLineEndings( File.ReadAllText(strInstallTXT) );

            string[] lines = strText.Split(new string[] { Environment.NewLine }, StringSplitOptions.None);

            for(int i=0; i<lines.Length; i++)
            {
                string strRom = lines[i];
                string strRomDir = strInstallBaseDir + strRom + "\\";
                string strRomXML = strInstallBaseDir + strRom + "\\" + strRom + ".xml";
                string strGameTitle = getGameTitle(strRomXML);

                if (strGameTitle.StartsWith("ZZZ(notgame)") == false)
                {
                    if (dictTitlethenRom.ContainsKey(strGameTitle))
                    {
                        List<string> list = dictTitlethenRom[strGameTitle];
                        list.Add(strRom);
                        dictTitlethenRom[strGameTitle] = list;
                    }
                    else
                    {
                        List<string> list = new List<string>();
                        list.Add(strRom);
                        dictTitlethenRom.Add(strGameTitle, list);
                    }
                }
            }

            return dictTitlethenRom;
        }

        private string getGameTitle(string strXML)
        {
            List<string> list = File.ReadAllLines(strXML).ToList<string>();
            string strGameTitle = string.Empty;

            foreach(string line in list)
            {
                if (line.Trim().StartsWith("<name>"))
                {
                    strGameTitle = line.Trim().Replace("<name>", "").Replace("</name>", "".Trim());
                }
            }

            return strGameTitle;
        }

        public string NormalizeLineEndings(string lines, string targetLineEnding = null)
        {
            if (string.IsNullOrEmpty(lines))
            {
                return lines;
            }

            targetLineEnding = Environment.NewLine;

            const string unixLineEnding = "\n";
            const string windowsLineEnding = "\r\n";
            const string macLineEnding = "\r";

            if (targetLineEnding != unixLineEnding && targetLineEnding != windowsLineEnding &&
                targetLineEnding != macLineEnding)
            {
                throw new ArgumentOutOfRangeException(nameof(targetLineEnding), "Unknown target line ending character(s).");
            }

            lines = lines
                .Replace(windowsLineEnding, unixLineEnding)
                .Replace(macLineEnding, unixLineEnding);

            if (targetLineEnding != unixLineEnding)
            {
                lines = lines.Replace(unixLineEnding, targetLineEnding);
            }

            return lines;
        }

        private string getInstallTXT()
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Title = "Select \"install.txt\" inside mcgames folder...";
            ofd.FileName = "install.txt";
            ofd.Filter = "install.txt|install.txt";
            ofd.ShowDialog();
            string strOut = ofd.FileName;

            if (strOut == null)
            {
                Application.Exit();
            }

            if (File.Exists(strOut) == false)
            {
                Application.Exit();
            }
            else
            {
                if (File.ReadAllLines(strOut).Length < 2)
                {
                    Application.Exit();
                }
            }

            return strOut;
        }

        private void showIntro()
        {
            string version = System.Windows.Forms.Application.ProductVersion;
            string strTitle = "Pandory Install.txt Title Sorter " + version;
            string strTextLine1 = "This tool will take your ':\\mcgames\\install.txt' and read in all the xml titles underneath it,";
            string strTextLine2 = "then it will sort the romlist by the actual title so that the resulting menu is alphabetical.";
            string strTextLine3 = "your existing install.txt will be backed up to install.txt.bak and the new install.txt in its place";
            string strTextLine4 = "Written by shaneb1982 - vykyan@gmail.com";
            string strPrompt = "Press any key to continue or CTRL+C to exit...";


            Console.WriteLine("".PadLeft(119, '='));
            Console.WriteLine("");
            Console.WriteLine(centerText(strTitle));
            Console.WriteLine("");
            Console.WriteLine(centerText(strTextLine1));
            Console.WriteLine(centerText(strTextLine2));
            Console.WriteLine(centerText(strTextLine3));
            Console.WriteLine("");
            Console.WriteLine(centerText(strTextLine4));
            Console.WriteLine("");
            Console.WriteLine("".PadLeft(119, '='));
            Console.WriteLine("");
            Console.Write(centerText(strPrompt));
            Console.WriteLine("");
            Console.ReadLine();

        }

        private string centerText(string strIn)
        {
            string strOut = String.Empty;

            strOut = "".PadLeft((120 - strIn.Length) / 2) + strIn + "".PadRight((119 - strIn.Length) / 2);

            return strOut;
        }

        private string randomText()
        {
            DateTime currentDateTime = DateTime.Now;
            return currentDateTime.ToString("ddmmyyhhmmss", CultureInfo.CreateSpecificCulture("en-US"));
        }
    }
}
