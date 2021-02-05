using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Text.RegularExpressions;
using System.Security.Cryptography;
using System.Threading;
using System.Diagnostics;
using System.Xml;
using System.Xml.Serialization;
using System.Runtime.InteropServices;

namespace StarCraftMapBrowser
{
    public partial class Main : Form
    {
        List<string> files;
        List<string> hashes;
        List<string> bigJPEGs;
        List<Map> maps;
        //Bitmap emptyBitmap;
        BlockingCollection<string> cannotOpen;
        BlockingCollection<Map> tempMaps;
        int processedMaps;
        string scenario = "staredit\\scenario.chk";
        string mapsDataBase = "mapsdb.xml";
        string startingFolder;
        //Dictionary for cleaning strings. Очистка тексту
        Dictionary<byte, List<byte>> replacementDictionary;
        List<byte> filteredChars;
        List<byte> systemIllegalChars;
        Random random;
        List<string> randomSequence;
        System.Windows.Forms.Timer updateProgressTimer;
        FileStream fout;
        Stopwatch stopwatch;
        BindingSource bindingSource;
        //Token source for cancelling Parallel.ForEach loop. Відміна паралельних циклів
        CancellationTokenSource tokenSource;
        ParallelOptions loopOptions;

        ContextMenuStrip mapMenu;
        ToolStripMenuItem moveOption;
        ToolStripMenuItem renameOption;
        ToolStripMenuItem copyOption;
        ToolStripMenuItem deleteOption;
        ToolStripMenuItem removeDupesOption;
        ToolStripMenuItem saveFullsizeJPEG;

