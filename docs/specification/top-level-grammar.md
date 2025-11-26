
# Спецификация языка

## 1. Пример программы

```
// Пример программы

fn main() {
    let a;
    let b;

    input(a);
    input(b);

    let sum = a + b;

    print("sum");
}
```

## 2. Ключевые особенности языка
- Язык динамически типизированный — тип переменной определяется по значению, но в объявлениях параметров функций и переменных указываются ожидаемые типы (int, float) для проверки совместимости.  
- Все переменные изменяемые (mutable).  
- Переменные объявляются через `let` с необязательным указанием типа .  
- Ввод и вывод реализуются встроенными функциями `input()` и `print()`.  
- Инструкции выполняются последовательно внутри функций.  
- Поддерживаются комментарии:  
  - Однострочные: `// ...`  
  - Многострочные: `/* ... */`  
- Каждая инструкция завершается точкой с запятой `;`.  
- Точка входа — функция `main()`. Выполнение программы начинается с неё.  
- Область видимости переменных ограничена телом функции или блока `{ ... }`.  
- Пользовательские функции объявляются до main() с обязательным указанием возвращаемого типа; main() не требует указания типа возврата.
- Поддерживаются пользовательские функции с параметрами и возвращаемым типом.
- Ветвления (if-else) и циклы (while, for) реализованы как инструкции.
- Логические операторы (&&, ||, !) возвращают 1 (true) или 0 (false).
- Порядок вычисления аргументов при вызове функции — слева направо.
- Функции без параметров допускаются.
- Процедуры (функции без возвращаемого значения) поддерживаются через специальный тип void.
- Встроенные функции: input(var) (ввод в переменную, поддерживает int/float), print(expr) (вывод значения).


## 3. Семантические правила
| Правило | Описание |
|---------|----------|
| Переменные должны быть объявлены перед использованием | `x = 10;` без `let` — ошибка |
| Повторное объявление переменной в одной области видимости запрещено | `let x = 1; let x = 2;` — ошибка |
| Имя переменной не может совпадать с ключевыми словами | `fn, let, print, input, return, main` и др — запрещено |
| Область видимости переменных ограничена блоком `{ ... }` | — |
| Функция `main()` обязательна | С неё начинается выполнение программы |
| Все функции возвращают значение | — |
| `Pi` и `Euler` — зарезервированные идентификаторы | — |
| Имена регистрозависимы | `count` ≠ `Count` |
| Висячий `else (dangling else)` разрешается прикреплением `else` к ближайшему `if` без вложенного `if` | Пример: `if (c1) if (c2) s1; else s2;` — else относится к внутреннему `if` |
| Аргументы функций вычисляются слева направо| | — |
| `break` и `continue` работают в циклах (`while`, `for`) | - |


## 4. Виды инструкций

В языке используются только полезные инструкции — те, которые выполняют действия (создают переменные, изменяют значения, выполняют ввод/вывод и т.д.).  
Бесполезные выражения запрещены:

```
x + 10;    // Ошибка — выражение ничего не делает
```

**Допустимые типы инструкций:**
| Тип | Пример | Назначение |
|-----|--------|------------|
| Объявление переменной | `let x = 10;` | Создаёт новую переменную (с возможным начальными значением) |
| Присваивание | `x = x + 1;` | Изменяет значение существующей переменной |
| Ввод | `input(x);` | Считывает значение с клавиатуры и сохраняет в переменную |
| Вывод | `print("x);` | Выводит  значения на экран |
| Возврат | `return x;` | Завершает выполнение функции, возвращая значение |
| Ветвление | `if (cond) { ... } else if(cond) {...} else { ... }` | Условное выполнение |
| Цикл while | `while (cond) { ... }` | Цикл с условием | 
| Цикл for | `for (init, cond, update) { ... }` | Цикл с итерацией по диапазону значений |
| Break | `break;` | Прерывание цикла |
| Continue | `continue;` | Пропуск тела до следующей итерации |

## 5. Структура программы

Программа состоит из набора определений функций, последнего определения — `main()`.

```
program = { function_definition } , entry_point ;
entry_point = "fn" , "main" , "(" , [ parameter_list ] , ")" , block ;
function_definition = "fn" , identifier , "(" , [ parameter_list ] , ")" , ":" , type , block ;
parameter_list = parameter , { "," , parameter } ;
parameter = identifier , ":" , type ;
type = "int" | "float" | "void" ;
block = "{" , { statement } , "}" ;
```

## 6. Инструкции

```
statement =
    variable_declaration , ";"
  | assignment_statement , ";"
  | function_call , ";"
  | return_statement , ";"
  | if_statement
  | while_statement
  | for_statement
  | "break" , ";"
  | "continue" , ";"

```

