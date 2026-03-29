import google.generativeai as genai
import time
import os

API_KEYS = [

]

INPUT_FILE = r"C:\Users\jakub\Desktop\XlsxReader\script\questions.txt"
OUTPUT_FILE = "questions_processed.txt"
MEDIA_FOLDER = r"D:\multimedia_do_pytan"
MODEL_NAME = 'gemini-3.1-flash-lite-preview'

PROMPT =  """Jesteś ekspertem ds. polskiego prawa jazdy i analizy obrazu. Twoim zadaniem jest obiektywna analiza załączonego materiału wizualnego oraz danych tekstowych w kontekście pytania egzaminacyjnego.



Zasady odpowiedzi:



1. Kopiowanie danych: Odpowiedź musi zaczynać się od dokładnego przepisania całego ciągu wejściowego otrzymanego od użytkownika (np. 99.0@ Czy... @ A,B,C).



2. Obiektywna analiza: Skoncentruj się wyłącznie na faktach widocznych na obrazie: rozmieszczeniu znaków, sygnałach świetlnych, zachowaniu pieszych oraz trajektorii innych pojazdów.



3. Zakaz instruowania: Nie używaj zwrotów „musisz” ani „powinieneś”. Zamiast pisać „powinieneś się zatrzymać”, opisz sytuację: „Sygnalizator nadaje sygnał czerwony, co oznacza zakaz wjazdu za sygnalizator”.



4. Struktura końcowa: Po przepisaniu ciągu danych dodaj analizę w formacie: DANE_WEJSCIOWE_OD_UZYTKOWNIKA@AI_CONTEXT@KATEGORIA@STATIC_RESPONSE



Wytyczne do sekcji:



1. AI Context (2-4 zdania): Opisz kluczowe elementy wizualne determinujące odpowiedź. Wyjaśnij relację między uczestnikami ruchu na podstawie przepisów oraz zidentyfikuj zagrożenia lub ograniczenia widoczne w klatce obrazu.



2. Kategorie do wyboru: [Intersections, RoadSigns, PedestriansAndCyclists, RailwayCrossings, Priority, BuiltupArea, MotorwaysAndExpressways, TechniqueAndSafety].



3. Static Response: Podaj konkretną podstawę prawną lub szczegółowe wyjaśnienie przepisów, które determinują prawidłową odpowiedź (np. przywołaj konkretny artykuł z Prawa o Ruchu Drogowym i dopisz przyjazne wyjaśnienie).



Przykład poprawnej odpowiedzi:

99.0@ Czy w tej sytuacji masz obowiązek zatrzymać pojazd?@ nan@ nan@ nan@ T@ AK_D05_06_org.wmv@ PODSTAWOWY@ 3.0@ A,B,C,D,T,AM,A1,A2,B1,C1,D1

Czy w tej sytuacji masz obowiązek zatrzymać pojazd? @ Na nagraniu widoczny jest tramwaj zatrzymujący się na przystanku, który nie jest wyposażony w wysepkę dla pasażerów, co zmusza ich do wchodzenia na jezdnię bezpośrednio z chodnika. Sytuacja ta stwarza konieczność zapewnienia bezpieczeństwa osobom przekraczającym torowisko i jezdnię. @ PedestriansAndCyclists @ Zgodnie z Art. 26 ust. 6 ustawy Prawo o ruchu drogowym, przy braku wysepki pasażerskiej na przystanku, kierujący jest zobowiązany do zatrzymania pojazdu w celu zapewnienia bezpiecznego i bezkolizyjnego dojścia pasażerów do tramwaju lub chodnika.



pytania maja byc od razu cale do skopiowania nie rob odsetpow pomiedzy @ format ma byc wszystko co ja ci przyslalem @ai context@kategoria@static response



JEŻELI PYTANIE NIE MA ZDJECIA/NAGRANIA (jest nan) TO I TAK DODAJ ANALIZE AI I KATEGORIE I STATIC RESPONSE

Format: DANE_WEJSCIOWE@AI_CONTEXT@KATEGORIA@STATIC_RESPONSE (bez pogrubień i spacji przy @)

"""

current_key_index = 0

def get_model():
    global current_key_index
    api_key = API_KEYS[current_key_index]
    print(f"\n[SYSTEM] Inicjalizacja klucza nr {current_key_index + 1}...")
    genai.configure(api_key=api_key)
    return genai.GenerativeModel(MODEL_NAME)

def rotate_key():
    global current_key_index
    if current_key_index < len(API_KEYS) - 1:
        current_key_index += 1
        return True
    return False

def process_questions():
    global current_key_index
    if not os.path.exists(INPUT_FILE):
        print(f"Błąd: Nie znaleziono pliku {INPUT_FILE}")
        return

    model = get_model()

    with open(INPUT_FILE, "r", encoding="utf-8") as f:
        lines = f.readlines()

    done_ids = set()
    if os.path.exists(OUTPUT_FILE):
        with open(OUTPUT_FILE, "r", encoding="utf-8") as f:
            for l in f:
                if "@" in l: 
                    done_ids.add(l.split("@")[0].strip())

    with open(OUTPUT_FILE, "a", encoding="utf-8") as out_f:
        for line in lines:
            line = line.strip()
            if not line: continue
            
            parts = line.split("@")
            q_id = parts[0].strip()
            
            if q_id in done_ids:
                continue

            media_filename = parts[6].strip() if len(parts) > 6 else "nan"
            media_path = os.path.join(MEDIA_FOLDER, media_filename)

            success = False
            while not success:
                uploaded_file = None
                try:
                    print(f"Przetwarzanie {q_id}: {media_filename}...", end=" ", flush=True)
                    content = [PROMPT, line]
                    
                    if media_filename != "nan" and os.path.exists(media_path):
                        uploaded_file = genai.upload_file(path=media_path)
                        while uploaded_file.state.name == "PROCESSING":
                            time.sleep(2)
                            uploaded_file = genai.get_file(uploaded_file.name)
                        content.append(uploaded_file)

                    response = model.generate_content(content)
                    
                    if not response.parts: 
                        result = f"{line}@Brak analizy (Safety Filter)@Safety@Zasady bezpieczeństwa uniemożliwiły analizę."
                    else:
                        result = response.text.strip()

                    out_f.write(result + "\n")
                    out_f.flush()
                    
                    if uploaded_file:
                        genai.delete_file(uploaded_file.name)
                    
                    print("OK")
                    success = True

                except Exception as e:
                    if uploaded_file:
                        try: genai.delete_file(uploaded_file.name)
                        except: pass
                    
                    err = str(e)
                    if "429" in err:
                        print("\n[LIMIT] Wykryto 429 (Rate Limit).")
                        if rotate_key():
                            model = get_model()
                            time.sleep(1)
                            continue 
                        else:
                            print("[SYSTEM] Wszystkie klucze wykorzystane. Czekam 60s...")
                            time.sleep(60)
                            continue
                    else:
                        print(f"\n[BŁĄD] Pomininięto {q_id} z powodu: {err}")

if __name__ == "__main__":
    process_questions()