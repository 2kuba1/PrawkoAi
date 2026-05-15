import { AuthContext } from "@/app/_layout";
import { useError } from "@/app/context/errorContext";
import api from "@/app/utils/api";
import { MaterialIcons } from "@expo/vector-icons";
import AsyncStorage from "@react-native-async-storage/async-storage";
import { useLocalSearchParams, useRouter } from "expo-router";
import { useVideoPlayer, VideoView } from "expo-video";
import React, { useContext, useEffect, useMemo, useState } from "react";
import {
  ActivityIndicator,
  Image,
  ScrollView,
  StatusBar,
  Text,
  TouchableOpacity,
  View,
} from "react-native";
import { useSafeAreaInsets } from "react-native-safe-area-context";

interface UserSetAnswer {
  questionId: string;
  selectedAnswerId: string;
  answeredAt: string;
  orderIndex: number;
}

export default function ExamSolvingScreen() {
  const insets = useSafeAreaInsets();
  const router = useRouter();
  const params = useLocalSearchParams<{
    categoryTag: string;
    categoryType: string;
    setNumber: string;
  }>();

  const { showError } = useError();

  const [questions, setQuestions] = useState<any[]>([]);
  const [currentIndex, setCurrentIndex] = useState(0);
  const [loading, setLoading] = useState(true);
  const [selectedId, setSelectedId] = useState<string | null>(null);
  const [isChecked, setIsChecked] = useState(false);
  const [correctCount, setCorrectCount] = useState(0);
  const [isPlaying, setIsPlaying] = useState(false);
  const [userAnswers, setUserAnswers] = useState<UserSetAnswer[]>([]);
  const [currentLanguage, setCurrentLanguage] = useState("pl");

  const { user } = useContext(AuthContext);

  useEffect(() => {
    let isMounted = true;
    const fetchQuestions = async () => {
      try {
        const lang = (await AsyncStorage.getItem("user-language")) || "PL";
        setCurrentLanguage(lang.toLowerCase());
        const response = await api.get("/learn/getQuestionsForSet", {
          params: { ...params, locale: lang.toUpperCase() },
        });
        if (isMounted) {
          setQuestions(response.data || []);
          setLoading(false);
        }
      } catch (e) {
        showError(
          "Wystąpił błąd podczas pobierania pytań. Proszę spróbować ponownie.",
        );
        console.error("Błąd API:", e);
        if (isMounted) setLoading(false);
      }
    };
    fetchQuestions();
    return () => {
      isMounted = false;
    };
  }, [params.setNumber]);

  const currentQuestion = questions[currentIndex];

  const sortedAnswers = useMemo(() => {
    if (!currentQuestion?.answers) return [];
    const answers = [...currentQuestion.answers];

    if (answers.length === 2) {
      const languageOrders = {
        pl: ["tak", "nie"],
        en: ["yes", "no"],
        uk: ["так", "ні"],
        de: ["ja", "nein"],
      };

      const lang = currentLanguage.toLowerCase();
      const order =
        lang in languageOrders
          ? languageOrders[lang as keyof typeof languageOrders]
          : languageOrders["pl"];

      return answers.sort((a, b) => {
        const indexA = order.indexOf(a.answerContent.toLowerCase());
        const indexB = order.indexOf(b.answerContent.toLowerCase());

        if (indexA === -1 || indexB === -1) return 0;
        return indexA - indexB;
      });
    }
    return answers;
  }, [currentQuestion, currentLanguage]);

  const handleCheck = () => {
    if (!selectedId || isChecked) return;

    const questionId = currentQuestion.id || currentQuestion.questionId;

    const newAnswer: UserSetAnswer = {
      questionId: questionId,
      selectedAnswerId: selectedId,
      answeredAt: new Date().toISOString(),
      orderIndex: currentIndex,
    };

    setUserAnswers((prev) => [...prev, newAnswer]);

    if (selectedId === currentQuestion.correctAnswerId) {
      setCorrectCount((prev) => prev + 1);
    }

    setIsChecked(true);
  };

  const handleNext = async () => {
    if (currentIndex < questions.length - 1) {
      setIsChecked(false);
      setSelectedId(null);
      setCurrentIndex((prev) => prev + 1);
    } else {
      try {
        const resultsData = {
          correctAnswersCount: correctCount,
          score: correctCount,
          maxScore: questions.length,
          startedAt: new Date().toISOString(),
          finishedAt: new Date().toISOString(),
          isPassed: correctCount / questions.length >= 0.8,
          correctAnswers: questions
            .filter(
              (q, idx) =>
                userAnswers.find(
                  (ua) => ua.questionId === (q.id || q.questionId),
                )?.selectedAnswerId === q.correctAnswerId,
            )
            .map((q, idx) =>
              formatToDetail(q, userAnswers, questions.indexOf(q)),
            ),
          incorrectAnswers: questions
            .filter((q) => {
              const ua = userAnswers.find(
                (ua) => ua.questionId === (q.id || q.questionId),
              );
              return ua && ua.selectedAnswerId !== q.correctAnswerId;
            })
            .map((q) => formatToDetail(q, userAnswers, questions.indexOf(q))),
          unanswered: questions
            .filter(
              (q) =>
                !userAnswers.find(
                  (ua) => ua.questionId === (q.id || q.questionId),
                ),
            )
            .map((q) => formatToDetail(q, userAnswers, questions.indexOf(q))),
        };

        await AsyncStorage.setItem(
          "last_set_result",
          JSON.stringify(resultsData),
        );

        const payload = {
          userId: user?.id,
          answers: userAnswers,
          categoryName: (await AsyncStorage.getItem("user-category")) ?? "B",
        };
        await api.post("/answer/answerSet", payload);

        const storageKey = `progress_${params.categoryTag}`;
        const existingData = await AsyncStorage.getItem(storageKey);
        const progress = existingData ? JSON.parse(existingData) : {};
        progress[params.setNumber] = {
          score: (correctCount / questions.length) * 100,
          completed: true,
        };
        await AsyncStorage.setItem(storageKey, JSON.stringify(progress));

        router.push({
          pathname: "/learning/setResult/setResult" as any,
          params: { source: "local" },
        });
      } catch (e: any) {
        showError("Wystąpił błąd podczas zapisu wyników.");
        console.error("Błąd zapisu wyników:", e);
      }
    }
  };

  const formatToDetail = (q: any, answers: UserSetAnswer[], index: number) => ({
    questionId: q.id || q.questionId,
    content: q.questionContent,
    createdAt:
      answers.find((a) => a.questionId === (q.id || q.questionId))
        ?.answeredAt || new Date().toISOString(),
    selectedAnswerId:
      answers.find((a) => a.questionId === (q.id || q.questionId))
        ?.selectedAnswerId || null,
    questionPoints: q.points || 1,
    questionNumber: q.questionNumber,
    index: index + 1,
    answers: q.answers.map((a: any) => ({
      id: a.answerId,
      content: a.answerContent,
    })),
  });

  const fullMediaUrl = React.useMemo(() => {
    if (!currentQuestion?.mediaUrl) return "";
    return `${process.env.EXPO_PUBLIC_SUPABASE_BUCKET_URL}/${currentQuestion.mediaUrl}`;
  }, [currentQuestion]);

  const isVideo = fullMediaUrl?.toLowerCase().endsWith(".mp4");

  const player = useVideoPlayer(fullMediaUrl ?? "", (p) => {
    p.loop = false;
  });

  useEffect(() => {
    if (isVideo && fullMediaUrl) {
      const loadVideo = async () => {
        try {
          await player.replaceAsync(fullMediaUrl);
        } catch (e) {
          showError("Wystąpił błąd podczas ładowania wideo.");
          console.error("Błąd ładowania wideo:", e);
        }
      };

      loadVideo();
      setIsPlaying(false);
    }
  }, [fullMediaUrl, isVideo]);

  const handlePlayVideo = () => {
    player.play();
    setIsPlaying(true);
  };

  if (loading)
    return (
      <View className="flex-1 justify-center items-center bg-[#f6f6f8] dark:bg-[#111621]">
        <ActivityIndicator size="large" color="#1544b2" />
      </View>
    );

  if (!currentQuestion) return null;

  return (
    <View className="flex-1 bg-[#f6f6f8] dark:bg-[#111621]">
      <StatusBar barStyle="default" />
      <View
        style={{ paddingTop: insets.top }}
        className="bg-white dark:bg-slate-900 px-4 pb-4 border-b border-slate-100 dark:border-slate-800"
      >
        <View className="flex-row items-center justify-between mb-2">
          <TouchableOpacity onPress={() => router.back()}>
            <MaterialIcons name="close" size={28} color="#1544b2" />
          </TouchableOpacity>
          <Text className="font-bold dark:text-white">
            Pytanie {currentIndex + 1} z {questions.length}
          </Text>
          <View className="w-10" />
        </View>
        <View className="h-1.5 bg-slate-100 dark:bg-slate-800 rounded-full overflow-hidden">
          <View
            style={{
              width: `${((currentIndex + 1) / questions.length) * 100}%`,
            }}
            className="h-full bg-[#1544b2]"
          />
        </View>
      </View>

      <ScrollView className="flex-1" showsVerticalScrollIndicator={false}>
        <View className="p-4">
          <View className="aspect-video rounded-2xl overflow-hidden bg-slate-900 shadow-xl border border-[#1544b2]/10 relative">
            {fullMediaUrl ? (
              isVideo ? (
                <>
                  <VideoView
                    player={player}
                    style={{ width: "100%", height: "100%" }}
                    allowsPictureInPicture
                    nativeControls={isPlaying}
                  />
                  {!isPlaying && (
                    <TouchableOpacity
                      onPress={handlePlayVideo}
                      activeOpacity={0.9}
                      className="absolute inset-0 items-center justify-center bg-black/20"
                    >
                      <View className="w-20 h-20 rounded-full bg-black/50 items-center justify-center backdrop-blur-sm">
                        <MaterialIcons
                          name="play-arrow"
                          size={50}
                          color="white"
                        />
                      </View>
                    </TouchableOpacity>
                  )}
                </>
              ) : (
                <Image
                  source={{ uri: fullMediaUrl }}
                  style={{ width: "100%", height: "100%" }}
                  resizeMode="cover"
                />
              )
            ) : (
              <View className="flex-1 items-center justify-center bg-slate-800">
                <MaterialIcons
                  name="image-not-supported"
                  size={48}
                  color="white"
                />
              </View>
            )}
          </View>
        </View>
        <View className="p-5">
          <Text className="text-xl font-bold dark:text-white mb-6 leading-7">
            {currentQuestion.questionContent}
          </Text>

          <View className="gap-3">
            {sortedAnswers.map((answer: any) => {
              const isSelected = selectedId === answer.answerId;
              const isCorrect =
                isChecked &&
                answer.answerId === currentQuestion.correctAnswerId;
              const isWrong = isChecked && isSelected && !isCorrect;

              let border = "border-slate-200 dark:border-slate-800";
              let bg = "bg-white dark:bg-slate-800/50";

              if (isCorrect) {
                border = "border-green-500";
                bg = "bg-green-50 dark:bg-green-900/20";
              } else if (isWrong) {
                border = "border-red-500";
                bg = "bg-red-50 dark:bg-red-900/20";
              } else if (isSelected) {
                border = "border-[#1544b2]";
                bg = "bg-blue-50 dark:bg-blue-900/20";
              }

              return (
                <TouchableOpacity
                  key={answer.answerId}
                  disabled={isChecked}
                  onPress={() => setSelectedId(answer.answerId)}
                  className={`p-5 rounded-2xl border-2 flex-row items-center justify-between ${bg} ${border}`}
                >
                  <Text
                    className={`text-base flex-1 font-semibold ${isSelected ? "text-[#1544b2] dark:text-white" : "text-slate-600 dark:text-slate-300"}`}
                  >
                    {answer.answerContent}
                  </Text>
                  {isCorrect && (
                    <MaterialIcons
                      name="check-circle"
                      size={24}
                      color="#22c55e"
                    />
                  )}
                  {isWrong && (
                    <MaterialIcons name="cancel" size={24} color="#ef4444" />
                  )}
                </TouchableOpacity>
              );
            })}
          </View>

          {isChecked && (
            <View className="mt-6 p-5 bg-white dark:bg-slate-800 rounded-3xl border border-slate-100 dark:border-slate-700">
              <Text className="font-bold text-[#1544b2] mb-2 uppercase text-xs">
                Wyjaśnienie
              </Text>
              <Text className="text-slate-600 dark:text-slate-300 leading-6">
                {currentQuestion.staticResponseContent}
              </Text>
            </View>
          )}
        </View>
      </ScrollView>

      <View
        style={{ paddingBottom: insets.bottom + 16 }}
        className="p-4 bg-white dark:bg-slate-900 border-t border-slate-100 dark:border-slate-800"
      >
        {!isChecked ? (
          <TouchableOpacity
            disabled={!selectedId}
            onPress={handleCheck}
            className={`p-5 rounded-2xl items-center ${selectedId ? "bg-[#1544b2]" : "bg-slate-200 dark:bg-slate-800"}`}
          >
            <Text
              className={`font-bold text-lg ${selectedId ? "text-white" : "text-slate-400"}`}
            >
              Sprawdź
            </Text>
          </TouchableOpacity>
        ) : (
          <TouchableOpacity
            onPress={handleNext}
            className="bg-[#1544b2] p-5 rounded-2xl items-center"
          >
            <Text className="text-white font-bold text-lg">
              {currentIndex < questions.length - 1
                ? "Następne pytanie"
                : "Zakończ zestaw"}
            </Text>
          </TouchableOpacity>
        )}
      </View>
    </View>
  );
}
