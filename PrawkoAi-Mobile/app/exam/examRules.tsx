import { MaterialCommunityIcons, MaterialIcons } from "@expo/vector-icons";
import AsyncStorage from "@react-native-async-storage/async-storage";
import { useRouter } from "expo-router";
import React, { useEffect, useState } from "react";
import {
  ActivityIndicator,
  Platform,
  ScrollView,
  StatusBar,
  Text,
  TouchableOpacity,
  View,
} from "react-native";
import { useSafeAreaInsets } from "react-native-safe-area-context";
import i18n from "../utils/translations";

export default function ExamRulesScreen() {
  const insets = useSafeAreaInsets();
  const router = useRouter();
  const [isLoading, setIsLoading] = useState(true);

  const paddingTop =
    Platform.OS === "android"
      ? insets.top > 0
        ? insets.top
        : StatusBar.currentHeight || 24
      : insets.top;

  useEffect(() => {
    const loadLanguage = async () => {
      const savedLanguage = await AsyncStorage.getItem("user-language");
      if (savedLanguage) {
        i18n.locale = savedLanguage;
      }
      setIsLoading(false);
    };
    loadLanguage();
  }, []);

  const startExam = () => {
    router.push("/exam/examSimulation");
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
      <StatusBar
        barStyle="dark-content"
        translucent
        backgroundColor="transparent"
      />

      <View
        style={{ paddingTop }}
        className="bg-white/95 dark:bg-[#1a1f2e] border-b border-blue-900/10 shadow-sm z-50"
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
            className="p-2 rounded-full"
            onPress={() =>
              router.canGoBack() ? router.back() : router.replace("/dashboard")
            }
          >
            <MaterialCommunityIcons
              name="arrow-left"
              size={24}
              color="#1544b2"
            />
          </TouchableOpacity>
          <Text className="text-xl font-bold ml-2 flex-1 text-slate-900 dark:text-white">
            {i18n.t("exam_rules.title")}
          </Text>
        </View>
      </View>

      <ScrollView
        className="flex-1 px-4"
        showsVerticalScrollIndicator={false}
        contentContainerStyle={{ paddingTop: 20, paddingBottom: 150 }}
      >
        <View className="flex-row justify-between items-center mb-4">
          <Text className="text-lg font-bold text-slate-800 dark:text-slate-200">
            {i18n.t("exam_rules.general_info")}
          </Text>
        </View>

        <View className="space-y-3">
          <View className="flex-row items-center bg-white dark:bg-slate-800 p-4 rounded-xl border border-[#1544b2]/5 shadow-sm mb-3">
            <View className="w-12 h-12 rounded-lg bg-blue-50 dark:bg-blue-900/20 items-center justify-center">
              <MaterialIcons name="quiz" size={26} color="#1544b2" />
            </View>
            <View className="ml-4">
              <Text className="text-slate-900 dark:text-white font-bold text-base">
                {i18n.t("exam_rules.questions_count")}
              </Text>
              <Text className="text-slate-500 dark:text-slate-400 text-xs">
                {i18n.t("exam_rules.official_database")}
              </Text>
            </View>
          </View>

          <View className="flex-row items-center bg-white dark:bg-slate-800 p-4 rounded-xl border border-[#1544b2]/5 shadow-sm mb-3">
            <View className="w-12 h-12 rounded-lg bg-blue-50 dark:bg-blue-900/20 items-center justify-center">
              <MaterialIcons name="timer" size={26} color="#1544b2" />
            </View>
            <View className="ml-4">
              <Text className="text-slate-900 dark:text-white font-bold text-base">
                {i18n.t("exam_rules.duration")}
              </Text>
              <Text className="text-slate-500 dark:text-slate-400 text-xs">
                {i18n.t("exam_rules.real_time")}
              </Text>
            </View>
          </View>
        </View>

        <Text className="text-lg font-bold text-slate-800 dark:text-slate-200 mt-6 mb-4">
          {i18n.t("exam_rules.test_structure")}
        </Text>

        <View className="bg-white dark:bg-slate-800 rounded-xl border border-[#1544b2]/5 shadow-sm overflow-hidden mb-3">
          <View className="p-4 border-b border-slate-50 dark:border-slate-700 flex-row justify-between items-center">
            <View>
              <Text className="text-[#1544b2] font-bold text-[10px] uppercase tracking-widest">
                {i18n.t("exam_rules.basic_label")}
              </Text>
              <Text className="text-slate-900 dark:text-white font-bold text-lg">
                20 {i18n.t("exam_rules.questions_unit")}
              </Text>
            </View>
            <View className="bg-slate-50 dark:bg-slate-700 px-3 py-1 rounded-full">
              <Text className="text-slate-500 dark:text-slate-400 text-[10px] font-bold">
                35{i18n.t("exam_rules.time_per_question")}
              </Text>
            </View>
          </View>
          <View className="p-4 flex-row justify-between items-center">
            <View>
              <Text className="text-[#1544b2] font-bold text-[10px] uppercase tracking-widest">
                {i18n.t("exam_rules.specialized_label")}
              </Text>
              <Text className="text-slate-900 dark:text-white font-bold text-lg">
                12 {i18n.t("exam_rules.questions_unit")}
              </Text>
            </View>
            <View className="bg-slate-50 dark:bg-slate-700 px-3 py-1 rounded-full">
              <Text className="text-slate-500 dark:text-slate-400 text-[10px] font-bold">
                50{i18n.t("exam_rules.time_per_question")}
              </Text>
            </View>
          </View>
        </View>

        <View className="mt-8 rounded-xl bg-[#1544b2] p-6 shadow-xl shadow-blue-900/20 relative overflow-hidden">
          <View className="absolute -right-6 -bottom-6 opacity-10">
            <MaterialCommunityIcons name="seal" size={120} color="white" />
          </View>

          <Text className="text-white/70 text-[10px] font-bold uppercase tracking-[2px] mb-4">
            {i18n.t("exam_rules.grading_system")}
          </Text>

          <View className="flex-row justify-between items-end mb-4">
            <View>
              <Text className="text-white/80 text-xs">
                {i18n.t("exam_rules.total_points")}
              </Text>
              <Text className="text-white text-3xl font-bold">74 pkt</Text>
            </View>
            <View className="items-end">
              <Text className="text-white/80 text-xs">
                {i18n.t("exam_rules.passing_threshold")}
              </Text>
              <Text className="text-white text-3xl font-bold">68 pkt</Text>
            </View>
          </View>

          <View className="h-[1px] bg-white/20 w-full mb-4" />

          <View className="flex-row items-center">
            <MaterialIcons name="stars" size={20} color="#fbbf24" />
            <Text className="text-white/90 text-sm ml-2 font-medium">
              {i18n.t("exam_rules.points_info")}
            </Text>
          </View>
        </View>

        <View className="mt-6 flex-row items-start p-4 bg-orange-50 dark:bg-orange-900/10 border border-orange-100 dark:border-orange-900/20 rounded-xl">
          <MaterialIcons name="info" size={20} color="#ea580c" />
          <Text className="flex-1 text-[12px] text-orange-800 dark:text-orange-400 ml-3 leading-5">
            <Text className="font-bold">
              {i18n.t("exam_rules.warning_title")}{" "}
            </Text>
            {i18n.t("exam_rules.warning_text")}
          </Text>
        </View>
      </ScrollView>

      <View
        style={{ paddingBottom: insets.bottom + 16 }}
        className="absolute bottom-0 left-0 right-0 p-4 bg-[#f6f6f8]/95 dark:bg-[#111621]/95 border-t border-[#1544b2]/10"
      >
        <TouchableOpacity
          onPress={startExam}
          activeOpacity={0.9}
          className="w-full bg-[#1544b2] h-14 rounded-xl flex-row items-center justify-center shadow-lg shadow-blue-900/30"
        >
          <Text className="text-white font-bold text-base mr-2">
            {i18n.t("exam_rules.start_btn")}
          </Text>
          <MaterialIcons name="chevron-right" size={24} color="white" />
        </TouchableOpacity>
      </View>
    </View>
  );
}
