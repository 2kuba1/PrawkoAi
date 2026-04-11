import { MaterialCommunityIcons, MaterialIcons } from "@expo/vector-icons";
import { useRouter } from "expo-router";
import React, { useContext, useEffect, useState } from "react";
import {
  ActivityIndicator,
  Dimensions,
  RefreshControl,
  ScrollView,
  StatusBar,
  Text,
  TouchableOpacity,
  View,
} from "react-native";
import { useSafeAreaInsets } from "react-native-safe-area-context";
import { AuthContext } from "../_layout";
import Footer from "../components/footer";
import api from "../utils/api";

const { width } = Dimensions.get("window");

export interface CategoryStat {
  accuracy: number;
  categoryTag: string;
  criticalErrors: number;
  mediumErrors: number;
  smallErrors: number;
  totalAttempts: number;
}

export interface StatisticsResponse {
  answersStats: CategoryStat[];
  averageExamScore: number;
  examTrend: number;
  passProbability: number;
  totalAccuracy: number;
  aiProgressAnalysis: string | null;
}

const categoryMap: Record<string, { name: string; icon: string }> = {
  SafetyFirstAidAndDocuments: {
    name: "Bezpieczeństwo i Dokumenty",
    icon: "medical-services",
  },
  OvertakingAndPassing: {
    name: "Wyprzedzanie i Mijanie",
    icon: "compare-arrows",
  },
  SocialBehaviourAndSecuring: {
    name: "Zachowanie i Zabezpieczenie",
    icon: "groups",
  },
  SpeedAndBrakingDistances: { name: "Prędkość i Hamowanie", icon: "speed" },
  EmergencyAndFitnessToDrive: {
    name: "Sytuacje Awaryjne i Zdrowie",
    icon: "psychology",
  },
  ManoeuvresAndPositioning: {
    name: "Manewry i Pozycjonowanie",
    icon: "directions-car",
  },
  MandatoryAndWarningSigns: {
    name: "Znaki Nakazu i Ostrzegawcze",
    icon: "warning",
  },
  SignalizedIntersectionsAndPedestrians: {
    name: "Sygnalizacja i Pieszy",
    icon: "traffic",
  },
  UncontrolledAndPriorityIntersections: {
    name: "Skrzyżowania i Pierwszeństwo",
    icon: "alt-route",
  },
  RailCrossingsAndPublicTransport: {
    name: "Przejazdy Kolejowe i Komunikacja",
    icon: "train",
  },
  VehicleLightsAndSignals: { name: "Światła i Sygnały", icon: "lightbulb" },
  InformationAndRoadMarkings: {
    name: "Informacja i Znaki Poziome",
    icon: "map",
  },
};

