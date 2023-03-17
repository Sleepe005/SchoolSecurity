import serial
import pandas as pd

# Открываем порт
ser = serial.Serial("COM5", 9600)
# Создаём DataFrame
df = pd.DataFrame(columns=["Temp", "Flame", "MQ2", "HaveFire"])

# Получаем данные с порта и загружаем в csv файл
while True:
    data = []
    for i in range(3):
        data.append(str(ser.readline(), 'UTF-8')[:-2:])
    data.append(1) # Есть пожар - 1; Нет пожара - 0

    df.loc[ len(df.index)] = data
    df.to_csv(r'DataYesFire.csv', index= False)
    print(data)


    




