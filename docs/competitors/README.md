# Сравнение C# и Python

## 1. Синтаксис циклов, ветвлений и блоков кода
- **C#:**
  ```csharp
  for (int i = 0; i < 5; i++) 
  {
      if (i % 2 == 0) 
      {
          Console.WriteLine(i);
      }
  }
  ```
- **Python:**
  ```python
  for i in range(5):
      if i % 2 == 0:
          print(i) 
  ```
 В C# используются фигурные скобки `{}`, в Python — отступы.

---

## 2. Объявление типов переменных и параметров
- **C#:** статическая типизация  
  ```csharp
  int x = 10;
  string name = "Alice";
  ```
- **Python:** динамическая типизация  
  ```python
  x = 10
  name = "Alice"
  ```

---

## 3. Набор типов данных
- **C#:** (`int`, `double`, `bool`, `char`, `string`), `struct`, `class`.  
- **Python:** (`int`, `float`, `bool`, `str`, `list`, `dict`, `tuple`, `set`), `class`.  

---

## 4. Операторы
- **Арифметические:** `+ - * / %` (в обоих языках).  
- **Логические:**  
  - C#: `&&`, `||`, `!`  
  - Python: `and`, `or`, `not`  
- **Сравнения:** одинаковый набор `== != > < >= <=`.  

---

## 5. Пользовательские функции
- **C#:**
  ```csharp
  int Sum(int a, int b) 
  {
      return a + b;
  }
  ```
- **Python:**
  ```python
  def sum(a, b):
      return a + b
  ```

---

## 6. Пользовательские структуры
- **C#:**
  ```csharp
  struct Point 
  {
      public int X;
      public int Y;
  }
  ```
- **Python (через dataclass):**
  ```python
  from dataclasses import dataclass

  @dataclass
  class Point:
      x: int
      y: int
  ```

---

## 7. Управление памятью
- **C#:** автоматический GC + управление ресурсами через `using` и `IDisposable`.  
- **Python:** автоматический GC + подсчёт ссылок (reference counting).  

---

## 8. Обработка ошибок
- **C#:**
  ```csharp
  try 
  {
      int x = int.Parse("abc");
  } 
  catch (FormatException e) 
  {
      Console.WriteLine(e.Message);
  }
  ```
- **Python:**
  ```python
  try:
      x = int("abc")
  except ValueError as e:
      print(e)
  ```
