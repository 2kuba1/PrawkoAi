import { useError } from "@/app/context/errorContext";
import { MaterialIcons } from "@expo/vector-icons";
import AsyncStorage from "@react-native-async-storage/async-storage";
import { LinearGradient } from "expo-linear-gradient";
import { useRouter } from "expo-router";
import React, { useEffect, useMemo, useState } from "react";
import {
  ActivityIndicator,
  ScrollView,
  Text,
  TouchableOpacity,
  View,
} from "react-native";

import { useSafeAreaInsets } from "react-native-safe-area-context";

export interface PossibleAnswer {
  id: string;
  content: string;
}

interface AnswerDetail {
  questionId: string;
  content: string;
  createdAt: string;
  selectedAnswerId: string | null;
  answers: PossibleAnswer[];
  questionPoints: number;
  questionNumber: number;
  index: number;
}

interface SetResultDetail {
  correctAnswersCount: number;
  score: number;
  maxScore: number;
  startedAt: string;
  finishedAt: string;
  isPassed: boolean;
  correctAnswers: AnswerDetail[];
  incorrectAnswers: AnswerDetail[];
  unanswered: AnswerDetail[];
}

export default function SetResultDetailPageScreen() {
  const router = useRouter();
  const insets = useSafeAreaInsets();
  const [setResult, setSetResult] = useState<SetResultDetail | null>(null);
  const [loading, setLoading] = useState(true);

  const { showError } = useError();

  useEffect(() => {
    const loadLocalResult = async () => {
      setLoading(true);

      try {
        const localData = await AsyncStorage.getItem("last_set_result");
        if (localData) {
          setSetResult(JSON.parse(localData));
        } else {
          showError("Nie znaleziono lokalnych wyników zestawu.");
          console.warn("Nie znaleziono lokalnych wyników zestawu.");
        }
      } catch (error) {
        showError("Wystąpił błąd podczas pobierania lokalnych wyników.");
        console.error("Błąd pobierania lokalnych wyników:", error);
      } finally {
        setLoading(false);
      }
    };

    loadLocalResult();
  }, []);

  useEffect(() => {
    const loadLocalResult = async () => {
      setLoading(true);

      try {
        const localData = await AsyncStorage.getItem("last_set_result");
        if (localData) {
          setSetResult(JSON.parse(localData));
        }
      } catch (error) {
        showError("Wystąpił błąd podczas pobierania lokalnych wyników.");
        console.error("Błąd pobierania lokalnych wyników:", error);
      } finally {
        setLoading(false);
      }
    };

    loadLocalResult();

    return () => {
      AsyncStorage.removeItem("last_set_result").catch((err) => {
        showError("Wystąpił błąd podczas usuwania wyników ze storage.");
        console.error("Błąd usuwania wyników ze storage:", err);
      });
    };
  }, []);

  const sortedQuestions = useMemo(() => {
    if (!setResult) return [];

    const all = [
      ...(setResult.correctAnswers ?? []).map((q) => ({
        ...q,
        type: "correct" as const,
      })),

      ...(setResult.incorrectAnswers ?? []).map((q) => ({
        ...q,
        type: "incorrect" as const,
      })),

      ...(setResult.unanswered ?? []).map((q) => ({
        ...q,
        type: "unanswered" as const,
      })),
    ];

    return all.sort((a, b) => {
      const dateA = a.createdAt ? new Date(a.createdAt).getTime() : 0;
      const dateB = b.createdAt ? new Date(b.createdAt).getTime() : 0;
      return dateA - dateB;
    });
  }, [setResult]);

  if (loading) {
    return (
      <View className="flex-1 justify-center items-center bg-[#f6f6f8] dark:bg-[#111621]">
        <ActivityIndicator size="large" color="#1544b2" />
        <Text className="mt-4 text-slate-500">
          Ładowanie wyników zestawu...
        </Text>
      </View>
    );
  }

  if (!setResult) {
    return (
      <View className="flex-1 justify-center items-center bg-[#f6f6f8] dark:bg-[#111621] p-6">
        <MaterialIcons name="error-outline" size={64} color="#ef4444" />

        <Text className="text-xl font-bold mt-4 text-slate-900 dark:text-white text-center">
          Ups! Coś poszło nie tak.
        </Text>

        <Text className="text-slate-600 dark:text-slate-400 text-center mt-2 mb-6">
          Nie udało się załadować wyników tego zestawu.
        </Text>

        <TouchableOpacity
          onPress={() => router.replace("/dashboard")}
          className="bg-[#1544b2] py-3 px-6 rounded-full"
        >
          <Text className="text-white font-bold">Wróć do pulpitu</Text>
        </TouchableOpacity>
      </View>
    );
  }

  return (
    <View className="flex-1 bg-[#f6f6f8] dark:bg-[#111621]">
      <View
        style={{ paddingTop: insets.top }}
        className="flex-row items-center p-4 bg-[#f6f6f8]/80 dark:bg-[#111621]/80 backdrop-blur-md border-b border-[#1544b2]/10"
      >
        <TouchableOpacity
          onPress={() => router.replace("/dashboard")}
          className="p-2 hover:bg-[#1544b2]/10 rounded-full"
        >
          <MaterialIcons name="arrow-back" size={24} color="#1544b2" />
        </TouchableOpacity>
        <Text className="text-xl font-bold ml-2 text-slate-900 dark:text-white">
          Wyniki Zestawu
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
                name="assignment"
                size={56}
                color="white"
                style={{ textAlign: "center", marginTop: 8 }}
              />
              <Text className="text-white text-xs font-semibold uppercase tracking-widest text-center mt-5 px-6 mb-8">
                Zestaw {setResult.isPassed ? "zaliczony" : "niezaliczony"}
              </Text>
            </LinearGradient>

            <View className="p-5">
              <View className="flex-row justify-between items-start">
                <View>
                  <Text className="text-slate-500 dark:text-slate-400 text-[10px] font-bold uppercase tracking-wider">
                    Data podejścia
                  </Text>
                  <Text className="text-slate-900 dark:text-slate-100 text-base font-semibold">
                    {new Date(setResult!.finishedAt).toLocaleString("pl-PL", {
                      day: "2-digit",
                      month: "long",
                      year: "numeric",
                      hour: "2-digit",
                      minute: "2-digit",
                    })}
                  </Text>
                </View>
                <View className="items-end">
                  <Text className="text-slate-500 dark:text-slate-400 text-[10px] font-bold uppercase tracking-wider">
                    Status
                  </Text>
                  <View className="bg-emerald-100 dark:bg-emerald-900/30 px-2.5 py-0.5 rounded-full mt-1">
                    {setResult?.isPassed ? (
                      <Text className="text-emerald-700 dark:text-emerald-400 text-[10px] font-bold">
                        POZYTYWNY
                      </Text>
                    ) : (
                      <Text className="text-red-700 dark:text-red-400 text-[10px] font-bold">
                        NEGATYWNY
                      </Text>
                    )}
                  </View>
                </View>
              </View>

              <View className="h-[1px] bg-slate-100 dark:bg-slate-800 my-4" />

              <View>
                <Text className="text-slate-500 dark:text-slate-400 text-[10px] font-bold uppercase tracking-wider">
                  Wynik końcowy
                </Text>
                <Text className="text-[#1544b2] text-3xl font-bold">
                  {setResult?.score} / {setResult?.maxScore} {""}
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
              {sortedQuestions.length} pytań w zestawie
            </Text>
          </View>
        </View>

        <View className="pb-20">
          {sortedQuestions.map((item, index) => (
            <QuestionItem
              key={item.questionId + index}
              index={index + 1}
              points={item.questionPoints}
              type={item.type}
              question={item.content}
              selectedAnswerId={item.selectedAnswerId}
              possibleAnswers={item.answers ?? []}
              questionId={item.questionId}
              questionNumber={item.questionNumber}
            />
          ))}
        </View>
      </ScrollView>
    </View>
  );
}

