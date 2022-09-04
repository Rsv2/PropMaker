# PropMaker

Утилитка для автоматизации добавления свойств и команд PropertyChanged в MVVM

При запуске указать файл проекта .sln, а затем в появившемся комбобоксе выбрать класс .cs в котором находится ваша ViewModel.

При добавлении Свойств PropertyChanged, в файл будут автоматически дописываться поля, свойства и описания, в случае с RelayCommand, так же будет добавляться пустой метод. В сам класс автоматически будет дописано using System.Collections.ObjectModel; и соответствующий интерфейс.

Данные будут автоматически поделены на 3 региона Поля, Свойства и Команды.

[!Видео](https://www.youtube.com/watch?v=R0_4aNNEtMU)