export default function Stats() {
  const insets = useSafeAreaInsets();
  const router = useRouter();

  const { user } = useContext(AuthContext);

  const [loading, setLoading] = useState(true);
  const [aiLoading, setAiLoading] = useState(false);
  const [refreshing, setRefreshing] = useState(false);
  const [stats, setStats] = useState<StatisticsResponse | null>(null);
  const [aiAnalysis, setAiAnalysis] = useState<string | null>(null);

  const fetchData = async () => {
    try {
      const response = await api.get<StatisticsResponse>("/api/user/stats", {
        params: { userId: user?.id },
      });
      if (response.data.aiProgressAnalysis) {
        setAiAnalysis(response.data.aiProgressAnalysis);
      }
      setStats(response.data);
    } catch (error) {
      console.error("Błąd pobierania statystyk:", error);
    } finally {
      setLoading(false);
      setRefreshing(false);
    }
  };

  const fetchAiAnalysis = async () => {
    setAiLoading(true);
    try {
      const response = await api.get<string>("/api/ai/analyzeUserProgress", {
        params: { userId: user?.id },
      });
      setAiAnalysis(response.data);
    } catch (error) {
      console.error("Błąd pobierania analizy AI:", error);
    } finally {
      setAiLoading(false);
    }
  };

  useEffect(() => {
    fetchData();
  }, []);

  const onRefresh = () => {
    setRefreshing(true);
    fetchData();
  };

  if (loading && !refreshing) {
    return (
      <View className="flex-1 justify-center items-center bg-[#f6f6f8] dark:bg-[#111621]">
        <ActivityIndicator size="large" color="#1544b2" />
      </View>
    );
  }

  const sumSmall =
    stats?.answersStats?.reduce((acc, curr) => acc + curr.smallErrors, 0) || 0;
  const sumMedium =
    stats?.answersStats?.reduce((acc, curr) => acc + curr.mediumErrors, 0) || 0;
  const sumCritical =
    stats?.answersStats?.reduce((acc, curr) => acc + curr.criticalErrors, 0) ||
    0;
  const totalErrors = sumSmall + sumMedium + sumCritical || 1;

  return (
    <View className="flex-1 bg-[#f6f6f8] dark:bg-[#111621]">
      <StatusBar barStyle="dark-content" />

      <View
        style={{ paddingTop: insets.top }}
        className="flex-row items-center p-4 bg-white/80 dark:bg-slate-900/80 border-b border-blue-900/10"
      >
        <TouchableOpacity
          className="p-2 rounded-full"
          onPress={() =>
            router.canGoBack() ? router.back() : router.replace("/dashboard")
          }
        >
          <MaterialCommunityIcons name="arrow-left" size={24} color="#1544b2" />
        </TouchableOpacity>
        <Text className="text-xl font-bold ml-2 flex-1 text-slate-900 dark:text-white">
          Twoje Statystyki
        </Text>
      </View>

      <ScrollView
        className="flex-1"
        contentContainerStyle={{ paddingBottom: insets.bottom + 20 }}
        showsVerticalScrollIndicator={false}
        refreshControl={
          <RefreshControl refreshing={refreshing} onRefresh={onRefresh} />
        }
      >
        <View className="m-4 p-6 rounded-3xl bg-[#1544b2] shadow-xl overflow-hidden relative">
          <View className="flex-row items-center justify-between mb-4">
            <View className="flex-row items-center">
              <MaterialIcons name="auto-awesome" size={20} color="white" />
              <Text className="text-white font-bold text-lg ml-2">
                Analiza AI
              </Text>
            </View>
            <TouchableOpacity
              onPress={fetchAiAnalysis}
              disabled={aiLoading}
              className="bg-white/20 px-3 py-1 rounded-full flex-row items-center"
            >
              {aiLoading ? (
                <ActivityIndicator size="small" color="white" />
              ) : (
                <>
                  <MaterialIcons name="refresh" size={14} color="white" />
                  <Text className="text-white text-xs font-bold ml-1">
                    GENERUJ
                  </Text>
                </>
              )}
            </TouchableOpacity>
          </View>

          <Text className="text-blue-100 text-sm leading-relaxed mb-4">
            Twoja szansa na zdanie egzaminu wynosi{" "}
            <Text className="text-white font-extrabold text-lg">
              {stats?.passProbability ?? 0}%
            </Text>
            .
            {aiAnalysis
              ? `\n\n${aiAnalysis}`
              : "\nKliknij generuj, aby otrzymać wskazówki."}
          </Text>

          <View className="pt-4 border-t border-white/10 flex-row items-start">
            <MaterialIcons name="lightbulb" size={20} color="#ffb59a" />
            <View className="ml-2 flex-1">
              <Text className="text-blue-200 text-[10px] font-bold uppercase tracking-widest mb-1">
                Status trendu:
              </Text>
              <Text className="text-white text-xs italic font-medium">
                {(stats?.examTrend ?? 0) == 0
                  ? "Twój wynik poprawia się!"
                  : "Musisz poświęcić więcej czasu na naukę."}
              </Text>
            </View>
          </View>
        </View>

        <View className="flex-row flex-wrap px-2">
          <MetricCard
            title="Szansa na zdanie"
            value={`${stats?.passProbability ?? 0}%`}
            progress={(stats?.passProbability ?? 0) / 100}
            color="#1544b2"
          />
          <MetricCard
            title="Trend"
            value={(stats?.examTrend ?? 0) == 0 ? "Wzrostowy" : "Spadkowy"}
            icon={
              (stats?.examTrend ?? 0) == 0 ? "trending-up" : "trending-down"
            }
            color={(stats?.examTrend ?? 0) == 0 ? "#22c55e" : "#ef4444"}
          />
          <MetricCard
            title="Dokładność"
            value={`${stats?.totalAccuracy ?? 0}%`}
            subtext="Skuteczność (ostatnie 500 pytań)"
            color="#3b82f6"
          />
          <MetricCard
            title="Średni wynik"
            value={`${Math.round(stats?.averageExamScore!) ?? 0} pkt`}
            subtext="Egzaminy (ostatnie 10 egzaminów)"
            subColor="#1544b2"
            color="#3b82f6"
          />
        </View>

        <View className="m-4 p-5 bg-white dark:bg-slate-900 rounded-3xl shadow-sm border border-slate-100 dark:border-slate-800">
          <Text className="font-bold text-slate-900 dark:text-white mb-4 flex-row items-center">
            <MaterialIcons name="warning" size={18} color="#ef4444" /> Struktura
            Punktowa Błędów
          </Text>

          <View className="h-4 w-full rounded-full bg-slate-100 dark:bg-slate-800 flex-row overflow-hidden mb-4">
            <View
              style={{ width: `${(sumSmall / totalErrors) * 100}%` }}
              className="bg-blue-300"
            />
            <View
              style={{ width: `${(sumMedium / totalErrors) * 100}%` }}
              className="bg-blue-600"
            />
            <View
              style={{ width: `${(sumCritical / totalErrors) * 100}%` }}
              className="bg-red-500"
            />
          </View>

          <View className="flex-row justify-between mb-4">
            <ScoreStat label="Małe" value={sumSmall} sub="1 pkt" />
            <ScoreStat label="Średnie" value={sumMedium} sub="2 pkt" />
            <ScoreStat
              label="Krytyczne"
              value={sumCritical}
              sub="3 pkt"
              highlight
            />
          </View>
        </View>

        <View className="px-4 mb-4">
          <Text className="font-bold text-slate-900 dark:text-white mb-3 ml-1">
            Analiza Kategorii
          </Text>
          {stats?.answersStats
            ?.sort((a, b) => a.accuracy - b.accuracy)
            .map((cat, idx) => {
              const categoryInfo = categoryMap[cat.categoryTag] || {
                name: cat.categoryTag,
                icon: "help-outline",
              };
              const isProblematic = cat.accuracy < 50;

              return (
                <TouchableOpacity
                  key={idx}
                  className="bg-white dark:bg-slate-900 p-4 rounded-2xl mb-2 flex-row items-center justify-between shadow-sm"
                >
                  <View className="flex-row items-center flex-1">
                    <View
                      style={{
                        backgroundColor: isProblematic
                          ? "#ef444420"
                          : "#1544b220",
                      }}
                      className="w-10 h-10 rounded-full items-center justify-center"
                    >
                      <MaterialIcons
                        name={categoryInfo.icon as any}
                        size={20}
                        color={isProblematic ? "#ef4444" : "#1544b2"}
                      />
                    </View>
                    <View className="ml-3 flex-1">
                      <Text
                        className="font-bold text-sm text-slate-900 dark:text-white"
                        numberOfLines={1}
                      >
                        {categoryInfo.name}
                      </Text>
                      <Text className="text-xs text-slate-500">
                        Skuteczność: {cat.accuracy}% | Prób: {cat.totalAttempts}
                      </Text>
                    </View>
                  </View>
                  <View className="w-16 h-1.5 bg-slate-100 dark:bg-slate-800 rounded-full overflow-hidden">
                    <View
                      style={{
                        width: `${cat.accuracy}%`,
                        backgroundColor: isProblematic ? "#ef4444" : "#1544b2",
                      }}
                      className="h-full"
                    />
                  </View>
                </TouchableOpacity>
              );
            })}
        </View>
      </ScrollView>

      <Footer />
    </View>
  );
}

