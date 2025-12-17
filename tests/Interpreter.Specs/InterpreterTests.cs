using Execution;

using Runtime;
using Xunit.Sdk;

namespace Interpreter.Specs;

public class InterpreterTests
{
    [Theory]
    [MemberData(nameof(GetSingleTypePrograms))]
    [MemberData(nameof(GetMixedTypePrograms))]
    public void Can_interpret_simple_programs(string source, Tuple<List<string>, List<string>> tuple)
    {
        List<string> programInput = tuple.Item1;
        List<string> expectedOutput = tuple.Item2;

        FakeEnvironment environment = new FakeEnvironment();
        Context context = new Context(environment);
        environment.SetProgramInput(programInput);

        List<string> evaluated = environment.GetEvaluated();
        Interpreter interpreter = new Interpreter(context, environment);
        interpreter.Execute(source);

        Assert.Equal(expectedOutput.Count, evaluated.Count);
        for (int i = 0; i < evaluated.Count; i++)
        {
            if (expectedOutput[i] != evaluated[i])
            {
                throw new XunitException($"Expected: {expectedOutput[i]}, got: {evaluated[i]}");
            }
        }
    }

    public static TheoryData<string, Tuple<List<string>, List<string>>> GetMixedTypePrograms()
    {
        return new TheoryData<string, Tuple<List<string>, List<string>>>
        {
            {
                @"fn is_vowel(ch: str): int
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
                prints(itos(vowels));",
                new Tuple<List<string>, List<string>>(
                    new List<string> { "Hello" },
                    new List<string> { "2" }
                )
            },
            {
                @"
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
                ",
                new Tuple<List<string>, List<string>>(
                    new List<string> { "123" },
                    new List<string> { "321" }
                )
            },
            {
                @"
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
                ",
                new Tuple<List<string>, List<string>>(
                    new List<string> { "15", "3", "5", "0" },
                    new List<string> { "FizzBuzz", "Fizz", "Buzz" }
                )
            },
        };
    }

