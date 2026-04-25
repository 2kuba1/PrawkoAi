import { MaterialIcons } from "@expo/vector-icons";
import AsyncStorage from "@react-native-async-storage/async-storage";
import { useRouter } from "expo-router";
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
import { AuthContext } from "../_layout";
import api from "../utils/api";
import i18n from "../utils/translations";

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
  const insets = useSafeAreaInsets();
  const router = useRouter();
  const { user } = useContext(AuthContext);
  const [examData, setExamData] = useState<ExamData | null>(null);
  const [loading, setLoading] = useState(true);
  const [currentIndex, setCurrentIndex] = useState(0);
  const [currentScope, setCurrentScope] = useState<"standard" | "specialized">(
    "standard",
  );
  const [selectedAnswerId, setSelectedAnswerId] = useState<string | null>(null);
  const [isFinishing, setIsFinishing] = useState(false);

  const [userAnswers, setUserAnswers] = useState<
    Record<string, { selectedAnswerId: string | null; answeredAt: string }>
  >({});

  const [timeLeft, setTimeLeft] = useState(20);
  const [videoStatus, setVideoStatus] = useState<
    "prep" | "playing" | "answering" | "idle"
  >("prep");

  const [hasRevealedMedia, setHasRevealedMedia] = useState(false);

  useEffect(() => {
    const fetchExamData = async () => {
      if (!user?.id) return;

      try {
        const response = await api.get<ExamData>("/api/exam/start", {
          params: {
            userId: user.id,
            category: "B",
            locale: (await AsyncStorage.getItem("user-language")) ?? "EN",
          },
        });
        setExamData(response.data);
      } catch (e) {
        console.error("Błąd pobierania danych:", e);
      } finally {
        setLoading(false);
      }
    };

    fetchExamData();
  }, [user?.id]);

  const questions = useMemo(() => {
    if (!examData) return [];
    return currentScope === "standard"
      ? examData.examQuestions.standard
      : examData.examQuestions.specialized;
  }, [examData, currentScope]);

  const currentQuestion = questions[currentIndex];

  useEffect(() => {
    if (!loading && currentQuestion) {
      setHasRevealedMedia(false);

      if (currentScope === "standard") {
        setVideoStatus("prep");
        setTimeLeft(20);
      } else {
        setVideoStatus("idle");
        setHasRevealedMedia(true);
        setTimeLeft(50);
      }
    }
  }, [currentIndex, currentScope, loading]);

  useEffect(() => {
    if (loading || isFinishing || !examData) return;

    if (
      currentScope === "standard" &&
      videoStatus === "prep" &&
      timeLeft <= 0
    ) {
      handleRevealMedia();
      return;
    }

    if (
      timeLeft <= 0 &&
      (videoStatus === "answering" || currentScope === "specialized")
    ) {
      handleNext();
      return;
    }

    const timer = setInterval(() => {
      if (videoStatus !== "playing") {
        setTimeLeft((prev) => (prev > 0 ? prev - 1 : 0));
      }
    }, 1000);

    return () => clearInterval(timer);
  }, [timeLeft, loading, isFinishing, videoStatus, currentScope]);

  const handleRevealMedia = () => {
    if (hasRevealedMedia) return;

    setHasRevealedMedia(true);
    const isVideo = currentQuestion?.mediaUrl?.toLowerCase().endsWith(".mp4");

    if (isVideo) {
      setVideoStatus("playing");
    } else if (currentScope === "standard") {
      setVideoStatus("answering");
      setTimeLeft(15);
    }
  };

  const onVideoFinished = () => {
    if (currentScope === "standard") {
      setVideoStatus("answering");
      setTimeLeft(15);
    } else {
      setVideoStatus("idle");
    }
  };

  const handleNext = async () => {
    if (!currentQuestion) return;

    const updatedAnswers = {
      ...userAnswers,
      [currentQuestion.id]: {
        selectedAnswerId: selectedAnswerId,
        answeredAt: new Date().toISOString(),
      },
    };

    setUserAnswers(updatedAnswers);
    setSelectedAnswerId(null);

    if (currentIndex < questions.length - 1) {
      setCurrentIndex(currentIndex + 1);
    } else if (currentScope === "standard") {
      setCurrentScope("specialized");
      setCurrentIndex(0);
    } else {
      await handleFinishExam(updatedAnswers);
    }
  };

  const handleFinishExam = async (
    finalAnswers?: Record<
      string,
      { selectedAnswerId: string | null; answeredAt: string }
    >,
  ) => {
    if (isFinishing || !examData || !user?.id) return;
    setIsFinishing(true);

    try {
      const language = (await AsyncStorage.getItem("user-language")) ?? "EN";

      const answersMap = finalAnswers || {
        ...userAnswers,
        [currentQuestion.id]: {
          selectedAnswerId: selectedAnswerId,
          answeredAt: new Date().toISOString(),
        },
      };

      const formattedAnswers = Object.entries(answersMap).map(
        ([qId, data]) => ({
          questionId: qId,
          selectedAnswerId: data.selectedAnswerId,
          answeredAt: data.answeredAt,
        }),
      );

      await api.put("/api/exam/finish", {
        userId: user.id,
        examSessionId: examData.examSession.id,
        locale: language,
        answers: formattedAnswers,
      });

      router.replace(`/exam/examResult/${examData.examSession.id}`);
    } catch (e) {
      console.error("Błąd podczas finalizacji:", e);
      setIsFinishing(false);
    }
  };
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

  if (loading) {
    return (
      <View className="flex-1 items-center justify-center bg-white dark:bg-slate-950">
        <ActivityIndicator size="large" color="#1544b2" />
        <Text className="mt-4 text-slate-500 font-medium">
          {i18n.t("exam.generating_sheet")}
        </Text>
      </View>
    );
  }

  if (!currentQuestion) return null;

  const isMediaVideo = currentQuestion.mediaUrl?.toLowerCase().endsWith(".mp4");

  return (
    <View className="flex-1 bg-[#f6f6f8] dark:bg-[#111621]">
      <StatusBar barStyle="default" />

      <View
        style={{ paddingTop: insets.top }}
        className="flex-row items-center justify-between px-4 py-4 bg-white dark:bg-slate-900 border-b border-slate-200 dark:border-slate-800 shadow-sm"
      >
        <View className="flex-row items-center gap-3">
          <View>
            <Text className="text-[10px] font-bold uppercase tracking-widest text-[#1544b2]">
              Prawko AI
            </Text>
            <Text className="text-[9px] font-medium text-slate-400 uppercase">
              {i18n.t("exam.exam_simulation_word")}
            </Text>
          </View>
        </View>

        <View className="flex-row gap-2">
          <View
            className={`flex-row items-center gap-2 px-3 py-2 rounded-xl border ${timeLeft <= 5 ? "bg-red-50 border-red-200" : "bg-slate-50 dark:bg-slate-800 border-slate-200 dark:border-slate-700"}`}
          >
            <MaterialIcons
              name="timer"
              size={16}
              color={timeLeft <= 5 ? "#ef4444" : "#1544b2"}
            />
            <Text
              className={`text-base font-bold tabular-nums ${timeLeft <= 5 ? "text-red-600" : "dark:text-white"}`}
            >
              {videoStatus === "playing"
                ? "WIDEO"
                : `0:${timeLeft < 10 ? `0${timeLeft}` : timeLeft}`}
            </Text>
          </View>

          <TouchableOpacity
            onPress={() => handleFinishExam()}
            className="bg-red-500 px-4 py-3 rounded-lg"
          >
            <Text className="text-white text-xs font-bold">
              {i18n.t("exam.finish_btn")}
            </Text>
          </TouchableOpacity>
        </View>
      </View>

      <ScrollView
        className="flex-1"
        showsVerticalScrollIndicator={false}
        contentContainerStyle={{ paddingBottom: 180 }}
      >
        <View className="p-4">
          <View className="aspect-video rounded-2xl overflow-hidden bg-slate-900 shadow-xl border border-[#1544b2]/10 items-center justify-center">
            {currentQuestion.mediaUrl ? (
              <>
                {isMediaVideo ? (
                  hasRevealedMedia ? (
                    <VideoComponent
                      url={
                        process.env.EXPO_PUBLIC_SUPABASE_BUCKET_URL +
                        "/" +
                        currentQuestion.mediaUrl
                      }
                      onFinished={onVideoFinished}
                      shouldPlay={videoStatus === "playing"}
                    />
                  ) : null
                ) : (
                  hasRevealedMedia && (
                    <Image
                      source={{
                        uri: encodeURI(
                          process.env.EXPO_PUBLIC_SUPABASE_BUCKET_URL +
                            "/" +
                            currentQuestion.mediaUrl,
                        ),
                      }}
                      style={{ width: "100%", height: "100%" }}
                      resizeMode="cover"
                    />
                  )
                )}

                {!hasRevealedMedia && (
                  <TouchableOpacity
                    onPress={handleRevealMedia}
                    className="absolute inset-0 items-center justify-center bg-black/20"
                  >
                    <MaterialIcons
                      name={isMediaVideo ? "play-circle-filled" : "image"}
                      size={64}
                      color="white"
                    />
                    <Text className="text-white font-bold mt-2">
                      {isMediaVideo
                        ? "Kliknij, aby obejrzeć wideo"
                        : "Kliknij, aby zobaczyć zdjęcie"}
                    </Text>
                  </TouchableOpacity>
                )}
              </>
            ) : null}
          </View>
        </View>

        <View className="px-5">
          <View className="flex-row items-center gap-2 mb-4">
            <View className="px-3 py-1 bg-blue-50 dark:bg-blue-900/30 rounded-full border border-blue-100 dark:border-blue-800">
              <Text className="text-[#1544b2] dark:text-blue-400 text-[10px] font-bold uppercase tracking-tight">
                {i18n.t("exam.question_count")} {globalProgress} /{" "}
                {totalQuestions}
              </Text>
            </View>

            <View className="px-3 py-1 bg-slate-100 dark:bg-slate-800 rounded-full">
              <Text className="text-slate-500 dark:text-slate-400 text-[10px] font-bold uppercase">
                {i18n.t("exam.points_label")}: {currentQuestion.points}
              </Text>
            </View>
          </View>

          <Text className="text-xl font-bold leading-tight text-slate-900 dark:text-white mb-8">
            {currentQuestion.content}
          </Text>

          <View className="gap-3">
            {sortedAnswers.map((answer: any) => {
              const isSelected = selectedAnswerId === answer.id;
              const isBinary = sortedAnswers.length === 2;

              return (
                <TouchableOpacity
                  key={answer.id}
                  onPress={() => setSelectedAnswerId(answer.id)}
                  activeOpacity={0.8}
                  className={`p-5 rounded-2xl border-2 flex-row items-center justify-between ${
                    isSelected
                      ? "border-[#1544b2] bg-blue-50/50 dark:bg-blue-900/20"
                      : "border-white dark:border-slate-800 bg-white dark:bg-slate-800 shadow-sm"
                  }`}
                >
                  <Text
                    className={`text-base flex-1 font-bold ${isSelected ? "text-[#1544b2] dark:text-blue-400" : "text-slate-700 dark:text-slate-200"} ${isBinary ? "text-center text-xl uppercase tracking-widest" : ""}`}
                  >
                    {answer.content}
                  </Text>
                </TouchableOpacity>
              );
            })}
          </View>
        </View>
      </ScrollView>

      <View
        style={{ paddingBottom: insets.bottom + 16 }}
        className="absolute bottom-0 left-0 right-0 bg-white/95 dark:bg-slate-900/95 border-t border-[#1544b2]/10 gap-4 p-4"
      >
        <TouchableOpacity
          onPress={handleNext}
          disabled={isFinishing}
          activeOpacity={0.9}
          className="h-14 rounded-xl flex-row items-center justify-center bg-[#1544b2] shadow-lg shadow-blue-900/30"
        >
          <Text className="text-white font-bold text-base mr-2">
            {isFinishing
              ? i18n.t("exam.saving")
              : currentScope === "specialized" &&
                  currentIndex === questions.length - 1
                ? i18n.t("exam.finish_exam")
                : i18n.t("exam.next_question")}
          </Text>

          {!isFinishing && (
            <MaterialIcons name="arrow-forward" size={20} color="white" />
          )}
        </TouchableOpacity>

        <View className="flex-row gap-4">
          <View
            className={`flex-1 ${currentScope !== "standard" ? "opacity-30" : ""}`}
          >
            <View className="flex-row justify-between mb-2">
              <Text className="text-[9px] font-bold uppercase text-slate-400">
                {i18n.t("exam.basic_scope")}
              </Text>
              <Text className="text-[9px] font-bold text-[#1544b2]">
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
              <Text className="text-[9px] font-bold uppercase text-slate-400">
                {i18n.t("exam.specialized_scope")}
              </Text>
              <Text className="text-[9px] font-bold text-slate-500">
                {currentScope === "specialized" ? currentIndex + 1 : 0}/
                {totalSpecialized}
              </Text>
            </View>
            <View className="h-1.5 bg-slate-100 dark:bg-slate-800 rounded-full overflow-hidden">
              <View
                className="h-full bg-[#1544b2]"
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

const VideoComponent = ({
  url,
  onFinished,
  shouldPlay,
}: {
  url: string;
  onFinished: () => void;
  shouldPlay: boolean;
}) => {
  const player = useVideoPlayer(encodeURI(url), (player) => {
    player.loop = false;
  });

  useEffect(() => {
    if (shouldPlay) {
      player.play();
    }
  }, [shouldPlay]);

  useEffect(() => {
    const subscription = player.addListener("playToEnd", () => {
      onFinished();
    });
    return () => subscription.remove();
  }, [player]);

  return (
    <VideoView
      player={player}
      style={{ width: "100%", height: "100%" }}
      fullscreenOptions={{ enable: true, autoExitOnRotate: true }}
      contentFit="cover"
    />
  );
};