function MetricCard({
  title,
  value,
  progress,
  icon,
  subtext,
  color = "#0f172a",
  subColor = "#94a3b8",
}: any) {
  return (
    <View
      style={{ width: (width - 48) / 2 }}
      className="bg-white dark:bg-slate-900 m-2 p-4 rounded-2xl shadow-sm border border-slate-50 dark:border-slate-800"
    >
      <Text className="text-[10px] uppercase font-bold text-slate-400 mb-1">
        {title}
      </Text>
      <View className="flex-row items-center justify-between">
        <Text style={{ color: color }} className="text-xl font-bold">
          {value}
        </Text>
        {icon && <MaterialCommunityIcons name={icon} size={20} color={color} />}
      </View>
      {progress !== undefined && (
        <View className="mt-2 h-1 bg-slate-100 rounded-full overflow-hidden">
          <View
            style={{ width: `${progress * 100}%`, backgroundColor: color }}
            className="h-full"
          />
        </View>
      )}
      {subtext && (
        <Text
          style={{ color: subColor }}
          className="text-[10px] mt-1 font-medium"
        >
          {subtext}
        </Text>
      )}
    </View>
  );
}

function ScoreStat({ label, value, sub, highlight }: any) {
  return (
    <View className="items-center">
      <Text className="text-[10px] font-bold text-slate-400 uppercase">
        {label}
      </Text>
      <Text
        className={`text-sm font-bold ${highlight ? "text-red-500" : "text-slate-900 dark:text-white"}`}
      >
        {value}
      </Text>
      <Text className="text-[9px] text-slate-400">{sub}</Text>
    </View>
  );
}
