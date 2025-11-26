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

fn main() 
{
    let n: int;
    input(n);
    if (n <= 0) 
    {
        print(1);
    } 
    else 
    {
        let result = factorial(n);
        print(result);
    }
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
    else if (n == 2) 
    {
        return 1;
    }
    else
    {
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
}

fn main()
 {
    let n;
    input(n);
    let result = is_prime(n);
    print(result);
}
```

### Программа QuadraticEquation
```
fn main() 
{
    let a: float;
    let b: float;
    let c: float;
    input(a);
    input(b);
    input(c);
    
    if (a == 0) 
    {
        if (b != 0) 
        {
            let root1 = -c / b;
            print(1);
            print(root1);
        } 
        else 
        {
            print(0);
        }
    } 
    else 
    {
        let disc = b * b - 4 * a * c;
        if (disc > 0) 
        {
            let sqrt_disc = pow(disc, 0.5);
            let root1 = (-b + sqrt_disc) / (2 * a);
            let root2 = (-b - sqrt_disc) / (2 * a);
            print(2);
            print(root1);
            print(root2);
        } 
        else if (disc == 0) 
        {
            let root1 = -b / (2 * a);
            print(1);
            print(root1);
        } 
        else 
        {
            print(0);
        }
    }
}
```