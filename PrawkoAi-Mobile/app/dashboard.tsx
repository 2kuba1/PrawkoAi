import { MaterialCommunityIcons, MaterialIcons } from "@expo/vector-icons";
import { useRouter } from "expo-router";
import React, { useContext, useEffect } from "react";
import {
  Platform,
  ScrollView,
  StatusBar,
  Text,
  TouchableOpacity,
  View,
} from "react-native";
import { useSafeAreaInsets } from "react-native-safe-area-context";
import { AuthContext } from "./_layout";
import Footer from "./components/footer";
import { useError } from "./context/errorContext";
import api from "./utils/api";
import i18n from "./utils/translations";

export interface DashboardData {
  worstPerformingCategory?: string;
  maxQuestionsCount: number;
  questionsAnsweredCount: number;
  streak: number;
  averageScore: number;
  todayQuestionsAnsweredCount: number;
}

export default function DashboardScreen() {
  const insets = useSafeAreaInsets();
  const { user, token } = useContext(AuthContext);
  const router = useRouter();
  const { showError } = useError();

  const [dashboardData, setDashboardData] = React.useState<DashboardData>();

  useEffect(() => {
    const getData = async () => {
      if (!user || !token) {
        showError("User not authenticated");
        router.replace("/index" as any);
      }
      try {
        const response = await api.get<DashboardData>(
          "/user/getDashboardData",
          {
            params: {
              userId: user?.id,
            },
          },
        );

        setDashboardData(response.data);
      } catch (error) {
        showError("Failed to load dashboard data");
      }
    };
    getData();
  }, []);

  const paddingTop =
    Platform.OS === "android"
      ? insets.top > 0
        ? insets.top
        : StatusBar.currentHeight || 24
      : insets.top;

  const startExam = () => {
    router.push("/exam/examRules");
  };

  return (
    <View className="flex-1 bg-[#f6f6f8] dark:bg-[#111621]">
      <StatusBar
        barStyle="dark-content"
        translucent
        backgroundColor="transparent"
      />

      <View
        style={{ paddingTop }}
        className="px-4 pb-4 flex-row items-center justify-between bg-white/95 dark:bg-[#1a1f2e] border-b border-[#1544b2]/10 shadow-sm z-50"
      >
        <View className="flex-row items-center gap-3">
          <View className="w-10 h-10 rounded-full bg-white dark:bg-slate-800 items-center justify-center border border-[#1544b2]/10 shadow-sm">
            <MaterialIcons name="person" size={24} color="#1544b2" />
          </View>
          <View>
            <Text className="text-lg font-bold text-slate-900 dark:text-white leading-none">
              PrawkoAi
            </Text>
            <Text className="text-[10px] text-[#1544b2] font-bold uppercase tracking-wider mt-1">
              {i18n.t("dashboard.premium_account")}
            </Text>
          </View>
        </View>

        <TouchableOpacity className="p-2 rounded-full active:bg-blue-50 dark:active:bg-slate-700">
          <MaterialIcons name="notifications-none" size={26} color="#1544b2" />
        </TouchableOpacity>
      </View>

      <ScrollView
        className="flex-1"
        showsVerticalScrollIndicator={false}
        contentContainerStyle={{
          paddingBottom: Platform.OS === "ios" ? 110 : 30,
        }}
      >
        <View className="p-4">
          <View className="bg-white dark:bg-slate-800 rounded-xl p-5 border border-[#1544b2]/5 shadow-sm">
            <View className="flex-row items-center justify-between mb-4">
              <Text className="text-sm text-slate-500 dark:text-slate-400 font-medium">
                {i18n.t("dashboard.learning_progress")}
              </Text>
              <Text className="text-[#1544b2] font-bold text-xl">
                {dashboardData?.maxQuestionsCount
                  ? Math.round(
                      (dashboardData.questionsAnsweredCount /
                        dashboardData.maxQuestionsCount) *
                        100,
                    )
                  : 0}
                %
              </Text>
            </View>
            <View className="w-full bg-slate-100 dark:bg-slate-700 rounded-full h-1.5 mb-3 overflow-hidden">
              <View
                className="bg-[#1544b2] h-full"
                style={{
                  width: dashboardData?.maxQuestionsCount
                    ? Math.round(
                        (dashboardData.questionsAnsweredCount /
                          dashboardData.maxQuestionsCount) *
                          100,
                      )
                    : 0,
                }}
              />
            </View>
            <View className="flex-row justify-between items-center">
              <Text className="text-[11px] text-slate-400 font-medium">
                {dashboardData?.questionsAnsweredCount ?? 0} z{" "}
                {dashboardData?.maxQuestionsCount ?? 0}{" "}
                {i18n.t("dashboard.questions_mastered")}
              </Text>
              <Text
                className="text-[11px] text-green-600 font-bold"
                style={{
                  color:
                    (dashboardData?.todayQuestionsAnsweredCount ?? 0) > 0
                      ? "#16a34a"
                      : "#dc2626",
                }}
              >
                +{dashboardData?.todayQuestionsAnsweredCount ?? 0}{" "}
                {i18n.t("dashboard.today")}
              </Text>
            </View>
          </View>
        </View>

        <View className="px-4 py-2">
          <View className="relative overflow-hidden rounded-xl bg-[#1544b2] p-6 shadow-md">
            <View className="flex-row items-center gap-2 mb-2">
              <MaterialCommunityIcons name="brain" size={16} color="white" />
              <Text className="text-[10px] font-bold tracking-widest uppercase text-white/80">
                {i18n.t("dashboard.ai_suggestion")}
              </Text>
            </View>
            <Text className="text-xl font-bold text-white mb-2">
              {i18n.t("dashboard.daily_plan")}
            </Text>
            <Text className="text-white/80 text-sm mb-5 leading-5">
              {i18n.t("dashboard.ai_recommendation_text")}
            </Text>
            <TouchableOpacity className="bg-white px-5 py-2 rounded-lg self-start active:opacity-90">
              <Text className="text-[#1544b2] font-bold text-xs uppercase">
                {i18n.t("dashboard.train_now")}
              </Text>
            </TouchableOpacity>
          </View>
        </View>

        <View className="p-4">
          <Text className="text-slate-800 dark:text-slate-200 font-bold mb-3 px-1">
            {i18n.t("dashboard.main_menu")}
          </Text>

          <TouchableOpacity
            className="bg-white dark:bg-slate-800 p-4 rounded-xl border border-[#1544b2]/5 shadow-sm mb-3 flex-row items-center justify-between"
            onPress={() => router.push("/learning/studyTopics")}
          >
            <View className="flex-row items-center gap-4">
              <View className="w-12 h-12 rounded-lg bg-blue-50 dark:bg-blue-900/20 items-center justify-center">
                <MaterialIcons name="local-library" size={26} color="#1544b2" />
              </View>
              <View>
                <Text className="font-bold text-slate-900 dark:text-white text-base">
                  {i18n.t("dashboard.start_learning")}
                </Text>
                <Text className="text-xs text-slate-500">
                  {i18n.t("dashboard.continue_course")}
                </Text>
              </View>
            </View>
            <MaterialIcons name="chevron-right" size={20} color="#cbd5e1" />
          </TouchableOpacity>

          <TouchableOpacity
            onPress={startExam}
            className="bg-white dark:bg-slate-800 p-4 rounded-xl border border-[#1544b2]/5 shadow-sm mb-3 flex-row items-center justify-between"
          >
            <View className="flex-row items-center gap-4">
              <View className="w-12 h-12 rounded-lg bg-orange-50 dark:bg-orange-900/20 items-center justify-center">
                <MaterialIcons
                  name="assignment-turned-in"
                  size={26}
                  color="#ea580c"
                />
              </View>
              <View>
                <Text className="font-bold text-slate-900 dark:text-white text-base">
                  {i18n.t("dashboard.exam_simulation")}
                </Text>
                <Text className="text-xs text-slate-500">
                  {i18n.t("dashboard.official_database")}
                </Text>
              </View>
            </View>
            <MaterialIcons name="chevron-right" size={20} color="#cbd5e1" />
          </TouchableOpacity>
        </View>

        <View className="px-4 py-2 flex-row gap-4">
          <View className="flex-1 bg-white dark:bg-slate-800 p-4 rounded-xl border border-[#1544b2]/5 shadow-sm">
            <Text className="text-[10px] font-bold text-slate-400 mb-2 uppercase">
              {i18n.t("dashboard.streak")}
            </Text>
            <View className="flex-row items-center gap-2">
              <MaterialCommunityIcons name="fire" size={20} color="#f97316" />
              <Text className="text-lg font-bold text-slate-900 dark:text-white">
                {dashboardData?.streak} {i18n.t("dashboard.days")}
              </Text>
            </View>
          </View>
          <View className="flex-1 bg-white dark:bg-slate-800 p-4 rounded-xl border border-[#1544b2]/5 shadow-sm">
            <Text className="text-[10px] font-bold text-slate-400 mb-2 uppercase">
              {i18n.t("dashboard.avg_score")}
            </Text>
            <View className="flex-row items-center gap-2">
              <MaterialIcons name="trending-up" size={20} color="#1544b2" />
              <Text className="text-lg font-bold text-slate-900 dark:text-white">
                {dashboardData?.averageScore}/74
              </Text>
            </View>
          </View>
        </View>
      </ScrollView>

      <Footer />
    </View>
  );
}
