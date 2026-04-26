import { MaterialIcons } from "@expo/vector-icons";
import AsyncStorage from "@react-native-async-storage/async-storage";
import { useRouter } from "expo-router";
import React, { useContext, useEffect, useState } from "react";
import {
  ActivityIndicator,
  Platform,
  RefreshControl,
  ScrollView,
  StatusBar,
  Text,
  TextInput,
  TouchableOpacity,
  View,
} from "react-native";
import { useSafeAreaInsets } from "react-native-safe-area-context";
import { AuthContext } from "../_layout";
import Footer from "../components/footer";
import { useError } from "../context/errorContext";
import api from "../utils/api";
import categoryMap from "../utils/categoryMap";

export interface StudyTopics {
  categoryTag: string;
  questionsCount: number;
  completedQuestions: number;
}

const sections = [
  {
    title: "Znaki i Oznakowanie",
    part: 1,
    type: "list",
    ids: ["MandatoryAndWarningSigns", "InformationAndRoadMarkings"],
  },
  {
    title: "Skrzyżowania i Ruch",
    part: 2,
    type: "grid",
    ids: [
      "UncontrolledAndPriorityIntersections",
      "SignalizedIntersectionsAndPedestrians",
      "ManoeuvresAndPositioning",
      "OvertakingAndPassing",
    ],
  },
  {
    title: "Pojazd i Bezpieczeństwo",
    part: 3,
    type: "compact",
    ids: [
      "VehicleLightsAndSignals",
      "SocialBehaviourAndSecuring",
      "RailCrossingsAndPublicTransport",
      "EmergencyAndFitnessToDrive",
      "SpeedAndBrakingDistances",
      "SafetyFirstAidAndDocuments",
    ],
  },
];

