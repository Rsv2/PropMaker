using Microsoft.Win32;
using System;
using System.Windows;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;

namespace PropMaker
{
    public class ViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        #region Поля
        private string prop;
        private string type;
        private string selectedType;
        private string selectedItem;
        private string descript;
        private string MainDir;
        private string CurClass;
        private string method;
        private Visibility combovis;
        private Visibility addvis;
        private ObservableCollection<string> classes;
        private RelayCommand addToClass;
        private RelayCommand selectionChanged;
        private RelayCommand setProject;
        #endregion

        #region Свойства

        public string Prop
        {
            get => prop;
            set
            {
                prop = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Prop)));
            }
        }

        public string Type
        {
            get => type;
            set
            {
                type = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Type)));
            }
        }

        public string Descript
        {
            get => descript;
            set
            {
                descript = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Descript)));
            }
        }

        public string Method
        {
            get => method;
            set
            {
                method = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Method)));
            }
        }

        public Visibility ComboVis
        {
            get => combovis;
            set
            {
                combovis = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ComboVis)));
            }
        }

        public Visibility AddVis
        {
            get => addvis;
            set
            {
                addvis = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(AddVis)));
            }
        }

        public string SelectedType
        {
            get => selectedType;
            set
            {
                selectedType = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SelectedType)));
            }
        }

        public string SelectedItem
        {
            get => selectedItem;
            set
            {
                selectedItem = value;
                if (value != null) AddVis = Visibility.Visible;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SelectedItem)));
            }
        }

        public ObservableCollection<string> Classes
        {
            get => classes;
            set
            {
                classes = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Classes)));
            }
        }
        #endregion

        #region Команды
        /// <summary>
        /// Добавить свойство в класс.
        /// </summary>
        public RelayCommand AddToClass => addToClass ?? (addToClass = new RelayCommand(obj => AddToClassComm()));
        /// <summary>
        /// Выбор значения в комбобоксе.
        /// </summary>
        public RelayCommand SelectionChanged => selectionChanged ?? (selectionChanged = new RelayCommand(obj => SelectionChangedComm()));
        /// <summary>
        /// Выбор папки проекта.
        /// </summary>
        public RelayCommand SetProject => setProject ?? (setProject = new RelayCommand(obj => SelectProjectComm()));
        #endregion

        /// <summary>
        /// Конструктор.
        /// </summary>
        public ViewModel()
        {
            ComboVis = Visibility.Hidden;
            AddVis = Visibility.Hidden;
        }

        /// <summary>
        /// Добавить свойство в класс.
        /// </summary>
        private void AddToClassComm()
        {
            string fields = $"\r\n\r\n        #region Поля\r\n        /// <summary>\r\n        /// {Descript}\r\n        /// </summary>\r\n        private {Type} {Prop.ToLower()};\r\n        #endregion\r\n";
            string[] patt = new string[1];
            patt[0] = "\r\n";
            CurClass = File.ReadAllText($"{MainDir}\\{SelectedItem}");
            string[] StringArray = CurClass.Split(patt, StringSplitOptions.None);
            if (!CurClass.Contains("using System.ComponentModel;"))
            {
                StringArray[0] = $"using System.ComponentModel;\r\n{StringArray[0]}";
            }
            if (!CurClass.Contains(": INotifyPropertyChanged"))
            {
                for (int i = 0; i < StringArray.Length; i++)
                {
                    if (StringArray[i].Contains("class"))
                    {
                        if (StringArray[i].Contains(":"))
                        {
                            StringArray[i] = $"{StringArray[i]}, INotifyPropertyChanged";
                        }
                        else
                        {
                            StringArray[i] = $"{StringArray[i]} : INotifyPropertyChanged";
                        }
                        StringArray[i + 1] = $"{StringArray[i + 1]}\r\n        public event PropertyChangedEventHandler PropertyChanged;\r\n";
                        break;
                    }
                }
            }
            if (!CurClass.Contains("#region Поля"))
            {
                for (int i = 0; i < StringArray.Length; i++)
                {
                    if (StringArray[i].Contains("PropertyChangedEventHandler"))
                    {
                        StringArray[i] = $"{StringArray[i]}{fields}";
                        break;
                    }
                }
            }
            else
            {
                for (int i = 0; i < StringArray.Length; i++)
                {
                    if (StringArray[i].Contains("#region Поля"))
                    {
                        StringArray[i] = $"{StringArray[i]}\r\n        /// <summary>\r\n        /// {Descript}\r\n        /// </summary>\r\n        private {Type} {Prop.ToLower()};";
                        break;
                    }
                }
            }
            if (Type != "RelayCommand")
            {
                if (!CurClass.Contains("#region Свойства"))
                {
                    for (int i = 0; i < StringArray.Length; i++)
                    {
                        if (StringArray[i].Contains("#region Поля"))
                        {
                            while (!StringArray[i].Contains("#endregion"))
                            {
                                i++;
                            }
                            StringArray[i] = $"{StringArray[i]}\r\n\r\n        #region Свойства\r\n        /// <summary>\r\n        /// {Descript}\r\n        /// </summary>\r\n        public {Type} {Prop}\r\n        {{\r\n" +
                                $"            get => {Prop.ToLower()};\r\n" +
                                $"            set\r\n" +
                                $"            {{\r\n" +
                                $"                {Prop.ToLower()} = value;\r\n" +
                                $"                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof({Prop})));\r\n" +
                                $"            }}\r\n" +
                                $"        }}\r\n" +
                                $"        #endregion\r\n";
                            break;
                        }
                    }
                }
                else
                {
                    for (int i = 0; i < StringArray.Length; i++)
                    {
                        if (StringArray[i].Contains("#region Свойства"))
                        {
                            StringArray[i] = $"{StringArray[i]}\r\n        /// <summary>\r\n        /// {Descript}\r\n        /// </summary>\r\n        public {Type} {Prop}\r\n        {{\r\n" +
                                $"            get => {Prop.ToLower()};\r\n" +
                                $"            set\r\n" +
                                $"            {{\r\n" +
                                $"                {Prop.ToLower()} = value;\r\n" +
                                $"                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof({Prop})));\r\n" +
                                $"            }}\r\n" +
                                $"        }}";
                            break;
                        }
                    }
                }
            }
            else if (Type == "RelayCommand" && !CurClass.Contains("#region Команды"))
            {
                for (int i = 0; i < StringArray.Length; i++)
                {
                    if (StringArray[i].Contains("#region Поля"))
                    {
                        while (!StringArray[i].Contains("#endregion"))
                        {
                            i++;
                        }
                        StringArray[i] = $"{StringArray[i]}\r\n\r\n        #region Команды\r\n        /// <summary>\r\n        /// {Descript}\r\n        /// </summary>\r\n        public RelayCommand {Prop} => {Prop.ToLower()} ?? ({Prop.ToLower()} = new RelayCommand(obj => {Method}()));\r\n        #endregion\r\n";
                        while (StringArray[i] != "    }")
                        {
                            i++;
                        }
                        StringArray[i - 1] = $"{StringArray[i - 1]}\r\n        /// <summary>\r\n        /// {Descript}\r\n        /// </summary>\r\n        private void {Method}()\r\n        {{\r\n        \r\n        }}";
                        break;
                    }
                }
            }
            else
            {
                for (int i = 0; i < StringArray.Length; i++)
                {
                    if (StringArray[i].Contains("#region Команды"))
                    {
                        StringArray[i] = $"{StringArray[i]}\r\n        /// <summary>\r\n        /// {Descript}\r\n        /// </summary>\r\n        public RelayCommand {Prop} => {Prop.ToLower()} ?? ({Prop.ToLower()} = new RelayCommand(obj => {Method}()));";
                        while (StringArray[i + 1] != "    }")
                        {
                            i++;
                        }
                        StringArray[i] = $"{StringArray[i]}\r\n        /// <summary>\r\n        /// {Descript}\r\n        /// </summary>\r\n        private void {Method}()\r\n        {{\r\n        \r\n        }}";
                        break;
                    }
                }
            }
            CurClass = "";
            for (int i = 0; i < StringArray.Length; i++)
            {
                CurClass += $"{StringArray[i]}\r\n";
            }
            File.WriteAllLines($"{MainDir}\\{SelectedItem}", StringArray);
        }
        /// <summary>
        /// Выбор значения в комбобоксе.
        /// </summary>
        private void SelectionChangedComm()
        {
            Type = SelectedType.Substring(SelectedType.LastIndexOf(" ") + 1);
        }
        /// <summary>
        /// Выбор папки проекта.
        /// </summary>
        private void SelectProjectComm()
        {
            Classes = new ObservableCollection<string>();
            OpenFileDialog OFD = new OpenFileDialog()
            {
                DefaultExt = "*.sln",
                Filter = "(*.sln)|*.sln"
            };
            if (OFD.ShowDialog() == true)
            {
                ComboVis = Visibility.Visible;
                string SrcFile = OFD.FileName;
                MainDir = Path.GetDirectoryName(SrcFile);
                string[] tempfiles = Directory.GetFiles(MainDir);
                for (int i = 0; i < tempfiles.Length; i++)
                {
                    if (tempfiles[i].Contains(".cs") && !tempfiles[i].Contains(".xaml") && !tempfiles[i].Contains(".csproj"))
                    {
                        Classes.Add(tempfiles[i].Substring(tempfiles[i].LastIndexOf("\\") + 1));
                    }
                }
            }
        }
    }
}




