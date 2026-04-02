# pip install pandas openpyxl
import pandas as pd

xlspath = r"C:\Users\jakub\Desktop\katalog_dla_kandydatów_na_kierowców_0220266.xlsx"

xls = pd.read_excel(xlspath, usecols=[
    'Numer pytania', 'Pytanie', 'Odpowiedź A', 'Odpowiedź B', 'Odpowiedź C', 
    'Poprawna odp', 'Media', 'Zakres struktury', 'Liczba punktów', 'Kategorie', 
    'Pytanie [EN]', 'Odpowiedź A [EN]', 'Odpowiedź B [EN]', 'Odpowiedź C [EN]', 
    'Pytanie [D]', 'Odpowiedź A [D]', 'Odpowiedź B [D]', 'Odpowiedź C [D]', 
    'Pytanie [UA]', 'Odpowiedź A [UA]', 'Odpowiedź B [UA]', 'Odpowiedź C [UA]'
])

print(xls.head())

with open("questions.txt", "a", encoding="utf-8") as f:
    for _, row in xls.iterrows():
        processed_row = [str(item).strip() for item in row]
        f.write("@".join(processed_row) + "\n")