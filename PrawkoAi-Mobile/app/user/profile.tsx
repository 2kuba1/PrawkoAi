import { MaterialCommunityIcons, MaterialIcons } from "@expo/vector-icons";
import AsyncStorage from "@react-native-async-storage/async-storage";
import { getLocales } from "expo-localization";
import { useRouter } from "expo-router";
import { useColorScheme } from "nativewind";
import React, { useContext, useEffect, useState } from "react";
import {
  ActivityIndicator,
  FlatList,
  Modal,
  Platform,
  ScrollView,
  StatusBar,
  Text,
  TouchableOpacity,
  View,
} from "react-native";
import { useSafeAreaInsets } from "react-native-safe-area-context";
import { AuthContext } from "../_layout";
import Footer from "../components/footer";
import { useError } from "../context/errorContext";
import api from "../utils/api";
import i18n from "../utils/translations";

interface Option {
  label: string;
  value: string;
  icon?: string;
  flag?: string;
}

interface ExamHistoryItem {
  examSessionId: string;
  startedAt: string;
  finishedAt: string;
  score?: number;
  isPassed: boolean;
  correctAnswersCount?: number;
}

interface ExamHistoryResponse {
  items: ExamHistoryItem[];
  pageNumber: number;
  totalCount: number;
  totalPages: number;
  hasNextPage: boolean;
}