export default function StudyTopicsScreen() {
  const insets = useSafeAreaInsets();
  const router = useRouter();

  const { user } = useContext(AuthContext);
  const { showError } = useError();

  const paddingTop =
    Platform.OS === "android"
      ? insets.top > 0
        ? insets.top
        : StatusBar.currentHeight || 24
      : insets.top;

  const [userProgress, setUserProgress] = React.useState<StudyTopics[] | null>(
    null,
  );
  const [loading, setLoading] = useState(true);
  const [refreshing, setRefreshing] = useState(false);

  const getProgress = (id: string) =>
    userProgress?.find((p) => p.categoryTag === id) || {
      CategoryTag: id,
      questionsCount: 0,
      completedQuestions: 0,
    };

  const fetchProgress = async () => {
    try {
      setLoading(true);
      const response = await api.get("/learn/getStudyTopics", {
        params: {
          userId: user?.id,
          category: (await AsyncStorage.getItem("user-category")) ?? "B",
        },
      });
      setUserProgress(response.data);
    } catch (e) {
      showError("Wystąpił błąd podczas pobierania postępu użytkownika");
      console.error("Błąd podczas pobierania postępu użytkownika:", e);
    } finally {
      setLoading(false);
      setRefreshing(false);
    }
  };

  const onRefresh = () => {
    setRefreshing(true);
    fetchProgress();
  };

  useEffect(() => {
    fetchProgress();
  }, []);

  if (loading && !refreshing) {
    return (
      <View className="flex-1 justify-center items-center bg-[#f6f6f8] dark:bg-[#111621]">
        <ActivityIndicator size="large" color="#1544b2" />
      </View>
    );
  }

  return (
    <View className="flex-1 bg-[#f6f6f8] dark:bg-[#111621]">
      <StatusBar
        barStyle="dark-content"
        translucent
        backgroundColor="transparent"
      />

      <View
        className="bg-white/95 dark:bg-[#1a1f2e] border-b border-blue-900/10 shadow-sm z-50"
        style={{ paddingTop }}
      >
        <View
          style={{
            height: 56,
            flexDirection: "row",
            alignItems: "center",
            paddingHorizontal: 12,
          }}
        >
          <TouchableOpacity
            onPress={() =>
              router.canGoBack() ? router.back() : router.replace("/dashboard")
            }
            className="p-2"
            hitSlop={{ top: 10, bottom: 10, left: 10, right: 10 }}
          >
            <MaterialIcons name="arrow-back" size={24} color="#1544b2" />
          </TouchableOpacity>
          <Text className="text-xl font-bold ml-2 flex-1 text-slate-900 dark:text-white">
            Tematy Nauki
          </Text>
        </View>
      </View>

      <ScrollView
        className="flex-1"
        contentContainerStyle={{
          paddingHorizontal: 16,
          paddingTop: 16,
          paddingBottom: Platform.OS === "ios" ? 100 : 20,
        }}
        showsVerticalScrollIndicator={false}
        refreshControl={
          <RefreshControl
            refreshing={refreshing}
            onRefresh={onRefresh}
            tintColor="#1544b2"
          />
        }
      >
        <View className="mb-6 flex-row items-center bg-white dark:bg-slate-800 rounded-2xl px-4 shadow-sm h-14">
          <MaterialIcons name="search" size={20} color="#475569" />
          <TextInput
            className="flex-1 ml-3 text-slate-800 dark:text-white"
            placeholder="Szukaj tematów..."
            placeholderTextColor="#94a3b8"
          />
        </View>

        <View className="mb-10 bg-[#1544b2] rounded-[32px] p-6 shadow-lg relative overflow-hidden">
          <View className="z-10 relative">
            <View className="bg-white/20 self-start px-3 py-1 rounded-full">
              <Text className="text-[10px] font-bold text-white uppercase tracking-widest">
                Codzienna porada
              </Text>
            </View>
            <Text className="text-2xl font-bold text-white mt-3">
              Opanuj skrzyżowania
            </Text>
            <Text className="text-blue-100 text-sm mt-1 max-w-[200px]">
              Skup się dzisiaj na zasadach pierwszeństwa.
            </Text>
            <TouchableOpacity className="mt-4 bg-white self-start px-6 py-2.5 rounded-xl">
              <Text className="text-[#1544b2] font-bold">Rozpocznij naukę</Text>
            </TouchableOpacity>
          </View>
          <MaterialIcons
            name="psychology"
            size={160}
            color="white"
            style={{
              position: "absolute",
              right: -30,
              bottom: -30,
              opacity: 0.1,
            }}
          />
        </View>

        {sections.map((section, sIdx) => (
          <View key={sIdx} className="mb-10">
            <View className="flex-row justify-between items-center mb-4 px-2">
              <Text className="font-bold text-lg text-slate-900 dark:text-white">
                {section.title}
              </Text>
              <View className="bg-[#e0e7ff] px-3 py-1 rounded-full">
                <Text className="text-[10px] font-bold text-[#1544b2] uppercase">
                  Część {section.part}
                </Text>
              </View>
            </View>

            {section.type === "list" && (
              <View>
                {section.ids.map((id) => {
                  const cat = categoryMap[id];
                  const prog = getProgress(id);
                  return (
                    <TouchableOpacity
                      key={id}
                      className="bg-white dark:bg-slate-800 p-5 rounded-2xl shadow-sm border border-blue-500/10 flex-row mb-4"
                      onPress={() =>
                        router.push({
                          pathname: "/learning/studyTopic/[id]" as any,
                          params: {
                            categoryId: id,
                            categoryName: cat.name,
                            questionsCount: prog.questionsCount.toString(),
                          },
                        })
                      }
                    >
                      <View className="w-12 h-12 rounded-xl bg-blue-50 dark:bg-blue-900/20 items-center justify-center mr-4">
                        <MaterialIcons
                          name={cat.icon as any}
                          size={24}
                          color="#1544b2"
                        />
                      </View>
                      <View className="flex-1">
                        <Text className="font-bold text-slate-800 dark:text-white">
                          {cat.name}
                        </Text>
                        <Text className="text-xs text-slate-500 mb-4">
                          Podstawy i zasady
                        </Text>
                        <View className="flex-row items-center">
                          <View className="flex-1 h-2 bg-slate-100 dark:bg-slate-700 rounded-full overflow-hidden mr-3">
                            <View
                              className="h-full bg-[#1544b2]"
                              style={{
                                width: `${(prog?.completedQuestions / prog?.questionsCount) * 100}%`,
                              }}
                            />
                          </View>
                          <Text className="text-[10px] font-bold text-slate-400">
                            {prog.completedQuestions}/{prog.questionsCount}
                          </Text>
                        </View>
                      </View>
                    </TouchableOpacity>
                  );
                })}
              </View>
            )}

            {section.type === "grid" && (
              <View
                className="flex-row flex-wrap justify-between"
                style={{ gap: 12 }}
              >
                {section.ids.map((id) => {
                  const cat = categoryMap[id];
                  const prog = getProgress(id);
                  const gridColors: any = {
                    UncontrolledAndPriorityIntersections: {
                      bg: "bg-orange-50",
                      text: "#ea580c",
                      icon: "priority-high",
                    },
                    SignalizedIntersectionsAndPedestrians: {
                      bg: "bg-emerald-50",
                      text: "#059669",
                      icon: "traffic",
                    },
                    ManoeuvresAndPositioning: {
                      bg: "bg-indigo-50",
                      text: "#4f46e5",
                      icon: "directions-car",
                    },
                    OvertakingAndPassing: {
                      bg: "bg-blue-50",
                      text: "#2563eb",
                      icon: "move-up",
                    },
                  };
                  const style = gridColors[id] || {
                    bg: "bg-blue-50",
                    text: "#1544b2",
                    icon: "help",
                  };

                  return (
                    <TouchableOpacity
                      key={id}
                      className="bg-white dark:bg-slate-800 p-4 rounded-2xl shadow-sm border border-blue-500/10 mb-1 flex-1 min-w-[45%]"
                      onPress={() =>
                        router.push({
                          pathname: "/learning/studyTopic/[id]" as any,
                          params: {
                            categoryId: id,
                            categoryName: cat.name,
                            questionsCount: prog.questionsCount.toString(),
                          },
                        })
                      }
                    >
                      <View
                        className={`w-10 h-10 rounded-lg ${style.bg} items-center justify-center mb-3`}
                      >
                        <MaterialIcons
                          name={style.icon as any}
                          size={20}
                          color={style.text}
                        />
                      </View>
                      <View style={{ minHeight: 42, justifyContent: "center" }}>
                        <Text className="font-bold text-slate-800 dark:text-white text-[11px] leading-4">
                          {cat.name}
                        </Text>
                      </View>
                      <View className="flex-row justify-between items-center mt-2">
                        <Text className="text-[9px] font-bold text-[#1544b2] uppercase">
                          Status
                        </Text>
                        <Text className="text-[10px] font-bold text-slate-400">
                          {prog.completedQuestions}/{prog.questionsCount}
                        </Text>
                      </View>
                    </TouchableOpacity>
                  );
                })}
              </View>
            )}

            {section.type === "compact" && (
              <View className="bg-white dark:bg-slate-800 rounded-2xl shadow-sm border border-blue-500/10 overflow-hidden">
                {section.ids.map((id, idx) => {
                  const cat = categoryMap[id];
                  const prog = getProgress(id);
                  return (
                    <TouchableOpacity
                      key={id}
                      className={`p-4 flex-row items-center border-b border-slate-50 dark:border-slate-700 ${idx === section.ids.length - 1 ? "border-b-0" : ""}`}
                      onPress={() =>
                        router.push({
                          pathname: "/learning/studyTopic/[id]" as any,
                          params: {
                            categoryId: id,
                            categoryName: cat.name,
                            questionsCount: prog.questionsCount.toString(),
                          },
                        })
                      }
                    >
                      <MaterialIcons
                        name={cat.icon as any}
                        size={20}
                        color="#94a3b8"
                      />
                      <Text className="flex-1 ml-4 text-sm font-medium text-slate-800 dark:text-white">
                        {cat.name}
                      </Text>
                      <Text className="text-[10px] font-bold text-slate-400 mr-2">
                        {prog.completedQuestions}/{prog.questionsCount}
                      </Text>
                      <MaterialIcons
                        name="chevron-right"
                        size={20}
                        color="#cbd5e1"
                      />
                    </TouchableOpacity>
                  );
                })}
              </View>
            )}
          </View>
        ))}
      </ScrollView>

      <Footer />
    </View>
  );
}
