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
        /// <summary>
        /// Свойство
        /// </summary>
        private string prop;
        /// <summary>
        /// Тип
        /// </summary>
        private string type;
        /// <summary>
        /// Выбор типа
        /// </summary>
        private string selectedType;
        /// <summary>
        /// Выбранный класс.
        /// </summary>
        private string selectedItem;
        /// <summary>
        /// Описание
        /// </summary>
        private string descript;
        /// <summary>
        /// Метод
        /// </summary>
        private string method;
        /// <summary>
        /// Видимость комбобокса
        /// </summary>
        private Visibility combovis;
        /// <summary>
        /// Видимость кнопки добавить.
        /// </summary>
        private Visibility addvis;
        /// <summary>
        /// Классы проекта
        /// </summary>
        private ObservableCollection<string> classes;
        /// <summary>
        /// Добавить свойство в класс.
        /// </summary>
        private RelayCommand addToClass;
        /// <summary>
        /// Выбор папки проекта.
        /// </summary>
        private RelayCommand setProject;
        #endregion

        #region Свойства
        /// <summary>
        /// Свойство
        /// </summary>
        public string Prop
        {
            get => prop;
            set
            {
                prop = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Prop)));
            }
        }
        /// <summary>
        /// Тип
        /// </summary>
        public string Type
        {
            get => type;
            set
            {
                type = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Type)));
            }
        }
        /// <summary>
        /// Описание
        /// </summary>
        public string Descript
        {
            get => descript;
            set
            {
                descript = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Descript)));
            }
        }
        /// <summary>
        /// Метод
        /// </summary>
        public string Method
        {
            get => method;
            set
            {
                method = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Method)));
            }
        }
        /// <summary>
        /// Видимость комбобокса
        /// </summary>
        public Visibility ComboVis
        {
            get => combovis;
            set
            {
                combovis = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ComboVis)));
            }
        }
        /// <summary>
        /// Видимость кнопки добавить.
        /// </summary>
        public Visibility AddVis
        {
            get => addvis;
            set
            {
                addvis = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(AddVis)));
            }
        }
        /// <summary>
        /// Выбор типа
        /// </summary>
        public string SelectedType
        {
            get => selectedType;
            set
            {
                selectedType = value;
                SelectionChangedComm();
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SelectedType)));
            }
        }
        /// <summary>
        /// Выбранный класс.
        /// </summary>
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
        /// <summary>
        /// Классы проекта
        /// </summary>
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

        #region Автосвойства
        /// <summary>
        /// Главная директория
        /// </summary>
        private string MainDir { get; set; }
        /// <summary>
        /// Текущий класс.
        /// </summary>
        private string CurClass { get; set; }
        /// <summary>
        /// Массив строк в классе
        /// </summary>
        private string[] StringArray { get; set; }
        /// <summary>
        /// Поля.
        /// </summary>
        private string Fields { get; set; }
        #endregion

        #region Команды
        /// <summary>
        /// Добавить свойство в класс.
        /// </summary>
        public RelayCommand AddToClass => addToClass ?? (addToClass = new RelayCommand(obj => AddToClassComm()));
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

        #region Методы
        /// <summary>
        /// Добавить свойство в класс.
        /// </summary>
        private void AddToClassComm()
        {
            Fields = $"\r\n\r\n        #region Поля\r\n        /// <summary>\r\n        /// {Descript}\r\n        /// </summary>\r\n        private {Type} {Prop.ToLower()};\r\n        #endregion\r\n";
            string[] patt = new string[1] { "\r\n" };
            CurClass = File.ReadAllText($"{MainDir}\\{SelectedItem}");
            StringArray = CurClass.Split(patt, StringSplitOptions.None);
            if (!CurClass.Contains("using System.ComponentModel;")) StringArray[0] = $"using System.ComponentModel;\r\n{StringArray[0]}";
            if (!CurClass.Contains(": INotifyPropertyChanged")) ContainesINotifyPropertyChanged();
            if (!CurClass.Contains("#region Поля")) ContainesFieldsRegion();
            else NotContainesFieldsRegion();
            if (Type != "RelayCommand") IfTypeNotRelayCommand();
            else if (Type == "RelayCommand" && !CurClass.Contains("#region Команды")) IfTypeIsRelayCommandAndNotContainesCommandsRegion();
            else CommandsOtherCases();
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
        /// <summary>
        /// В классе присутствует INotifyPropertyChanged;
        /// </summary>
        private void ContainesINotifyPropertyChanged()
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
        /// <summary>
        /// В классе присутствует регион Поля;
        /// </summary>
        private void ContainesFieldsRegion()
        {
            for (int i = 0; i < StringArray.Length; i++)
            {
                if (StringArray[i].Contains("PropertyChangedEventHandler"))
                {
                    StringArray[i] = $"{StringArray[i]}{Fields}";
                    break;
                }
            }
        }
        /// <summary>
        /// В классе отсутствует регион Поля;
        /// </summary>
        private void NotContainesFieldsRegion()
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
        /// <summary>
        /// В классе присутствует регион Свойства;
        /// </summary>
        private void ContainesPropRegion()
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
        /// <summary>
        /// В классе отсутствует регион Свойства;
        /// </summary>
        private void NotContainesPropRegion()
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
        /// <summary>
        /// Если тип не RelayCommand
        /// </summary>
        private void IfTypeNotRelayCommand()
        {
            if (!CurClass.Contains("#region Свойства")) ContainesPropRegion();
            else NotContainesPropRegion();
        }
        /// <summary>
        /// Если тип RelayCommand и класс не содержит регион Комманды.
        /// </summary>
        private void IfTypeIsRelayCommandAndNotContainesCommandsRegion()
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
        /// <summary>
        /// Прочие варианты команд.
        /// </summary>
        private void CommandsOtherCases()
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
        #endregion
    }
}