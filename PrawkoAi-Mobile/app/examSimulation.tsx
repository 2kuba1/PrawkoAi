import { MaterialIcons } from "@expo/vector-icons";
import { Stack, useRouter } from "expo-router";
import React, { useContext, useEffect, useMemo, useState } from "react";
import {
  ActivityIndicator,
  ImageBackground,
  ScrollView,
  StatusBar,
  Text,
  TouchableOpacity,
  View,
} from "react-native";
import {
  SafeAreaProvider,
  useSafeAreaInsets,
} from "react-native-safe-area-context";
import { AuthContext } from "./_layout";

export interface Answer {
  id: string;
  questionId: string;
  content: string;
  createdAt: string;
}

export interface Question {
  id: string;
  content: string;
  questionNumber: number;
  structureScope: "standard" | "specialized";
  points: 1 | 2 | 3;
  mediaUrl: string;
  answers: Answer[];
}

export interface ExamSession {
  id: string;
  startedAt: string;
}

export interface ExamData {
  examSession: ExamSession;
  examQuestions: {
    standard: Question[];
    specialized: Question[];
  };
}

export default function ExamSimulationScreen() {
  return (
    <SafeAreaProvider>
      <Stack.Screen options={{ headerShown: false }} />
      <ExamSimulation />
    </SafeAreaProvider>
  );
}

