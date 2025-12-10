using Execution;

namespace Interpreter.Specs;

public class InterpreterTests
{
    [Theory]
    [MemberData(nameof(GetExamplePrograms))]
    public void Can_interpret_simple_programs(string source, Tuple<List<decimal>, List<decimal>> tuple)
    {
        List<decimal> programInput = tuple.Item1;
        List<decimal> expectedOutput = tuple.Item2;

        FakeEnvironment environment = new FakeEnvironment();
        Context context = new Context(environment);
        environment.SetProgramInput(programInput);

        List<decimal> evaluated = environment.GetEvaluated();
        Interpreter interpreter = new Interpreter(context, environment);
        interpreter.Execute(source);

        Assert.Equal(expectedOutput.Count, evaluated.Count);

        for (int i = 0; i < evaluated.Count; i++)
        {
            if (!decimal.Equals(decimal.Round(expectedOutput[i], 2), decimal.Round(evaluated[i], 2)))
            {
                throw new Xunit.Sdk.XunitException($"Expected {expectedOutput[i]} but got {evaluated[i]}");
            }
        }
    }

    public static TheoryData<string, Tuple<List<decimal>, List<decimal>>> GetExamplePrograms()
    {
        return new TheoryData<string, Tuple<List<decimal>, List<decimal>>>
        {
            {
                @"fn fibonacci(n) {

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
                new Tuple<List<decimal>, List<decimal>>(
                    new List<decimal> { },
                    new List<decimal> { 55 }
                )
            },
            {
                @"fn power(a, b) {
                    return a ^ b;
                }
                print(power(2, 3));",
                new Tuple<List<decimal>, List<decimal>>(
                    new List<decimal> { },
                    new List<decimal> { 8 }
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
                new Tuple<List<decimal>, List<decimal>>(
                    new List<decimal> { },
                    new List<decimal> { 1, 2, 4, 5 }
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
                new Tuple<List<decimal>, List<decimal>>(
                    new List<decimal> { },
                    new List<decimal> { 5 }
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
                new Tuple<List<decimal>, List<decimal>>(
                    new List<decimal> { },
                    new List<decimal> { 5 }
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
                new Tuple<List<decimal>, List<decimal>>(
                    new List<decimal> { },
                    new List<decimal> { 1, 2, 4, 5 }
                )
            },
            {
                @"for (let x = 0; x < 5; x = x + 1) {
                      if (x % 2 == 0 && x != 0) {
                          print(x);
                      }
                  }",
                new Tuple<List<decimal>, List<decimal>>(
                    new List<decimal> { },
                    new List<decimal> { 2, 4 }
                )
            },
            {
                @"let i = 0;
                  let x = 1;
                  for (x = 0; x < 5; x = x + 1) {
                      i = i + 1;
                  }
                  print(i);",
                new Tuple<List<decimal>, List<decimal>>(
                    new List<decimal> { },
                    new List<decimal> { 5 }
                )
            },
            {
                @"let x = 0;
                  for (let j = 0; j < 5; j = j + 1) {
                      x = x + 1;
                  }
                  print(x);",
                new Tuple<List<decimal>, List<decimal>>(
                    new List<decimal> { },
                    new List<decimal> { 5 }
                )
            },
            {
                @"let i = 0;
                  while (i < 5) {
                      let temp = i + 1;
                      i = temp;
                  }
                  print(i);",
                new Tuple<List<decimal>, List<decimal>>(
                    new List<decimal> { },
                    new List<decimal> { 5 }
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
                new Tuple<List<decimal>, List<decimal>>(
                    new List<decimal> { },
                    new List<decimal> { 2 }
                )
            },
            {
                @"let x = 4;
                  let y = 2;
                  if (pow(x, 0.5) == y) {
                      let z = 2;
                      print((x + y) * z);
                  } else {
                    print(x - y);
                  }",
                new Tuple<List<decimal>, List<decimal>>(
                    new List<decimal> { },
                    new List<decimal> { 12 }
                )
            },
            {
                @"let x = 6;
                  let y = 2;
                  if ((x + y) == 8 && pow(2, 3) == 8) {
                      print(x + y);
                  }",
                new Tuple<List<decimal>, List<decimal>>(
                    new List<decimal> { },
                    new List<decimal> { 8 }
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
                new Tuple<List<decimal>, List<decimal>>(
                    new List<decimal> { },
                    new List<decimal> { 8 }
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
                new Tuple<List<decimal>, List<decimal>>(
                    new List<decimal> { },
                    new List<decimal> { 4 }
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
                new Tuple<List<decimal>, List<decimal>>(
                    new List<decimal> { },
                    new List<decimal> { 0 }
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
                new Tuple<List<decimal>, List<decimal>>(
                    new List<decimal> { },
                    new List<decimal> { 3 }
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
                new Tuple<List<decimal>, List<decimal>>(
                    new List<decimal> { },
                    new List<decimal> { -1 }
                )
            },
            {
                @"let x = 2;
                  let y = 1;
                  if (x > y) {
                      print(x + y);
                  }",
                new Tuple<List<decimal>, List<decimal>>(
                    new List<decimal> { },
                    new List<decimal> { 3 }
                )
            },
            {
                @"let x = 2;
                  let y = 1;
                  if (x < y) {
                      print(x + y);
                  }",
                new Tuple<List<decimal>, List<decimal>>(
                    new List<decimal> { },
                    new List<decimal> { }
                )
            },
            {
                "let x = 1;" +
                "let y = 2;" +
                "let z = y > x;" +
                "print(z);",
                new Tuple<List<decimal>, List<decimal>>(
                    new List<decimal> { },
                    new List<decimal> { 1 }
                )
            },
            {
                "let x = 1;" +
                "let y = 2;" +
                "let z = y < x;" +
                "print(z);",
                new Tuple<List<decimal>, List<decimal>>(
                    new List<decimal> { },
                    new List<decimal> { 0 }
                )
            },
            {
                "let x = 1;" +
                "let y = 2;" +
                "let z = y <= x;" +
                "print(z);",
                new Tuple<List<decimal>, List<decimal>>(
                    new List<decimal> { },
                    new List<decimal> { 0 }
                )
            },
            {
                "let x = 1;" +
                "let y = 2;" +
                "let z = y >= x;" +
                "print(z);",
                new Tuple<List<decimal>, List<decimal>>(
                    new List<decimal> { },
                    new List<decimal> { 1 }
                )
            },
            {
                "let x = input();" +
                "let y = input();" +
                "let z = x != y;" +
                "print(z);",
                new Tuple<List<decimal>, List<decimal>>(
                    new List<decimal> { 1, 2 },
                    new List<decimal> { 1 }
                )
            },
            {
                "let x = input();" +
                "let y = input();" +
                "let z = x == y;" +
                "print(z);",
                new Tuple<List<decimal>, List<decimal>>(
                    new List<decimal> { 1, 1 },
                    new List<decimal> { 1 }
                )
            },
            {
                "let x = input();" +
                "let y = input();" +
                "let z = x != y && x == 0;" +
                "print(z);",
                new Tuple<List<decimal>, List<decimal>>(
                    new List<decimal> { 1, 2 },
                    new List<decimal> { 0 }
                )
            },
            {
                "let x = input();" +
                "let y = input();" +
                "let z = x != y || x == 0;" +
                "print(z);",
                new Tuple<List<decimal>, List<decimal>>(
                    new List<decimal> { 1, 2 },
                    new List<decimal> { 1 }
                )
            }, 
            {
                "let x = input();" +
                "let y = input();" +
                "let z = (x != y) || (x == 0);" +
                "print(z);",
                new Tuple<List<decimal>, List<decimal>>(
                    new List<decimal> { 1, 2 },
                    new List<decimal> { 1 }
                )
            },
            {
                "let x = input(); " +
                "let y = input(); " +
                "print(x + y);",
                new Tuple<List<decimal>, List<decimal>>(
                    new List<decimal> { 1, 2 },
                    new List<decimal> { 3 }
                )
            },
            {
                "let x = input(); " +
                "let y = input(); " +
                "y = 3; " +
                "print(x + y);",
                new Tuple<List<decimal>, List<decimal>>(
                    new List<decimal> { 1, 2 },
                    new List<decimal> { 4 }
                )
            },
            {
                "let x = input(); " +
                "let y = input(); " +
                "let result = pow(x + y, 0.5); " +
                "print(result);",
                new Tuple<List<decimal>, List<decimal>>(
                    new List<decimal> { 2, 2 },
                    new List<decimal> { 2 }
                )
            },
            {
                "let x = 2; " +
                "let y = 2; " +
                "let result = (x + y) ^ 0.5; " +
                "print(result);",
                new Tuple<List<decimal>, List<decimal>>(
                    new List<decimal> { 2, 2 },
                    new List<decimal> { 2 }
                )
            },
            {
                "let PI = input(); " +
                "let radius = input(); " +
                "let area = (radius ^ 2) * PI; " +
                "print(area);",
                new Tuple<List<decimal>, List<decimal>>(
                    new List<decimal> { 3.14159265358979323846m, 10m },
                    new List<decimal> { 100.0m * 3.14159265358979323846m }
                )
            },
        };
    }
}
