#pip install pandas openpyxl
import pandas as pd

xlspath = r""

xls = pd.read_excel(xlspath, usecols=['Numer pytania', 'Pytanie', 'Odpowiedź A', 'Odpowiedź B', 'Odpowiedź C', 'Poprawna odp', 'Media', 'Zakres struktury', 'Liczba punktów', 'Kategorie'])
print(xls.head())

with open("questions.txt", "a", encoding="utf-8") as f:
    for _, row in xls.iterrows():
        f.write("@ ".join(str(item) for item in row) + "\n")