# Примеры программ на языке ----

## 1. SumNumbers

```
fn main() {
    let a;
    let b;

    input(a);
    input(b);

    let sum = a + b;

    print(sum);
}
```

---

## 2. CircleSquare

```
fn main() {
    let r;
    r = 1;
    input(r);

    let area = Pi * r * r;

    print(area);
}
```

---

## 3. GeometricMean

```
fn main() {
    // Вычисление среднего геометрического двух чисел
    let a;
    let b;

    /*
     Ввод двух чисел
    */
    input(a);
    input(b);

    let gmean = (a * b) ** 0.5;

    // Вывод результата
    print(gmean);
}
```