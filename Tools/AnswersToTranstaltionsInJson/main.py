import pandas as pd
import json

XLS_PATH = r"C:\Users\jakub\Desktop\katalog_dla_kandydatów_na_kierowców_0220266.xlsx"
TXT_PATH = r"C:\Users\jakub\Desktop\XlsxReader\script\questions_processed.txt"

def merge_questions_to_json(txt_file_path, excel_file_path, output_json_path):
    txt_rows = []
    with open(txt_file_path, 'r', encoding='utf-8') as f:
        for line in f:
            if line.strip():
                parts = [p.strip() for p in line.strip().split('@')]
                txt_rows.append(parts)

    df_excel = pd.read_excel(excel_file_path)
    df_excel.columns = [str(c).strip() for c in df_excel.columns]

    excel_key = 'Numer pytania'
    
    df_excel[excel_key] = df_excel[excel_key].astype(str).str.replace('.0', '', regex=False).str.strip()

    target_columns = [
        'Pytanie [EN]', 'Odpowiedź A [EN]', 'Odpowiedź B [EN]', 'Odpowiedź C [EN]',
        'Pytanie [D]', 'Odpowiedź A [D]', 'Odpowiedź B [D]', 'Odpowiedź C [D]',
        'Pytanie [UA]', 'Odpowiedź A [UA]', 'Odpowiedź B [UA]', 'Odpowiedź C [UA]'
    ]

    final_output = []

    for row in txt_rows:
        question_no_from_txt = str(row[0]).replace('.0', '').strip()
        
        excel_match = df_excel[df_excel[excel_key] == question_no_from_txt]
        
        combined_data = list(row)
        
        if not excel_match.empty:
            for col_name in target_columns:
                alt_name = col_name.replace('ź', 'z')
                
                if col_name in df_excel.columns:
                    val = excel_match[col_name].iloc[0]
                    combined_data.append(str(val) if pd.notna(val) else "")
                elif alt_name in df_excel.columns:
                    val = excel_match[alt_name].iloc[0]
                    combined_data.append(str(val) if pd.notna(val) else "")
                else:
                    combined_row.append("")
        else:
            combined_data.extend([""] * 12)

        final_output.append({
            "id": question_no_from_txt,
            "data": combined_data
        })

    with open(output_json_path, 'w', encoding='utf-8') as jf:
        json.dump(final_output, jf, indent=4, ensure_ascii=False)

    print(f"Sukces! Połączono dane używając kolumny '{excel_key}'.")
    print(f"Zapisano {len(final_output)} pytań do {output_json_path}")

if __name__ == "__main__":
    merge_questions_to_json(TXT_PATH, XLS_PATH, 'output_questions.json')