export default function ProfileScreen() {
  const insets = useSafeAreaInsets();
  const router = useRouter();
  const { user, signOut } = useContext(AuthContext);
  const { setColorScheme } = useColorScheme();
  const { showError } = useError();

  const getNormalizedDeviceLanguage = () => {
    const locale = getLocales()[0];
    if (!locale) return "EN";
    const code = locale.languageCode || locale.languageTag.split("-")[0];
    return code.toUpperCase();
  };

  const languages: Option[] = [
    { label: "Polski", value: "PL", flag: "🇵🇱" },
    { label: "English", value: "EN", flag: "🇬🇧" },
    { label: "Deutsch", value: "DE", flag: "🇩🇪" },
    { label: "Українська", value: "UA", flag: "🇺🇦" },
  ];

  const themes: Option[] = [
    { label: "Jasny", value: "light" },
    { label: "Ciemny", value: "dark" },
  ];

  const [language, setLanguage] = useState({
    label: "Polski",
    value: "PL",
    flag: "🇵🇱",
  });
  const [theme, setTheme] = useState({ label: "Ciemny", value: "dark" });
  const [category, setCategory] = useState({
    label: "Kategoria B",
    value: "B",
  });

  const [modalVisible, setModalVisible] = useState(false);
  const [modalTitle, setModalTitle] = useState("");
  const [currentOptions, setCurrentOptions] = useState<Option[]>([]);
  const [activeType, setActiveType] = useState<"lang" | "theme" | "cat" | null>(
    null,
  );
  const [lastExams, setLastExams] = useState<ExamHistoryItem[]>([]);
  const [isLoading, setIsLoading] = useState(false);
  const [categories, setCategories] = useState<Option[]>([]);

  useEffect(() => {
    const initData = async () => {
      try {
        setIsLoading(true);

        const fetchedCats = await fetchCategories();
        await Promise.all([loadSettings(fetchedCats), fetchLastExams()]);
      } catch (e) {
        showError("Błąd podczas inicjalizacji danych. Spróbuj ponownie");
        console.error("Błąd podczas inicjalizacji danych:", e);
      } finally {
        setIsLoading(false);
      }
    };

    const loadSettings = async (availableCategories: Option[]) => {
      try {
        const [savedLang, savedTheme, savedCat] = await Promise.all([
          AsyncStorage.getItem("user-language"),
          AsyncStorage.getItem("user-theme"),
          AsyncStorage.getItem("user-category"),
        ]);

        const targetLang = savedLang
          ? savedLang.toUpperCase()
          : getNormalizedDeviceLanguage();
        const foundLang =
          languages.find((l) => l.value === targetLang) || languages[0];

        setLanguage({
          label: foundLang.label,
          value: foundLang.value,
          flag: foundLang.flag!,
        });
        i18n.locale = foundLang.value;

        if (savedTheme) {
          const isDark = savedTheme === "dark";
          setTheme(
            isDark
              ? { label: "Ciemny", value: "dark" }
              : { label: "Jasny", value: "light" },
          );
          setColorScheme(isDark ? "dark" : "light");
        }

        if (savedCat) {
          const found = availableCategories.find(
            (c) => c.value === savedCat.toUpperCase(),
          );
          if (found) {
            setCategory(found);
          }
        }
      } catch (e) {
        showError("Nie można załadować ustawień.");
      }
    };

    const fetchLastExams = async () => {
      try {
        const response = await api.get<ExamHistoryResponse>(
          "/api/exam/userHistory",
          {
            params: { userId: user?.id, pageNumber: 1, pageSize: 3 },
          },
        );
        setLastExams(response.data.items || []);
      } catch (e) {
        showError("Nie można pobrać historii egzaminów.");
      }
    };

    const fetchCategories = async (): Promise<Option[]> => {
      try {
        const response = await api.get<string[]>(
          "/api/category/getAllCategoriesNames",
        );
        const newCategories = response.data.map((categoryName) => ({
          label: "Kategoria " + categoryName,
          value: categoryName.toUpperCase(),
        }));

        setCategories(newCategories);
        return newCategories;
      } catch (e) {
        showError("Nie można pobrać kategorii.");
        return [];
      }
    };

    initData();
  }, []);

  const openPicker = (
    title: string,
    type: "lang" | "theme" | "cat",
    options: Option[],
  ) => {
    setModalTitle(title);
    setActiveType(type);
    setCurrentOptions(options);
    setModalVisible(true);
  };

  const handleSelect = async (option: Option) => {
    try {
      if (activeType === "lang") {
        setLanguage({
          label: option.label,
          value: option.value,
          flag: option.flag!,
        });
        i18n.locale = option.value;
        await AsyncStorage.setItem("user-language", option.value);
      } else if (activeType === "theme") {
        setTheme({ label: option.label, value: option.value });
        setColorScheme(option.value as "light" | "dark");
        await AsyncStorage.setItem("user-theme", option.value);
      } else if (activeType === "cat") {
        setCategory({ label: option.label, value: option.value });
        await AsyncStorage.setItem("user-category", option.value);
      }
    } catch (e) {
      showError("Nie można zapisać ustawień. Spróbuj ponownie.");
      console.error("Błąd zapisu:", e);
    }
    setModalVisible(false);
  };

  const paddingTop =
    Platform.OS === "android"
      ? insets.top > 0
        ? insets.top
        : StatusBar.currentHeight || 24
      : insets.top;

  if (isLoading) {
    return (
      <View className="flex-1 items-center justify-center bg-[#f6f6f8] dark:bg-[#111621]">
        <ActivityIndicator size="large" color="#1544b2" />
      </View>
    );
  }

  return (
    <View className="flex-1 bg-[#f6f6f8] dark:bg-[#111621]">
      <StatusBar
        barStyle={theme.value === "dark" ? "light-content" : "dark-content"}
        translucent
        backgroundColor="transparent"
      />

      <View
        style={{ paddingTop }}
        className="bg-white/95 dark:bg-[#1a1f2e] border-b border-blue-900/10 shadow-sm z-50"
      >
        <View className="h-14 flex-row items-center px-3">
          <TouchableOpacity
            className="p-2 rounded-full"
            onPress={() =>
              router.canGoBack() ? router.back() : router.replace("/dashboard")
            }
          >
            <MaterialCommunityIcons
              name="arrow-left"
              size={24}
              color="#1544b2"
            />
          </TouchableOpacity>
          <Text className="text-xl font-bold ml-2 flex-1 text-slate-900 dark:text-white">
            Profil użytkownika
          </Text>
        </View>
      </View>

      <ScrollView
        className="flex-1 px-6"
        showsVerticalScrollIndicator={false}
        contentContainerStyle={{ paddingTop: 24, paddingBottom: 140 }}
      >
        <View className="mb-8">
          <Text className="text-lg font-bold text-slate-800 dark:text-slate-200 mb-4 px-2">
            Ustawienia Nauki
          </Text>
          <View className="bg-white dark:bg-slate-800 rounded-[24px] p-6 shadow-sm border border-blue-900/5 space-y-5">
            <View>
              <View className="flex-row items-center gap-2 mb-3">
                <MaterialIcons name="public" size={20} color="#1544b2" />
                <Text className="text-sm font-semibold text-slate-500">
                  Język aplikacji
                </Text>
              </View>
              <TouchableOpacity
                onPress={() => openPicker("Wybierz język", "lang", languages)}
                className="w-full bg-slate-50 dark:bg-slate-700/50 border border-slate-200 dark:border-slate-600 rounded-2xl py-3.5 px-5 flex-row justify-between items-center"
              >
                <Text className="font-medium text-slate-900 dark:text-white">
                  {language.flag} {language.label}
                </Text>
                <MaterialIcons name="expand-more" size={24} color="#94a3b8" />
              </TouchableOpacity>
            </View>

            <View className="mt-4">
              <View className="flex-row items-center gap-2 mb-3">
                <MaterialIcons name="category" size={20} color="#1544b2" />
                <Text className="text-sm font-semibold text-slate-500">
                  Kategoria prawa jazdy
                </Text>
              </View>
              <TouchableOpacity
                onPress={() =>
                  openPicker("Wybierz kategorię", "cat", categories)
                }
                className="w-full bg-slate-50 dark:bg-slate-700/50 border border-slate-200 dark:border-slate-600 rounded-2xl py-3.5 px-5 flex-row justify-between items-center"
              >
                <Text className="font-medium text-slate-900 dark:text-white">
                  {category.label}
                </Text>
                <MaterialIcons name="expand-more" size={24} color="#94a3b8" />
              </TouchableOpacity>
            </View>

            <View className="mt-4">
              <View className="flex-row items-center gap-2 mb-3">
                <MaterialIcons name="contrast" size={20} color="#1544b2" />
                <Text className="text-sm font-semibold text-slate-500">
                  Motyw
                </Text>
              </View>
              <TouchableOpacity
                onPress={() => openPicker("Wybierz motyw", "theme", themes)}
                className="w-full bg-slate-50 dark:bg-slate-700/50 border border-slate-200 dark:border-slate-600 rounded-2xl py-3.5 px-5 flex-row justify-between items-center"
              >
                <Text className="font-medium text-slate-900 dark:text-white">
                  {theme.label}
                </Text>
                <MaterialIcons name="expand-more" size={24} color="#94a3b8" />
              </TouchableOpacity>
            </View>
          </View>
        </View>

        <View className="mb-8">
          <View className="flex-row items-center justify-between px-2 mb-4">
            <Text className="text-lg font-bold text-slate-800 dark:text-slate-200">
              Historia egzaminów
            </Text>
            <TouchableOpacity onPress={() => router.push("/exam/examHistory")}>
              <Text className="text-[#1544b2] text-sm font-medium">
                Zobacz wszystko
              </Text>
            </TouchableOpacity>
          </View>

          <View className="bg-white dark:bg-slate-800 rounded-[24px] shadow-sm overflow-hidden border border-blue-900/5">
            {lastExams.length === 0 ? (
              <View className="p-6 items-center">
                <MaterialIcons name="history" size={40} color="#94a3b8" />
                <Text className="text-sm text-slate-500 mt-2">
                  Brak historii egzaminów
                </Text>
              </View>
            ) : (
              lastExams.map((exam) => (
                <TouchableOpacity
                  key={exam.examSessionId}
                  className="flex-row items-center justify-between p-5 border-b border-slate-50 dark:border-slate-700"
                  onPress={() =>
                    router.push(`/exam/examResult/${exam.examSessionId}`)
                  }
                >
                  <View className="flex-row items-center gap-4">
                    <View
                      className={`w-10 h-10 rounded-full items-center justify-center ${
                        exam.isPassed
                          ? "bg-green-50 dark:bg-green-900/20"
                          : "bg-red-50 dark:bg-red-900/20"
                      }`}
                    >
                      <MaterialIcons
                        name={
                          exam.isPassed
                            ? "assignment-turned-in"
                            : "assignment-late"
                        }
                        size={22}
                        color={exam.isPassed ? "#16a34a" : "#dc2626"}
                      />
                    </View>
                    <View>
                      <Text className="text-sm font-semibold text-slate-900 dark:text-white">
                        {new Date(exam.startedAt).toLocaleString()}
                      </Text>
                      <Text className="text-xs text-slate-400">
                        Egzamin teoretyczny
                      </Text>
                    </View>
                  </View>
                  <View className="items-end">
                    <Text className="text-lg font-bold text-slate-900 dark:text-white">
                      {exam.score ? `${exam.score}/74` : "Brak wyniku"}
                    </Text>
                    <View
                      className={`${
                        exam.isPassed
                          ? "bg-green-100 dark:bg-green-900/30"
                          : "bg-red-100 dark:bg-red-900/30"
                      } px-3 py-1 rounded-full`}
                    >
                      <Text
                        className={`text-[10px] font-bold uppercase ${
                          exam.isPassed
                            ? "text-green-700 dark:text-green-400"
                            : "text-red-700 dark:text-red-400"
                        }`}
                      >
                        {exam.isPassed ? "POZYTYWNY" : "NEGATYWNY"}
                      </Text>
                    </View>
                  </View>
                </TouchableOpacity>
              ))
            )}
          </View>
        </View>

        <View className="mb-8">
          <Text className="text-lg font-bold text-slate-800 dark:text-slate-200 px-2 mb-3">
            Konto i system
          </Text>
          <View className="bg-white dark:bg-slate-800 rounded-[24px] shadow-sm overflow-hidden border border-blue-900/5">
            <TouchableOpacity className="flex-row items-center justify-between p-5 border-b border-slate-50 dark:border-slate-700">
              <View className="flex-row items-center gap-4">
                <View className="w-10 h-10 rounded-2xl bg-slate-50 dark:bg-slate-700 items-center justify-center">
                  <MaterialIcons
                    name="account-circle"
                    size={24}
                    color="#64748b"
                  />
                </View>
                <Text className="font-medium text-slate-900 dark:text-white">
                  Ustawienia konta
                </Text>
              </View>
              <MaterialIcons name="chevron-right" size={24} color="#cbd5e1" />
            </TouchableOpacity>

            <TouchableOpacity
              className="flex-row items-center p-5"
              onPress={signOut}
            >
              <View className="w-10 h-10 rounded-2xl bg-red-50 dark:bg-red-900/10 items-center justify-center">
                <MaterialIcons name="logout" size={24} color="#ef4444" />
              </View>
              <Text className="font-medium text-red-600 ml-4">Wyloguj</Text>
            </TouchableOpacity>
          </View>
        </View>
      </ScrollView>

      <Modal
        animationType="slide"
        transparent={true}
        visible={modalVisible}
        onRequestClose={() => setModalVisible(false)}
      >
        <View className="flex-1 justify-end bg-black/50">
          <View className="bg-white dark:bg-[#1a1f2e] rounded-t-[32px] p-6 pb-12">
            <View className="flex-row justify-between items-center mb-6">
              <Text className="text-xl font-bold text-slate-900 dark:text-white">
                {modalTitle}
              </Text>
              <TouchableOpacity
                onPress={() => setModalVisible(false)}
                className="p-2"
              >
                <MaterialIcons name="close" size={24} color="#94a3b8" />
              </TouchableOpacity>
            </View>

            <FlatList
              data={currentOptions}
              keyExtractor={(item) => item.value}
              renderItem={({ item }) => {
                const isActive =
                  (activeType === "lang" && language.value === item.value) ||
                  (activeType === "theme" && theme.value === item.value) ||
                  (activeType === "cat" && category.value === item.value);

                return (
                  <TouchableOpacity
                    className={`py-4 px-4 mb-2 rounded-2xl flex-row items-center justify-between ${isActive ? "bg-blue-50 dark:bg-blue-900/20" : ""}`}
                    onPress={() => handleSelect(item)}
                  >
                    <Text
                      className={`text-base ${isActive ? "font-bold text-[#1544b2] dark:text-blue-400" : "text-slate-700 dark:text-slate-200"}`}
                    >
                      {item.flag ? `${item.flag}  ` : ""}
                      {item.label}
                    </Text>
                    {isActive && (
                      <MaterialIcons
                        name="check-circle"
                        size={20}
                        color="#1544b2"
                      />
                    )}
                  </TouchableOpacity>
                );
              }}
            />
          </View>
        </View>
      </Modal>

      <Footer />
    </View>
  );
}