        DataSet dataSet;
        DataTable mapsTable;
        //Different degrees of cleaning. Різні рівні очистки тексту
        public enum TextType
        {
            map,
            description
        };
        public enum ImageSize
        {
            thumbnail,
            fullsize,
            both
        };
        //Graphics. Графіка
        #region Tilesets
        byte[] ashworld_cv5;
        byte[] ashworld_vx4;
        byte[] ashworld_vr4;
        byte[] ashworld_wpe;
        byte[] badlands_cv5;
        byte[] badlands_vx4;
        byte[] badlands_vr4;
        byte[] badlands_wpe;
        byte[] Desert_cv5;
        byte[] Desert_vx4;
        byte[] Desert_vr4;
        byte[] Desert_wpe;
        byte[] Ice_cv5;
        byte[] Ice_vx4;
        byte[] Ice_vr4;
        byte[] Ice_wpe;
        byte[] install_cv5;
        byte[] install_vx4;
        byte[] install_vr4;
        byte[] install_wpe;
        byte[] jungle_cv5;
        byte[] jungle_vx4;
        byte[] jungle_vr4;
        byte[] jungle_wpe;
        byte[] platform_cv5;
        byte[] platform_vx4;
        byte[] platform_vr4;
        byte[] platform_wpe;
        byte[] Twilight_cv5;
        byte[] Twilight_vx4;
        byte[] Twilight_vr4;
        byte[] Twilight_wpe;
        #endregion
        Dictionary<int, List<byte[]>> tilesetDic;
        //static Regex _wordRegex = new Regex(@"5\w+5", RegexOptions.Compiled); //faster performance
        public Main()
        {
            InitializeComponent();
            progressBar.Visible = false;
            progressLBL.Visible = false;
            FilenameTextBox.Visible = true;
            random = new Random();
            InitializeReplacements();
            updateProgressTimer = new System.Windows.Forms.Timer();
            updateProgressTimer.Interval = 1000;
            updateProgressTimer.Tick += new EventHandler(UpdateProgressbar);
            InitializeContextMenu();
            InitializeGrid();
            LoadLastSession();
            ReadTileSets();
            stopwatch = new Stopwatch();
            startingFolder = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            files = new List<string>();
            tokenSource = new CancellationTokenSource();
            loopOptions =
                new ParallelOptions()
                {
                    CancellationToken = tokenSource.Token
                };

            this.bgWorker1.DoWork += new System.ComponentModel.DoWorkEventHandler(bgWorker1_DoWork);
            //this.bgWorker1.ProgressChanged += new ProgressChangedEventHandler(bgWorker1_ProgressChanged);
            this.bgWorker1.RunWorkerCompleted += new RunWorkerCompletedEventHandler(bgWorker1_RunWorkerCompleted);
        }
        //Завантаження попередньої сесії
        void LoadLastSession()
        {
            //dataBase
            if (File.Exists(mapsDataBase))
            {
                maps = DeserializeFromXML(mapsDataBase);
                if (maps.Count > 0)
                {
                    List<Map> tempMaps = new List<Map>();
                    foreach (Map map in maps)
                    {
                        if (File.Exists(map.orgFilename)) tempMaps.Add(map);
                    }
                    maps = new List<Map>(tempMaps);

                    SerializeToXML(mapsDataBase);
                }
                RefreshDataGridView();
                infoBox.AppendText(maps.Count() + " maps found.");
                infoBox.AppendText(Environment.NewLine);
            }
        }
        //Select folder button. Кнопка вибору папки
        public void selectFolderBtn_Click(object sender, EventArgs e)
        {
            if(selectFolderBtn.Text == "Select Folder")
            {
                using (var fbd = new FolderBrowserDialog())
                {

                    //fbd.SelectedPath = @"D:\2\Test folder";
                    fbd.SelectedPath = startingFolder;
                    DialogResult result = fbd.ShowDialog();

                    if (result == DialogResult.OK && !string.IsNullOrWhiteSpace(fbd.SelectedPath))
                    {
                        startingFolder = fbd.SelectedPath;
                        List<string> tempfiles = Directory.GetFiles(fbd.SelectedPath, "*.sc?", SearchOption.AllDirectories).ToList();
                        Regex filterextensions = new Regex(@"(.*?)([^\\]+)(\.scm|\.scx)", RegexOptions.IgnoreCase);
                        files.Clear();
                        foreach(string tempfile in tempfiles)
                        {
                            Match match = filterextensions.Match(tempfile);
                            if(match.Success) files.Add(tempfile);
                        }
                        hashes = new List<string>();
                        cannotOpen = new BlockingCollection<string>();
                        tempMaps = new BlockingCollection<Map>();

                        processedMaps = 0;
                        progressBar.Value = 0;
                        progressBar.Visible = true;
                        progressLBL.Visible = true;
                        FilenameTextBox.Visible = false;

                        selectFolderBtn.Text = "Cancel";
                        updateProgressTimer.Enabled = true;
                        infoBox.AppendText("Working...");
                        infoBox.AppendText(Environment.NewLine);
                        stopwatch.Reset();
                        stopwatch.Start();
                        if (!this.bgWorker1.IsBusy) bgWorker1.RunWorkerAsync();
                    }
                }
            }
            else
            {
                tokenSource.Cancel();
                infoBox.AppendText("Cancelled.");
                infoBox.AppendText(Environment.NewLine);
                selectFolderBtn.Text = "Select Folder";
                SerializeToXML(mapsDataBase);
            }
        }
        //Rename All button. Кнопка "Перейменувати все"
        public void renameAllBtn_Click(object sender, EventArgs e)
        {
            if (maps != null)
            {
                int dupCount = 0;
                int moveCount = 0;
                hashes = new List<string>();
                foreach (Map map in maps)
                {
                    if (!hashes.Contains(map.hash))
                    {
                        hashes.Add(map.hash);
                        string newFilename = RenameFile(map.orgFilename, map.name);
                        MoveJpeg(map.orgFilename, newFilename);
                        map.orgFilename = newFilename;
                        moveCount++;
                    }
                    else if (DupeMenu.Checked) //remove duplicates menu checkbox
                    {
                        foreach (Map m in maps)
                        {
                            if (m.orgFilename != map.orgFilename && m.hash == map.hash)
                            {
                                uint size1 = GetScenarioSize(m.orgFilename);
                                uint size2 = GetScenarioSize(map.orgFilename);
                                if (size1 == size2)
                                {
                                    if (File.Exists(map.orgFilename))
                                    {
                                        FileAttributes fa = File.GetAttributes(map.orgFilename);
                                        if ((fa & FileAttributes.ReadOnly) != 0)
                                        {
                                            // Use the exclusive-or operator (^) to toggle the ReadOnly flag
                                            fa ^= FileAttributes.ReadOnly;
                                            File.SetAttributes(map.orgFilename, fa);
                                        }
                                        File.Delete(map.orgFilename);
                                    } 
                                    DeleteJpeg(map.orgFilename);
                                    dupCount++;
                                    break;
                                }
                            }
                        }
                    }
                    else
                    {
                        string newFilename = RenameFile(map.orgFilename, map.name);
                        MoveJpeg(map.orgFilename, newFilename);
                        map.orgFilename = newFilename;
                        moveCount++;
                    }
                }
                List<Map> tempMaps = new List<Map>();
                foreach (Map map in maps)
                {
                    if (File.Exists(map.orgFilename)) tempMaps.Add(map);
                }
                maps = new List<Map>(tempMaps);

                SerializeToXML(mapsDataBase);
                RefreshDataGridView();
                if (DupeMenu.Checked) infoBox.AppendText("Renamed " + moveCount + " file(s)." + " Deleted " + dupCount + " duplicate(s).");
                else infoBox.AppendText("Renamed " + moveCount + " file(s).");
                infoBox.AppendText(Environment.NewLine);
            }
        }
        //Select folder button runs this worker. Кнопка "Вибрати папку" запускає робітника
        public void bgWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                Parallel.ForEach(files, loopOptions, () => 0,
                    (string file, ParallelLoopState loopState, int tlsValue) =>
                    {
                        byte[] fileContents = ReadScenarioFile(file);
                        if (fileContents != null)
                        {
                            //hash uniquely identifies map as a whole. Унікальність карти
                            string hash = getMd5Hash(fileContents);
                            List<byte[]> mapStrings = GetMapStrings(fileContents);
                            string name = Encoding.ASCII.GetString(CleanName(mapStrings[0], TextType.map)); //map
                            string description = Encoding.ASCII.GetString(CleanName(mapStrings[1], TextType.description)); //description
                            //tilehash uniquely identifies map terrain. Унікальність ландшафту
                            string tilehash = ProcessImage(fileContents, file, ImageSize.thumbnail);
                            tempMaps.Add(new Map(file, name, description, hash, tilehash));
                        }
                        tlsValue++; //number of processed maps. Кількість оброблених карт
                        return tlsValue;
                    },
                        tlsValue => Interlocked.Add(ref processedMaps, tlsValue));
            }
            catch (OperationCanceledException)
            {
                //
            }

        }
        //Select Folder finished processing. Дії після завершення пошуку карт по кнопці "Вибрати папку"
        public void bgWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            stopwatch.Stop();
            updateProgressTimer.Enabled = false;
            progressBar.Visible = false;
            progressLBL.Visible = false;
            FilenameTextBox.Visible = true;
            selectFolderBtn.Text = "Select Folder";
            maps = new List<Map>(tempMaps);
            SaveReportToFile(maps);

            foreach (string error in cannotOpen)
            {
                infoBox.AppendText("Could not open: " + error);
                infoBox.AppendText(Environment.NewLine);
            }
            foreach (Map map in maps)
            {
                if (!hashes.Contains(map.hash)) hashes.Add(map.hash);
            }

            SerializeToXML(mapsDataBase);
            infoBox.AppendText("Found " + hashes.Count + " unique maps and "
                + (maps.Count - hashes.Count - cannotOpen.Count).ToString()
                + " duplicate(s) (" + maps.Count + " maps total).");
            TimeSpan ts = stopwatch.Elapsed;
            infoBox.AppendText(Environment.NewLine);
            infoBox.AppendText("Completed in " + ts.ToString("mm\\:ss\\.ff"));
            infoBox.AppendText(Environment.NewLine);

            RefreshDataGridView();
        }
        //Save full size JPEGs. Збереження повномасштабних зображень
        private void bgWorkerBigJPEGs_DoWork(object sender, DoWorkEventArgs e)
        {
            stopwatch.Reset();
            stopwatch.Start();
            Parallel.ForEach(bigJPEGs, file =>
            {
                ProcessImage(ReadScenarioFile(file), file, ImageSize.fullsize);
            });
        }
        public void bgWorkerBigJPEGs_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            stopwatch.Stop();
            TimeSpan ts = stopwatch.Elapsed;
            infoBox.AppendText("Completed in " + ts.ToString("mm\\:ss\\.ff"));
            infoBox.AppendText(Environment.NewLine);
            bigJPEGs.Clear();
        }
        //Not used. Не використовується
        public static DataTable ToDataTable<T>(IList<T> data)
        {
            PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(typeof(T));
            DataTable table = new DataTable();
            foreach (PropertyDescriptor prop in properties)
                table.Columns.Add(prop.Name, Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType);
            foreach (T item in data)
            {
                DataRow row = table.NewRow();
                foreach (PropertyDescriptor prop in properties)
                    row[prop.Name] = prop.GetValue(item) ?? DBNull.Value;
                table.Rows.Add(row);
            }
            return table;
        }
        //Оновлення списку карт на екрані
        void RefreshDataGridView()
        {
            mapsTable.Clear();

            foreach (Map m in maps)
            {
                mapsTable.Rows.Add(m.orgFilename, m.name, m.description, m.hash, m.tileHash);
            }

            grid.Focus();
        }
        //Create DataSet, DataTable and add columns to DataGridView
        //Створити базу даних, таблиці та додати стовпчики до представлення DataGridView
        void InitializeGrid()
        {
            dataSet = new DataSet();
            mapsTable = new DataTable("Maps");
            dataSet.Tables.Add(mapsTable);
            //DataTable mapsTable = ds.Tables.Add("Maps");
            mapsTable.Columns.Add("Filename", typeof(string));
            mapsTable.Columns.Add("Mapname", typeof(string));
            mapsTable.Columns.Add("Description", typeof(string));
            mapsTable.Columns.Add("Hash", typeof(string));
            mapsTable.Columns.Add("Tilehash", typeof(string));

            bindingSource = new BindingSource();
            bindingSource.DataSource = dataSet;
            bindingSource.DataMember = mapsTable.TableName;

            grid.AutoGenerateColumns = false;
            grid.ColumnCount = 3;
            grid.Columns[0].Name = "Filename"; // name
            grid.Columns[0].HeaderText = "Filename"; // header text
            grid.Columns[0].DataPropertyName = "Filename"; // field name
            grid.Columns[0].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            grid.Columns[0].FillWeight = 50F;
            grid.Columns[1].Name = "Mapname"; // name
            grid.Columns[1].HeaderText = "Mapname"; // header text
            grid.Columns[1].DataPropertyName = "Mapname"; // field name
            grid.Columns[1].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            grid.Columns[1].FillWeight = 40F;
            grid.Columns[2].Name = "Tilehash"; // name
            grid.Columns[2].HeaderText = "Tilehash"; // header text
            grid.Columns[2].DataPropertyName = "Tilehash"; // field name
            grid.Columns[2].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            grid.Columns[2].FillWeight = 10F;

            DescrTextBox.DataBindings.Add("Text", bindingSource, "Description");
            NameTextBox.DataBindings.Add("Text", bindingSource, "Mapname");
            FilenameTextBox.DataBindings.Add("Text", bindingSource, "Filename");

            grid.DataSource = bindingSource;
        }
        //Оновлення стрічки прогресу
        public void UpdateProgressbar(object sender, EventArgs e)
        {
            double progress = (double)processedMaps / files.Count * 100;
            progressBar.Value = (int)progress;
            progressLBL.Text = ((int)progress).ToString() + "%";
        }
        //Збереження файлу-звіту
        void SaveReportToFile(List<Map> maps)
        {
            if (File.Exists("report.txt")) File.Delete("report.txt");
            fout = new FileStream("report.txt", FileMode.Append);
            StreamWriter fstr_out = new StreamWriter(fout);
            fstr_out.Write("Name\t" + "Filename\t" + "Hash\n" + "Tilehash\n");
            foreach (Map map in maps)
            {
                fstr_out.Write(map.name + "\t" + map.orgFilename + "\t" + map.hash + "\t" + map.tileHash + "\n");
            }
            fstr_out.Close();
        }
        //Перейменування файлу
        public string RenameFile(string orgFile, string name)
        {
            if (File.Exists(orgFile))
            {
                string newFileName = GetNewFileName(orgFile, name);
                while (File.Exists(newFileName))
                {
                    newFileName = GetNewFileName(orgFile, name);
                }
                File.Move(orgFile, newFileName);
                return newFileName;
            }
            else return null;
        }
        //Map name should be 27 chars + dot + extension. 23+_XXX = 27. Обрізка задовгих назв карт
        public string TruncateString(string name)
        {
            if (name.Length == 0) name = "Noname";
            if (name.Length > 23) name = name.Substring(0, 23);
            return name;
        }
        //Maps often have same names. This makes them unique. Генерація унікального ID
        public string GenerateMapID()
        {
            string mapID = "_";
            for (int i = 0; i < 3; i++) mapID += randomSequence[random.Next(0, 36)];
            return mapID;
        }
        //Generate a filename from name and unique ID during renaming. Генерація імені файлу при перейменуванні
        public string GetNewFileName(string orgFile, string name)
        {
            string newFile = orgFile;
            string replacement = "${1}" + TruncateString(name) + GenerateMapID() + "${3}";
            newFile = Regex.Replace(newFile, @"(.*?)([^\\]+)(\.sc.)", replacement, RegexOptions.IgnoreCase);
            return newFile;
        }
        //Read scenario.chk. Читання даних карти
        public byte[] ReadScenarioFile(string path)
        {
            IntPtr mpqHandle = new IntPtr();
            if (StormLib.SFileOpenArchive(path, 0, 256, out mpqHandle)) //256 - read only
            {
                uint pdwFileSizeHigh = 0;
                IntPtr fileHandle = new IntPtr();
                StormLib.SFileOpenFileEx(mpqHandle, scenario, 0, out fileHandle);
                uint dataSize = StormLib.SFileGetFileSize(fileHandle, ref pdwFileSizeHigh);
                byte[] arr = new byte[dataSize];
                uint bytesRead = 0;
                StormLib.SFileReadFile(fileHandle, arr, dataSize, out bytesRead, IntPtr.Zero);
                StormLib.SFileCloseFile(fileHandle);
                StormLib.SFileCloseArchive(mpqHandle);
                return arr;
            }
            else
            {
                //uint error = StormLib.GetLastError();
                //int error2 = Marshal.GetLastWin32Error();
                cannotOpen.Add(path);
                return null;
            }
        }
        //Get size of scenario.chk. Визначення розміру файлу
        uint GetScenarioSize(string path)
        {
            IntPtr mpqHandle = new IntPtr();
            if (StormLib.SFileOpenArchive(path, 0, 256, out mpqHandle)) //256 - read only
            {
                uint pdwFileSizeHigh = 0;
                IntPtr fileHandle = new IntPtr();
                StormLib.SFileOpenFileEx(mpqHandle, scenario, 0, out fileHandle);
                uint dataSize = StormLib.SFileGetFileSize(fileHandle, ref pdwFileSizeHigh);
                StormLib.SFileCloseFile(fileHandle);
                StormLib.SFileCloseArchive(mpqHandle);
                return dataSize;
            }
            else return 0;
        }
        //Get name & description from scenario.chk data. Визначення імені та опису карти
        public List<byte[]> GetMapStrings(byte[] arr)
        {
            int MapNameIndex = 0; //map name index
            int MapDescIndex = 0; //map description index
            int MapNameOffset = 0; //map name offset
            int MapDescOffset = 0; //map description offset
            List<byte[]> result = new List<byte[]>();
            //string[] MapStrings = new string[2];
            string text = Encoding.ASCII.GetString(arr);
            List<int> indexesList = new List<int>();
            //scoring is used to determine the real section in protected maps
            List<int> scoreList = new List<int>();
            int score = 0;
            for (int index = 0; ; index += 4)
            {
                index = text.IndexOf("SPRP", index);
                if (index == -1) break;
                if (BitConverter.ToInt32(arr, index + 4) == 4) //size of block
                {
                    score++;
                    MapNameIndex = BitConverter.ToUInt16(arr, index + 8); //map name
                    MapDescIndex = BitConverter.ToUInt16(arr, index + 10); //map description
                    if (MapNameIndex == 1) score++; //index of map name
                    if (MapDescIndex == 2) score++; //index of map description
                    if (MapNameIndex < 1025 && MapNameIndex > 0) score++; //index of map name
                    if (MapDescIndex < 1025 && MapDescIndex > 0) score++; //index of map description
                }
                scoreList.Add(score);
                score = 0;
                indexesList.Add(index);
            }
            int idx = indexesList[scoreList.IndexOf(scoreList.Max())];
            MapNameIndex = BitConverter.ToUInt16(arr, idx + 8); //map name
            MapDescIndex = BitConverter.ToUInt16(arr, idx + 10); //map description

            indexesList.Clear();
            scoreList.Clear();

            for (int index = 0; ; index += 4)
            {
                index = text.IndexOf("STR ", index);
                if (index == -1) break;

                //STR size value is in bounds of a flie
                if (index + 7 < arr.Length)
                {
                    int STRBlockSize = BitConverter.ToInt32(arr, index + 4);
                    //STR block is in bounds of a flie
                    //had to do "<=" for a few maps where STR block ends where file ends. Must usually be "<"
                    if (STRBlockSize + index + 8 <= arr.Length)
                    {
                        //map & description offset values are in bounds of a file
                        if (index + 10 + (MapNameIndex - 1) * 2 < arr.Length && index + 10 + (MapDescIndex - 1) * 2 < arr.Length)
                        {
                            MapNameOffset = BitConverter.ToUInt16(arr, index + 10 + (MapNameIndex - 1) * 2);
                            MapDescOffset = BitConverter.ToUInt16(arr, index + 10 + (MapDescIndex - 1) * 2);
                            //map offset is in bounds of a string section
                            //if (MapNameOffset < STRBlockSize && MapDescOffset < STRBlockSize)
                            if (MapNameOffset < STRBlockSize)
                            {
                                score = 0;
                                for (int i = 0; ; i++)
                                {
                                    if (arr[index + 8 + MapNameOffset + i] == 0 ||
                                        BitConverter.ToUInt16(arr, index + 8 + MapNameOffset + i) == 0xFFFF ||
                                        index + 8 + MapNameOffset + i == arr.Length) break;
                                    score++;
                                }
                                if (score > 70) score = 0;
                            }
                        }
                    }
                }
                scoreList.Add(score);
                score = 0;
                indexesList.Add(index);
            }
            idx = indexesList[scoreList.IndexOf(scoreList.Max())];
            MapNameOffset = idx + 8 + BitConverter.ToUInt16(arr, idx + 10 + (MapNameIndex - 1) * 2); //map name
            MapDescOffset = idx + 8 + BitConverter.ToUInt16(arr, idx + 10 + (MapDescIndex - 1) * 2); //map description

            //get map name
            if (MapNameIndex > 0)
            {
                List<byte> temp = new List<byte>();
                for (int i = 0; ; i++)
                {
                    byte data = arr[MapNameOffset + i];
                    if (data == 0) break;
                    temp.Add(data);
                }
                string tmpTxt = Encoding.ASCII.GetString(temp.ToArray());
                Regex consistsOfSpaces = new Regex("^ *$");
                Match match = consistsOfSpaces.Match(tmpTxt);
                if (temp.Count == 0) result.Add(Encoding.ASCII.GetBytes("Noname")); //zero bytes
                else if (match.Success) result.Add(Encoding.ASCII.GetBytes("Noname")); //consists of spaces
                else result.Add(temp.ToArray()); //seems ok
            }
            else result.Add(Encoding.ASCII.GetBytes("Noname"));

            //get map description
            if (MapDescIndex > 0)
            {
                List<byte> temp = new List<byte>();
                for (int i = 0; ; i++)
                {
                    byte data = arr[MapDescOffset + i];
                    if (data == 0) break;
                    temp.Add(data);
                }
                string tmpTxt = Encoding.ASCII.GetString(temp.ToArray());
                Regex consistsOfSpaces = new Regex("^ *$");
                Match match = consistsOfSpaces.Match(tmpTxt);
                if (temp.Count == 0) result.Add(Encoding.ASCII.GetBytes("No description")); //zero bytes
                else if (match.Success) result.Add(Encoding.ASCII.GetBytes("No description")); //consists of spaces
                else result.Add(temp.ToArray()); //seems ok
            }
            else result.Add(Encoding.ASCII.GetBytes("No description"));
            return result;
        }
        //Remove & replace some characters from map name and description. Видалення деяких символів з назв та описів карт
        public byte[] CleanName(byte[] name, TextType type)
        {
            //replace accents (diacritical marks) and beautified characters with regular letters. Видалення діакритики
            List<byte> byteBuilder = new List<byte>();
            foreach (byte symbol in name)
            {
                if (replacementDictionary.ContainsKey(symbol))
                {
                    List<byte> tempList = replacementDictionary[symbol];
                    foreach (byte b in tempList) byteBuilder.Add(b);
                }
                else byteBuilder.Add(symbol);
            }

            //delete illegal characters and garbage
            List<byte> cleanBytes = new List<byte>();
            List<byte> allFilteredChars = new List<byte>();
            //delete file system illegal chars
            if (type == TextType.map)
            {
                allFilteredChars.AddRange(systemIllegalChars);
            }
            allFilteredChars.AddRange(filteredChars);

            foreach (byte symbol in byteBuilder)
            {
                if (allFilteredChars.IndexOf(symbol) == -1)
                    cleanBytes.Add(symbol);
            }

            //clean up
            string tempName = Encoding.ASCII.GetString(cleanBytes.ToArray());
            tempName = Regex.Replace(tempName, "(.)\\1{4,}", "$1"); //repeating chars. Повторювані символи
            tempName = Regex.Replace(tempName, "^ *(.*?) *$", "$1"); //trailing spaces. Пробіли в кінці і на початку
            return Encoding.ASCII.GetBytes(tempName);
        }
        //Calculate MD5 hash. Хешування
        public static string getMd5Hash(byte[] input)
        {
            MD5CryptoServiceProvider md5Hasher = new MD5CryptoServiceProvider();
            byte[] data = md5Hasher.ComputeHash(input);
            StringBuilder sBuilder = new StringBuilder();
            for (int i = 0; i < data.Length; i++)
            {
                sBuilder.Append(data[i].ToString("x2"));
            }
            return sBuilder.ToString();
        }
        //Replace & remove some characters in texts. Заміна символів
        void InitializeReplacements()
        {
            replacementDictionary = new Dictionary<byte, List<byte>>();
            {
                replacementDictionary.Add(0x80, new List<byte> { 0x65 }); //EURO SIGN-e
                replacementDictionary.Add(0x83, new List<byte> { 0x66 }); //LATIN SMALL LETTER F WITH HOOK-f
                replacementDictionary.Add(0x86, new List<byte> { 0x74 }); //DAGGER-t
                replacementDictionary.Add(0x87, new List<byte> { 0x74 }); //DOUBLE DAGGER-t
                replacementDictionary.Add(0x8A, new List<byte> { 0x73 }); //LATIN CAPITAL LETTER S WITH CARON-s
                replacementDictionary.Add(0x8C, new List<byte> { 0x6F, 0x65 }); //LATIN CAPITAL LIGATURE OE - oe
                replacementDictionary.Add(0x8E, new List<byte> { 0x7A }); //LATIN CAPITAL LETTER Z WITH CARON-z
                replacementDictionary.Add(0x99, new List<byte> { 0x74, 0x6D }); //TRADE MARK SIGN-tm
                replacementDictionary.Add(0x9A, new List<byte> { 0x73 }); //LATIN SMALL LETTER S WITH CARON-s
                replacementDictionary.Add(0x9C, new List<byte> { 0x6F, 0x65 }); //LATIN SMALL LIGATURE OE-oe
                replacementDictionary.Add(0x9E, new List<byte> { 0x7A }); //LATIN SMALL LETTER Z WITH CARON-z
                replacementDictionary.Add(0x9F, new List<byte> { 0x79 }); //LATIN CAPITAL LETTER Y WITH DIAERESIS-y
                replacementDictionary.Add(0xA1, new List<byte> { 0x69 }); //INVERTED EXCLAMATION MARK-i
                replacementDictionary.Add(0xA2, new List<byte> { 0x63 }); //CENT SIGN-c
                replacementDictionary.Add(0xA3, new List<byte> { 0x66 }); //POUND SIGN-f
                replacementDictionary.Add(0xA5, new List<byte> { 0x79 }); //YEN SIGN-y
                replacementDictionary.Add(0xA7, new List<byte> { 0x73 }); //SECTION SIGN-s
                replacementDictionary.Add(0xA9, new List<byte> { 0x63 }); //COPYRIGHT SIGN-c
                replacementDictionary.Add(0xAA, new List<byte> { 0x61 }); //FEMININE ORDINAL INDICATOR-a
                replacementDictionary.Add(0xAE, new List<byte> { 0x72 }); //REGISTERED SIGN-r
                replacementDictionary.Add(0xB0, new List<byte> { 0x6F }); //DEGREE SIGN-o
                replacementDictionary.Add(0xB1, new List<byte> { 0x74 }); //PLUS-MINUS SIGN-t
                replacementDictionary.Add(0xB2, new List<byte> { 0x32 }); //SUPERSCRIPT TWO-2
                replacementDictionary.Add(0xB3, new List<byte> { 0x33 }); //SUPERSCRIPT THREE-3
                replacementDictionary.Add(0xB5, new List<byte> { 0x6D }); //MICRO SIGN-m
                replacementDictionary.Add(0xB9, new List<byte> { 0x31 }); //SUPERSCRIPT ONE-1
                replacementDictionary.Add(0xBA, new List<byte> { 0x6F }); //MASCULINE ORDINAL INDICATOR-o
                replacementDictionary.Add(0xC0, new List<byte> { 0x41 }); //A
                replacementDictionary.Add(0xC1, new List<byte> { 0x41 }); //A
                replacementDictionary.Add(0xC2, new List<byte> { 0x41 }); //A
                replacementDictionary.Add(0xC3, new List<byte> { 0x41 }); //A
                replacementDictionary.Add(0xC4, new List<byte> { 0x41 }); //A
                replacementDictionary.Add(0xC5, new List<byte> { 0x41 }); //A
                replacementDictionary.Add(0xC6, new List<byte> { 0x61, 0x65 }); //ae
                replacementDictionary.Add(0xC7, new List<byte> { 0x63 }); //C
                replacementDictionary.Add(0xC8, new List<byte> { 0x45 }); //E
                replacementDictionary.Add(0xC9, new List<byte> { 0x45 }); //E
                replacementDictionary.Add(0xCA, new List<byte> { 0x45 }); //E
                replacementDictionary.Add(0xCB, new List<byte> { 0x45 }); //E
                replacementDictionary.Add(0xCC, new List<byte> { 0x49 }); //I
                replacementDictionary.Add(0xCD, new List<byte> { 0x49 }); //I
                replacementDictionary.Add(0xCE, new List<byte> { 0x49 }); //I
                replacementDictionary.Add(0xCF, new List<byte> { 0x49 }); //I
                replacementDictionary.Add(0xD0, new List<byte> { 0x44 }); //D
                replacementDictionary.Add(0xD1, new List<byte> { 0x4E }); //N
                replacementDictionary.Add(0xD2, new List<byte> { 0x4F }); //O
                replacementDictionary.Add(0xD3, new List<byte> { 0x4F }); //O
                replacementDictionary.Add(0xD4, new List<byte> { 0x4F }); //O
                replacementDictionary.Add(0xD5, new List<byte> { 0x4F }); //O
                replacementDictionary.Add(0xD6, new List<byte> { 0x4F }); //O
                replacementDictionary.Add(0xD7, new List<byte> { 0x78 }); //MULTIPLICATION SIGN-x
                replacementDictionary.Add(0xD8, new List<byte> { 0x4F }); //O
                replacementDictionary.Add(0xD9, new List<byte> { 0x55 }); //U
                replacementDictionary.Add(0xDA, new List<byte> { 0x55 }); //U
                replacementDictionary.Add(0xDB, new List<byte> { 0x55 }); //U
                replacementDictionary.Add(0xDC, new List<byte> { 0x55 }); //U
                replacementDictionary.Add(0xDD, new List<byte> { 0x59 }); //Y
                replacementDictionary.Add(0xDE, new List<byte> { 0x70 }); //p
                replacementDictionary.Add(0xDF, new List<byte> { 0x42 }); //B
                replacementDictionary.Add(0xE0, new List<byte> { 0x61 }); //a
                replacementDictionary.Add(0xE1, new List<byte> { 0x61 }); //a
                replacementDictionary.Add(0xE2, new List<byte> { 0x61 }); //a
                replacementDictionary.Add(0xE3, new List<byte> { 0x61 }); //a
                replacementDictionary.Add(0xE4, new List<byte> { 0x61 }); //a
                replacementDictionary.Add(0xE5, new List<byte> { 0x61 }); //a
                replacementDictionary.Add(0xE6, new List<byte> { 0x61, 0x65 }); //ae
                replacementDictionary.Add(0xE7, new List<byte> { 0x63 }); //c
                replacementDictionary.Add(0xE8, new List<byte> { 0x65 }); //e
                replacementDictionary.Add(0xE9, new List<byte> { 0x65 }); //e
                replacementDictionary.Add(0xEA, new List<byte> { 0x65 }); //e
                replacementDictionary.Add(0xEB, new List<byte> { 0x65 }); //e
                replacementDictionary.Add(0xEC, new List<byte> { 0x69 }); //i
                replacementDictionary.Add(0xED, new List<byte> { 0x69 }); //i
                replacementDictionary.Add(0xEE, new List<byte> { 0x69 }); //i
                replacementDictionary.Add(0xEF, new List<byte> { 0x69 }); //i
                replacementDictionary.Add(0xF0, new List<byte> { 0x6F }); //o
                replacementDictionary.Add(0xF1, new List<byte> { 0x6E }); //n
                replacementDictionary.Add(0xF2, new List<byte> { 0x6F }); //o
                replacementDictionary.Add(0xF3, new List<byte> { 0x6F }); //o
                replacementDictionary.Add(0xF4, new List<byte> { 0x6F }); //o
                replacementDictionary.Add(0xF5, new List<byte> { 0x6F }); //o
                replacementDictionary.Add(0xF6, new List<byte> { 0x6F }); //o
                replacementDictionary.Add(0xF8, new List<byte> { 0x6F }); //o
                replacementDictionary.Add(0xF9, new List<byte> { 0x75 }); //u
                replacementDictionary.Add(0xFA, new List<byte> { 0x75 }); //u
                replacementDictionary.Add(0xFB, new List<byte> { 0x75 }); //u
                replacementDictionary.Add(0xFC, new List<byte> { 0x75 }); //u
                replacementDictionary.Add(0xFD, new List<byte> { 0x79 }); //y
                replacementDictionary.Add(0xFE, new List<byte> { 0x70 }); //p
                replacementDictionary.Add(0xFF, new List<byte> { 0x79 }); //y
            }

            filteredChars = new List<byte>();
            //chars with codes 1-31
            for (char c = '\u0001'; c <= '\u001F'; c++)
                filteredChars.Add((byte)c);
            //remove newline from banned chars (for map description)
            filteredChars.Remove(0x0A);
            filteredChars.Remove(0x0D);

            //chars with codes 127-256
            for (char c = '\u007F'; c <= '\u00FF'; c++)
                filteredChars.Add((byte)c);

            systemIllegalChars = new List<byte>();
            //file system illegal
            {
                systemIllegalChars.Add(Encoding.ASCII.GetBytes("<")[0]);
                systemIllegalChars.Add(Encoding.ASCII.GetBytes(">")[0]);
                systemIllegalChars.Add(Encoding.ASCII.GetBytes(":")[0]);
                systemIllegalChars.Add(Encoding.ASCII.GetBytes("\"")[0]);
                systemIllegalChars.Add(Encoding.ASCII.GetBytes("/")[0]);
                systemIllegalChars.Add(Encoding.ASCII.GetBytes("\\")[0]);
                systemIllegalChars.Add(Encoding.ASCII.GetBytes("|")[0]);
                systemIllegalChars.Add(Encoding.ASCII.GetBytes("?")[0]);
                systemIllegalChars.Add(Encoding.ASCII.GetBytes("*")[0]);
                systemIllegalChars.Add(0x0A);
                systemIllegalChars.Add(0x0D);
            }

            //random sequence, used in uniqe ID
            randomSequence = new List<string>();
            //numbers (10)
            for (char c = '\u0030'; c <= '\u0039'; c++)
                randomSequence.Add(c.ToString());
            //capital letters (26)
            for (char c = '\u0041'; c <= '\u005A'; c++)
                randomSequence.Add(c.ToString());
        }
        //About menu. Меню "Про програму"
        public void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            About about = new About();
            about.Show();
        }
        //Контекстне меню
        void InitializeContextMenu()
        {
            mapMenu = new ContextMenuStrip();
            moveOption = new ToolStripMenuItem();
            renameOption = new ToolStripMenuItem();
            copyOption = new ToolStripMenuItem();
            deleteOption = new ToolStripMenuItem();
            removeDupesOption = new ToolStripMenuItem();
            saveFullsizeJPEG = new ToolStripMenuItem();
            moveOption.Name = "Move";
            moveOption.Text = "&Move";
            copyOption.Name = "Copy";
            copyOption.Text = "&Copy";
            renameOption.Name = "Rename";
            renameOption.Text = "&Rename";
            deleteOption.Name = "Delete";
            deleteOption.Text = "&Delete";
            removeDupesOption.Name = "Removedupes";
            removeDupesOption.Text = "Remove D&uplicates";
            saveFullsizeJPEG.Name = "Save";
            saveFullsizeJPEG.Text = "&Save full size JPEG";
            mapMenu.Items.AddRange(new ToolStripItem[] { moveOption, copyOption, renameOption, deleteOption, removeDupesOption, saveFullsizeJPEG });

            //mapMenu.Items.Add("Move").Name = "Move";
            //mapMenu.Items.Add("Rename").Name = "Rename";


            mapMenu.ItemClicked += new ToolStripItemClickedEventHandler(ContextMenu_Click);
            grid.ContextMenuStrip = mapMenu;
        }
        private void ContextMenu_Click(object sender, ToolStripItemClickedEventArgs e)
        {
            mapMenu.Hide();
            ToolStripItem item = e.ClickedItem;
            if (item.Text == "&Delete")
            {
                int deleted = 0;
                foreach (DataGridViewRow row in grid.SelectedRows)
                {
                    string file = row.Cells[0].Value.ToString();
                    if (File.Exists(file))
                    {
                        FileAttributes fa = File.GetAttributes(file);
                        if ((fa & FileAttributes.ReadOnly) != 0)
                        {
                            // Use the exclusive-or operator (^) to toggle the ReadOnly flag
                            fa ^= FileAttributes.ReadOnly;
                            File.SetAttributes(file, fa);
                        }
                        File.Delete(file);

                        DeleteJpeg(file);
                        deleted++;
                    }
                    foreach (Map map in maps)
                    {
                        if (map.orgFilename == file)
                        {
                            maps.Remove(map);
                            break;
                        }
                    }
                    grid.Rows.RemoveAt(row.Index);
                }
                if(maps.Count == 0) MapImage.BackgroundImage = null;
                SerializeToXML(mapsDataBase);
                infoBox.AppendText("Deleted " + deleted + " file(s).");
                infoBox.AppendText(Environment.NewLine);
            }
            if (item.Text == "&Copy")
            {
                int copied = 0;
                using (var fbd = new FolderBrowserDialog())
                {

                    fbd.SelectedPath = startingFolder;
                    DialogResult result = fbd.ShowDialog();

                    if (result == DialogResult.OK && !string.IsNullOrWhiteSpace(fbd.SelectedPath))
                    {
                        foreach (DataGridViewRow row in grid.SelectedRows)
                        {
                            string file = row.Cells[0].Value.ToString();
                            if (File.Exists(file))
                            {
                                Regex extractFilename = new Regex(@"(.*?)([^\\]+)(\.sc.)", RegexOptions.IgnoreCase);
                                Match match = extractFilename.Match(file);
                                string destination = fbd.SelectedPath + "\\" + match.Groups[2].Value + match.Groups[3].Value;
                                if (!File.Exists(destination))
                                {
                                    File.Copy(file, destination);
                                    CopyJpeg(file, destination);
                                    //string filename = ((DataRowView)row.DataBoundItem).Row.ItemArray[0].ToString();
                                    string name = ((DataRowView)row.DataBoundItem).Row.ItemArray[1].ToString();
                                    string description = ((DataRowView)row.DataBoundItem).Row.ItemArray[2].ToString();
                                    string hash = ((DataRowView)row.DataBoundItem).Row.ItemArray[3].ToString();
                                    string tilehash = ((DataRowView)row.DataBoundItem).Row.ItemArray[4].ToString();
                                    ((DataRowView)row.DataBoundItem).DataView.Table.Rows.Add(destination, name, description, hash, tilehash);
                                    maps.Add(new Map(destination, name, description, hash, tilehash));
                                    copied++;
                                }
                                else
                                {
                                    infoBox.AppendText("File " + destination + " already exists.");
                                    infoBox.AppendText(Environment.NewLine);
                                }
                            }
                        }
                        SerializeToXML(mapsDataBase);
                        infoBox.AppendText("Copied " + copied + " file(s).");
                        infoBox.AppendText(Environment.NewLine);
                    }
                }
            }
            if (item.Text == "&Move")
            {
                int moved = 0;
                using (var fbd = new FolderBrowserDialog())
                {

                    fbd.SelectedPath = startingFolder;
                    DialogResult result = fbd.ShowDialog();

                    if (result == DialogResult.OK && !string.IsNullOrWhiteSpace(fbd.SelectedPath))
                    {
                        foreach (DataGridViewRow row in grid.SelectedRows)
                        {
                            string file = row.Cells[0].Value.ToString();
                            if (File.Exists(file))
                            {
                                Regex extractFilename = new Regex(@"(.*?)([^\\]+)(\.sc.)", RegexOptions.IgnoreCase);
                                Match match = extractFilename.Match(file);
                                string destination = fbd.SelectedPath + "\\" + match.Groups[2].Value + match.Groups[3].Value;
                                if (!File.Exists(destination))
                                {
                                    File.Move(file, destination);
                                    MoveJpeg(file, destination);
                                    foreach (Map map in maps)
                                    {
                                        if (map.orgFilename == file)
                                        {
                                            map.orgFilename = destination;
                                            break;
                                        }
                                    }
                                    row.Cells[0].Value = destination;
                                    moved++;
                                }
                                else
                                {
                                    infoBox.AppendText("File " + destination + " already exists.");
                                    infoBox.AppendText(Environment.NewLine);
                                }
                            }
                        }
                        SerializeToXML(mapsDataBase);
                        infoBox.AppendText("Moved " + moved + " file(s).");
                        infoBox.AppendText(Environment.NewLine);
                    }
                }
            }
            if (item.Text == "&Rename")
            {
                int renamed = 0;
                foreach (DataGridViewRow row in grid.SelectedRows)
                {
                    string orgFilename = row.Cells[0].Value.ToString();
                    string name = row.Cells[1].Value.ToString();
                    string newFileName = RenameFile(orgFilename, name);
                    MoveJpeg(orgFilename, newFileName);
                    row.Cells[0].Value = newFileName;
                    renamed++;
                    foreach (Map map in maps)
                    {
                        if (map.orgFilename == orgFilename)
                        {
                            map.orgFilename = newFileName;
                            break;
                        }
                    }
                }
                SerializeToXML(mapsDataBase);
                infoBox.AppendText("Renamed " + renamed + " file(s).");
                infoBox.AppendText(Environment.NewLine);
            }
            if (item.Text == "Remove D&uplicates")
            {
                int removed = 0;
                List<string> hashTable = new List<string>();
                List<string> filesToDelete = new List<string>();
                List<Map> tempMaps = new List<Map>();
                foreach (DataGridViewRow row in grid.SelectedRows)
                {
                    string orgFilename = row.Cells[0].Value.ToString();
                    string hash = ((DataRowView)row.DataBoundItem).Row.ItemArray[3].ToString();
                    if (!hashTable.Contains(hash)) hashTable.Add(hash);
                    else
                    {
                        filesToDelete.Add(orgFilename);
                        grid.Rows.RemoveAt(row.Index);
                    }
                }
                foreach (Map map in maps)
                {
                    if (!filesToDelete.Contains(map.orgFilename))
                    {
                        tempMaps.Add(map);
                    }
                }
                maps = tempMaps;
                foreach (string file in filesToDelete)
                {
                    if (File.Exists(file))
                    {
                        FileAttributes fa = File.GetAttributes(file);
                        if ((fa & FileAttributes.ReadOnly) != 0)
                        {
                            // Use the exclusive-or operator (^) to toggle the ReadOnly flag
                            fa ^= FileAttributes.ReadOnly;
                            File.SetAttributes(file, fa);
                        }
                        File.Delete(file);
                    }
                    DeleteJpeg(file);
                    removed++;
                }
                SerializeToXML(mapsDataBase);
                infoBox.AppendText("Removed " + removed + " duplicate(s).");
                infoBox.AppendText(Environment.NewLine);
            }

            if (item.Text == "&Save full size JPEG")
            {
                infoBox.AppendText("Working ...");
                infoBox.AppendText(Environment.NewLine);
                bigJPEGs = new List<string>();
                foreach (DataGridViewRow row in grid.SelectedRows)
                {
                    string orgFilename = row.Cells[0].Value.ToString();
                    bigJPEGs.Add(orgFilename);
                }
                if (!bgWorkerBigJPEGs.IsBusy) bgWorkerBigJPEGs.RunWorkerAsync();

            }
        }
        //Filter fields. Поля фільтрації
        private void FilenameTextBox_TextChanged(object sender, EventArgs e)
        {
            try
            {
                bindingSource.Filter = string.Format("Filename like '*{0}*'", EscapeSqlLike(FilenameFilter.Text));
                FilenameFilter.BackColor = SystemColors.Window;
            }
            catch (InvalidExpressionException)
            {
                FilenameFilter.BackColor = Color.Pink;
            }
        }
        private void MapnameTextBox_TextChanged(object sender, EventArgs e)
        {
            try
            {
                bindingSource.Filter = string.Format("Mapname like '*{0}*'", EscapeSqlLike(MapnameFilter.Text));
                MapnameFilter.BackColor = SystemColors.Window;
            }
            catch (InvalidExpressionException)
            {
                MapnameFilter.BackColor = Color.Pink;
            }
        }
        //Replace some symbols, so that filtering works without errors. Заміна введених символів у вікнах фільтрації
        static string EscapeSqlLike(string s_)
        {
            StringBuilder s = new StringBuilder(s_);
            for (int i = 0; i < s.Length; i++)
            {
                if (s[i] == '\'')
                {
                    s.Insert(i++, '\'');
                    continue;
                }
                if (s[i] == '[' || s[i] == '*' || s[i] == '?')
                {
                    s.Insert(i++, '[');
                    s.Insert(++i, ']');
                }
            }
            return s.ToString();
        }
        //Серіалізація
        public void SerializeToXML(string filename)
        {
            XmlWriterSettings ws = new XmlWriterSettings();
            ws.NewLineHandling = NewLineHandling.Entitize;
            XmlSerializer s = new XmlSerializer(typeof(List<Map>));
            using (XmlWriter wr = XmlWriter.Create(filename, ws))
            {
                s.Serialize(wr, maps);
            }
        }
        List<Map> DeserializeFromXML(string filename)
        {
            List<Map> mapsFromFile = new List<Map>();
            XmlSerializer s = new XmlSerializer(typeof(List<Map>));
            FileStream fs = new FileStream(filename, FileMode.Open);
            try
            {
                mapsFromFile = (List<Map>)s.Deserialize(fs);
            }
            catch
            {
                infoBox.AppendText("Loading previous session failed.");
                infoBox.AppendText(Environment.NewLine);
            }

            fs.Close();
            return mapsFromFile;
        }
        //Зміна розміру зображення
        private Bitmap ResizeImage(Bitmap bm, float scale)
        {
            int width = (int)(bm.Width * scale);
            int height = (int)(bm.Height * scale);
            Bitmap result_bm = new Bitmap(width, height);
            using (Graphics gr = Graphics.FromImage(result_bm))
            {
                PointF[] dest_points =
                {
                    new PointF(0, 0),
                    new PointF(width, 0),
                    new PointF(0, height),
                };
                RectangleF src_rect = new RectangleF(
                    0, 0, bm.Width, bm.Height);
                //Draws the specified portion of the specified 
                //Image at the specified location and with the specified size.
                //GraphicsUnit.Pixel specifies the units of measure used by the srcRect parameter
                gr.DrawImage(bm, dest_points, src_rect, GraphicsUnit.Pixel);
            }
            return result_bm;
        }
        //Load graphics. Завантаження графіки
        void ReadTileSets()
        {
            ashworld_cv5 = File.ReadAllBytes("Tileset\\ashworld.cv5");
            ashworld_vx4 = File.ReadAllBytes("Tileset\\ashworld.vx4");
            ashworld_vr4 = File.ReadAllBytes("Tileset\\ashworld.vr4");
            ashworld_wpe = File.ReadAllBytes("Tileset\\ashworld.wpe");
            badlands_cv5 = File.ReadAllBytes("Tileset\\badlands.cv5");
            badlands_vx4 = File.ReadAllBytes("Tileset\\badlands.vx4");
            badlands_vr4 = File.ReadAllBytes("Tileset\\badlands.vr4");
            badlands_wpe = File.ReadAllBytes("Tileset\\badlands.wpe");
            Desert_cv5 = File.ReadAllBytes("Tileset\\Desert.cv5");
            Desert_vx4 = File.ReadAllBytes("Tileset\\Desert.vx4");
            Desert_vr4 = File.ReadAllBytes("Tileset\\Desert.vr4");
            Desert_wpe = File.ReadAllBytes("Tileset\\Desert.wpe");
            Ice_cv5 = File.ReadAllBytes("Tileset\\Ice.cv5");
            Ice_vx4 = File.ReadAllBytes("Tileset\\Ice.vx4");
            Ice_vr4 = File.ReadAllBytes("Tileset\\Ice.vr4");
            Ice_wpe = File.ReadAllBytes("Tileset\\Ice.wpe");
            install_cv5 = File.ReadAllBytes("Tileset\\install.cv5");
            install_vx4 = File.ReadAllBytes("Tileset\\install.vx4");
            install_vr4 = File.ReadAllBytes("Tileset\\install.vr4");
            install_wpe = File.ReadAllBytes("Tileset\\install.wpe");
            jungle_cv5 = File.ReadAllBytes("Tileset\\jungle.cv5");
            jungle_vx4 = File.ReadAllBytes("Tileset\\jungle.vx4");
            jungle_vr4 = File.ReadAllBytes("Tileset\\jungle.vr4");
            jungle_wpe = File.ReadAllBytes("Tileset\\jungle.wpe");
            platform_cv5 = File.ReadAllBytes("Tileset\\platform.cv5");
            platform_vx4 = File.ReadAllBytes("Tileset\\platform.vx4");
            platform_vr4 = File.ReadAllBytes("Tileset\\platform.vr4");
            platform_wpe = File.ReadAllBytes("Tileset\\platform.wpe");
            Twilight_cv5 = File.ReadAllBytes("Tileset\\Twilight.cv5");
            Twilight_vx4 = File.ReadAllBytes("Tileset\\Twilight.vx4");
            Twilight_vr4 = File.ReadAllBytes("Tileset\\Twilight.vr4");
            Twilight_wpe = File.ReadAllBytes("Tileset\\Twilight.wpe");
            tilesetDic = new Dictionary<int, List<byte[]>>();
            tilesetDic.Add(0, new List<byte[]> { badlands_cv5, badlands_vx4, badlands_vr4, badlands_wpe });
            tilesetDic.Add(1, new List<byte[]> { platform_cv5, platform_vx4, platform_vr4, platform_wpe });
            tilesetDic.Add(2, new List<byte[]> { install_cv5, install_vx4, install_vr4, install_wpe });
            tilesetDic.Add(3, new List<byte[]> { ashworld_cv5, ashworld_vx4, ashworld_vr4, ashworld_wpe });
            tilesetDic.Add(4, new List<byte[]> { jungle_cv5, jungle_vx4, jungle_vr4, jungle_wpe });
            tilesetDic.Add(5, new List<byte[]> { Desert_cv5, Desert_vx4, Desert_vr4, Desert_wpe });
            tilesetDic.Add(6, new List<byte[]> { Ice_cv5, Ice_vx4, Ice_vr4, Ice_wpe });
            tilesetDic.Add(7, new List<byte[]> { Twilight_cv5, Twilight_vx4, Twilight_vr4, Twilight_wpe });
        }
        //Saves jpeg with image of map terrain, returns terrain hash. Зберігає малюнок карти, повертає хеш ландшафту
        string ProcessImage(byte[] arr, string file, ImageSize imageSize)
        {
            Regex makeitjpeg = new Regex(@"(.*?)([^\\]+)(\.sc.)", RegexOptions.IgnoreCase);
            Match match = makeitjpeg.Match(file);
            string jpegfile = match.Groups[1].Value + match.Groups[2].Value + ".jpg";
            string bigjpegfile = match.Groups[1].Value + match.Groups[2].Value + "_big" + ".jpg";
            string pngfile = match.Groups[1].Value + match.Groups[2].Value + ".png";
            int ERAOffset = 0; //tileset
            int DIMOffset = 0; //dimensions
            //int MTXMOffset = 0; //map name offset
            string text = Encoding.ASCII.GetString(arr);
            List<int> indexesList = new List<int>();
            List<int> scoreList = new List<int>();
            int score = 0;
            //checks for protected maps ERA (tileset)
            for (int index = 0; ; index += 4)
            {
                index = text.IndexOf("ERA ", index);
                if (index == -1) break;
                if (BitConverter.ToUInt32(arr, index + 4) == 2) score++; //size of block should be 2
                scoreList.Add(score);
                score = 0;
                indexesList.Add(index);
            }
            ERAOffset = indexesList[scoreList.IndexOf(scoreList.Max())]; //choose index with hightest score
            indexesList.Clear();
            scoreList.Clear();
            //mask 0111, bits after the third place (anything after the value "7") are removed
            int tileset = (BitConverter.ToUInt16(arr, ERAOffset + 8)) & 7;

            List<byte[]> tilesetdata = tilesetDic[tileset]; //cv5, vx4, vr4, wpe

            //checks for protected maps DIM (map dimensions)
            for (int index = 0; ; index += 4)
            {
                index = text.IndexOf("DIM ", index);
                if (index == -1) break;
                if (BitConverter.ToUInt32(arr, index + 4) == 4) //size of block should be 4
                {
                    score++;
                    uint size_x = BitConverter.ToUInt16(arr, index + 8);
                    uint size_y = BitConverter.ToUInt16(arr, index + 10);
                    if (size_x > 0 && size_y > 0) score++;
                    if ((size_x == 64 || size_x == 96 || size_x == 128 || size_x == 192 || size_x == 256) &&
                        (size_y == 64 || size_y == 96 || size_y == 128 || size_y == 192 || size_y == 256)) score++;
                    if (size_x <= 256 && size_y <= 256) score++;
                }
                scoreList.Add(score);
                score = 0;
                indexesList.Add(index);
            }
            DIMOffset = indexesList[scoreList.IndexOf(scoreList.Max())];
            int width = BitConverter.ToUInt16(arr, DIMOffset + 8); //width
            int height = BitConverter.ToUInt16(arr, DIMOffset + 10); //height
            indexesList.Clear();
            scoreList.Clear();

            //checks for protected maps MTXM (graphics)
            //int MTXMexpectedSize = width * height * 2;
            Dictionary<int, int> mtxmdic = new Dictionary<int, int>();
            byte[] mtxmarr = new byte[width * height * 2];
            int mtxmarrpos = 0;
            for (int index = 0; ; index += 4)
            {
                index = text.IndexOf("MTXM", index);
                if (index == -1) break;
                if (BitConverter.ToUInt32(arr, index + 4) < index + 8 + arr.Length)
                {
                    mtxmdic.Add(index, BitConverter.ToInt32(arr, index + 4)); //key = index, value = size
                }
            }
            if (mtxmdic.Count > 1)
            {
                for (int i = mtxmdic.Count - 1; i >= 0; i--)
                {
                    if (i < mtxmdic.Count - 1)
                    {
                        for (int j = 0; j < mtxmdic.ElementAt(i).Value - mtxmdic.ElementAt(i + 1).Value; j++)
                        {
                            mtxmarr[mtxmarrpos] = arr[mtxmdic.ElementAt(i).Key + 8 + j + mtxmdic.ElementAt(i + 1).Value];
                            mtxmarrpos++;
                            if (mtxmarrpos == mtxmarr.Length) break;
                        }
                    }
                    else
                    {
                        for (int j = 0; j < mtxmdic.ElementAt(i).Value; j++)
                        {
                            mtxmarr[mtxmarrpos] = arr[mtxmdic.ElementAt(i).Key + 8 + j];
                            mtxmarrpos++;
                            if (mtxmarrpos == mtxmarr.Length) break;
                        }
                    }
                    if (mtxmarrpos == mtxmarr.Length) break;
                }
            }
            else
            {
                for (int i = 0; i < mtxmdic.ElementAt(0).Value; i++)
                {
                    mtxmarr[mtxmarrpos] = arr[mtxmdic.ElementAt(0).Key + 8 + i];
                    mtxmarrpos++;
                    if (mtxmarrpos == mtxmarr.Length) break;
                }
            }
            arr = null;
            string tilehash = getMd5Hash(mtxmarr);
            if ((imageSize == ImageSize.thumbnail && !File.Exists(jpegfile) && ThumbMenu.Checked) || (imageSize == ImageSize.fullsize && !File.Exists(bigjpegfile)))
            {
                //Construct image
                byte[] pixels = new byte[width * 32 * height * 32 * 3];
                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        // FFFF (4+4+4+4 bits)
                        // +++- (pluses are group, minus is index)
                        //Need to shift group 4 bits right. Need to AND index - last 4 bits (make mask 1111 = F)
                        //*2 because we move by 2 bytes
                        int group = (BitConverter.ToUInt16(mtxmarr, y * 2 * width + x * 2)) >> 4;
                        int index = (BitConverter.ToUInt16(mtxmarr, y * 2 * width + x * 2)) & 0xF;
                        int cv5position = 52 * group + 20 + index * 2;
                        if (cv5position >= tilesetdata[0].Length)
                        {
                            group = 0;
                            index = 0;
                        }
                        //get megatile and 4 * 4 minitiles inside
                        //read cv5. 20 bytes are garbage at start
                        //index * 2 because we step by bytes and each index is 2 bytes. Indexes are 16 x 16 bits
                        int megatile = BitConverter.ToUInt16(tilesetdata[0], 52 * group + 20 + index * 2);
                        for (int suby = 0; suby < 4; suby++)
                        {
                            for (int subx = 0; subx < 4; subx++)
                            {
                                //Find and render the image, vx4
                                //image for the top 15 bits
                                //4x4 minitiles each has 8x8 pixels
                                int minitileindex = (BitConverter.ToUInt16(tilesetdata[1], 32 * megatile + (suby * 4 + subx) * 2)) >> 1;
                                //lower bit for reversed drawing
                                bool flipped = ((BitConverter.ToUInt16(tilesetdata[1], 32 * megatile + (suby * 4 + subx) * 2)) & 1) == 1 ? true : false;
                                int draw_offsetx = x * 32 + subx * 8;
                                int draw_offsety = y * 32 + suby * 8;
                                //render 8 * 8 of minitile
                                for (int j = 0; j < 8; j++)
                                {
                                    for (int i = 0; i < 8; i++)
                                    {
                                        int drawx = draw_offsetx + (flipped == true ? 7 - i : i);
                                        int drawy = draw_offsety + j;
                                        uint vr4data = tilesetdata[2][64 * minitileindex + j * 8 + i];
                                        byte r = tilesetdata[3][vr4data * 4];
                                        byte g = tilesetdata[3][vr4data * 4 + 1];
                                        byte b = tilesetdata[3][vr4data * 4 + 2];
                                        int p = (drawy * width * 32 + drawx) * 3;
                                        pixels[p] = b;
                                        pixels[p + 1] = g;
                                        pixels[p + 2] = r;
                                    }
                                }
                            }
                        }
                    }
                }
                //each megatile is 32x32px
                using (Bitmap bmp = new Bitmap(width * 32, height * 32, PixelFormat.Format24bppRgb))
                {
                    Rectangle rect = new Rectangle(0, 0, bmp.Width, bmp.Height);
                    BitmapData bmpData = bmp.LockBits(rect, ImageLockMode.ReadWrite, bmp.PixelFormat);
                    // Get the address of the first line.
                    IntPtr ptr = bmpData.Scan0;
                    //Stride - Gets or sets the stride width (also called scan width) of the Bitmap object.
                    int bytes = Math.Abs(bmpData.Stride) * bmp.Height;
                    Marshal.Copy(pixels, 0, ptr, bytes);
                    bmp.UnlockBits(bmpData);

                    float resizeScale;
                    if (bmp.Width > bmp.Height) resizeScale = 256F / bmp.Width;
                    else resizeScale = 256F / bmp.Height;

                    using (Bitmap smallbmp = ResizeImage(bmp, resizeScale))
                    {
                        //save jpeg
                        System.Drawing.Imaging.Encoder myEncoder = System.Drawing.Imaging.Encoder.Quality;
                        EncoderParameter ep90 = new EncoderParameter(myEncoder, 90L);
                        EncoderParameters eps90 = new EncoderParameters(1);
                        eps90.Param[0] = ep90;
                        EncoderParameter ep75 = new EncoderParameter(myEncoder, 75L);
                        EncoderParameters eps75 = new EncoderParameters(1);
                        eps75.Param[0] = ep75;
                        ImageCodecInfo myImageCodecInfo = GetEncoderInfo("image/jpeg");
                        if (imageSize == ImageSize.thumbnail && !File.Exists(jpegfile)) smallbmp.Save(jpegfile, myImageCodecInfo, eps90);
                        if (imageSize == ImageSize.fullsize && !File.Exists(bigjpegfile)) bmp.Save(bigjpegfile, myImageCodecInfo, eps75);

                        //save png
                        //smallbmp.Save(pngfile, ImageFormat.Png);
                    }

                    pixels = null;
                    GC.Collect();
                }
            }
            return tilehash;
        }
        //Used for saving JPEGs. Визначення інформації про кодек
        private static ImageCodecInfo GetEncoderInfo(String mimeType)
        {
            int j;
            ImageCodecInfo[] encoders;
            encoders = ImageCodecInfo.GetImageEncoders();
            for (j = 0; j < encoders.Length; ++j)
            {
                if (encoders[j].MimeType == mimeType)
                    return encoders[j];
            }
            return null;
        }
        //Update map image when changing selected item. Оновлення зображення карти при переміщенні по списку
        private void grid_CurrentCellChanged(object sender, EventArgs e)
        {
            if ((sender as DataGridView).CurrentCell != null)
            {
                string mapfilename = (sender as DataGridView).CurrentRow.Cells[0].Value.ToString();
                Regex makeitjpeg = new Regex(@"(.*?)([^\\]+)(\.sc.)", RegexOptions.IgnoreCase);
                Match match = makeitjpeg.Match(mapfilename);
                string jpegfile = match.Groups[1].Value + match.Groups[2].Value + ".jpg";

                if (File.Exists(jpegfile))
                {
                    FileStream file = new FileStream(jpegfile, FileMode.Open);
                    MapImage.BackgroundImage = Image.FromStream(file);
                    file.Close();
                }
                else MapImage.BackgroundImage = null;
            }
        }
        //Methods for manipulating JPEG files. Маніпуляція файлами зображень
        void DeleteJpeg(string path)
        {
            Regex makeitjpeg = new Regex(@"(.*?)([^\\]+)(\.sc.)", RegexOptions.IgnoreCase);
            Match match = makeitjpeg.Match(path);
            string jpegfile = match.Groups[1].Value + match.Groups[2].Value + ".jpg";
            string bigjpegfile = match.Groups[1].Value + match.Groups[2].Value + "_big" + ".jpg";
            if (File.Exists(jpegfile)) File.Delete(jpegfile);
            if (File.Exists(bigjpegfile)) File.Delete(bigjpegfile);
        }
        void MoveJpeg(string source, string destination)
        {
            Regex makeitjpeg = new Regex(@"(.*?)([^\\]+)(\.sc.)", RegexOptions.IgnoreCase);
            Match s_match = makeitjpeg.Match(source);
            string s_jpegfile = s_match.Groups[1].Value + s_match.Groups[2].Value + ".jpg";
            string s_bigjpegfile = s_match.Groups[1].Value + s_match.Groups[2].Value + "_big" + ".jpg";

            Match d_match = makeitjpeg.Match(destination);
            string d_jpegfile = d_match.Groups[1].Value + d_match.Groups[2].Value + ".jpg";
            string d_bigjpegfile = d_match.Groups[1].Value + d_match.Groups[2].Value + "_big" + ".jpg";

            if (!File.Exists(d_jpegfile) && File.Exists(s_jpegfile)) File.Move(s_jpegfile, d_jpegfile);
            if (!File.Exists(d_bigjpegfile) && File.Exists(s_bigjpegfile)) File.Move(s_bigjpegfile, d_bigjpegfile);
        }
        void CopyJpeg(string source, string destination)
        {
            Regex makeitjpeg = new Regex(@"(.*?)([^\\]+)(\.sc.)", RegexOptions.IgnoreCase);
            Match s_match = makeitjpeg.Match(source);
            string s_jpegfile = s_match.Groups[1].Value + s_match.Groups[2].Value + ".jpg";
            string s_bigjpegfile = s_match.Groups[1].Value + s_match.Groups[2].Value + "_big" + ".jpg";

            Match d_match = makeitjpeg.Match(destination);
            string d_jpegfile = d_match.Groups[1].Value + d_match.Groups[2].Value + ".jpg";
            string d_bigjpegfile = d_match.Groups[1].Value + d_match.Groups[2].Value + "_big" + ".jpg";

            if (!File.Exists(d_jpegfile) && File.Exists(s_jpegfile)) File.Copy(s_jpegfile, d_jpegfile);
            if (!File.Exists(d_bigjpegfile) && File.Exists(s_bigjpegfile)) File.Copy(s_bigjpegfile, d_bigjpegfile);
        }
        //Clear list from files that are no longer there. Очистка списку від записів, файли яких були видалені
        private void ClearMissingBtn_Click(object sender, EventArgs e)
        {
            if (maps.Count > 0)
            {
                List<Map> tempMaps = new List<Map>();
                foreach (Map map in maps)
                {
                    if (File.Exists(map.orgFilename)) tempMaps.Add(map);
                }
                maps = new List<Map>(tempMaps);

                SerializeToXML(mapsDataBase);
            }
            RefreshDataGridView();
            infoBox.AppendText(maps.Count() + " maps found.");
            infoBox.AppendText(Environment.NewLine);
        }
        //Clear map database. Очистка бази даних карт
        private void ClearAllBtn_Click(object sender, EventArgs e)
        {
            maps.Clear();
            MapImage.BackgroundImage = null;
            SerializeToXML(mapsDataBase);
            RefreshDataGridView();
            infoBox.AppendText("Done clearing database.");
            infoBox.AppendText(Environment.NewLine);
        }
    }
}

//test button

//private void button1_Click(object sender, EventArgs e)
//{
//    if (File.Exists("errors.txt")) File.Delete("errors.txt");
//    fout = new FileStream("errors.txt", FileMode.Append);
//    StreamWriter fstr_out = new StreamWriter(fout);

//    foreach (Map map in maps)
//    {
//        Regex regex = new Regex(@"(.*?)([^\\_]+)(_.{3})(\.sc.)");
//        Match match = regex.Match(map.orgFilename);
//        string cppname = match.Groups[2].Value;
//        if (!map.name.Contains(cppname))
//        {
//            fstr_out.Write(map.orgFilename + "\t" + map.name + "\n");
//        }
//    }
//    fstr_out.Close();
//}