import pandas as pd

def active(y):
    return 1 if y > 0 else -1

noFire = pd.read_csv('SchoolSecurity\\DataNoFire.csv')
yesFire = pd.read_csv('SchoolSecurity\\DataYesFire.csv')

df = pd.concat([noFire, yesFire])

columns = [i for i in df]

noWeights = [1,1,1]
yesWeights = [1,1,1]
b = 1

sp = 0.1

# Learn no fire
for i in range(500):
    for j in range(len(df)):
        y1 = b
        # Делаем предсказание
        for h in range(len(noWeights)):
            y1 += noWeights[h]*df.iloc[j][columns[h]]
        # Активируем и проверяем
        y = active(df.iloc[j]['HaveFire'])
        if active(y1) != y:
            for h in range(len(noWeights)):
                noWeights[h] = noWeights[h] + sp * (y-active(y1)) * df.iloc[j][columns[h]]
print(noWeights)

# [-4, -28, -22]

# Learn yes fire
# for i in range(500):
#     for j in range(len(yesFire)):
#         y1 = b
#         # Делаем предсказание
#         for h in range(len(yesWeights)):
#             y1 += yesWeights[h]*yesFire.iloc[h][columns[h]]
#         # Активируем и проверяем
#         if active(y1) == -1:
#             for h in range(len(yesWeights)):
#                 yesWeights[h] = yesWeights[h] + sp * 2 * yesFire.iloc[h][columns[h]]

# print(yesWeights)
        
