### Программа Factorial
```
fn factorial(n) 
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
fn is_prime(n) 
{
    if (n < 2) 
    {
        return 0;
    }
    if (n == 2) 
    {
        return 1;
    }

    let limit = pow(n, 0.5);
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
let a = input();
let b = input();
let c = input();

fn solve() 
{
    if (a == 0) 
    {
        if (b != 0) 
        {
            let root1 = -c / b;
            print(root1);
            return 1;
        } 
        return 0;
    } 
    else 
    {
        let disc = b * b - 4 * a * c;
        if (disc > 0) 
        {
            let sqrt_disc = pow(disc, 0.5);
            let root1 = (-b + sqrt_disc) / (2 * a);
            let root2 = (-b - sqrt_disc) / (2 * a);
            print(root1);
            print(root2);
            return 2;
        }
        if (disc == 0) 
        {
            let root1 = -b / (2 * a);
            print(root1);
            return 1;
        } 
        return 0;
    }
}
let result = solve();
```