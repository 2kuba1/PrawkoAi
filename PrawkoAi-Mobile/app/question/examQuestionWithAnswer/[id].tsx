import Footer from "@/app/components/footer";
import { PossibleAnswer } from "@/app/exam/examResult/[id]";
import { MaterialCommunityIcons, MaterialIcons } from "@expo/vector-icons";
import { useRouter } from "expo-router";
import React from "react";
import {
  ImageBackground,
  ScrollView,
  StatusBar,
  Text,
  TouchableOpacity,
  View,
} from "react-native";
import { useSafeAreaInsets } from "react-native-safe-area-context";

interface MediaAndExplanationResponse {
  mediaUrl: string;
  explanation: string;
}

interface QuestionWithAnswerParams {
  id: string;
  questionNumber: number;
  question: string;
  possibleAnswers: PossibleAnswer[];
  selectedAnswerId: string;
  wasCorrect: boolean;
  points: number;
}

export default function ExamQuestionWithAnswer() {
  const insets = useSafeAreaInsets();
  const router = useRouter();
  //   const params = useLocalSearchParams<{
  //     id: string;
  //     questionNumber: string;
  //     question: string;
  //     possibleAnswers: string; // JSON string
  //     selectedAnswerId: string;
  //     wasCorrect: string;
  //     points: string;
  //   }>();

  //   const questionData: QuestionWithAnswerParams = {
  //     id: params.id,
  //     questionNumber: Number(params.questionNumber),
  //     question: params.question,
  //     selectedAnswerId: params.selectedAnswerId,
  //     wasCorrect: params.wasCorrect === "true",
  //     points: Number(params.points),
  //     possibleAnswers: params.possibleAnswers
  //       ? JSON.parse(params.possibleAnswers)
  //       : [],
  //   };

  //   const [mediaAndExplanation, setMediaAndExplanation] =
  //     useState<MediaAndExplanationResponse | null>(null);

  //   useEffect(() => {
  //     const fetchdMediaAndStaticResonse = async () => {
  //       try {
  //         const response = await api.get<MediaAndExplanationResponse>(
  //           "/api/question/mediaAndStaticResponse",
  //           {
  //             params: {
  //               questionId: questionData.id,
  //             },
  //           },
  //         );
  //         setMediaAndExplanation(response.data);
  //       } catch (error) {
  //         console.error("Error fetching media and explanation:", error);
  //       }
  //     };
  //     fetchdMediaAndStaticResonse();
  //   }, []);

  return (
    <View className="flex-1 bg-[#f6f6f8] dark:bg-[#111621]">
      <StatusBar barStyle="default" />

      {/* --- HEADER --- */}
      <View
        style={{ paddingTop: insets.top }}
        className="px-4 pb-4 flex-row items-center gap-3 bg-white dark:bg-slate-900 border-b border-slate-200 dark:border-slate-800 shadow-sm"
      >
        <TouchableOpacity
          onPress={() =>
            router.canGoBack() ? router.back() : router.push("/dashboard")
          }
          className="p-2 hover:bg-[#1544b2]/10 rounded-full"
        >
          <MaterialIcons name="arrow-back" size={24} color="#1544b2" />
        </TouchableOpacity>
        <View>
          <Text className="text-lg font-bold text-slate-900 dark:text-white">
            Pytanie 1
          </Text>
          <Text className="text-[10px] text-slate-400 font-bold uppercase tracking-widest">
            Symulacja Egzaminu
          </Text>
        </View>
      </View>

      <ScrollView
        className="flex-1"
        showsVerticalScrollIndicator={false}
        contentContainerStyle={{ paddingBottom: 160 }}
      >
        <View className="p-4">
          <View className="aspect-video rounded-2xl overflow-hidden bg-slate-900 shadow-xl">
            <ImageBackground
              source={{
                uri: "https://images.unsplash.com/photo-1541899481282-d53bffe3c35d?q=80&w=1000",
              }}
              className="flex-1 items-center justify-center"
            >
              <TouchableOpacity className="w-14 h-14 rounded-full bg-white/20 items-center justify-center border border-white/40 backdrop-blur-md">
                <MaterialIcons name="play-arrow" size={36} color="white" />
              </TouchableOpacity>
            </ImageBackground>
          </View>
        </View>

        <View className="px-5">
          <View className="flex-row items-center gap-2 mb-4">
            <View className="px-3 py-1 bg-blue-50 dark:bg-blue-900/30 rounded-full border border-blue-100 dark:border-blue-800">
              <Text className="text-[#1544b2] dark:text-blue-400 text-[10px] font-bold uppercase">
                Punkty: 3
              </Text>
            </View>
          </View>

          <Text className="text-xl font-bold leading-tight text-slate-900 dark:text-white mb-8">
            Czy w przedstawionej sytuacji masz pierwszeństwo przed pojazdem
            nadjeżdżającym z prawej strony?
          </Text>

          <View className="gap-3 mb-8">
            <View className="p-5 rounded-2xl border-2 border-white dark:border-slate-800 bg-white dark:bg-slate-800 shadow-sm">
              <Text className="text-xl font-black text-center uppercase tracking-widest text-slate-700 dark:text-slate-200">
                TAK
              </Text>
            </View>

            <View className="p-5 rounded-2xl border-2 border-red-500 bg-red-50 dark:bg-red-900/20 flex-row items-center justify-center">
              <Text className="text-xl font-black text-center uppercase tracking-widest text-red-700 dark:text-red-400">
                NIE
              </Text>
              <View className="absolute right-5">
                <MaterialIcons name="cancel" size={24} color="#ef4444" />
              </View>
            </View>
          </View>

          <View className="bg-white dark:bg-slate-800 rounded-3xl p-6 border border-slate-100 dark:border-slate-700 shadow-sm">
            <View className="flex-row items-center gap-2 mb-3">
              <MaterialCommunityIcons
                name="lightbulb-on"
                size={20}
                color="#1544b2"
              />
              <Text className="text-sm font-black text-[#1544b2] uppercase tracking-tighter">
                Wyjaśnienie
              </Text>
            </View>
            <Text className="text-slate-600 dark:text-slate-300 leading-6 font-medium">
              W tej sytuacji znajdujesz się na skrzyżowaniu równorzędnym.
              Zgodnie z zasadą "prawej ręki", masz obowiązek ustąpić
              pierwszeństwa pojazdowi nadjeżdżającemu z Twojej prawej strony.
              Twoja odpowiedź była błędna, ponieważ w tym przypadku nie
              posiadasz pierwszeństwa przejazdu.
            </Text>
          </View>
        </View>
      </ScrollView>

      <View className="absolute bottom-32 right-6">
        <TouchableOpacity className="w-16 h-16 bg-[#1544b2] rounded-full shadow-2xl items-center justify-center border-4 border-white dark:border-slate-900">
          <MaterialCommunityIcons name="auto-fix" size={28} color="white" />
        </TouchableOpacity>
      </View>

      <Footer />
    </View>
  );
}
