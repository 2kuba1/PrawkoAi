from urllib import response

import google.generativeai as genai
import time
import os

API_KEYS = [

]

INPUT_FILE = r"C:\Users\jakub\Desktop\PrawkoAi\Tools\AiQuestionAnalyzer\script\questions.txt"
OUTPUT_FILE = "questions_processed.txt"
MEDIA_FOLDER = r"D:\multimedia_do_pytan"
MODEL_NAME = 'gemini-3.1-flash-lite-preview'

PROMPT = """
Jesteś robotem przetwarzającym dane (Data Processor). Analizujesz pytania na prawo jazdy i załączone do nich materiały wizualne.

kazde pytanie które ci wysyłam sklada sie z kilku czesci oddzielonych znakiem @:
1. ID pytania (np. 99.0)
2. Pytanie PL - pytanie w jezyku polskim, moze byc nan jesli brak
3. Odpowiedz PL A - moze byc nan
4. Odpowiedz PL B - moze byc nan
5. Odpowiedz PL C - moze byc nan
6. Correct answer (np. "A", "B", "C", "T", "N")
7. Nazwa pliku z materialem wizualnym (np. "AK_D11_43org.wmv" lub "nan" jeśli brak)
8. Kategoria pytania (np. "PODSTAWOWY", "SPECJALISTYCZNY")
9. Ilość punktów za poprawną odpowiedź (np. "3.0")
10. Kategoria prawa jazdy (np. "A", "B", "C", "D", "T", "AM", "A1", "A2", "B1", "C1", "D1")
11. Pytanie EN - pytanie przetłumaczone na angielski, może być nan jeśli brak
12. Odpowiedz EN A - moze byc nan 
13. Odpowiedz EN B - moze byc nan
14. Odpowiedz EN C - moze byc nan
15. Pytanie DE - pytanie przetłumaczone na niemiecki, może być nan jeśli brak
16. Odpowiedz DE A - moze byc nan
17. Odpowiedz DE B - moze byc nan
18. Odpowiedz DE C - moze byc nan
19. Pytanie UA - pytanie przetłumaczone na ukraiński, może być nan lub puste jeśli brak
20. Odpowiedz UA A - moze byc nan lub puste 
21. Odpowiedz UA B - moze byc nan lub puste 
22. Odpowiedz UA C - moze byc nan lub puste 

NAJWAZNIEJSZE! Nie dodawaj żadnych numerów typu 00:06 nie mozesz nigdzie dodac time stampu bo rozwalisz tym prace, NIE DAWAJ NIGDZIE ENTERA!,nie wklejej nigdzie indziej nazwy nagrania/filmu. Nie wymyślaj sobie żadnych dodatkowych informacji, nie zakładaj niczego, co nie jest widoczne na obrazie. Odpowiadaj tylko na podstawie tego, co jest faktycznie pokazane.
DO TEGO NIE MOŻESZ WPISYWAĆ NIGDZIE @AI_CONTEXT, @KATEGORIA, @STATIC_RESPONSE_PL, @STATIC_RESPONSE_EN, @STATIC_RESPONSE_DE, @STATIC_RESPONSE_UA - to są tylko znaczniki do oddzielenia poszczególnych sekcji odpowiedzi, nie możesz ich używać w treści odpowiedzi.

MASZ KATEGORYCZNY ZAKAZ ROBIENIA NOWYCH LINI I DOPISYWANIA RZECZY TYPU ai context i innych wariantow tego typu

nie dodawaj nic swojego do tej struktury. struktura ma byc taka jak w przykladzie ma byc dokladnie tyle samo znakow @ ani jednego mniej ani jednego wiecej

DLA JEZYKA UKRAINSKIEGO UZYWAJ CYRLICY!

JEŻELI pytanie nie ma załączonego materiału wizualnego (.jpg/.wmv tylko jest "nan"), to i tak przeprowadź analizę na podstawie samego tekstu pytania (nie dodawaj zadnego NAPISU typu @ai_context@ ani @nan@nan@nan w loopie bo to psuje aplikacje)

Zasady odpowiedzi:

1. Kopiowanie danych: Odpowiedź musi zaczynać się od dokładnego przepisania całego ciągu wejściowego otrzymanego od użytkownika (np. 99.0@Czy... @A,B,C).

2. Obiektywna analiza: Skoncentruj się wyłącznie na faktach widocznych na obrazie: rozmieszczeniu znaków, sygnałach świetlnych, zachowaniu pieszych oraz trajektorii innych pojazdów.

3. Zakaz instruowania: Nie używaj zwrotów „musisz” ani „powinieneś”. Zamiast pisać „powinieneś się zatrzymać”, opisz sytuację: „Sygnalizator nadaje sygnał czerwony, co oznacza zakaz wjazdu za sygnalizator”.

4. Struktura końcowa: Po przepisaniu ciągu danych dodaj analizę w formacie: DANE_WEJSCIOWE_OD_UZYTKOWNIKA@AI_CONTEXT@KATEGORIA@STATIC_RESPONSE_PL@STATIC_RESPONSE_EN@STATIC_RESPONSE_DE@STATIC_RESPONSE_UA

Wytyczne do sekcji:

1. AI Context (2-4 zdania): Opisz kluczowe elementy wizualne determinujące odpowiedź. Wyjaśnij relację między uczestnikami ruchu na podstawie przepisów oraz zidentyfikuj zagrożenia lub ograniczenia widoczne w klatce obrazu.

2. KATEGORIA Wybierz dokładnie jedną z listy: [MandatoryAndWarningSigns, InformationAndRoadMarkings, UncontrolledAndPriorityIntersections, SignalizedIntersectionsAndPedestrians, ManoeuvresAndPositioning, OvertakingAndPassing, VehicleLightsAndSignals, SocialBehaviourAndSecuring, RailCrossingsAndPublicTransport, EmergencyAndFitnessToDrive, SpeedAndBrakingDistances, SafetyFirstAidAndDocuments].

3. Static Response Pl: Podaj konkretną podstawę prawną lub szczegółowe wyjaśnienie przepisów, które determinują prawidłową odpowiedź (np. przywołaj konkretny artykuł z Prawa o Ruchu Drogowym i dopisz przyjazne wyjaśnienie).

4. Static Response En: Przetlumacz to co napisałeś w Static Response Pl na język angielski.

5. Static Response De: Przetlumacz to co napisałeś w Static Response Pl na język niemiecki.

6. Static Response UA: Przetlumacz to co napisałeś w Static Response Pl na język ukraiński.


Przykład poprawnej odpowiedzi:
109.0@Czy w przedstawionej sytuacji masz prawo kontynuować jazdę?@nan@nan@nan@N@AK_D11_43org.wmv@PODSTAWOWY@3.0@A,B,C,D,T,AM,A1,A2,B1,C1,D1@Can you continue driving in this situation?@nan@nan@nan@Darfst du in der dargestellten Situation die Fahrt fortsetzen?@nan@nan@nan@Чи Ви маєте право продовжувати рух у представленій ситуації?@nan@nan@nan@Na drodze znajduje się oznakowane przejście dla pieszych, na którym osoba uprawniona (funkcjonariusz lub osoba nadzorująca bezpieczny przejazd dzieci) używa znaku STOP w celu zatrzymania ruchu. Widoczny sygnał nakazuje kierującemu pojazdem bezwzględne zatrzymanie się przed przejściem. Kontynuowanie jazdy w tym momencie stanowi naruszenie przepisów i bezpośrednie zagrożenie bezpieczeństwa osób znajdujących się na drodze.@PedestriansAndCyclists@Zgodnie z art. 5 ust. 1 ustawy Prawo o ruchu drogowym, uczestnik ruchu jest obowiązany stosować się do poleceń i sygnałów dawanych przez osoby uprawnione do kierowania ruchem. Znak STOP trzymany przez osobę kierującą ruchem jest wiążącym sygnałem do zatrzymania pojazdu, co oznacza, że kontynuowanie jazdy jest zabronione do czasu zmiany sygnału lub wydania przez tę osobę polecenia umożliwiającego przejazd.@According to Art. 5 paragraph 1 of the Road Traffic Act, a road user is obliged to comply with the commands and signals given by persons authorized to direct traffic. The STOP sign held by a person directing traffic is a binding signal to stop the vehicle, which means that continuing to drive is prohibited until the signal is changed or the person gives an order allowing passage.@Gemäß Art. 5 Abs. 1 der Straßenverkehrsordnung ist ein Verkehrsteilnehmer verpflichtet, die Anweisungen und Signale von Personen zu befolgen, die zur Verkehrsregelung befugt sind. Das von einer Person, die den Verkehr regelt, gehaltene STOP-Schild ist ein verbindliches Signal zum Anhalten des Fahrzeugs, was bedeutet, dass die Weiterfahrt untersagt ist, bis das Signal geändert wird oder die Person ein Signal zur Weiterfahrt gibt.@Згідно зі ст. 5 ч. 1 Закону про дорожній рух, учасник дорожнього руху зобов'язаний підпорядковуватися наказам та сигналам, що подаються особами, уповноваженими керувати дорожнім рухом. Знак STOP, який тримає особа, що керує рухом, є обов'язковим сигналом до зупинки транспортного засобу, а це означає, що продовжувати рух заборонено до моменту зміни сигналу або надання цією особою дозволу на проїзд.

NIE UZYWAJ LIST POGRUBIONEGO TEKSTU ANI ZADNYCH TAKICH UPIĘKSZACZY TEKSTU
pytania maja byc od razu cale do skopiowania nie rob odsetpow pomiedzy @ format ma byc wszystko co ja ci przyslalem @ai context@kategoria@static response

JEŻELI PYTANIE NIE MA ZDJECIA/NAGRANIA (jest nan) TO I TAK DODAJ ANALIZE AI I KATEGORIE I STATIC RESPONSE

RESTRYKCJE:
- ZERO nowych linii (\n). Cała odpowiedź musi być w JEDNEJ LINII.
- ZERO pogrubień (**), list, czy dodatkowych komentarzy.
- Liczba znaków @ w Twojej odpowiedzi musi wynosić dokładnie: (liczba @ w INPUT) + 6.
- Nie używaj słów "AI Context:", "Kategoria:" w treści – wstawiaj tylko czysty tekst między znakami @.

FORMAT WYJŚCIOWY (BEZWZGLĘDNY):
ID pytania (np. 99.0)@Pytanie PL - pytanie w jezyku polskim, moze byc nan jesli brak@Odpowiedz PL A - moze byc nan@Odpowiedz PL B - moze byc nan@Odpowiedz PL C - moze byc nan@Correct answer (np. "A", "B", "C", "T", "N")@Nazwa pliku z materialem wizualnym (np. "AK_D11_43org.wmv" lub "nan" jeśli brak)@Kategoria pytania (np. "PODSTAWOWY", "SPECJALISTYCZNY")@Ilość punktów za poprawną odpowiedź (np. "3.0")@Kategoria prawa jazdy (np. "A", "B", "C", "D", "T", "AM", "A1", "A2", "B1", "C1", "D1")@Pytanie EN - pytanie przetłumaczone na angielski, może być nan jeśli brak@Odpowiedz EN A - moze byc nan@Odpowiedz EN B - moze byc nan@Odpowiedz EN C - moze byc nan@Pytanie DE - pytanie przetłumaczone na niemiecki, może być nan jeśli brak@Odpowiedz DE A - moze byc nan@Odpowiedz DE B - moze byc nan@Odpowiedz DE C - moze byc nan@Pytanie UA - pytanie przetłumaczone na ukraiński, może być nan lub puste jeśli brak@Odpowiedz UA A - moze byc nan lub puste@Odpowiedz UA B - moze byc nan lub puste@Odpowiedz UA C - moze byc nan lub puste@Ai context@Kategoria@Static response pl@Static response en@Static response de@Static respone ua
FORMAT WYJŚCIOWY (BEZWZGLĘDNY):
{INPUT}@AI_CONTEXT@KATEGORIA@STATIC_RESPONSE_PL@STATIC_RESPONSE_EN@STATIC_RESPONSE_DE@STATIC_RESPONSE_UA

INPUT:
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
                    full_prompt = f"{PROMPT}\n{line}"
                    content = [full_prompt]  

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
                        result = response.text.replace('\n', ' ').strip()

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