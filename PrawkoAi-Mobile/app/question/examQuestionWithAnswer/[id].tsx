import { AuthContext } from "@/app/_layout";
import Footer from "@/app/components/footer";
import api from "@/app/utils/api";
import i18n from "@/app/utils/translations";
import { MaterialCommunityIcons, MaterialIcons } from "@expo/vector-icons";
import AsyncStorage from "@react-native-async-storage/async-storage";
import { useLocalSearchParams, useRouter } from "expo-router";
import React, { useContext, useEffect, useRef, useState } from "react";
import {
  ActivityIndicator,
  ImageBackground,
  KeyboardAvoidingView,
  Modal,
  Platform,
  ScrollView,
  StatusBar,
  Text,
  TextInput,
  TouchableOpacity,
  View,
} from "react-native";
import { useSafeAreaInsets } from "react-native-safe-area-context";

interface MediaAndExplanationResponse {
  mediaUrl: string;
  staticResponse: string;
  correctAnswerId: string;
}

interface ChatMessage {
  role: "user" | "model";
  content: string;
}

export default function ExamQuestionWithAnswer() {
  const insets = useSafeAreaInsets();
  const router = useRouter();
  const scrollViewRef = useRef<ScrollView>(null);
  const { token } = useContext(AuthContext);

  const [isLoading, setIsLoading] = useState(true);
  const [isAiModalVisible, setIsAiModalVisible] = useState(false);
  const [aiQuery, setAiQuery] = useState("");
  const [chatHistory, setChatHistory] = useState<ChatMessage[]>([]);
  const [isAiLoading, setIsAiLoading] = useState(false);
  const [mediaAndExplanation, setMediaAndExplanation] =
    useState<MediaAndExplanationResponse | null>(null);

  const params = useLocalSearchParams<{
    questionId: string;
    questionNumber: string;
    question: string;
    possibleAnswers: string;
    selectedAnswerId: string;
    wasCorrect: string;
    points: string;
  }>();

  const questionData = {
    questionId: params.questionId,
    questionNumber: Number(params.questionNumber),
    question: params.question,
    selectedAnswerId: params.selectedAnswerId,
    possibleAnswers: params.possibleAnswers
      ? JSON.parse(params.possibleAnswers)
      : [],
    points: Number(params.points),
  };

  useEffect(() => {
    const fetchData = async () => {
      try {
        const savedLanguage =
          (await AsyncStorage.getItem("user-language")) || "PL";
        i18n.locale = savedLanguage;

        const response = await api.get<MediaAndExplanationResponse>(
          "/api/questions/getQuestionAdditionalData",
          {
            params: {
              questionId: questionData.questionId,
              locale: savedLanguage.toUpperCase(),
            },
          },
        );
        setMediaAndExplanation(response.data);
      } catch (error) {
        console.error("Error fetching question data:", error);
      } finally {
        setIsLoading(false);
      }
    };
    fetchData();
  }, [questionData.questionId]);

  const handleAskAi = async () => {
    if (!aiQuery.trim() || isAiLoading) return;

    const userMessage: ChatMessage = { role: "user", content: aiQuery };
    setChatHistory((prev) => [...prev, userMessage]);
    const currentQuery = aiQuery;
    setAiQuery("");
    setIsAiLoading(true);

    const animateText = async (fullText: string) => {
      let displayedText = "";
      for (let i = 0; i < fullText.length; i++) {
        displayedText += fullText[i];

        setChatHistory((prev) => {
          const newHistory = [...prev];
          const lastIndex = newHistory.length - 1;
          if (lastIndex >= 0 && newHistory[lastIndex].role === "model") {
            newHistory[lastIndex] = {
              ...newHistory[lastIndex],
              content: displayedText,
            };
          }
          return newHistory;
        });

        await new Promise((resolve) => setTimeout(resolve, 20));
      }
    };

    try {
      const savedLanguage =
        (await AsyncStorage.getItem("user-language")) || "PL";

      const response = await fetch(
        `${process.env.EXPO_PUBLIC_API_URL}/api/ai/aiExplanation`,
        {
          method: "POST",
          headers: {
            "Content-Type": "application/json",
            Authorization: `Bearer ${token?.accessToken}`,
          },
          body: JSON.stringify({
            questionId: questionData.questionId,
            userQuery: currentQuery,
            locale: savedLanguage.toUpperCase(),
          }),
        },
      );

      if (!response.ok) throw new Error("Connection error, try again later");

      const rawText = await response.text();

      const cleanText = rawText
        .split("\n")
        .filter((line) => line.startsWith("data: "))
        .map((line) => line.replace("data: ", ""))
        .join("")
        .trim();

      setChatHistory((prev) => [...prev, { role: "model", content: "" }]);

      await animateText(cleanText);
    } catch (error) {
      console.error("AI Error:", error);
      setChatHistory((prev) => [
        ...prev,
        { role: "model", content: "Connection error, try again later" },
      ]);
    } finally {
      setIsAiLoading(false);
    }
  };

  if (isLoading) {
    return (
      <View className="flex-1 items-center justify-center bg-[#f6f6f8] dark:bg-[#111621]">
        <ActivityIndicator size="large" color="#1544b2" />
      </View>
    );
  }

  return (
    <View className="flex-1 bg-[#f6f6f8] dark:bg-[#111621]">
      <StatusBar barStyle="default" />

      <View
        style={{ paddingTop: insets.top }}
        className="px-4 pb-4 flex-row items-center gap-3 bg-white dark:bg-slate-900 border-b border-slate-200 dark:border-slate-800 shadow-sm"
      >
        <TouchableOpacity
          onPress={() => router.back()}
          className="p-2 rounded-full"
        >
          <MaterialIcons name="arrow-back" size={24} color="#1544b2" />
        </TouchableOpacity>
        <View>
          <Text className="text-lg font-bold text-slate-900 dark:text-white">
            Pytanie {questionData.questionNumber}
          </Text>
          <Text className="text-[10px] text-slate-400 font-bold uppercase tracking-widest">
            Tryb Przeglądu
          </Text>
        </View>
      </View>

      <ScrollView
        ref={scrollViewRef}
        className="flex-1"
        showsVerticalScrollIndicator={false}
        contentContainerStyle={{ paddingBottom: 160 }}
      >
        <View className="p-4">
          <View className="aspect-video rounded-2xl overflow-hidden bg-slate-900 shadow-xl border border-[#1544b2]/10">
            <ImageBackground
              source={{
                uri:
                  mediaAndExplanation?.mediaUrl ||
                  "https://images.unsplash.com/photo-1541899481282-d53bffe3c35d?q=80&w=1000",
              }}
              className="flex-1 items-center justify-center"
            >
              <TouchableOpacity className="w-14 h-14 rounded-full bg-white/20 items-center justify-center border border-white/40 backdrop-blur-md">
                <MaterialIcons name="play-arrow" size={36} color="white" />
              </TouchableOpacity>
            </ImageBackground>
          </View>

          <Text className="text-xl font-bold leading-tight text-slate-900 dark:text-white mb-6 mt-6">
            {questionData.question}
          </Text>

          <View className="gap-3 mb-8">
            {questionData.possibleAnswers.map((answer: any) => {
              const isSelected = questionData.selectedAnswerId === answer.id;
              const isCorrect =
                mediaAndExplanation?.correctAnswerId === answer.id;

              let borderStyle =
                "border-white dark:border-slate-800 bg-white dark:bg-slate-800";
              if (isCorrect)
                borderStyle =
                  "border-green-500 bg-green-50 dark:bg-green-900/20";
              else if (isSelected && !isCorrect)
                borderStyle = "border-red-500 bg-red-50 dark:bg-red-900/20";

              return (
                <View
                  key={answer.id}
                  className={`p-5 rounded-2xl border-2 flex-row items-center justify-between ${borderStyle}`}
                >
                  <Text
                    className={`text-base flex-1 font-bold ${isCorrect ? "text-green-700" : "text-slate-700 dark:text-slate-200"}`}
                  >
                    {answer.content}
                  </Text>
                  {isCorrect && (
                    <MaterialIcons
                      name="check-circle"
                      size={24}
                      color="#22c55e"
                    />
                  )}
                  {isSelected && !isCorrect && (
                    <MaterialIcons name="cancel" size={24} color="#ef4444" />
                  )}
                </View>
              );
            })}
          </View>

          <View className="bg-white dark:bg-slate-800 rounded-3xl p-6 border border-slate-100 dark:border-slate-700 shadow-sm">
            <View className="flex-row items-center gap-2 mb-3">
              <MaterialCommunityIcons
                name="lightbulb-on"
                size={20}
                color="#1544b2"
              />
              <Text className="text-sm font-black text-[#1544b2] uppercase">
                Oficjalne wytłumaczenie
              </Text>
            </View>
            <Text className="text-slate-600 dark:text-slate-300 leading-6 font-medium">
              {mediaAndExplanation?.staticResponse}
            </Text>
          </View>
        </View>
      </ScrollView>

      <View className="absolute bottom-32 right-6">
        <TouchableOpacity
          onPress={() => setIsAiModalVisible(true)}
          className="w-16 h-16 bg-[#1544b2] rounded-full shadow-2xl items-center justify-center border-4 border-white dark:border-slate-900"
        >
          <MaterialCommunityIcons name="auto-fix" size={28} color="white" />
        </TouchableOpacity>
      </View>

      <Modal
        visible={isAiModalVisible}
        animationType="slide"
        transparent={true}
      >
        <KeyboardAvoidingView
          behavior={Platform.OS === "ios" ? "padding" : "height"}
          className="flex-1 justify-end bg-black/50"
        >
          <View className="bg-slate-50 dark:bg-slate-900 rounded-t-[40px] p-6 h-[85%]">
            <View className="flex-row justify-between items-center mb-6">
              <Text className="text-2xl font-black dark:text-white tracking-tighter">
                Asystent PrawkoAI
              </Text>
              <TouchableOpacity
                onPress={() => setIsAiModalVisible(false)}
                className="bg-slate-200 dark:bg-slate-800 p-2 rounded-full"
              >
                <MaterialIcons name="close" size={20} color="gray" />
              </TouchableOpacity>
            </View>

            <ScrollView
              ref={scrollViewRef}
              onContentSizeChange={() =>
                scrollViewRef.current?.scrollToEnd({ animated: true })
              }
              className="flex-1 mb-4"
              showsVerticalScrollIndicator={false}
            >
              {chatHistory.length === 0 && (
                <View className="mt-10 items-center">
                  <MaterialCommunityIcons
                    name="comment-question-outline"
                    size={40}
                    color="#1544b2"
                  />
                  <Text className="text-slate-400 text-center mt-4 italic px-10">
                    Masz wątpliwości do tego pytania? Napisz do mnie, wyjaśnię
                    Ci przepisy!
                  </Text>
                </View>
              )}
              {chatHistory.map((msg, index) => (
                <View
                  key={index}
                  className={`mb-4 max-w-[85%] p-4 rounded-3xl ${
                    msg.role === "user"
                      ? "bg-[#1544b2] self-end rounded-tr-none"
                      : "bg-white dark:bg-slate-800 self-start rounded-tl-none shadow-sm"
                  }`}
                >
                  <Text
                    className={`text-[15px] font-medium ${msg.role === "user" ? "text-white" : "text-slate-800 dark:text-slate-200"}`}
                  >
                    {msg.content}
                  </Text>
                </View>
              ))}
              {isAiLoading &&
                chatHistory[chatHistory.length - 1]?.role === "user" && (
                  <View className="self-start ml-4 bg-white dark:bg-slate-800 p-3 rounded-2xl">
                    <ActivityIndicator size="small" color="#1544b2" />
                  </View>
                )}
            </ScrollView>

            <View className="bg-white dark:bg-slate-800 rounded-3xl shadow-lg border border-slate-100 dark:border-slate-800">
              <View className="flex-row gap-2 items-center p-2">
                <TextInput
                  className="flex-1 p-4 dark:text-white font-medium"
                  placeholder="Zapytaj o ten przepis..."
                  value={aiQuery}
                  onChangeText={setAiQuery}
                  multiline
                  maxLength={500}
                />
                <TouchableOpacity
                  onPress={handleAskAi}
                  disabled={isAiLoading || !aiQuery.trim()}
                  className={`w-12 h-12 rounded-2xl items-center justify-center ${
                    aiQuery.trim()
                      ? "bg-[#1544b2]"
                      : "bg-slate-200 dark:bg-slate-700"
                  }`}
                >
                  <MaterialIcons name="send" size={22} color="white" />
                </TouchableOpacity>
              </View>
              <View className="px-4 pb-2 items-end">
                <Text className="text-[10px] text-slate-400">
                  {aiQuery.length} / 500
                </Text>
              </View>
            </View>
          </View>
        </KeyboardAvoidingView>
      </Modal>

      <Footer />
    </View>
  );
}
