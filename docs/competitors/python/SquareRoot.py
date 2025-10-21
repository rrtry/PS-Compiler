import math

number = float(input("Введите действительное число:"))
if number >= 0:
    print("Корень числа ", number, " равен", math.sqrt(number))
else:
    print("ERROR")
