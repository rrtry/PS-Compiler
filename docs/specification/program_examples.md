# Примеры программ на языке ----

## 1. SumNumbers

```
let a = stoi(input());
let b = stoi(input());
let sum = a + b;
print(sum);
```

---

## 2. CircleSquare

```
let r = input();
let area = Pi * r * r;
printf(area, 2);
```
---

## 3. GeometricMean

```
let a = stoi(input());
let b = stoi(input());
let gmean = (a * b) ** 0.5;
printf(gmean, 2);
```

### Программа Factorial
```
fn factorial(n: int): int
{
    let fact = 1;
    for (let i = 1, i <= n, i = i + 1) 
    {
        fact = fact * i;
    }
    return fact;
}
```

### Программа IsPrime
```
fn is_prime(n: int): int 
{
    if (n < 2) 
    {
        return 0;
    }
    if (n == 2) 
    {
        return 1;
    }

    let limit = sqrt(itof(n));
    let i = 3;
    while (i <= limit) 
    {
        if (n % i == 0) 
        {
            return 0;
        }
        i = i + 2;
    }
    return 1;
}
```

### Программа QuadraticEquation
```
let x: float = stof(input());
let y: float = stof(input());
let z: float = stof(input());

fn solve(a: float, b: float, c: float): int
{
    if (a == 0) 
    {
        if (b != 0) 
        {
            let root1 = -c / b;
            printf(root1, 2);
            return 1;
        }
    }
    else 
    {
        let disc = b * b - 4 * a * c;
        if (disc > 0) 
        {
            let sqrt_disc = sqrt(disc);
            let root1 = (-b + sqrt_disc) / (2.0 * a);
            let root2 = (-b - sqrt_disc) / (2.0 * a);
            printf(root1, 2);
            printf(root2, 2);
            return 2;
        }
        if (disc == 0) 
        {
            let root1 = -b / (2 * a);
            printf(root1, 2);
            return 1;
        }
    }
    return 0;
}
let result = solve(x, y, z);
print(result);
```

## 9. Примеры программ

### ReverseString

```
fn reverse(s: str): str
{
    let len = strlen(s);
    let result = """";

    let i = len - 1;
    while (i >= 0) 
    {
        let ch = substr(s, i, 1);
        result = sconcat(result, ch);
        i = i - 1;
    }

    return result;
}

let text = input();
let reversed = reverse(text);
prints(reversed);
```


### FizzBuzz
```
let x = 1;
while (x) 
{
    let n = stoi(input());
    if (n == 0) 
    {
        break;
    }

    if (n % 15 == 0) 
    {
        prints(""FizzBuzz"");
        continue;
    }
    if (n % 3 == 0) 
    {
        prints(""Fizz"");
        continue;
    }
    if (n % 5 == 0) 
    {
        prints(""Buzz"");
        continue;
    }
    prints(itos(n));
}
```


### CountVowels

```
"fn is_vowel(ch: str): int
{
    if (ch == ""A"" || ch == ""E"" || ch == ""I"" || ch == ""O"" || ch == ""U"" || ch == ""Y"") 
    {
        return 1;
    }
    if (ch == ""a"" || ch == ""e"" || ch == ""i"" || ch == ""o"" || ch == ""u"" || ch == ""y"") 
    {
        return 1;
    }
    return 0;
}

fn count_vowels(s: str): int 
{
    let count = 0;
    let len = strlen(s);
    let i = 0;

    while (i < len) 
    {
        let sub = substr(s, i, 1);
        if (is_vowel(sub)) 
        {
            count = count + 1;
        }
        i = i + 1;
    }
    return count;
}
let text = input();
let vowels = count_vowels(text);
prints(itos(vowels));
```