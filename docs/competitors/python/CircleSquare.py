import math
radiusCircle = float(input("Введите радиус круга: "))
if radiusCircle > 0:
    print("Площадь круга: ", 
          math.pi * radiusCircle * 
          radiusCircle)
    
else:
    print("Радиус должен быть положительным числом")
