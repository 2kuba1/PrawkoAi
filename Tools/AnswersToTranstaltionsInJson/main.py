import pandas as pd
import json
import re

XLS_PATH = r""
TXT_PATH = r""
OUTPUT_JSON = 'output_questions.json'

def merge_questions_to_json(txt_file_path, excel_file_path, output_json_path):
    txt_rows = []
    
    try:
        with open(txt_file_path, 'r', encoding='utf-8') as f:
            content = f.read()
            
        raw_blocks = re.split(r'\n(?=\d+\.0@)', content.strip())
        
        for block in raw_blocks:
            if not block.strip():
                continue
            
            clean_block = block.replace('\n', ' ').replace('\r', ' ').strip()
            
            parts = [p.strip() for p in clean_block.split('@')]
            if parts:
                txt_rows.append(parts)
                
    except FileNotFoundError:
        print(f"BŁĄD: Nie znaleziono pliku TXT: {txt_file_path}")
        return
    except Exception as e:
        print(f"Wystąpił nieoczekiwany błąd przy czytaniu TXT: {e}")
        return

    try:
        df_excel = pd.read_excel(excel_file_path)
    except FileNotFoundError:
        print(f"BŁĄD: Nie znaleziono pliku Excel: {excel_file_path}")
        return

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
        raw_id = str(row[0])
        question_id = raw_id.replace('.0', '').strip()
        
        excel_match = df_excel[df_excel[excel_key] == question_id]
        
        combined_data = list(row)
        
        if not excel_match.empty:
            for col_name in target_columns:
                alt_name = col_name.replace('ź', 'z')
                
                found_val = ""
                if col_name in df_excel.columns:
                    found_val = excel_match[col_name].iloc[0]
                elif alt_name in df_excel.columns:
                    found_val = excel_match[alt_name].iloc[0]
                
                combined_data.append(str(found_val) if pd.notna(found_val) else "")
        else:
            combined_data.extend([""] * 12)

        final_output.append({
            "id": question_id,
            "data": combined_data
        })

    with open(output_json_path, 'w', encoding='utf-8') as jf:
        json.dump(final_output, jf, indent=4, ensure_ascii=False)

    print("-" * 30)
    print(f"PROCES ZAKOŃCZONY POMYŚLNIE")
    print(f"Przetworzono pytań: {len(final_output)}")
    print(f"Plik wynikowy: {output_json_path}")
    print("-" * 30)

if __name__ == "__main__":
    merge_questions_to_json(TXT_PATH, XLS_PATH, OUTPUT_JSON)