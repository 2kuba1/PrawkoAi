import { MaterialIcons } from "@expo/vector-icons";
import { LinearGradient } from "expo-linear-gradient";
import { useRouter } from "expo-router";
import { useLocalSearchParams } from "expo-router/build/hooks";
import React from "react";
import { ScrollView, Text, TouchableOpacity, View } from "react-native";
import { useSafeAreaInsets } from "react-native-safe-area-context";

export default function ExamResultDetail() {
  const { id } = useLocalSearchParams();
  const router = useRouter();
  const insets = useSafeAreaInsets();

  return (
    <View className="flex-1 bg-[#f6f6f8] dark:bg-[#111621]">
      {/* Header */}
      <View
        style={{ paddingTop: insets.top }}
        className="flex-row items-center p-4 bg-[#f6f6f8]/80 dark:bg-[#111621]/80 backdrop-blur-md border-b border-[#1544b2]/10"
      >
        <TouchableOpacity
          onPress={() => router.push("/dashboard")}
          className="p-2 hover:bg-[#1544b2]/10 rounded-full"
        >
          <MaterialIcons name="arrow-back" size={24} color="#1544b2" />
        </TouchableOpacity>
        <Text className="text-xl font-bold ml-2 text-slate-900 dark:text-white">
          Wyniki Egzaminu
        </Text>
      </View>

      <ScrollView className="flex-1" showsVerticalScrollIndicator={false}>
        <View className="p-4">
          <View className="bg-white dark:bg-slate-900 rounded-3xl shadow-sm border border-slate-200 dark:border-slate-800 overflow-hidden">
            <LinearGradient
              colors={["#1544b2", "#0a235c"]}
              start={{ x: 0, y: 0 }}
              end={{ x: 1, y: 1 }}
              className="py-12 w-full items-center justify-center"
            >
              <MaterialIcons
                name="verified"
                size={56}
                color="white"
                style={{ textAlign: "center", marginTop: 8 }}
              />
              <Text className="text-white text-xs font-semibold uppercase tracking-widest text-center mt-5 px-6 mb-8">
                Symulacja egzaminu word
              </Text>
            </LinearGradient>

            <View className="p-5">
              <View className="flex-row justify-between items-start">
                <View>
                  <Text className="text-slate-500 dark:text-slate-400 text-[10px] font-bold uppercase tracking-wider">
                    Data podejścia
                  </Text>
                  <Text className="text-slate-900 dark:text-slate-100 text-base font-semibold">
                    12 marca 2026, 14:20
                  </Text>
                </View>
                <View className="items-end">
                  <Text className="text-slate-500 dark:text-slate-400 text-[10px] font-bold uppercase tracking-wider">
                    Status
                  </Text>
                  <View className="bg-emerald-100 dark:bg-emerald-900/30 px-2.5 py-0.5 rounded-full mt-1">
                    <Text className="text-emerald-700 dark:text-emerald-400 text-[10px] font-bold">
                      POZYTYWNY
                    </Text>
                  </View>
                </View>
              </View>

              <View className="h-[1px] bg-slate-100 dark:bg-slate-800 my-4" />

              <View>
                <Text className="text-slate-500 dark:text-slate-400 text-[10px] font-bold uppercase tracking-wider">
                  Wynik końcowy
                </Text>
                <Text className="text-[#1544b2] text-3xl font-bold">
                  68 / 74{" "}
                  <Text className="text-sm font-normal text-slate-400">
                    pkt
                  </Text>
                </Text>
              </View>
            </View>
          </View>
        </View>

        <View className="px-4 py-2 flex-row items-center justify-between">
          <Text className="text-slate-900 dark:text-slate-100 text-lg font-bold">
            Przegląd pytań
          </Text>
          <View className="bg-slate-200 dark:bg-slate-800 px-2 py-1 rounded">
            <Text className="text-[10px] font-medium text-slate-500">
              32 pytania
            </Text>
          </View>
        </View>

        {/* Question list */}
        <View className="pb-20">
          <QuestionItem
            index={1}
            points={2}
            isCorrect={true}
            question="Czy w tej sytuacji masz obowiązek zachować szczególną ostrożność zbliżając się do przejścia dla pieszych?"
            yourAnswer="Tak"
          />

          <QuestionItem
            index={2}
            points={3}
            isCorrect={false}
            question="Jaka jest dopuszczalna prędkość pojazdu na obszarze zabudowanym w godzinach 23:00 - 05:00?"
            yourAnswer="60 km/h"
            correctAnswer="50 km/h"
          />
        </View>
      </ScrollView>
    </View>
  );
}

function QuestionItem({
  index,
  points,
  isCorrect,
  question,
  yourAnswer,
  correctAnswer,
}: any) {
  return (
    <View className="bg-white dark:bg-slate-900 px-4 py-5 border-b border-slate-100 dark:border-slate-800">
      <View className="flex-row gap-4">
        <View
          className={`size-10 rounded-lg items-center justify-center ${isCorrect ? "bg-emerald-100 dark:bg-emerald-900/30" : "bg-red-100 dark:bg-red-900/30"}`}
        >
          <MaterialIcons
            name={isCorrect ? "check-circle" : "cancel"}
            size={24}
            color={isCorrect ? "#10b981" : "#ef4444"}
          />
        </View>

        <View className="flex-1">
          <Text className="text-[10px] font-bold text-slate-400 uppercase tracking-tighter mb-1">
            Pytanie {index} • {points} pkt
          </Text>
          <Text className="text-slate-900 dark:text-slate-100 text-sm font-semibold leading-relaxed mb-3">
            {question}
          </Text>

          <View
            className={`px-3 py-2 rounded-lg border ${isCorrect ? "bg-emerald-50 dark:bg-emerald-900/20 border-emerald-100" : "bg-red-50 dark:bg-red-900/20 border-red-100"}`}
          >
            <View className="flex-row items-center gap-2">
              <MaterialIcons
                name={isCorrect ? "done" : "close"}
                size={16}
                color={isCorrect ? "#065f46" : "#991b1b"}
              />
              <View>
                <Text
                  className={`text-[11px] font-bold ${isCorrect ? "text-emerald-800 dark:text-emerald-300" : "text-red-800 dark:text-red-300"}`}
                >
                  Twoja odpowiedź: {yourAnswer}
                </Text>
                <Text
                  className={`text-[10px] ${isCorrect ? "text-emerald-600/80" : "text-red-600/80"}`}
                >
                  {isCorrect ? "Poprawna odpowiedź" : "Błędna odpowiedź"}
                </Text>
              </View>
            </View>
          </View>

          {!isCorrect && (
            <View className="mt-2 px-3 py-2 rounded-lg border border-dashed border-emerald-200 dark:border-emerald-800 bg-emerald-50/50 dark:bg-emerald-900/10">
              <View className="flex-row items-center gap-2">
                <MaterialIcons name="check-circle" size={16} color="#10b981" />
                <Text className="text-[11px] font-medium text-emerald-800 dark:text-emerald-300">
                  Poprawna: {correctAnswer}
                </Text>
              </View>
            </View>
          )}
        </View>
      </View>
    </View>
  );
}