### Объявление переменной

```
variable_declaration =
    "let" , identifier , [ ":" , type ] , [ "=" , expression ] ;

```

Создаёт новую переменную. Если указано выражение — оно вычисляется, и результат присваивается переменной.
Тип необязателен (динамическая типизация с проверкой)

### Присваивание

```
assignment_statement =
    identifier, "=", expression ;
```

Изменяет значение существующей переменной.
Присваивание не является выражением — это отдельная инструкция.

### Вызов функции

```
function_call = 
identifier, "(", [ argument_list ], ")" ;

argument_list = 
expression, { ",", expression } ;
```
Вызов функции — `identifier("(" [ arg_list ] ")" )`. 
`print(...)` и `input(...)` — встроенные функции, реализованные как обычные функции языка (вызов — свободен внутри `main` и других функций).

Примеры:

```
input(a); 
input(b);                  
print(a + b);
```

### Возврат из функции

```
return_statement =
    "return", [ expression ] ;
```
Все пользовательские функции (кроме void) обязаны иметь return.

### Ветвление if-else

```
if_statement =
    "if" , "(" , expression , ")" , block , { "else" , "if" , "(" , expression , ")" , block } , [ "else" , block ] ;
```
Поддерживает цепочки else if.  Висячий else прикрепляется к ближайшему if.

### Цикл while

```
while_statement =
    "while" , "(" , expression , ")" , block ;
```

### Цикл for

```
for_statement =
    "for" , "(" , [ init ] , expression , [ update ] , ")" , block ;
init = variable_declaration | assignment_statement ;
update = assignment_statement ;
```
Инициализация ( let i = 0), условие, обновление (i = i + 1 или i++ через постфикс).

## 7. Выражения

Присваивание не является выражением, а только инструкцией.  
Примеры допустимых выражений:

```
a + b
(a - 3) * 2
```

## 8. Полный ebnf программы

```
ebnf

program = { function_definition } , entry_point ;
entry_point = "fn" , "main" , "(" , [ parameter_list ] , ")" , block ;
function_definition = "fn" , identifier , "(" , [ parameter_list ] , ")" , ":" , type , block ;
parameter_list = parameter , { "," , parameter } ;
parameter = identifier , ":" , type ;
type = "int" | "float" | "void" ;
block = "{" , { statement } , "}" ;

statement =
    variable_declaration , ";"
  | assignment_statement , ";"
  | function_call , ";"
  | return_statement , ";"
  | if_statement
  | while_statement
  | for_statement
  | "break" , ";"
  | "continue" , ";"
  ;

variable_declaration =
    "let" , identifier , [ ":" , type ] , [ "=" , expression ] ;

assignment_statement =
    identifier , "=" , expression ;

function_call = 
    identifier , "(" , [ argument_list ] , ")" ;

argument_list = 
    expression , { "," , expression } ;

return_statement =
    "return" , [ expression ] ;

if_statement =
    "if" , "(" , expression , ")" , block , { "else" , "if" , "(" , expression , ")" , block } , [ "else" , block ] ;

while_statement =
    "while" , "(" , expression , ")" , block ;

for_statement =
    "for" , "(" , [ init ] , expression , [ update ] , ")" , block ;
init = variable_declaration | assignment_statement ;
update = assignment_statement ;

expression = logical_or ;

logical_or = logical_and , { "||" , logical_and } ;
logical_and = equality , { "&&" , equality } ;

equality = relational , { ( "==" | "!=" ) , relational } ;
relational = additive , { ( "<" | ">" | "<=" | ">=" ) , additive } ;

additive = multiplicative , { ( "+" | "-" ) , multiplicative } ;
multiplicative = unary , { ( "*" | "/" | "%" ) , unary } ;

unary = { ( "+" | "-" | "!" | "++" | "--" ) } , power ;

power = postfix , { ( "^" | "**" ) , power } ;
postfix = primary , { "++" | "--" } ;

primary = number
        | constant
        | bool_literal
        | identifier , [ "(" , [ argument_list ] , ")" ]
        | "(" , expression , ")" ;

bool_literal = "true" | "false" | "null" ;
constant = "Pi" | "Euler" ;
number = integer | float ;
integer = [ "-" ] , digit , { digit } | "0x" , hex_digit , { hex_digit } ;
float = [ "-" ] , integer , "." , integer ;
digit = "0" | "1" | "2" | "3" | "4" | "5" | "6" | "7" | "8" | "9" ;
hex_digit = digit | "A" | "B" | "C" | "D" | "E" | "F" | "a" | "b" | "c" | "d" | "e" | "f" ;

identifier = letter , { letter | digit | "_" } ;
letter = "A" | "B" | ... | "Z" | "a" | ... | "z" ;
```

