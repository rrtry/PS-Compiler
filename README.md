# Шаблон проекта на C# для студентов

Шаблон предназначен для .NET 8 (C# 12): https://dotnet.microsoft.com/en-us/download/dotnet/8.0

## Структура проекта на C#

Ликбез по типам файлов:

1. Файл `*.sln` (Solution) — основной файл «решения» (то есть проекта), обычно один на репозиторий с кодом
2. Файл `*.csproj` (C# Project) — файл с описанием одного Assembly (_рус._ Сборка), компилируется в один DLL
3. Файл `*.dll` — содержит машинный код для виртуальной машины .NET
    - Расширение `*.dll` также используется для разделяемых библиотек в ОС Windows; в Linux аналогом являются файлы `*.so` (shared library)

Код на C# компилируется в MSIL — код виртуальной машины платформы .NET. Такой код сохраняется в файлах `.dll` для всех платформ, где работает .NET.

Структура проекта создана такими командами:

```bash
# Создаём Solution (основной проект).
dotnet new sln

# Создаём библиотеку ExampleLib в каталоге src/ и добавляем её в Solution
dotnet new classlib -o src/ExampleLib
dotnet sln add src/ExampleLib/

# Создаём проект модульных тестов на тестовом фреймворке XUnit
dotnet new xunit -o tests/ExampleLib.UnitTests
dotnet sln add tests/ExampleLib.UnitTests/

# Добавляем проекту тестов ссылку на проект
dotnet add tests/ExampleLib.UnitTests/ reference src/ExampleLib/
```