function QuestionItem({
  index,
  points,
  type,
  question,
  selectedAnswerId,
  possibleAnswers,
  questionId,
  questionNumber,
}: {
  index: number;
  points: number;
  type: "correct" | "incorrect" | "unanswered";
  question: string;
  selectedAnswerId: string | null;
  possibleAnswers: PossibleAnswer[];
  questionId: string;
  questionNumber: number;
}) {
  const isCorrect = type === "correct";
  const isUnanswered = type === "unanswered";

  const router = useRouter();

  return (
    <TouchableOpacity
      onPress={() =>
        router.push({
          pathname: `/question/examQuestionWithAnswer/${questionId}` as any,
          params: {
            questionId: questionId,
            questionNumber: questionNumber.toString(),
            question: question,
            possibleAnswers: JSON.stringify(possibleAnswers),
            selectedAnswerId: selectedAnswerId ?? "",
            wasCorrect: type,
            points: points.toString(),
          },
        })
      }
      className="bg-white dark:bg-slate-900 px-4 py-5 border-b border-slate-100 dark:border-slate-800"
    >
      <View className="flex-row gap-4">
        <View
          className={`size-10 rounded-lg items-center justify-center ${
            isCorrect
              ? "bg-emerald-100 dark:bg-emerald-900/30"
              : "bg-red-100 dark:bg-red-900/30"
          }`}
        >
          <MaterialIcons
            name={
              isCorrect ? "check-circle" : isUnanswered ? "timer-off" : "cancel"
            }
            size={24}
            color={isCorrect ? "#10b981" : "#ef4444"}
          />
        </View>

        <View className="flex-1">
          <Text className="text-[10px] font-bold text-slate-400 uppercase tracking-tighter mb-1">
            Pytanie {index} • {points} pkt
          </Text>

          <Text className="text-slate-900 dark:text-slate-100 text-sm font-semibold leading-relaxed mb-4">
            {question}
          </Text>

          <View className="gap-2">
            {possibleAnswers.map((answer) => {
              const isUserChoice = answer.id === selectedAnswerId;

              let containerClass =
                "bg-slate-50 dark:bg-slate-800/50 border-slate-100 dark:border-slate-800";
              let textClass = "text-slate-700 dark:text-slate-300";
              let icon = null;

              if (isUserChoice) {
                if (isCorrect) {
                  containerClass =
                    "bg-emerald-50 dark:bg-emerald-900/40 border-emerald-500";
                  textClass =
                    "text-emerald-900 dark:text-emerald-100 font-bold";
                  icon = (
                    <MaterialIcons
                      name="check-circle"
                      size={16}
                      color="#059669"
                    />
                  );
                } else {
                  containerClass =
                    "bg-red-50 dark:bg-red-900/40 border-red-500";
                  textClass = "text-red-900 dark:text-red-100 font-bold";
                  icon = (
                    <MaterialIcons name="cancel" size={16} color="#dc2626" />
                  );
                }
              }

              return (
                <View
                  key={answer.id}
                  className={`flex-row items-center justify-between px-4 py-3 rounded-xl border-2 ${containerClass}`}
                >
                  <Text className={`text-[13px] flex-1 pr-2 ${textClass}`}>
                    {answer.content}
                  </Text>
                  {icon}
                </View>
              );
            })}
          </View>

          {isUnanswered && (
            <View className="mt-3 py-2 px-3 bg-red-50 dark:bg-red-900/20 rounded-lg flex-row items-center">
              <MaterialIcons name="error-outline" size={14} color="#ef4444" />
              <Text className="ml-2 text-[11px] font-bold text-red-600 uppercase">
                Brak Twojej odpowiedzi
              </Text>
            </View>
          )}
        </View>
      </View>
    </TouchableOpacity>
  );
}