    public static TheoryData<string, Tuple<List<string>, List<string>>> GetSingleTypePrograms()
    {
        return new TheoryData<string, Tuple<List<string>, List<string>>>
        {
            {
                @"
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
                            let root1 = (-b + sqrt_disc) / (2 * a);
                            let root2 = (-b - sqrt_disc) / (2 * a);
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
                print(result);",
                new Tuple<List<string>, List<string>>(
                    new List<string> { "2", "3", "-2" },
                    new List<string> { "0.50", "-2.00", "2" }
                )
            },
            {
                @"fn is_prime(n: int): int {
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
                print(is_prime(139));
                ",
                new Tuple<List<string>, List<string>>(
                    new List<string> { },
                    new List<string> { "1" }
                )
            },
            {
                @"fn factorial(n: int): int {
                    let fact = 1;
                    for (let i: int = 1; i <= n; i = i + 1) 
                    {
                        fact = fact * i;
                    }
                    return fact;
                }
                print(factorial(5));",
                new Tuple<List<string>, List<string>>(
                    new List<string> { },
                    new List<string> { "120" }
                )
            },
            {
                @"fn fibonacci(n: int): int {

                    let prev1 = 1;
                    let prev2 = 0;
                    let curr  = 0;

                    for (let i = 2; i <= n; i = i + 1) {
                        curr = prev1 + prev2;
                        prev2 = prev1;
                        prev1 = curr;
                    }

                    return curr;
                }
                print(fibonacci(10));",
                new Tuple<List<string>, List<string>>(
                    new List<string> { },
                    new List<string> { "55" }
                )
            },
            {
                @"let i = 0;
                  if (!i) {
                    print(!i);
                  }",
                new Tuple<List<string>, List<string>>(
                    new List<string> { },
                    new List<string> { "1" }
                )
            },
            {
                @"fn add(a: int, b: int): int {
                    return a + b;
                }
                let z = add(2, 3);
                print(z);",
                new Tuple<List<string>, List<string>>(
                    new List<string> { },
                    new List<string> { "5" }
                )
            },
            {
                @"let x = 0;
                  for (let j = 0; j < 5; j = j + 1) {
                      x = x + 1;
                      if (x == 3) {
                        continue;
                      }
                      print(x);
                  }",
                new Tuple<List<string>, List<string>>(
                    new List<string> { },
                    new List<string> { "1", "2", "4", "5" }
                )
            },
            {
                @"let x = 0;
                  for (let j = 0; j < 10; j = j + 1) {
                      x = x + 1;
                      if (x == 5) {
                          break;
                      }
                  }
                  print(x);",
                new Tuple<List<string>, List<string>>(
                    new List<string> { },
                    new List<string> { "5" }
                )
            },
            {
                @"let i = 0;
                  while (i < 10) {
                      let temp = i + 1;
                      i = temp;
                      if (i == 5) {
                          break;
                      }
                  }
                  print(i);",
                new Tuple<List<string>, List<string>>(
                    new List<string> { },
                    new List<string> { "5" }
                )
            },
            {
                @"let i = 0;
                  while (i < 5) {
                      i = i + 1;
                      if (i == 3) {
                          continue;
                      }
                      print(i);
                  }",
                new Tuple<List<string>, List<string>>(
                    new List<string> { },
                    new List<string> { "1", "2", "4", "5" }
                )
            },
            {
                @"let x = 4;
                  let y = 2;
                  if (x + y < x) {
                      let z = 2;
                      print((x + y) * z);
                  } else {
                    print(x - y);
                  }",
                new Tuple<List<string>, List<string>>(
                    new List<string> { },
                    new List<string> { "2" }
                )
            },
            {
                @"let x = 6;
                  let y = 2;
                  if ((x + y) == 8 && pow(2, 3) == 8) {
                      print(x + y);
                  }",
                new Tuple<List<string>, List<string>>(
                    new List<string> { },
                    new List<string> { "8" }
                )
            },
            {
                @"let x = 6;
                  let y = 2;
                  if ((x + y) == 8 || (x - y) == 2) {
                      print(x + y);
                  } else {
                    print(x - y);
                  }",
                new Tuple<List<string>, List<string>>(
                    new List<string> { },
                    new List<string> { "8" }
                )
            },
            {
                @"let x = 2;
                  let y = 2;
                  if (x == y) {
                      print(x + y);
                  } else {
                      print(x - y);
                  }",
                new Tuple<List<string>, List<string>>(
                    new List<string> { },
                    new List<string> { "4" }
                )
            },
            {
                @"let x = 2;
                  let y = 2;
                  if (x != y) {
                      print(x + y);
                  } else {
                      print(x - y);
                  }",
                new Tuple<List<string>, List<string>>(
                    new List<string> { },
                    new List<string> { "0" }
                )
            },
            {
                @"let x = 1;
                  let y = 2;
                  if (x < y) {
                      print(x + y);
                  } else {
                      print(x - y);
                  }",
                new Tuple<List<string>, List<string>>(
                    new List<string> { },
                    new List<string> { "3" }
                )
            },
            {
                @"let x = 1;
                  let y = 2;
                  if (x > y) {
                      print(x + y);
                  } else {
                      print(x - y);
                  }",
                new Tuple<List<string>, List<string>>(
                    new List<string> { },
                    new List<string> { "-1" }
                )
            },
            {
                @"let x = 2;
                  let y = 1;
                  if (x > y) {
                      print(x + y);
                  }",
                new Tuple<List<string>, List<string>>(
                    new List<string> { },
                    new List<string> { "3" }
                )
            },
            {
                @"let x = 2;
                  let y = 1;
                  if (x < y) {
                      print(x + y);
                  }",
                new Tuple<List<string>, List<string>>(
                    new List<string> { },
                    new List<string> { }
                )
            },
            {
                "let x = 1;" +
                "let y = 2;" +
                "let z = y > x;" +
                "print(z);",
                new Tuple<List<string>, List<string>>(
                    new List<string> { },
                    new List<string> { "1" }
                )
            },
            {
                "let x = 1;" +
                "let y = 2;" +
                "let z = y < x;" +
                "print(z);",
                new Tuple<List<string>, List<string>>(
                    new List<string> { },
                    new List<string> { "0" }
                )
            },
            {
                "let x = 1;" +
                "let y = 2;" +
                "let z = y <= x;" +
                "print(z);",
                new Tuple<List<string>, List<string>>(
                    new List<string> { },
                    new List<string> { "0" }
                )
            },
            {
                "let x = 1;" +
                "let y = 2;" +
                "let z = y >= x;" +
                "print(z);",
                new Tuple<List<string>, List<string>>(
                    new List<string> { },
                    new List<string> { "1" }
                )
            },
            {
                "let x = stoi(input());" +
                "let y = stoi(input());" +
                "let z = x != y;" +
                "print(z);",
                new Tuple<List<string>, List<string>>(
                    new List<string> { "1", "2" },
                    new List<string> { "1" }
                )
            },
            {
                "let x = stoi(input());" +
                "let y = stoi(input());" +
                "let z = x == y;" +
                "print(z);",
                new Tuple<List<string>, List<string>>(
                    new List<string> { "1", "1" },
                    new List<string> { "1" }
                )
            },
            {
                "let x = stoi(input());" +
                "let y = stoi(input());" +
                "let z = x != y && x == 0;" +
                "print(z);",
                new Tuple<List<string>, List<string>>(
                    new List<string> { "1", "2" },
                    new List<string> { "0" }
                )
            },
            {
                "let x = stoi(input());" +
                "let y = stoi(input());" +
                "let z = x != y || x == 0;" +
                "print(z);",
                new Tuple<List<string>, List<string>>(
                    new List<string> { "1", "2" },
                    new List<string> { "1" }
                )
            },
            {
                "let x = stoi(input());" +
                "let y = stoi(input());" +
                "let z = (x != y) || (x == 0);" +
                "print(z);",
                new Tuple<List<string>, List<string>>(
                    new List<string> { "1", "2" },
                    new List<string> { "1" }
                )
            },
            {
                "let x = stoi(input()); " +
                "let y = stoi(input()); " +
                "print(x + y);",
                new Tuple<List<string>, List<string>>(
                    new List<string> { "1", "2" },
                    new List<string> { "3" }
                )
            },
            {
                "let x = stoi(input()); " +
                "let y = stoi(input()); " +
                "y = 3; " +
                "print(x + y);",
                new Tuple<List<string>, List<string>>(
                    new List<string> { "1", "2" },
                    new List<string> { "4" }
                )
            },
        };
    }
}
