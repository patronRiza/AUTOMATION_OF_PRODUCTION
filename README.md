# AUTOMATION_OF_PRODUCTION
## Описание проекта
Автоматизированное решение для постпроцессинга программ ЧПУ в Siemens NX CAM. Основные функции:
- Сбор данных о программах обработки из дерева операций NX
- Валидация операций перед постпроцессингом
- Настройка параметров через GUI
- Автоматический запуск постпроцессора
- Проверка результатов
- Логирование процесса
## Архитектура
### Основные компоненты
| Компонент                  | Назначение                                                                |
|----------------------------|---------------------------------------------------------------------------|
| `ProgramService`           | Сбор данных о программах и операциях                                      |
| `PostprocessConfigurator`  | Настройка параметров постпроцессинга                                      |
| `CAMValidator`             | Валидация операций и результатов                                          |
| `NXLogger`                 | Система логирования                                                       |
## Конфигурация
```csharp
// Путь сохранения управляющих программ
nativeFolderBrowser0.Path = @"D:\NC_PROGRAMS\"; 

// Расширение файлов по умолчанию
string0.Value = "NC";

// Настройки логирования
_logpath = @"D:\NX_Logs\journal_log.txt";
MaxLogSize = 10 * 1024 * 1024; // 10MB
```
## Настройка переменных окружения
Для корректной работы системы необходимо установить следующие переменные окружения:
### Обязательные переменные

1. **UGII_CAM_POST_DIR** - путь к директории с постпроцессорами NX
   ```bash
   UGII_CAM_POST_DIR = C:\Program Files\Siemens\NX2412\MACH\resource\postprocessor\
2. **UGII_USER_DIR** - путь к директории с кастомным диалоговым окном NX
   ```bash
   UGII_USER_DIR = D:\NX_Custom\
## Установка
1. Скопируйте файлы проекта в папку:
   ```bash
   UGII_USER_DIR/application/
2. Скопируйте файл кастомного диалогового окна папку:
   ```bash
   UGII_USER_DIR/application/
2. Запустите NX
3. Добавть пользовательскую кнопку с выполнениям скомпелированной библиотеки
