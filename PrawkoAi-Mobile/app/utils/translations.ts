import { I18n } from "i18n-js";

const translations = {
  PL: {
    // Login Screen
    welcome: "Zacznij naukę na prawo jazdy z pomocą AI",
    google_btn: "Kontynuuj z Google",
    guest_btn: "Wypróbuj jako gość",
    footer: "Inteligentny Asystent Kierowcy",

    // Dashboard - Header & Stats
    premium_account: "Konto Premium",
    learning_progress: "Twój progres nauki",
    questions_mastered: "pytań opanowanych",
    today: "dziś",
    streak: "Streak",
    days: "Dni",
    avg_score: "Średni wynik",

    // Dashboard - AI Card
    ai_suggestion: "Sugestia AI",
    daily_plan: "Dzienny plan",
    ai_recommendation_text:
      "Skup się dzisiaj na dziale skrzyżowania, tam masz najwięcej błędów.",
    train_now: "Trenuj teraz",

    // Dashboard - Menu
    main_menu: "Menu główne",
    start_learning: "Zacznij naukę",
    continue_course: "Kontynuuj kurs",
    exam_simulation: "Symulacja egzaminu WORD",
    official_database: "Oficjalna baza pytań",
    logout: "Wyloguj się",

    // Navigation
    nav_home: "Home",
    nav_stats: "Statystyki",
    nav_learn: "Nauka",
    nav_school: "Szkoła",
    nav_profile: "Profil",
  },
  EN: {
    welcome: "Start learning for your driving license with AI",
    google_btn: "Continue with Google",
    guest_btn: "Try as guest",
    footer: "Intelligent Driver Assistant",

    premium_account: "Premium Account",
    learning_progress: "Your learning progress",
    questions_mastered: "questions mastered",
    today: "today",
    streak: "Streak",
    days: "Days",
    avg_score: "Avg score",

    ai_suggestion: "AI Suggestion",
    daily_plan: "Daily plan",
    ai_recommendation_text:
      "Focus today on the intersections section, that's where you have the most mistakes.",
    train_now: "Train now",

    main_menu: "Main menu",
    start_learning: "Start learning",
    continue_course: "Continue course",
    exam_simulation: "WORD exam simulation",
    official_database: "Official question bank",
    logout: "Log out",

    nav_home: "Home",
    nav_stats: "Stats",
    nav_learn: "Learn",
    nav_school: "School",
    nav_profile: "Profile",
  },
  DE: {
    welcome: "Lerne für deinen Führerschein mit AI",
    google_btn: "Mit Google fortfahren",
    guest_btn: "Als Gast versuchen",
    footer: "Intelligenter Fahrer-Assistent",

    premium_account: "Premium-Konto",
    learning_progress: "Dein Lernfortschritt",
    questions_mastered: "Fragen gemeistert",
    today: "heute",
    streak: "Streak",
    days: "Tage",
    avg_score: "Durchschnitt",

    ai_suggestion: "KI-Vorschlag",
    daily_plan: "Tagesplan",
    ai_recommendation_text:
      "Konzentrieren Sie sich heute auf den Bereich Kreuzungen, dort machen Sie die meisten Fehler.",
    train_now: "Jetzt trainieren",

    main_menu: "Hauptmenü",
    start_learning: "Lernen starten",
    continue_course: "Kurs fortsetzen",
    exam_simulation: "WORD Prüfungssimulation",
    official_database: "Offizielle Fragenbank",
    logout: "Abmelden",

    nav_home: "Home",
    nav_stats: "Statistiken",
    nav_learn: "Lernen",
    nav_school: "Fahrschule",
    nav_profile: "Profil",
  },
  UA: {
    welcome: "Почніть навчання на посвідчення водія з AI",
    google_btn: "Продовжити з Google",
    guest_btn: "Спробувати як гість",
    footer: "Інтелектуальний помічник водія",

    premium_account: "Преміум акаунт",
    learning_progress: "Ваш прогрес навчання",
    questions_mastered: "питань освоєно",
    today: "сьогодні",
    streak: "Серія",
    days: "Днів",
    avg_score: "Сер. результат",

    ai_suggestion: "Пропозиція AI",
    daily_plan: "Денний план",
    ai_recommendation_text:
      "Зосередьтеся сьогодні на розділі перехрестя, там у вас найбільше помилок.",
    train_now: "Тренуватися зараз",

    main_menu: "Головне меню",
    start_learning: "Почати навчання",
    continue_course: "Продовжити курс",
    exam_simulation: "Симуляція іспиту WORD",
    official_database: "Офіційна база питань",
    logout: "Вийти",

    nav_home: "Головна",
    nav_stats: "Статистика",
    nav_learn: "Навчання",
    nav_school: "Автошкола",
    nav_profile: "Профіль",
  },
};

const i18n = new I18n(translations);
i18n.enableFallback = true;
i18n.defaultLocale = "PL";

export default i18n;