function ExamSimulation() {
  const insets = useSafeAreaInsets();
  const router = useRouter();
  const { token, user } = useContext(AuthContext);

  const [examData, setExamData] = useState<ExamData | null>(null);
  const [loading, setLoading] = useState(true);
  const [currentIndex, setCurrentIndex] = useState(0);
  const [currentScope, setCurrentScope] = useState<"standard" | "specialized">(
    "standard",
  );
  const [selectedAnswerId, setSelectedAnswerId] = useState<string | null>(null);

  useEffect(() => {
    const fetchExamData = async () => {
      try {
        const response = await fetch(
          `https://amenities-implies-acquisition-drawings.trycloudflare.com/api/exam/start?userId=${user?.id}`,
          {
            method: "GET",
            headers: {
              "Content-Type": "application/json",
              Authorization: `Bearer ${token?.accessToken}`,
            },
          },
        );
        const result = await response.json();
        setExamData(result);
      } catch (e) {
        console.error("Błąd pobierania danych:", e);
      } finally {
        setLoading(false);
      }
    };
    fetchExamData();
  }, []);

  const questions = useMemo(() => {
    if (!examData) return [];
    return currentScope === "standard"
      ? examData.examQuestions.standard
      : examData.examQuestions.specialized;
  }, [examData, currentScope]);

  const currentQuestion = questions[currentIndex];

  const sortedAnswers = useMemo(() => {
    if (!currentQuestion?.answers) return [];

    const answers = [...currentQuestion.answers];

    if (currentScope === "standard" && answers.length === 2) {
      return answers.sort((a, b) => {
        const order = ["tak", "nie"];
        return (
          order.indexOf(a.content.toLowerCase()) -
          order.indexOf(b.content.toLowerCase())
        );
      });
    }

    return answers;
  }, [currentQuestion, currentScope]);

  const totalStandard = examData?.examQuestions.standard.length || 0;
  const totalSpecialized = examData?.examQuestions.specialized.length || 0;
  const globalProgress =
    currentScope === "standard"
      ? currentIndex + 1
      : totalStandard + currentIndex + 1;
  const totalQuestions = totalStandard + totalSpecialized;

  const handleNext = () => {
    setSelectedAnswerId(null);

    if (currentIndex < questions.length - 1) {
      setCurrentIndex(currentIndex + 1);
    } else if (currentScope === "standard") {
      setCurrentScope("specialized");
      setCurrentIndex(0);
    } else {
      alert("Egzamin zakończony! Wyniki zostaną przeliczone.");
      router.back();
    }
  };

  if (loading) {
    return (
      <View className="flex-1 items-center justify-center bg-white dark:bg-slate-950">
        <ActivityIndicator size="large" color="#1544b2" />
        <Text className="mt-4 text-slate-500 font-medium">
          Generowanie arkusza...
        </Text>
      </View>
    );
  }

  if (!currentQuestion) return null;

  return (
    <View className="flex-1 bg-[#f8fafc] dark:bg-[#0f172a]">
      <StatusBar barStyle="dark-content" />

      {/* --- HEADER --- */}
      <View
        style={{ paddingTop: insets.top }}
        className="flex-row items-center justify-between px-4 py-4 bg-white dark:bg-slate-900 border-b border-slate-200 dark:border-slate-800 shadow-sm"
      >
        <View className="flex-row items-center gap-3">
          <TouchableOpacity
            onPress={() => router.back()}
            className="p-2 rounded-lg bg-slate-100 dark:bg-slate-800"
          >
            <MaterialIcons
              name="close"
              size={20}
              className="text-slate-500 dark:text-slate-400"
            />
          </TouchableOpacity>
          <View>
            <Text className="text-[10px] font-bold uppercase tracking-widest text-[#1544b2]">
              Prawko AI
            </Text>
            <Text className="text-[9px] font-medium text-slate-400 uppercase">
              Symulacja egzaminu word
            </Text>
          </View>
        </View>

        <View className="flex-row items-center gap-2 bg-slate-50 dark:bg-slate-800 px-3 py-2 rounded-xl border border-slate-200 dark:border-slate-700">
          <MaterialIcons name="timer" size={16} className="text-[#1544b2]" />
          <Text className="text-base font-bold tabular-nums dark:text-white">
            25:00
          </Text>
        </View>

        <TouchableOpacity className="bg-red-500 px-4 py-3 rounded-lg">
          <Text className="text-white text-xs font-bold">Zakończ</Text>
        </TouchableOpacity>
      </View>

      <ScrollView
        className="flex-1"
        contentContainerStyle={{ paddingBottom: 160 }}
      >
        {/* --- MEDIA SECTION --- */}
        <View className="p-4">
          <View className="relative aspect-video rounded-3xl overflow-hidden bg-slate-900 shadow-2xl border border-slate-200 dark:border-slate-800">
            <ImageBackground
              source={{
                uri:
                  currentQuestion.mediaUrl ||
                  "https://images.unsplash.com/photo-1541899481282-d53bffe3c35d?q=80&w=1000",
              }}
              className="flex-1 items-center justify-center"
            >
              <View className="absolute inset-0 bg-black/10" />
              <TouchableOpacity className="w-16 h-16 rounded-full bg-white/20 items-center justify-center border border-white/30 backdrop-blur-md">
                <MaterialIcons name="play-arrow" size={40} color="white" />
              </TouchableOpacity>
            </ImageBackground>
          </View>
        </View>

        {/* --- Question --- */}
        <View className="px-5">
          <View className="flex-row items-center gap-2 mb-3">
            <View className="px-2.5 py-1 bg-[#1544b2]/10 rounded-md">
              <Text className="text-[#1544b2] text-[10px] font-black uppercase">
                Pytanie {globalProgress} z {totalQuestions}
              </Text>
            </View>
            <View className="px-2.5 py-1 bg-slate-100 dark:bg-slate-800 rounded-md">
              <Text className="text-slate-600 dark:text-slate-400 text-[10px] font-black uppercase">
                Punkty: {currentQuestion.points}
              </Text>
            </View>
          </View>

          <Text className="text-xl font-bold leading-snug text-slate-800 dark:text-slate-100 mb-8">
            {currentQuestion.content}
          </Text>

          {/* --- Answers --- */}
          <View className="flex-col gap-3">
            {sortedAnswers.map((answer) => {
              const isSelected = selectedAnswerId === answer.id;
              const isBinary = sortedAnswers.length === 2;

              return (
                <TouchableOpacity
                  key={answer.id}
                  onPress={() => setSelectedAnswerId(answer.id)}
                  activeOpacity={0.7}
                  className={`min-h-[64px] px-6 rounded-3xl border-2 flex-row items-center justify-between transition-all ${
                    isSelected
                      ? "border-[#1544b2] bg-[#1544b2]/5"
                      : "border-slate-200 dark:border-slate-800 bg-white dark:bg-slate-900"
                  }`}
                >
                  <Text
                    className={`text-lg flex-1 font-bold ${
                      isSelected
                        ? "text-[#1544b2]"
                        : "text-slate-800 dark:text-slate-200"
                    } ${isBinary ? "text-center text-xl uppercase" : ""}`}
                  >
                    {answer.content}
                  </Text>
                  {isSelected && !isBinary && (
                    <MaterialIcons
                      name="check-circle"
                      size={24}
                      color="#1544b2"
                    />
                  )}
                </TouchableOpacity>
              );
            })}
          </View>

          {/* Next BUTTON*/}
          <View className="flex-row items-center justify-end mt-10">
            <TouchableOpacity
              onPress={handleNext}
              className={
                "flex-row items-center gap-2 px-8 py-4 rounded-2xl shadow-lg bg-[#1544b2] shadow-[#1544b2]/30"
              }
            >
              <Text className="text-white font-bold text-lg">
                {currentScope === "specialized" &&
                currentIndex === questions.length - 1
                  ? "Zakończ"
                  : "Następne"}
              </Text>
              <MaterialIcons name="chevron-right" size={24} color="white" />
            </TouchableOpacity>
          </View>
        </View>
      </ScrollView>

      {/* --- FOOTER PROGRESS --- */}
      <View
        style={{ paddingBottom: insets.bottom + 20 }}
        className="absolute bottom-0 left-0 right-0 bg-white dark:bg-slate-950 border-t border-slate-200 dark:border-slate-800 p-5"
      >
        <View className="flex-row items-center justify-center gap-6">
          <View
            className={`flex-1 ${currentScope !== "standard" ? "opacity-30" : ""}`}
          >
            <View className="flex-row justify-between mb-2">
              <Text className="text-[9px] font-black uppercase text-slate-400">
                Podstawowe
              </Text>
              <Text className="text-[9px] font-black text-[#1544b2]">
                {currentScope === "standard" ? currentIndex + 1 : totalStandard}
                /{totalStandard}
              </Text>
            </View>
            <View className="h-1.5 bg-slate-100 dark:bg-slate-800 rounded-full overflow-hidden">
              <View
                className="h-full bg-[#1544b2]"
                style={{
                  width: `${currentScope === "standard" ? ((currentIndex + 1) / totalStandard) * 100 : 100}%`,
                }}
              />
            </View>
          </View>

          <View
            className={`flex-1 ${currentScope !== "specialized" ? "opacity-30" : ""}`}
          >
            <View className="flex-row justify-between mb-2">
              <Text className="text-[9px] font-black uppercase text-slate-400">
                Specjalistyczne
              </Text>
              <Text className="text-[9px] font-black text-slate-400">
                {currentScope === "specialized" ? currentIndex + 1 : 0}/
                {totalSpecialized}
              </Text>
            </View>
            <View className="h-1.5 bg-slate-100 dark:bg-slate-800 rounded-full overflow-hidden">
              <View
                className="h-full bg-slate-400"
                style={{
                  width: `${currentScope === "specialized" ? ((currentIndex + 1) / totalSpecialized) * 100 : 0}%`,
                }}
              />
            </View>
          </View>
        </View>
      </View>
    </View>
  );
}
